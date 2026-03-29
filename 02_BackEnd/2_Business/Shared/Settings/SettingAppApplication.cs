namespace Shared.Settings
{
    public class SettingAppApplication
    {
        public SettingAppApplication()
        {
            Policies = [];
        }

        public string Build { get; set; }
        public string Environment { get; set; }
        public string Release { get; set; }
        public string Name { get; set; }
        public List<SettingAppApplicationPolicies> Policies { get; set; }
        public bool UseProxy { get; set; }
        public string WebUri { get; set; }
    }

    public class SettingAppApplicationPolicies
    {
        public string Name { get; set; }
        public string Scope { get; set; }
    }
}
