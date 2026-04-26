namespace Shared.Settings
{
    public class SettingAppServices
    {
        public SettingAppServices()
        {
            ViaCep = new SettingAppServicesViaCep();
        }

        public SettingAppServicesViaCep ViaCep { get; set; }
    }

    public class SettingAppServicesViaCep
    {
        public SettingAppServicesViaCep()
        {
        }

        public string BaseUrl { get; set; }
    }
}