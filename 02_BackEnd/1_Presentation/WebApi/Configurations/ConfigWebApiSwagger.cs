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
                        Description = $"Documentação WebApi",
                    });

                    if (SettingApp.Application.Environment.Equals("PROD", StringComparison.CurrentCultureIgnoreCase) == false)
                    {
                        x.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
                        {
                            Description = "Insira o token JWT desta forma: Bearer {seu token}",
                            Name = "Authorization",
                            Scheme = "Bearer",
                            BearerFormat = "JWT",
                            In = ParameterLocation.Header,
                            Type = SecuritySchemeType.ApiKey
                        });
                    }

                    // Configuração mais robusta para documentação XML
                    try
                    {
                        var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                        var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);

                        if (File.Exists(xmlPath))
                        {
                            x.IncludeXmlComments(xmlPath);
                        }
                        else
                        {
                            // Log que o arquivo XML não foi encontrado, mas não falha
                            Console.WriteLine($"Arquivo de documentação XML não encontrado: {xmlPath}");
                        }
                    }
                    catch (Exception ex)
                    {
                        // Log do erro mas não quebra a aplicação
                        Console.WriteLine($"Erro ao carregar documentação XML: {ex.Message}");
                    }

                    // Configurações adicionais para evitar problemas com enums e tipos complexos
                    x.UseInlineDefinitionsForEnums();
                    x.DescribeAllParametersInCamelCase();

                    // Tratamento para métodos sem documentação explícita
                    x.CustomSchemaIds(type => type.FullName?.Replace("+", "."));
                });
            }
        }

        extension(WebApplication app)
        {
            public void UseConfigSwagger()
            {
                app.UseSwagger();
                app.UseSwaggerUI(x =>
                {
                    x.SwaggerEndpoint("v1/swagger.json", SettingApp.Application.Name);
                    x.DocumentTitle = SettingApp.Application.Name;
                });
            }
        }
    }
}
