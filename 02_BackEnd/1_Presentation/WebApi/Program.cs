using InversionOfControl;
using Shared.Settings;
using WebApi.Configurations;

#region Configurações WebApllicationBuilder

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

//Obtém as configurações do appsettings.json e do ambiente
SettingApp.Start(builder.Configuration, builder.Environment.WebRootPath);

//Inicializa as configurações do IOC
Dependencies.Start(builder.Services);

//Inicializa as configurações do WebApi
builder.Initializer();

//Configura a documentação da API
builder.AddApiDocumentationSwagger();

//Configura a autenticação da API (ex: JWT, OAuth, etc.)
builder.AddAuthentication();

//Configura os HealthChecks da API e do banco de dados
builder.AddHealthChecks();

//Configura o Serilog como provedor de logs (console + banco de dados)
builder.AddLogs();

#endregion Configurações WebApllicationBuilder

#region Configurações WebApplication

WebApplication app = builder.Build();

//Configura as repostas globais da API (ex: tratamento de erros, formatação de respostas, etc.)
app.AddGlobalResponses();

//Expõe a documentação OpenAPI em /openapi/v1.json (apenas em desenvolvimento)
app.UseApiDocumentationSwagger();

//Inicializa as configurações do WebApi (middlewares, endpoints, etc.)
app.Initializer();

//Mapeia os endpoints de HealthCheck (/health e /health-sql)
app.UseHealthChecks();

#endregion Configurações WebApplication

//Inicia a aplicação em modo assíncrono
await app.RunAsync();
