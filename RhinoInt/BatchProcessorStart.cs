using Rhino;
using Rhino.Commands;
using Interfaces;
using DInjection;
using Microsoft.Extensions.DependencyInjection;
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
            services.AddTransient<IRhinoCommOut, RhinoCommOut>(); // Rhino-specific here
            return services.BuildServiceProvider();
        }

        public override string EnglishName => "BatchProcessor";

        protected override Result RunCommand(RhinoDoc doc, RunMode mode)
        {
            try
            {
                bool success = _orchestrator.RunBatchAsync(null, CancellationToken.None).GetAwaiter().GetResult();
                return success ? Result.Success : Result.Failure;
            }
            catch (Exception ex)
            {
                RhinoApp.WriteLine($"Error in BatchProcessor: {ex.Message}");
                return Result.Failure;
            }
        }
    }

    public class RhinoCommOut : IRhinoCommOut
    {
        public void ShowMessage(string message)
        {
            RhinoApp.WriteLine(message);
        }

        public void ShowError(string error)
        {
            RhinoApp.WriteLine($"Error: {error}");
        }
    }
}