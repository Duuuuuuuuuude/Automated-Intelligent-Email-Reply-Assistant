using System.Text.RegularExpressions;

namespace AIERA.AIEmailClient.RegularExpression;

public static partial class RegexPatterns
{
    /// <summary>
    /// Regex pattern matches 
    /// - 'Re[#]:', 
    /// - 'Re:',
    /// - 'Sv:',
    /// - 'FW:' or 
    /// - 'FWD:' 
    /// at the beginning of the string.
    /// This regex is used for detecting if an email has been replied or forwarded by looking at the email subject.
    /// </summary>
    [GeneratedRegex(@"^Re\[\d+\]:|^Re:|^FW:|^FWD:|^Sv:", RegexOptions.IgnoreCase, matchTimeoutMilliseconds: 50)]
    public static partial Regex IsRepliedEmailSubjectRegex();
}
