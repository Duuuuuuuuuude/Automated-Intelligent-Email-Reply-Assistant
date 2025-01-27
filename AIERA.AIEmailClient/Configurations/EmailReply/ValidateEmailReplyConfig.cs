using Microsoft.Extensions.Options;

namespace AIERA.AIEmailClient.Configurations.EmailReply;


[OptionsValidator]
public partial class ValidateEmailReplyConfig : IValidateOptions<EmailReplyConfig> { }