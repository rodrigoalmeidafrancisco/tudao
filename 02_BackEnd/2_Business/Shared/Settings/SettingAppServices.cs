namespace Shared.Settings
{
    public class SettingAppServices
    {
        public SettingAppServices()
        {
            ViaCepApi = new SettingAppServicesViaCepApi();
        }

        public SettingAppServicesViaCepApi ViaCepApi { get; set; }
    }

    public class SettingAppServicesViaCepApi
    {
        public string UriCepJson { get; set; }
    }
}
