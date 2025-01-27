using AIERA.AIChatClient;
using AIERA.AIEmailClient.Configurations.Emailserver;
using AIERA.AIEmailClient.Exceptions;
using Common.Helpers;
using MailKit;
using MailKit.Net.Imap;
using MailKit.Net.Smtp;
using MailKit.Search;
using MailKit.Security;
using Microsoft.Extensions.Options;
using Microsoft.Identity.Client;
using MimeKit;
using System.Runtime.CompilerServices;
using System.Text;
using static AIERA.AIEmailClient.IoC.Bootstrapper;

namespace AIERA.AIEmailClient;


public sealed partial class AIEmailService : IAIEmailService, IDisposable
{
    private readonly IImapClient _imapClient;
    private readonly ISmtpClient _smtpClient;
    private readonly AuthenticationResult _authenticationResult;
    private readonly IReplyVisitorFactory _replyVisitorFactory;

    private readonly EmailserverConfig _emailServerConfig;
    private readonly AIChatService _aiChatService;

    /// <summary>
    /// Returns a new instance of the <see cref="AIEmailService"/> class, that can be used to interact with an email account.
    /// </summary>
    /// <remarks> Should not be created directly. Use the <see cref="IAIEmailServiceFactory"/> instead.</remarks>
    /// <param name="imapClient"></param>
    /// <param name="smtpClient"></param>
    /// <param name="emailserverOptions"></param>
    /// <param name="aiChatService">Used to create the body text for the reply message.</param>
    /// <param name="authenticationResult"></param>
    /// <param name="replyVisitorFactory">Used to create the reply <see cref="MimeMessage"/> with all the information from the original <see cref="MimeMessage"/>.</param>
    public AIEmailService(IImapClient imapClient, ISmtpClient smtpClient, IOptions<EmailserverConfig> emailserverOptions,
                          AIChatService aiChatService, AuthenticationResult authenticationResult,
                          IReplyVisitorFactory replyVisitorFactory)
    {
        _imapClient = imapClient;
        _smtpClient = smtpClient;
        _emailServerConfig = emailserverOptions.Value;
        _aiChatService = aiChatService;
        _authenticationResult = authenticationResult;
        _replyVisitorFactory = replyVisitorFactory;
    }


    public async Task<IMailFolder> GetMessageFolderAsync(string folderPath, bool createIfNotFound, CancellationToken cancellationToken = default)
    {
        try
        {
            await EnsureImapClientConnectedAsync(cancellationToken).ConfigureAwait(false);
            await EnsureImapClientAuthenticatedAsync(cancellationToken).ConfigureAwait(false);

            return await _imapClient.GetFolderAsync(folderPath, cancellationToken).ConfigureAwait(false);
        }
        catch (ArgumentException ex) when (ex.Message.Equals("The name is not a legal folder name. Arg_ParamName_Name", StringComparison.Ordinal)) // LOG
        {
            throw new FolderNameNotLegalException(folderPath, ex);
        }
        catch (FolderNotFoundException) // LOG
        {
            if (createIfNotFound)
            {
                return await CreateMessageFolderAsync(folderPath, cancellationToken).ConfigureAwait(false);
            }
            throw;
        }
    }

    /// <summary>
    ///     Creates a new messageFolder.
    /// </summary>
    /// <param name="folderPath">Path to the folder relative to the root mailbox folder. Either '/' or '\' can be used as directory seperator.
    ///     <example>
    ///         E.g.: 'AIERA/Replied' or 'AIERA\Replied'
    ///     </example>   
    ///</param>
    /// <param name="folderPath"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<IMailFolder> CreateMessageFolderAsync(string folderPath, CancellationToken cancellationToken)
    {
        await EnsureImapClientConnectedAsync(cancellationToken).ConfigureAwait(false);
        await EnsureImapClientAuthenticatedAsync(cancellationToken).ConfigureAwait(false);

        // Starts with the root mailbox folder (e.g., personal namespace)
        FolderNamespace personalNamespace = _imapClient.PersonalNamespaces[0];
        IMailFolder currentMailFolder = await _imapClient.GetFolderAsync(personalNamespace.Path, cancellationToken).ConfigureAwait(false);

        // Folders in the relative folder path, seperated by '/' or '\'.
        string[] folders = folderPath.Split(['/', '\\'], StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);

        // Keep track of the folder path traversed so far (for the error message).
        StringBuilder currentRelativePath = new("..");

        foreach (var currentFolderName in folders)
        {
            _ = currentRelativePath.Append('/').Append(currentFolderName);

            // Check if the folder already exists.
            var subFolders = await currentMailFolder.GetSubfoldersAsync(subscribedOnly: false, cancellationToken).ConfigureAwait(false);
            IMailFolder? existingFolder = subFolders.SingleOrDefault(folder => folder.Name.Equals(currentFolderName, StringComparison.OrdinalIgnoreCase));

            // Create the folder if it does not exist and set it as the current folder.
            currentMailFolder = existingFolder is null
                ? await currentMailFolder.CreateAsync(currentFolderName, isMessageFolder: true, cancellationToken).ConfigureAwait(false)
                : existingFolder;
        }

        return currentMailFolder;
    }


    public async IAsyncEnumerable<string> ReplyAllWithAIMessageAsync(IMailFolder replyFolder,
                                                                     IMailFolder repliedFolder,
                                                                     [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        _ = await replyFolder.OpenAsync(FolderAccess.ReadWrite, cancellationToken).ConfigureAwait(false);
        IList<UniqueId> messageUids = await replyFolder.SearchAsync(SearchQuery.NotAnswered, cancellationToken).ConfigureAwait(false);

        foreach (UniqueId messageToBeRepliedUid in messageUids)
        {
            cancellationToken.ThrowIfCancellationRequested();

            await EnsureSmtpClientConnectedAsync(cancellationToken).ConfigureAwait(false);
            await EnsureSmtpClientAuthenticatedAsync(cancellationToken).ConfigureAwait(false);
            await EnsureImapClientConnectedAsync(cancellationToken).ConfigureAwait(false);
            await EnsureImapClientAuthenticatedAsync(cancellationToken).ConfigureAwait(false);

            MimeMessage messageToBeReplied = await replyFolder.GetMessageAsync(messageToBeRepliedUid, cancellationToken).ConfigureAwait(false);

            MimeMessage replyMessage = await CreateAIReplyMessageAsync(messageToBeReplied,
                                                                       replyToAll: true,
                                                                       cancellationToken).ConfigureAwait(false);

            _ = await _smtpClient.SendAsync(replyMessage, cancellationToken).ConfigureAwait(false);

            // Mark sent message as replied.
            await replyFolder.AddFlagsAsync(messageToBeRepliedUid,
                                            MessageFlags.Answered,
                                            silent: true,
                                            cancellationToken).ConfigureAwait(false);

            // Move to replied folder.
            _ = await replyFolder.MoveToAsync(messageToBeRepliedUid, repliedFolder, cancellationToken).ConfigureAwait(false);

            yield return replyMessage.MessageId;
        }
        await replyFolder.CloseAsync(expunge: false, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="folder"></param>
    /// <param name="items"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<IList<IMessageSummary>> GetMessageSummariesAsync(IMailFolder folder, MessageSummaryItems items, CancellationToken cancellationToken)
    {
        await EnsureImapClientConnectedAsync(cancellationToken).ConfigureAwait(false);
        await EnsureImapClientAuthenticatedAsync(cancellationToken).ConfigureAwait(false);

        // Step 1: Get the UniqueIds of all messages in the folder
        _ = await folder.OpenAsync(FolderAccess.ReadWrite, cancellationToken).ConfigureAwait(false);
        IList<UniqueId> messageUids = await folder.SearchAsync(SearchQuery.NotAnswered, cancellationToken).ConfigureAwait(false);

        // Step 2: Fetch metadata for the messages (e.g., headers or full MimeMessage)
        IList<IMessageSummary> messages = await folder.FetchAsync(messageUids,
                                                                  items,
                                                                  cancellationToken).ConfigureAwait(false);

        await folder.CloseAsync(expunge: false, cancellationToken).ConfigureAwait(false);
        return messages;
    }

    public async Task<IList<UniqueId>> GetUnansweredEmailsInEmailFolder(IMailFolder replyFolder, CancellationToken cancellationToken)
    {
        await replyFolder.OpenAsync(FolderAccess.ReadWrite, cancellationToken).ConfigureAwait(false);
        var unansweredEmails = await replyFolder.SearchAsync(SearchQuery.NotAnswered, cancellationToken).ConfigureAwait(false);
        await replyFolder.CloseAsync(expunge: false, cancellationToken).ConfigureAwait(false);

        return unansweredEmails;
    }

    public async Task<MimeMessage> CreateAIReplyMessageAsync(MimeMessage message, bool replyToAll, CancellationToken cancellationToken)
    {
        var from = new MailboxAddress(name: _authenticationResult.ClaimsPrincipal.Identity?.Name ?? string.Empty,
                                      address: AuthenticationResultHelper.TryGetEmailFromAuthenticationResult(_authenticationResult, account: null, out string? emailAddress) ? emailAddress! : string.Empty);

        string replyText = await _aiChatService.GetAIEmailResponseAsync(message, cancellationToken).ConfigureAwait(false);

        ReplyVisitor visitor = _replyVisitorFactory.Create(from, replyToAll, replyText);

        visitor.Visit(message);

        return visitor.Reply!;
    }

    #region Ensure connection and authentication
    /// <summary>
    /// Checks if the <see cref="_imapClient"/> is connected, and connects if not.
    /// </summary>
    /// <param name="cancellationToken"></param>
    private Task EnsureImapClientConnectedAsync(CancellationToken cancellationToken)
    {
        if (_imapClient.IsConnected)
            return Task.CompletedTask;

        return _imapClient.ConnectAsync(_emailServerConfig.MicrosoftServer!.ImapServer!.Host, _emailServerConfig.MicrosoftServer.ImapServer.Port, _emailServerConfig.MicrosoftServer.ImapServer.SecureSocketOptions, cancellationToken);
    }

    /// <summary>
    /// Checks if the <see cref="_imapClient"/> is authenticated, and authenticates if not.
    /// </summary>
    /// <param name="cancellationToken"></param>
    private Task EnsureImapClientAuthenticatedAsync(CancellationToken cancellationToken)
    {
        if (_imapClient.IsAuthenticated)
            return Task.CompletedTask;

        return _imapClient.AuthenticateAsync(new SaslMechanismOAuth2(_authenticationResult.Account.Username, _authenticationResult.AccessToken), cancellationToken);
    }

    /// <summary>
    /// Checks if the <see cref="ISmtpClient"/> is connected, and connects if not.
    /// </summary>
    /// <param name="cancellationToken"></param>
    private Task EnsureSmtpClientConnectedAsync(CancellationToken cancellationToken)
    {
        if (_smtpClient.IsConnected)
            return Task.CompletedTask;

        return _smtpClient.ConnectAsync(_emailServerConfig.MicrosoftServer.SmtpServer.Host, _emailServerConfig.MicrosoftServer.SmtpServer.Port, _emailServerConfig.MicrosoftServer.SmtpServer.SecureSocketOptions, cancellationToken);
    }

    /// <summary>
    /// Checks if the <see cref="ISmtpClient"/> is authenticated, and authenticates if not.
    /// </summary>
    /// <param name="cancellationToken"></param>
    private Task EnsureSmtpClientAuthenticatedAsync(CancellationToken cancellationToken)
    {
        if (_smtpClient.IsAuthenticated)
            return Task.CompletedTask;

        return _smtpClient.AuthenticateAsync(new SaslMechanismOAuth2(_authenticationResult.Account.Username, _authenticationResult.AccessToken), cancellationToken);
    }
    #endregion

    #region IDisposable implementation
    /// <summary>
    /// <para>
    ///     Disconnects from the <see cref="IImapClient"/> and <see cref="ISmtpClient"/> (if connected) before calling the Dispose method on them.
    /// </para>
    /// </summary>
    /// <remarks>
    ///     The disconnect before Dispose, is per recommendation from the creater of the MailKit library <see href="https://github.com/jstedfast/MailKit/issues/941"/>
    /// </remarks
    public void Dispose()
    {
        if (_imapClient.IsConnected)
            _imapClient.Disconnect(quit: true);

        if (_smtpClient.IsConnected)
            _smtpClient.Disconnect(quit: true);

        _imapClient.Dispose();
        _smtpClient.Dispose();
        #endregion
    }
}