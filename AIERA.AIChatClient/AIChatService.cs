using MimeKit;

namespace AIERA.AIChatClient;

public class AIChatService
{
    // TODO: Implement GetAIEmailResponse.
    public async Task<string> GetAIEmailResponseAsync(IMimeMessage originalEmailMessage, CancellationToken cancellationToken = default)
    {
        await Task.Delay(1000, cancellationToken).ConfigureAwait(false);

        return $"Denne email vil i fremtiden være automatisk generet af en AI. - {originalEmailMessage.Date.DateTime.ToLocalTime()}";
    }
}
