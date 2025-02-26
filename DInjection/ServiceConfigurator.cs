using Microsoft.Extensions.DependencyInjection;
using Config;
using Config.Interfaces;
using Interfaces;
using Core;
using FileDir;
using Commons.Logging;
using Commons.Interfaces;

namespace DInjection
{
    public static class ServiceConfigurator
    {
        public static IServiceCollection ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<IConfigState, ConfigState>();
            services.AddTransient<IConfigSelUI, ConfigSelUI>();
            services.AddTransient<IConfigParser, ConfigParser>();
            services.AddTransient<IFileDirScanner, FileDirScanner>();
            services.AddTransient<ITheOrchestrator, TheOrchestrator>();
            return services;
        }
    }
}