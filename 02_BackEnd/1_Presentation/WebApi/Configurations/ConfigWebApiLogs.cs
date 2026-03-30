using Serilog;
using Serilog.Events;
using Serilog.Sinks.MSSqlServer;
using Shared.Settings;

namespace WebApi.Configurations
{
    public static class ConfigWebApiLogs
    {
        extension(WebApplicationBuilder builder)
        {
            /// <summary>
            /// Configura o Serilog como provedor de logs da aplicação,
            /// gravando no console e na tabela AppLogs do banco de dados.
            /// </summary>
            public void AddLogs()
            {
                Log.Logger = new LoggerConfiguration()
                    .MinimumLevel.Information()
                    .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                    .MinimumLevel.Override("System", LogEventLevel.Warning)
                    .Enrich.FromLogContext()
                    .WriteTo.Console()
                    .WriteTo.MSSqlServer(
                        connectionString: SettingApp.ConnectionStrings.Default,
                        sinkOptions: new MSSqlServerSinkOptions
                        {
                            SchemaName = "Logs",
                            TableName = "Serilog",
                            AutoCreateSqlTable = true
                        })
                    .CreateLogger();

                builder.Host.UseSerilog();
            }
        }
    }
}
