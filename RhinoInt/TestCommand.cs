using System;
using Microsoft.Extensions.DependencyInjection;
using Rhino;
using Rhino.Commands;
using Interfaces;
using DInjection; // Add this

namespace RhinoInt
{
    public class TestCommand : Command
    {
        private readonly IConfigSelector _configSelector;

        // Constructor injection
        public TestCommand()
        {
            var serviceProvider = ServiceConfigurator.ConfigureServices();
            _configSelector = serviceProvider.GetService<IConfigSelector>();
        }

        // For testing, allow injection override
        internal TestCommand(IConfigSelector configSelector)
        {
            _configSelector = configSelector ?? throw new ArgumentNullException(nameof(configSelector));
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