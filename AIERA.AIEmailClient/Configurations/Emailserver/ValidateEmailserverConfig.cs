using Microsoft.Extensions.Options;

namespace AIERA.AIEmailClient.Configurations.Emailserver;


[OptionsValidator]
public partial class ValidateEmailserverConfig : IValidateOptions<EmailserverConfig> { }