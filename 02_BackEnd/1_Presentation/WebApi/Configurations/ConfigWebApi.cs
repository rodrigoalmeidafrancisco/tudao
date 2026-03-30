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
            public void Initializer()
            {
                // Habilita a exibição de informações de identificação pessoal (PII) nos logs do Identity Model
                IdentityModelEventSource.ShowPII = true;

                // Configura a integração com IIS
                builder.WebHost.UseIISIntegration();

                // Verifica se o proxy está habilitado nas configurações
                if (SettingApp.Parameters.Proxy.Enable)
                {
                    // Monta a URL do host do proxy com URI e porta
                    string proxyHost = $"{SettingApp.Parameters.Proxy.Uri}:{SettingApp.Parameters.Proxy.Port}";

                    // Configura o proxy padrão para todas as requisições HttpClient
                    HttpClient.DefaultProxy = new WebProxy(new Uri(proxyHost), true, SettingApp.Parameters.Proxy.ByPass)
                    {
                        // Não utiliza as credenciais padrão do sistema
                        UseDefaultCredentials = false,
                        // Define as credenciais do cache padrão
                        Credentials = CredentialCache.DefaultCredentials
                    };
                }

                builder.Services.AddControllers().AddJsonOptions(options =>
                {
                    // Define a política de nomenclatura como camelCase (ex: nomePropriedade)
                    options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;

                    // Formata o JSON com identação para melhor legibilidade
                    options.JsonSerializerOptions.WriteIndented = true;

                    // Ignora ciclos de referência para evitar exceções de serialização
                    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;

                    // Ignora propriedades com valor null durante a serialização
                    options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
                });

                // Adiciona suporte para exploração de endpoints da API
                builder.Services.AddEndpointsApiExplorer();

                // Configura uma política CORS permissiva que aceita qualquer origem, método e cabeçalho
                builder.Services.AddCors(x => x.AddPolicy("AllowAll", y =>
                {
                    y.AllowAnyOrigin()   // Permite requisições de qualquer origem
                     .AllowAnyMethod()   // Permite qualquer método HTTP (GET, POST, PUT, DELETE, etc.)
                     .AllowAnyHeader();  // Permite qualquer cabeçalho HTTP
                }));

                //Configura o comportamento padrão da API
                builder.Services.Configure<ApiBehaviorOptions>(options =>
                {
                    // Suprime o filtro automático de validação do ModelState para controle manual
                    options.SuppressModelStateInvalidFilter = true;
                });

                //Configura a compressão de respostas HTTP
                builder.Services.AddResponseCompression(options =>
                {
                    // Adiciona o provedor de compressão Gzip
                    options.Providers.Add<GzipCompressionProvider>();

                    // Define os tipos MIME que devem ser comprimidos (padrões + application/json)
                    options.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(["application/json"]);
                });
            }
        }

        extension(WebApplication app)
        {
            public void Initializer()
            {
                // Habilita o uso de arquivos padrão (index.html, default.html, etc.)
                app.UseDefaultFiles();

                // Habilita o serviço de arquivos estáticos da pasta wwwroot
                app.UseStaticFiles();

                // Configuração de Ambiente (Development/Production)
                if (app.Environment.IsDevelopment())
                {
                    // Em desenvolvimento: exibe página detalhada de erros (MapOpenApi() é chamado via UseApiDocumentation())
                    app.UseDeveloperExceptionPage();
                }
                else
                {
                    // Em produção: habilita HTTP Strict Transport Security para maior segurança
                    app.UseHsts();
                }

                // Habilita o roteamento de requisições
                app.UseRouting();

                // Força o redirecionamento de HTTP para HTTPS
                app.UseHttpsRedirection();

                // Aplica a política CORS permitindo qualquer origem, método e cabeçalho
                app.UseCors(x => x.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());

                // Habilita a autenticação de usuários
                app.UseAuthentication();

                // Habilita a autorização baseada em roles/claims
                app.UseAuthorization();

                // Mapeia os controllers para os endpoints da API
                app.MapControllers();
            }
        }

    }
}
