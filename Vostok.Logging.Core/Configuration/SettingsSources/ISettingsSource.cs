namespace Vostok.Logging.Core.Configuration.SettingsSources
{
    internal interface ISettingsSource<out TSettings>
    {
        TSettings GetSettings();
    }
}