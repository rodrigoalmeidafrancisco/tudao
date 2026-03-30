using Data.Context;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace WebApi.Configurations
{
    public static class ConfigWebApiHealthCheck
    {
        extension(WebApplicationBuilder builder)
        {
            public void AddHealthChecks()
            {
                builder.Services.AddHealthChecks()

                    // Verifica a disponibilidade da própria API (sem dependências externas)
                    .AddCheck("api", () => HealthCheckResult.Healthy("API operacional"), tags: ["api"])

                    // Verifica a conectividade com o banco de dados via DbContext
                    .AddDbContextCheck<DefaultContext>("sql-server", tags: ["sql"]);
            }
        }

        extension(WebApplication app)
        {
            public void UseHealthChecks()
            {
                // Endpoint de saúde da API — retorna status da aplicação
                app.MapHealthChecks("/health", new HealthCheckOptions
                {
                    Predicate = check => check.Tags.Contains("api"),
                    ResultStatusCodes =
                    {
                        [HealthStatus.Healthy]   = StatusCodes.Status200OK,
                        [HealthStatus.Degraded]  = StatusCodes.Status200OK,
                        [HealthStatus.Unhealthy] = StatusCodes.Status503ServiceUnavailable
                    }
                });

                // Endpoint de saúde do banco de dados SQL Server
                app.MapHealthChecks("/health-sql", new HealthCheckOptions
                {
                    Predicate = check => check.Tags.Contains("sql"),
                    ResultStatusCodes =
                    {
                        [HealthStatus.Healthy]   = StatusCodes.Status200OK,
                        [HealthStatus.Degraded]  = StatusCodes.Status200OK,
                        [HealthStatus.Unhealthy] = StatusCodes.Status503ServiceUnavailable
                    }
                });
            }
        }
    }
}
