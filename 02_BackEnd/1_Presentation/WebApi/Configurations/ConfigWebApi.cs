using Microsoft.IdentityModel.Logging;

namespace WebApi.Configurations
{
    public static class ConfigWebApi
    {
        extension(WebApplicationBuilder builder)
        {
            public void AddConfigWebApi()
            {
                //habilitar a visualização de logs de PII
                IdentityModelEventSource.ShowPII = true;

                //Configura para utilizar o IIS, quando publicar.
                builder.WebHost.UseIISIntegration();

                //Configura para exibir os logs no console ao debugar a aplicação.
                builder.Logging.ClearProviders().AddConsole();

                builder.Services.AddControllers();
                builder.Services.AddOpenApi();
            }
        }

        extension(WebApplication app)
        {
            public void UseConfigWebApi()
            {
                if (app.Environment.IsDevelopment())
                {
                    app.MapOpenApi();
                }

                app.UseHttpsRedirection();
                app.UseAuthorization();
                app.MapControllers();
            }
        }
    }
}
