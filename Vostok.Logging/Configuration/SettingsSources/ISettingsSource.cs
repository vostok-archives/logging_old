namespace Vostok.Logging.Configuration.SettingsSources
{
    internal interface ISettingsSource<out TSettings>
    {
        TSettings GetSettings();
    }
}