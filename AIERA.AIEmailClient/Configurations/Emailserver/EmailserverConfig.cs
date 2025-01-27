using MailKit.Security;
using Microsoft.Extensions.Options;
using System.ComponentModel.DataAnnotations;
using static AIERA.AIEmailClient.Configurations.Emailserver.EmailserverConfig.IEmailServerConfig;

namespace AIERA.AIEmailClient.Configurations.Emailserver;

public class EmailserverConfig
{
    [ValidateObjectMembers]
    [Required]
    public MicrosoftMailServer MicrosoftServer { get; private set; } = new();

    public class MicrosoftMailServer : IEmailServerConfig
    {
        public MicrosoftMailServer() { }

        [ValidateObjectMembers]
        [Required]
        public ServerConfig ImapServer { get; } = new ServerConfig()
        {
            Host = "outlook.office365.com",
            Port = 993,
            SecureSocketOptions = SecureSocketOptions.SslOnConnect,
        };

        [ValidateObjectMembers]
        [Required]
        public ServerConfig SmtpServer { get; } = new ServerConfig()
        {
            Host = "smtp-mail.outlook.com",
            Port = 587,
            SecureSocketOptions = SecureSocketOptions.StartTls,
        };
    }


    public interface IEmailServerConfig
    {
        [Required]
        public ServerConfig ImapServer { get; }

        [Required]
        public ServerConfig SmtpServer { get; }



        public class ServerConfig
        {
            internal ServerConfig() { }

            /// <summary>
            /// Host server, e.g. "outlook.office365.com" for a Microsoft imap server or "smtp-mail.outlook.com" for a Microsoft smtp server.
            /// </summary>
            [Required]
            public required string Host { get; init; }

            [Required]
            public required int Port { get; init; }

            [Required]
            public required SecureSocketOptions SecureSocketOptions { get; init; }
        }
    }
}