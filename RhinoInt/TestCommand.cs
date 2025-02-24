using System;
using Rhino;
using Rhino.Commands;
using Interfaces; // Add this if missing

namespace RhinoInt
{
    public class TestCommand : Command
    {
        public override string EnglishName => "TestCommand";

        protected override Result RunCommand(RhinoDoc doc, RunMode mode)
        {
            IRhinoService service = new RhinoService(); // Implementation below
            service.RunTestCommand();
            return Result.Success;
        }
    }

    // Temporary implementation until DI is added
    public class RhinoService : IRhinoService
    {
        public void RunTestCommand()
        {
            try
            {
                RhinoApp.WriteLine("Test command executed successfully!");
            }
            catch (Exception ex)
            {
                RhinoApp.WriteLine($"Error: {ex.Message}");
            }
        }
    }
}