using System.Diagnostics;

class Program
{
    static void Main(string[] args)
    {
        var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = @"C:\Program Files\Rhino 8\Rhino.exe",
                Arguments = "/netcore",
                UseShellExecute = true
            }
        };
        process.Start();
    }
}