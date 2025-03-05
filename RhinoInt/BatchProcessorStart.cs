using Rhino;
using Rhino.Commands;
using Interfaces;
using DInjection;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading;

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
            ServiceConfigurator.ConfigureServices(services); // Non-Rhino services
            RhinoServiceConfigurator.ConfigureRhinoServices(services); // Rhino-specific services
            return services.BuildServiceProvider();
        }

        public override string EnglishName => "BatchProcessor";

        protected override Result RunCommand(RhinoDoc doc, RunMode mode)
        {
            RhinoApp.WriteLine($"DEBUG: Entering BatchProcessorStart.RunCommand at {DateTime.Now}");
            try
            {
                RhinoApp.WriteLine($"DEBUG: Calling RunBatch at {DateTime.Now}");
                bool success = _orchestrator.RunBatch(null, CancellationToken.None);
                RhinoApp.WriteLine($"DEBUG: RunBatch completed at {DateTime.Now} with result: {success}");
                return success ? Result.Success : Result.Failure;
            }
            catch (Exception ex)
            {
                RhinoApp.WriteLine($"DEBUG: BatchProcessor failed at {DateTime.Now}: {ex.Message}");
                return Result.Failure;
            }
        }
    }
}