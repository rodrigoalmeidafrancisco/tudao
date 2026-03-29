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
builder.AddApiDocumentation();

#endregion Configurações WebApllicationBuilder

#region Configurações WebApplication

WebApplication app = builder.Build();

//Configura as repostas globais da API (ex: tratamento de erros, formatação de respostas, etc.)
app.AddGlobalResponses();

//Expõe a documentação OpenAPI em /openapi/v1.json (apenas em desenvolvimento)
app.UseApiDocumentation();

//Inicializa as configurações do WebApi (middlewares, endpoints, etc.)
app.Initializer();

#endregion Configurações WebApplication

//Inicia a aplicação em modo assíncrono
await app.RunAsync();
