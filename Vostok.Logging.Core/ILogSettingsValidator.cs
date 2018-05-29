namespace Vostok.Logging.Core
{
    internal interface ILogSettingsValidator<in TSettings>
    {
        SettingsValidationResult Validate(TSettings settings);
    }
}