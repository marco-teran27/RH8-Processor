using Microsoft.Extensions.DependencyInjection;
using Interfaces;

namespace DInjection
{
    public static class ServiceConfigurator
    {
        public static IServiceCollection ConfigureServices(IServiceCollection services)
        {
            // Only register non-Rhino dependencies here
            services.AddTransient<IConfigSelector, Config.ConfigSelUI>();
            services.AddTransient<IConfigParser, Config.PlaceholderConfigParser>();
            services.AddTransient<ITheOrchestrator, Core.TheOrchestrator>();
            return services;
        }
    }
}