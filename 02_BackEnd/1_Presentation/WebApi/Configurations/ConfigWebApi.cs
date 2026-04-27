using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.IdentityModel.Logging;
using Shared.Settings;
using System.Net;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace WebApi.Configurations
{
    public static class ConfigWebApi
    {
        extension(WebApplicationBuilder builder)
        {
            public void AddConfigWebApi()
            {
                //habilitar a visualização de logs de PII
                IdentityModelEventSource.ShowPII = builder.Environment.IsDevelopment();

                //Configura para utilizar o IIS, quando publicar.
                builder.WebHost.UseIISIntegration();

                //Configura para exibir os logs no console ao debugar a aplicação.
                builder.Logging.AddConsole();

                //Configurando o proxy
                if (SettingApp.Parameters.Proxy.Enable)
                {
                    HttpClient.DefaultProxy = new WebProxy(new Uri(SettingApp.Parameters.Proxy.UriString), true, SettingApp.Parameters.Proxy.ByPassArray)
                    {
                        UseDefaultCredentials = false,
                        Credentials = CredentialCache.DefaultCredentials
                    };
                }

                // Configura os parâmetros do System.Text.Json para o retorno da API
                builder.Services.AddControllers(options =>
                {
                    options.Filters.Add(new ProducesAttribute("application/json"));
                }).AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
                    options.JsonSerializerOptions.WriteIndented = true;
                    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
                    options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
                });

                builder.Services.AddEndpointsApiExplorer();
                builder.Services.AddCors(x => x.AddPolicy("AllowAll", y => { y.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader(); }));

                //Permite fazer a validação do ComponentModel.Annotations
                builder.Services.Configure<ApiBehaviorOptions>(options =>
                {
                    options.SuppressModelStateInvalidFilter = true;
                });

                //Comprime o Json no Retorno da API, diminuindo o seu tamanho
                builder.Services.AddResponseCompression(options =>
                {
                    options.Providers.Add<GzipCompressionProvider>();
                    options.Providers.Add<BrotliCompressionProvider>(); // Melhor que Gzip
                    options.EnableForHttps = true; // Se usar HTTPS
                });

                builder.Services.Configure<GzipCompressionProviderOptions>(options =>
                {
                    options.Level = System.IO.Compression.CompressionLevel.Fastest;
                });
            }
        }

        extension(WebApplication app)
        {
            public void UseConfigWebApi()
            {
                //Força a API responder apenas em HTTPS
                app.UseHttpsRedirection();

                //Comprime o Json no Retorno da API, diminuindo o seu tamanho
                app.UseResponseCompression();

                //Informo que irei utilizar arquivos estáticos (wwwroot)
                app.UseDefaultFiles();
                app.UseStaticFiles();

                //Habilita o roteamento por padrão
                app.UseRouting();

                //Poder realizar chamadas localhost em tempo de desenvolvimento
                app.UseCors(x => x.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
                app.UseAuthentication(); // Autenticação
                app.UseAuthorization(); // Roles

                //Padrão de rotas do MVC
                app.MapControllers();

                if (app.Environment.IsDevelopment())
                {
                    app.UseDeveloperExceptionPage();
                }
                else
                {
                    app.UseHsts();
                }
            }
        }
    }
}
