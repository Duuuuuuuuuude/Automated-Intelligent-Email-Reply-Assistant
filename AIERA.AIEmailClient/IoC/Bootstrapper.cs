using AIERA.AIChatClient;
using AIERA.AIEmailClient.Configurations.EmailReply;
using AIERA.AIEmailClient.Configurations.Emailserver;
using AIERA.AIEmailClient.Logging;
using MailKit.Net.Imap;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.Identity.Client;
using MimeKit;

namespace AIERA.AIEmailClient.IoC;

public static class Bootstrapper
{
    public static IServiceCollection AddAIEmailService(this IServiceCollection services, IConfiguration config)
    {
        // Register the EmailReplyConfig as an option.
        _ = services.AddOptions<EmailReplyConfig>().Bind(config.GetSection(EmailReplyConfig.ConfigurationSectionName),
                                                         options => { options.BindNonPublicProperties = true; })
            .ValidateOnStart();
        _ = services.AddSingleton<IValidateOptions<EmailReplyConfig>, ValidateEmailReplyConfig>()


        // Injects the EmailserverConfig as an option.
        .AddOptions<EmailserverConfig>().ValidateOnStart(); // Doesn't have values in the configuration file, so no need to bind it.
        _ = services.AddSingleton<IValidateOptions<EmailserverConfig>, ValidateEmailserverConfig>();


        // Inject a ReplyVisitor factory that takes the necessary arguments from the caller and lets the DI container take care of the rest of the dependencies.
        _ = services.AddSingleton<Func<MailboxAddress, bool, string, ReplyVisitor>>(provider =>
         {
             return (from, replyToAll, ReplyText) => ActivatorUtilities.CreateInstance<ReplyVisitor>(provider, from, replyToAll, ReplyText);
         })
        .AddSingleton<IReplyVisitorFactory, ReplyVisitorFactory>()


        // Injects the AIChatService.
        .AddTransient<AIChatService>()


        // Injects the logger for the ImapClient and SmtpClient.
        .AddTransient<ProtocolLoggerILogger<ImapClient>>()
        .AddTransient<ProtocolLoggerILogger<SmtpClient>>()

        // Injects the AIEmailService factory.
        .AddSingleton<Func<AuthenticationResult, IAIEmailService>>(provider =>
        {
            return authResult =>
            {
                ImapClient imapClient = new(provider.GetRequiredService<ProtocolLoggerILogger<ImapClient>>());
                SmtpClient smtpClient = new(provider.GetRequiredService<ProtocolLoggerILogger<SmtpClient>>());
                return ActivatorUtilities.CreateInstance<AIEmailService>(provider, imapClient, smtpClient, authResult);
            };
        })
        .AddSingleton<IAIEmailServiceFactory, AIEmailServiceFactory>();

        return services;
    }

    #region ReplyVisitor factory
    public interface IReplyVisitorFactory
    {
        ReplyVisitor Create(MailboxAddress from, bool replyToAll, string replyText);
    }

    public class ReplyVisitorFactory : IReplyVisitorFactory
    {
        private readonly Func<MailboxAddress, bool, string, ReplyVisitor> _factory;
        public ReplyVisitorFactory(Func<MailboxAddress, bool, string, ReplyVisitor> factory) => _factory = factory;

        public ReplyVisitor Create(MailboxAddress from, bool replyToAll, string replyText) => _factory(from, replyToAll, replyText);
    }
    #endregion


    #region AIEmailServiceFactory
    public interface IAIEmailServiceFactory
    {
        IAIEmailService Create(AuthenticationResult authResult);
    }

    public class AIEmailServiceFactory : IAIEmailServiceFactory
    {
        private readonly Func<AuthenticationResult, IAIEmailService> _factory;
        public AIEmailServiceFactory(Func<AuthenticationResult, IAIEmailService> factory) => _factory = factory;

        /// <summary>
        /// Returns a new instance of <see cref="IAIEmailService"/> using the provided <paramref name="authenticationResult"/>.
        /// </summary>
        /// <remarks>
        /// As per Microsoft own guidelines (<see href="https://learn.microsoft.com/en-us/dotnet/core/extensions/dependency-injection-guidelines"/>),
        /// IAIEmailService should be created using a factory, to be able to manually control its lifetime, even though it is injected into a class that lives for as long as the application.
        /// Also, transient instances that implement IDisposable should not be registered in the dependency injection container,
        /// but using factory pattern, as we are doing here.</remarks>
        /// <param name="authenticationResult"></param>
        /// <returns></returns>
        public IAIEmailService Create(AuthenticationResult authenticationResult) => _factory(authenticationResult);
    }
    #endregion
}