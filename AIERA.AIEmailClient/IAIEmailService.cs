using MailKit;
using MimeKit;

namespace AIERA.AIEmailClient;


public interface IAIEmailService : IDisposable
{
    /// <summary>
    ///     Gets the message folder on the authenticated email account, or creates a new one if not found and <paramref name="createIfNotFound"/> is set to true.
    /// </summary>
    /// <param name="folderPath">Path to the folder relative to the root mailbox folder. Either '/' or '\' can be used as directory seperator.
    ///     <example>
    ///         E.g.: 'AIERA/Replied' or 'AIERA\Replied'
    ///     </example>
    ///</param>
    /// <param name="createIfNotFound">If set to true, will create a new folder at the root, in case the folder was not found.</param>
    /// <param name="cancellationToken"></param>
    /// <returns>Found or newly created folder on the authenticated email account.</returns>
    /// <exception cref="FolderNotFoundException">The folder could not be found.</exception>
    /// <exception cref="FolderNameNotLegalException">If the name is not a legal folder name.</exception>
    Task<IMailFolder> GetMessageFolderAsync(string aiReplyFolderPath,
                                            bool createIfNotFound,
                                            CancellationToken cancellationToken = default);

    /// <summary>
    /// Replies to the message in the <paramref name="replyFolder"/> using the AI chat completion.
    /// </summary>
    /// <param name="replyFolder">Messages from this folder will be replied to using AI.</param>
    /// <param name="repliedFolder">The replied message will be moved to this folder when it has been replied.</param>
    /// <param name="cancellationToken"></param>
    /// <returns>The <see cref="MimeMessage.MessageId"/> from the email reply message.</returns>
    IAsyncEnumerable<string> ReplyAllWithAIMessageAsync(IMailFolder replyFolder,
                                                        IMailFolder repliedFolder,
                                                        CancellationToken cancellationToken = default);

    Task<IList<UniqueId>> GetUnansweredEmailsInEmailFolder(IMailFolder replyFolder, CancellationToken cancellationToken);
}