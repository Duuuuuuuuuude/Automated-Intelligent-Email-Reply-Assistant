using Microsoft.Identity.Client;
using Polly;
using Polly.Retry;
using System.Diagnostics;
using System.Net.Http.Headers;

namespace AIERA.Desktop.WinForms.Authentication.Polly;


public class PollyRetryLogic
{
    /// <summary>
    /// Retry policy for any <see cref="MsalException"/> marked as retryable and any <see cref="HttpRequestException"/>.
    /// </summary>
    /// <param name="retryIntervals"></param>
    /// <returns></returns>
    public static AsyncRetryPolicy GetMsalRetryPolicy(params TimeSpan[]? retryIntervals)
    {
        // Retry policy at token request level

        TimeSpan retryAfter = TimeSpan.Zero;

        if (retryIntervals is null || retryIntervals.Length is 0)
        {
#if DEBUG
            retryIntervals = [TimeSpan.FromSeconds(1),];
#else
            retryIntervals = [ TimeSpan.FromSeconds(2),
                               TimeSpan.FromSeconds(5),
                               TimeSpan.FromSeconds(10), ];
#endif
        }

        var retryPolicy = Policy.Handle<Exception>(ex =>
        {
            return IsMsalRetryableException(ex, out retryAfter);
        }).WaitAndRetryAsync(
                             // simple retry 0s, 3s, 5s + and "retry after" hint from the server
                             new[] { retryAfter }.Concat(retryIntervals),
                             onRetry: (ex, ts) =>
            // Do some logging
            Debug.WriteLine($"MSAL call failed. Trying again after {ts}. Exception was {ex}")); // LOG

        return retryPolicy;
    }

    /// <summary>
    ///  Retry any MsalException marked as retryable - see <see cref="MsalException.IsRetryable"/> and <see cref="HttpRequestException"/>
    ///  If <see cref="HttpResponseHeaders.RetryAfter"/> header is present on <see cref="MsalServiceException"/>, return the value.
    /// </summary>
    /// <remarks>
    /// In MSAL 4.47.2 <see cref="MsalException.IsRetryable"/> includes HTTP 408, 429 and 5xx Azure AD errors but may be expanded to transient Azure AD errors in the future. 
    /// </remarks>
    private static bool IsMsalRetryableException(Exception ex, out TimeSpan retryAfter)
    {
        retryAfter = TimeSpan.Zero;

        if (ex is HttpRequestException)
            return true;

        if (ex is MsalException msalException && msalException.IsRetryable)
        {
            if (msalException is MsalServiceException msalServiceException)
            {
                retryAfter = GetRetryAfterValue(msalServiceException.Headers);
            }

            return true;
        }

        return false;
    }

    private static TimeSpan GetRetryAfterValue(HttpResponseHeaders headers)
    {
        var date = headers?.RetryAfter?.Date;
        if (date.HasValue)
        {
            return date.Value - DateTimeOffset.Now;
        }

        var delta = headers?.RetryAfter?.Delta;
        if (delta.HasValue)
        {
            return delta.Value;
        }

        return TimeSpan.Zero;
    }
}
