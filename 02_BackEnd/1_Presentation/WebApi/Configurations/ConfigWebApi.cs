using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.IdentityModel.Logging;
using Shared.Settings;
using System.Net;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace WebApi.Configurations
{
    /// <summary>
    /// Classe estática responsável por configurar a API Web
    /// </summary>
    public static class ConfigWebApi
    {
        #region Configuração do Builder

        /// <summary>
        /// Inicializa as configurações do WebApplicationBuilder
        /// </summary>
        public static void Initializer(this WebApplicationBuilder builder)
        {
            // Configurações de Segurança e Logging
            ConfigureLogging(builder);

            // Configuração de Proxy (se habilitado)
            ConfigureProxy(builder);

            // Configuração de Controllers e Serialização JSON
            ConfigureControllers(builder);

            // Configuração de API Explorer
            ConfigureApiExplorer(builder);

            // Configuração de CORS
            ConfigureCors(builder);

            // Configuração de Comportamento da API
            ConfigureApiBehavior(builder);

            // Configuração de Compressão de Resposta
            ConfigureResponseCompression(builder);
        }

        /// <summary>
        /// Configura o logging e exibição de informações sensíveis (apenas para desenvolvimento)
        /// </summary>
        private static void ConfigureLogging(WebApplicationBuilder builder)
        {
            // Habilita a exibição de informações de identificação pessoal (PII) nos logs do Identity Model
            IdentityModelEventSource.ShowPII = true;

            // Configura a integração com IIS
            builder.WebHost.UseIISIntegration();

            // Remove provedores de log padrão e adiciona apenas o console
            builder.Logging.ClearProviders().AddConsole();
        }

        /// <summary>
        /// Configura o proxy HTTP se habilitado nas configurações
        /// </summary>
        private static void ConfigureProxy(WebApplicationBuilder builder)
        {
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
        }

        /// <summary>
        /// Configura os controllers e as opções de serialização JSON
        /// </summary>
        private static void ConfigureControllers(WebApplicationBuilder builder)
        {
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
        }

        /// <summary>
        /// Configura o API Explorer para documentação automática da API
        /// </summary>
        private static void ConfigureApiExplorer(WebApplicationBuilder builder)
        {
            // Adiciona suporte para exploração de endpoints da API
            builder.Services.AddEndpointsApiExplorer();
        }

        /// <summary>
        /// Configura as políticas de CORS (Cross-Origin Resource Sharing)
        /// </summary>
        private static void ConfigureCors(WebApplicationBuilder builder)
        {
            // Configura uma política CORS permissiva que aceita qualquer origem, método e cabeçalho
            builder.Services.AddCors(x => x.AddPolicy("AllowAll", y =>
            {
                y.AllowAnyOrigin()   // Permite requisições de qualquer origem
                 .AllowAnyMethod()   // Permite qualquer método HTTP (GET, POST, PUT, DELETE, etc.)
                 .AllowAnyHeader();  // Permite qualquer cabeçalho HTTP
            }));
        }

        /// <summary>
        /// Configura o comportamento padrão da API
        /// </summary>
        private static void ConfigureApiBehavior(WebApplicationBuilder builder)
        {
            builder.Services.Configure<ApiBehaviorOptions>(options =>
            {
                // Suprime o filtro automático de validação do ModelState para controle manual
                options.SuppressModelStateInvalidFilter = true;
            });
        }

        /// <summary>
        /// Configura a compressão de respostas HTTP
        /// </summary>
        private static void ConfigureResponseCompression(WebApplicationBuilder builder)
        {
            builder.Services.AddResponseCompression(options =>
            {
                // Adiciona o provedor de compressão Gzip
                options.Providers.Add<GzipCompressionProvider>();

                // Define os tipos MIME que devem ser comprimidos (padrões + application/json)
                options.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(["application/json"]);
            });
        }

        #endregion Configuração do Builder

        #region Configuração da Application

        /// <summary>
        /// Inicializa e configura o pipeline de requisições da aplicação
        /// </summary>
        public static void Initializer(this WebApplication app)
        {
            // Configuração de Arquivos Estáticos
            ConfigureStaticFiles(app);

            // Configuração de Ambiente (Development/Production)
            ConfigureEnvironment(app);

            // Configuração do Pipeline de Middleware
            ConfigureMiddlewarePipeline(app);

            // Configuração de Endpoints
            ConfigureEndpoints(app);
        }

        /// <summary>
        /// Configura o uso de arquivos estáticos e arquivos padrão
        /// </summary>
        private static void ConfigureStaticFiles(WebApplication app)
        {
            // Habilita o uso de arquivos padrão (index.html, default.html, etc.)
            app.UseDefaultFiles();

            // Habilita o serviço de arquivos estáticos da pasta wwwroot
            app.UseStaticFiles();
        }

        /// <summary>
        /// Configura middlewares específicos por ambiente (Development/Production)
        /// </summary>
        private static void ConfigureEnvironment(WebApplication app)
        {
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
        }

        /// <summary>
        /// Configura o pipeline de middleware da aplicação
        /// </summary>
        private static void ConfigureMiddlewarePipeline(WebApplication app)
        {
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
        }

        /// <summary>
        /// Configura o mapeamento de endpoints da API
        /// </summary>
        private static void ConfigureEndpoints(WebApplication app)
        {
            // Mapeia os controllers para os endpoints da API
            app.MapControllers();
        }

        #endregion Configuração da Application
    }
}
