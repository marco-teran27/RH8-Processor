using Microsoft.Extensions.DependencyInjection;
using Interfaces;
using Config;

namespace DInjection
{
    public static class ServiceConfigurator
    {
        public static IServiceProvider ConfigureServices()
        {
            var services = new ServiceCollection();
            services.AddTransient<IConfigSelector, ConfigSelUI>();
            return services.BuildServiceProvider();
        }
    }
}