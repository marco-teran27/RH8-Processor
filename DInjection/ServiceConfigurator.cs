using Microsoft.Extensions.DependencyInjection;
using Config;
using Config.Interfaces;
using Interfaces;
using Core;
using Core.Batch;
using FileDir;
using Commons.LogFile;
using Commons.Interfaces;
using Commons.LogComm;

namespace DInjection
{
    public static class ServiceConfigurator
    {
        public static IServiceCollection ConfigureServices(IServiceCollection services)
        {
            services.AddTransient<IRhinoFileDirScanner, RhinoFileDirScanner>();
            services.AddTransient<RhinoFileDirValComm>();
            services.AddTransient<ITheOrchestrator, TheOrchestrator>(provider =>
                new TheOrchestrator(
                    provider.GetService<IConfigSelUI>(),
                    provider.GetService<IConfigParser>(),
                    provider.GetService<RhinoFileDirValComm>(),
                    provider.GetService<IBatchService>(),
                    provider.GetService<IRhinoFileDirScanner>()));
            return services;
        }
    }
}