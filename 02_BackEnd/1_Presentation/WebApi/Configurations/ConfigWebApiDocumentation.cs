using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi;
using Shared.Settings;

namespace WebApi.Configurations
{
    /// <summary>
    /// Configurações de documentação da API Web.
    /// Responsável por registrar e configurar o OpenAPI na aplicação, incluindo
    /// metadados, segurança, servidores e transformadores de operações e esquemas.
    /// </summary>
    public static class ConfigWebApiDocumentation
    {
        // Identificador do esquema de segurança JWT reutilizado nos transformadores
        private const string JwtSchemeName = "Bearer";

        extension(WebApplicationBuilder builder)
        {
            /// <summary>
            /// Registra os serviços necessários para a documentação da API via OpenAPI.
            /// </summary>
            /// <remarks>
            /// Configura metadados gerais, segurança JWT, servidores e transformadores de operações.
            /// Deve ser chamado durante a configuração dos serviços (antes do <c>Build()</c>).
            /// </remarks>
            public void AddApiDocumentation()
            {
                // Registra o explorador de endpoints necessário para o OpenAPI enumerar as rotas da API
                builder.Services.AddEndpointsApiExplorer();

                // Configura o gerador de documentação OpenAPI com todas as opções do projeto
                builder.Services.AddOpenApi(options =>
                {
                    // ─── Transformador de Documento ──────────────────────────────────────────────
                    // Executado uma vez para definir os metadados globais do documento OpenAPI
                    options.AddDocumentTransformer((document, context, cancellationToken) =>
                    {
                        // Define as informações principais exibidas no topo da documentação
                        document.Info = new()
                        {
                            // Título da API exibido na interface de documentação
                            Title = $"{SettingApp.Application.Name} WebApi",

                            // Versão atual da API
                            Version = "v1",

                            // Descrição geral da API com informações técnicas
                            Description = $"API REST moderna construída com .NET 10 seguindo as melhores práticas. " +
                                          $"Build: {SettingApp.Application.Build} | " +
                                          $"Release: {SettingApp.Application.Release} | " +
                                          $"Ambiente: {SettingApp.Application.Environment}",

                            // Informações de contato do responsável pela API
                            Contact = new OpenApiContact
                            {
                                // Nome do responsável ou equipe
                                Name = SettingApp.Application.Name,

                                // URL do portal ou repositório do projeto
                                Url = new Uri(SettingApp.Application.WebUri ?? "https://localhost")
                            },

                            // Informações de licença de uso da API
                            License = new OpenApiLicense
                            {
                                // Nome da licença
                                Name = "MIT License",

                                // URL com os termos completos da licença
                                Url = new Uri("https://opensource.org/licenses/MIT")
                            },
                        };

                        // Define os servidores disponíveis para execução das requisições na UI
                        document.Servers =
                        [
                            new OpenApiServer
                            {
                                // URL base do servidor configurada no appsettings
                                Url = SettingApp.Application.WebUri ?? "https://localhost",

                                // Descrição do ambiente de execução
                                Description = $"Servidor {SettingApp.Application.Environment}"
                            }
                        ];

                        // Garante que a seção de componentes esteja inicializada
                        document.Components ??= new OpenApiComponents();

                        // ── Esquema de Segurança JWT Bearer ──────────────────────────────────────
                        // Registra o esquema de autenticação JWT para ser referenciado nas operações
                        document.Components.SecuritySchemes[JwtSchemeName] = new OpenApiSecurityScheme
                        {
                            // Define o tipo como HTTP (portador de token no header Authorization)
                            Type = SecuritySchemeType.Http,

                            // Define o esquema padrão Bearer
                            Scheme = "bearer",

                            // Informa que o token segue o formato JWT
                            BearerFormat = "JWT",

                            // Instrução exibida na UI para o desenvolvedor inserir o token
                            Description = "Insira o token JWT obtido no endpoint de autenticação. " +
                                          "Exemplo: eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..."
                        };

                        return Task.CompletedTask;
                    });

                    // ─── Transformador de Operações ───────────────────────────────────────────────
                    // Executado para cada endpoint registrado na API
                    options.AddOperationTransformer((operation, context, cancellationToken) =>
                    {
                        // Verifica se o endpoint possui o atributo [Authorize] aplicado
                        bool requerAutenticacao = context.Description.ActionDescriptor
                            .EndpointMetadata
                            .OfType<AuthorizeAttribute>()
                            .Any();

                        if (requerAutenticacao)
                        {
                            // Garante que a lista de requisitos de segurança esteja inicializada
                            operation.Security ??= [];

                            // Adiciona o requisito de segurança JWT à operação autenticada
                            operation.Security.Add(new OpenApiSecurityRequirement
                            {
                                // Cria uma referência ao esquema JWT pelo nome e documento atual (API do Microsoft.OpenApi 2.x)
                                [new OpenApiSecuritySchemeReference(JwtSchemeName, context.Document, null)] = []
                            });

                            // Documenta a resposta 401 retornada quando o token está ausente ou inválido
                            operation.Responses.TryAdd("401", new OpenApiResponse
                            {
                                Description = "Não autorizado — token JWT ausente ou inválido."
                            });

                            // Documenta a resposta 403 retornada quando o usuário não tem permissão
                            operation.Responses.TryAdd("403", new OpenApiResponse
                            {
                                Description = "Acesso proibido — o usuário não possui a role ou claim necessária."
                            });
                        }

                        // Documenta a resposta 500 presente em todas as operações da API
                        operation.Responses.TryAdd("500", new OpenApiResponse
                        {
                            Description = "Erro interno do servidor — falha inesperada no processamento da requisição."
                        });

                        return Task.CompletedTask;
                    });
                });
            }
        }

        extension(WebApplication app)
        {
            /// <summary>
            /// Expõe o endpoint de documentação OpenAPI no pipeline da aplicação.
            /// </summary>
            /// <remarks>
            /// Em desenvolvimento, expõe o JSON do OpenAPI em <c>/openapi/v1.json</c>.
            /// Para visualização, adicione um cliente de UI como Scalar ou Swagger UI.
            /// </remarks>
            public void UseApiDocumentation()
            {
                // Expõe a documentação apenas em ambiente de desenvolvimento por segurança
                if (app.Environment.IsDevelopment())
                {
                    // Registra o endpoint que serve o documento OpenAPI em /openapi/v1.json
                    app.MapOpenApi();
                }
            }
        }
    }
}
