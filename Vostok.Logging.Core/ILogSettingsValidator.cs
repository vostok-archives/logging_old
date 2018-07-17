namespace Vostok.Logging.Core
{
    internal interface ILogSettingsValidator<in TSettings>
    {
        SettingsValidationResult TryValidate(TSettings settings);
    }
}