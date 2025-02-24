using Rhino;
using Rhino.Commands;
using Interfaces;
using DInjection;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace RhinoInt
{
    public class TestCommand : Command
    {
        private readonly IConfigSelector _configSelector;
        private readonly IRhinoService _rhinoService; // Add this

        public TestCommand()
        {
            var serviceProvider = ServiceConfigurator.ConfigureServices();
            _configSelector = serviceProvider.GetService<IConfigSelector>();
            _rhinoService = new RhinoService(); // Temporary, will use DI later
        }

        internal TestCommand(IConfigSelector configSelector, IRhinoService rhinoService)
        {
            _configSelector = configSelector ?? throw new ArgumentNullException(nameof(configSelector));
            _rhinoService = rhinoService ?? throw new ArgumentNullException(nameof(rhinoService));
        }

        public override string EnglishName => "TestCommand";

        protected override Result RunCommand(RhinoDoc doc, RunMode mode)
        {
            try
            {
                string configFilePath = _configSelector.SelectConfigFile();
                if (string.IsNullOrEmpty(configFilePath))
                {
                    RhinoApp.WriteLine("No config file selected. Command aborted.");
                    return Result.Cancel;
                }

                RhinoApp.WriteLine($"Selected config file: {configFilePath}");
                _rhinoService.RunTestCommand(); // Use the service
                return Result.Success;
            }
            catch (Exception ex)
            {
                RhinoApp.WriteLine($"Error in TestCommand: {ex.Message}");
                return Result.Failure;
            }
        }
    }
}