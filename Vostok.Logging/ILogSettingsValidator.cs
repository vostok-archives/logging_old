namespace Vostok.Logging
{
    internal interface ILogSettingsValidator<in TSettings>
    {
        SettingsValidationResult Validate(TSettings settings);
    }
}