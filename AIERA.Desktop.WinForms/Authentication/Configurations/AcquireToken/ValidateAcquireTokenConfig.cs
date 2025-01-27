using AIERA.Desktop.WinForms.Authentication.Configurations.AcquireToken;
using Microsoft.Extensions.Options;

namespace AIERA.Desktop.WinForms.Authentication.Configurations;

[OptionsValidator]
public partial class ValidateAcquireTokenConfig : IValidateOptions<AcquireTokenConfig> { }
