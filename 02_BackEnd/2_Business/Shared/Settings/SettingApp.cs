using Microsoft.Extensions.Configuration;

namespace Shared.Settings
{
    public static class SettingApp
    {
        public static void Start(IConfiguration configuration, string webRootPath)
        {
            Application = new SettingAppApplication();
            configuration.GetSection("Application").Bind(Application);

            ApplicationInsights = new SettingAppApplicationInsights();
            configuration.GetSection("ApplicationInsights").Bind(ApplicationInsights);

            ConnectionStrings = new SettingAppConnectionStrings();
            configuration.GetSection("ConnectionStrings").Bind(ConnectionStrings);

            Parameters = new SettingAppParameters();
            configuration.GetSection("Parameters").Bind(Parameters);

            Services = new SettingAppServices();
            configuration.GetSection("Services").Bind(Services);

            WebRootPath = webRootPath;
            WebRootPathImages = Path.Combine(WebRootPath, "images");
        }

        public static SettingAppApplication Application { get; set; }
        public static SettingAppApplicationInsights ApplicationInsights { get; set; }
        public static SettingAppConnectionStrings ConnectionStrings { get; set; }
        public static SettingAppParameters Parameters { get; set; }
        public static SettingAppServices Services { get; set; }
        public static string WebRootPath { get; set; }
        public static string WebRootPathImages { get; set; }
    }
}
