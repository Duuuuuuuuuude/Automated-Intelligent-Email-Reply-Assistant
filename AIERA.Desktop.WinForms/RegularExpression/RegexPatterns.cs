using System.Text.RegularExpressions;

namespace AIERA.Desktop.WinForms.RegularExpression;

public static partial class RegexPatterns
{

    /// <summary>
    /// Matches a base URL of any domain extension.
    /// This would match domains like This would match domains like 
    /// - https://www.freecodecamp.org, http://www.freecodecamp.org/, 
    /// - freeCodeCamp.org, 
    /// - google.co.uk, 
    /// - facebook.net, 
    /// - google.com.ng, 
    /// - google.com.in, and many other base URLs.
    /// </summary>
    /// <returns></returns>
    [GeneratedRegex(@"(https:\/\/www\.|http:\/\/www\.|https:\/\/|http:\/\/)?[a-zA-Z0-9]{2,}(\.[a-zA-Z0-9]{2,})(\.[a-zA-Z0-9]{2,})?",
                    RegexOptions.ExplicitCapture | RegexOptions.Compiled, matchTimeoutMilliseconds: 2000  /*DoS prevention*/)]
    public static partial Regex BaseUrlRegex();
}
