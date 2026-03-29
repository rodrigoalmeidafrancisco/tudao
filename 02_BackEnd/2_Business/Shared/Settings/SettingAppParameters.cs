namespace Shared.Settings
{
    public class SettingAppParameters
    {
        public SettingAppParameters()
        {
            Policies = [];
            Proxy = new SettingAppParametersProxy();
        }

        public string KeyToken { get; set; } = "CthXvnuPMRtN9mWt2dpU5TrUaG6Z4MmSdYn8E/vTcpTNKl3SRT+gn5GwUgv/ptiqXEozblc/v3LdfxRgKFhbgg==";
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
