namespace Shared.Settings
{
    public class SettingAppParameters
    {
        public SettingAppParameters()
        {
            Proxy = new SettingAppParametersProxy();
        }

        public SettingAppParametersProxy Proxy { get; set; }
    }

    public class SettingAppParametersProxy
    {
        public bool Enable { get; set; }
        public string Host { get; set; }
        public int Port { get; set; }
    }
}
