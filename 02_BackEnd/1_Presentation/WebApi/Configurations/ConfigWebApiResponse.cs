using Microsoft.AspNetCore.Diagnostics;
using Shared.Commands._Base;

namespace WebApi.Configurations
{
    // Classe estática responsável por configurar as respostas globais da API
    public static class ConfigWebApiResponse
    {
        public static void AddGlobalResponses(this WebApplication app)
        {
            // Registra o middleware de tratamento global de exceções não capturadas
            app.UseExceptionHandler(exceptionHandlerApp =>
            {
                // Define o handler assíncrono que será executado quando uma exceção ocorrer
                exceptionHandlerApp.Run(async context =>
                {
                    // Obtém a feature que contém os detalhes da exceção e do caminho da requisição
                    var exceptionFeature = context.Features.Get<IExceptionHandlerPathFeature>();
                    // Extrai o objeto de exceção da feature (pode ser nulo se não houver erro)
                    var exception = exceptionFeature?.Error;

                    // Processa a resposta apenas se uma exceção foi de fato capturada
                    if (exception != null)
                    {
                        // Gera um identificador único para rastreabilidade do erro no banco de logs
                        var errorId = Guid.NewGuid();

                        // Obtém o logger via injeção de dependência e registra o erro no Serilog (salvo no banco de dados)
                        var loggerFactory = context.RequestServices.GetRequiredService<ILoggerFactory>();
                        var logger = loggerFactory.CreateLogger("WebApi.Errors");
                        logger.LogError(exception, "Unhandled exception [ErrorId: {ErrorId}] at {Path}", errorId, context.Request.Path);

                        // Define o status HTTP 500 (Internal Server Error) na resposta
                        context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                        // Define o tipo de conteúdo da resposta como JSON
                        context.Response.ContentType = "application/json";

                        // Cria o objeto de resposta padronizado com os dados do erro interno
                        var commandResult = new CommandResult<string>
                        {
                            // Propaga o código de status 500 no corpo da resposta
                            StatusCode = StatusCodes.Status500InternalServerError,
                            // Retorna o ID do erro gravado no banco de logs para rastreabilidade
                            ErrorId = errorId,
                            // Mensagem genérica de erro com timestamp UTC para rastreabilidade
                            Message = $"[{DateTime.UtcNow}] Ocorreu um erro interno no processamento da requisição.",
                            // Registra o caminho da requisição que gerou o erro
                            Path = context.Request.Path,
                        };

                        // Em ambiente de desenvolvimento, expõe detalhes adicionais do erro
                        if (app.Environment.IsDevelopment())
                        {
                            // Concatena a mensagem de exceção original para facilitar o diagnóstico
                            commandResult.Message += $" Detalhes: {exception.Message}";
                            // Inclui o stack trace completo para identificar a origem do erro
                            commandResult.StackTrace = exception.StackTrace;
                        }

                        // Serializa e envia o objeto de resposta como JSON ao cliente
                        await context.Response.WriteAsJsonAsync(commandResult);
                    }
                });
            });

            // Registra um handler de fallback para rotas não mapeadas (404)
            app.MapFallback(async context =>
            {
                // Define o status HTTP 404 (Not Found) na resposta
                context.Response.StatusCode = StatusCodes.Status404NotFound;
                // Define o tipo de conteúdo da resposta como JSON
                context.Response.ContentType = "application/json";

                // Cria o objeto de resposta padronizado para rota não encontrada
                var errorResponse = new CommandResult<string>
                {
                    // Propaga o código de status 404 no corpo da resposta
                    StatusCode = StatusCodes.Status404NotFound,
                    // Mensagem informativa indicando que o endpoint solicitado não existe
                    Message = "Rota não encontrada. O caminho solicitado não corresponde a nenhum endpoint válido.",
                    // Registra o caminho inválido que foi requisitado
                    Path = context.Request.Path
                };

                // Serializa e envia o objeto de resposta como JSON ao cliente
                await context.Response.WriteAsJsonAsync(errorResponse);
            });
        }
    }
}
