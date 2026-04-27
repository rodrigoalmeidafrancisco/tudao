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
        public string ByPass { get; set; }
        public string[] ByPassArray => ByPass.Split('|');
        public bool Enable { get; set; }
        public string Host { get; set; }
        public int Port { get; set; }
        public string UriString => $"{Host}:{Port}";
    }
}
