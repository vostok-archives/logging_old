using Vostok.Configuration.Abstractions.Validation;

namespace Vostok.Logging.Core
{
    internal interface ILogSettingsValidator<in TSettings> : ISettingsValidator<TSettings>
    {
        SettingsValidationResult TryValidate(TSettings settings);
    }
}