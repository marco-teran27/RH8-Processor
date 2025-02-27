using Rhino;
using Rhino.Commands;
using Interfaces;
using DInjection;
using Microsoft.Extensions.DependencyInjection;
using Core.Batch;
using System.Threading;
using System;

namespace RhinoInt
{
    public class BatchProcessorStart : Command
    {
        private readonly ITheOrchestrator _orchestrator;
        private static IServiceProvider _serviceProvider;

        public BatchProcessorStart()
        {
            _serviceProvider ??= InitializeServices();
            _orchestrator = _serviceProvider.GetService<ITheOrchestrator>();
        }

        internal BatchProcessorStart(ITheOrchestrator orchestrator)
        {
            _orchestrator = orchestrator ?? throw new ArgumentNullException(nameof(orchestrator));
        }

        private static IServiceProvider InitializeServices()
        {
            var services = new ServiceCollection();
            ServiceConfigurator.ConfigureServices(services);
            services.AddTransient<IRhinoCommOut, RhinoCommOut>();
            services.AddTransient<IRhinoBatchServices, RhinoBatchServices>();
            services.AddTransient<IRhinoScriptServices, RhinoScriptServices>();
            return services.BuildServiceProvider();
        }

        public override string EnglishName => "BatchProcessor";

        protected override Result RunCommand(RhinoDoc doc, RunMode mode)
        {
            RhinoApp.WriteLine("Entering RunCommand...");
            try
            {
                var task = _orchestrator.RunBatchAsync(null, CancellationToken.None);
                task.Wait();
                bool success = task.Result;
                RhinoApp.WriteLine("RunBatchAsync completed.");
                return success ? Result.Success : Result.Failure;
            }
            catch (Exception ex)
            {
                RhinoApp.WriteLine($"Error in BatchProcessor: {ex.Message}");
                return Result.Failure;
            }
        }
    }
}