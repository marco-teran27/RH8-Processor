using System;
using Interfaces;
using Rhino;

namespace RhinoInt
{
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