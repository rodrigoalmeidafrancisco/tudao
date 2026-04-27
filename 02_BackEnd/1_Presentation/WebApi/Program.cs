#region WebApplicationBuilder

using Shared.Settings;
using WebApi.Configurations;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// Obtém as configurações do appsettings.json e do ambiente, e as disponibiliza para a aplicação.
SettingApp.Start(builder.Configuration, builder.Environment.WebRootPath);

// Método de extensão para configurar o WebApplicationBuilder
builder.AddConfigWebApi();

// Método de extensão para configurar a autenticação, como JWT, políticas, etc.
builder.AddConfigAuthentication();

// Método de extensão para configurar o Swagger, como documentação da API, UI, etc.
builder.AddConfigSwagger();

#endregion WebApplicationBuilder

#region WebApplication

WebApplication app = builder.Build();

// Método de extensão para configurar o WebApplication, como middlewares, rotas, etc.
app.UseConfigWebApi();

// Método de extensão para configurar o Swagger, como middlewares, rotas, etc.
app.UseConfigSwagger();


#endregion WebApplication

await app.RunAsync();
