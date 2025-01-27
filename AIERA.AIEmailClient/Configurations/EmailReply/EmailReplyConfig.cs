using System.ComponentModel.DataAnnotations;

namespace AIERA.AIEmailClient.Configurations.EmailReply;

public class EmailReplyConfig
{
    internal const string ConfigurationSectionName = $"AIEmailClient:{nameof(EmailReplyConfig)}";

    /// <summary>
    /// The prefix to add to the subject of the replied email.
    /// <example>eg. "Re:" for "Re: Subject" or "sv:" for "Sv: Subject".</example>
    /// </summary>
    [Required]
    public string SubjectRepliedPrefix { get; private set; } = "Re:";

    /// <summary>
    /// The prefix to add to the body of the replied email, telling the user that it was replied to using AI.
    /// </summary>
    [Required]
    public string AIReplyBodyPrefix { get; private set; } = "(This Email has been replied to automatically using AI.)";

}