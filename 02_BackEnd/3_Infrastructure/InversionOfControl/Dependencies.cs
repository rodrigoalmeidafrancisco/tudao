using Data.Context;
using Data.UnitOfWork;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Shared.Settings;

namespace InversionOfControl
{
    public static class Dependencies
    {
        public static void Start(IServiceCollection services)
        {
            Contexts(services);
            Domain(services);
            DataRepositories(services);
            DataServices(services);
        }

        private static void Contexts(IServiceCollection services)
        {
            services.AddDbContext<DefaultContext>(options =>
            {
                options.UseSqlServer(
                    SettingApp.ConnectionStrings.Default,
                    sqlOptions =>
                    {
                        sqlOptions.EnableRetryOnFailure(maxRetryCount: 3, maxRetryDelay: TimeSpan.FromSeconds(5), errorNumbersToAdd: null);
                    }).UseLoggerFactory(LoggerFactory.Create(builder => builder.AddConsole()));
            });

            services.AddScoped<IUnitOfWork, UnitOfWork>();
        }

        private static void Domain(IServiceCollection services)
        {
            //services.AddScoped<XXX, XXX>();
        }

        private static void DataRepositories(IServiceCollection services)
        {
            //services.AddScoped<XXX, XXX>();
        }

        private static void DataServices(IServiceCollection services)
        {
            //services.AddScoped<XXX, XXX>();
        }
    }
}
