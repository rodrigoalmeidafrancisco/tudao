using Microsoft.OpenApi;
using Shared.Settings;
using System.Reflection;

namespace WebApi.Configurations
{
    public static class ConfigWebApiSwagger
    {
        extension(WebApplicationBuilder builder)
        {
            public void AddConfigSwagger()
            {
                builder.Services.AddSwaggerGen(x =>
                {
                    x.SwaggerDoc("v1", new OpenApiInfo
                    {
                        Version = "v1",
                        Title = SettingApp.Application.Name,
                        Description = "Documentação WebApi",
                    });

                    // Define o esquema de segurança JWT para todos os ambientes
                    x.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                    {
                        Description = "Insira o token JWT desta forma: Bearer {seu token}",
                        Name = "Authorization",
                        Scheme = "Bearer",
                        BearerFormat = "JWT",
                        In = ParameterLocation.Header,
                        Type = SecuritySchemeType.Http,
                    });

                    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);

                    if (File.Exists(xmlPath))
                    {
                        x.IncludeXmlComments(xmlPath);
                    }

                    x.UseInlineDefinitionsForEnums();
                    x.DescribeAllParametersInCamelCase();
                    x.CustomSchemaIds(type => type.FullName?.Replace("+", "."));
                });
            }
        }

        extension(WebApplication app)
        {
            public void UseConfigSwagger()
            {
                // Swagger disponível apenas fora de produção
                if (app.Environment.IsProduction())
                {
                    return;
                }

                app.UseSwagger();
                app.UseSwaggerUI(x =>
                {
                    x.SwaggerEndpoint("/swagger/v1/swagger.json", SettingApp.Application.Name);
                    x.DocumentTitle = SettingApp.Application.Name;
                    x.DisplayRequestDuration();
                });
            }
        }
    }
}
