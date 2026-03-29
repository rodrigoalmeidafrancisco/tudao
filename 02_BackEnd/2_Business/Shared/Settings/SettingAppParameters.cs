namespace Shared.Settings
{
    public class SettingAppParameters
    {
        public SettingAppParameters()
        {
            Policies = [];
            Proxy = new SettingAppParametersProxy();
        }

        public List<SettingAppParametersPolicies> Policies { get; set; }
        public SettingAppParametersProxy Proxy { get; set; }
    }

    public class SettingAppParametersPolicies
    {
        public string Name { get; set; }
        public string Scope { get; set; }
    }

    public class SettingAppParametersProxy
    {
        public SettingAppParametersProxy()
        {
            ByPass = [];
        }

        public string[] ByPass { get; set; }
        public bool Enable { get; set; }
        public string Port { get; set; }
        public string Uri { get; set; }
    }
}
