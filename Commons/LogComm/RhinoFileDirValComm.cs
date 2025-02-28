using System.Linq;
using Interfaces;
using Commons.Params;
using Commons.Interfaces; // Updated: For IValidationResult

namespace Commons.LogComm
{
    public class RhinoFileDirValComm
    {
        private readonly IRhinoCommOut _rhinoCommOut;
        private readonly IBatchService _batchService;
        private readonly IRhinoFileDirScanner _scanner;

        public RhinoFileDirValComm(IRhinoCommOut rhinoCommOut, IBatchService batchService, IRhinoFileDirScanner scanner)
        {
            _rhinoCommOut = rhinoCommOut;
            _batchService = batchService;
            _scanner = scanner;
        }

        public bool LogValidationAndScanResults(IValidationResult result, int matchedCount, int expectedCount)
        {
            _rhinoCommOut.ShowMessage("\nRHINO FILE DIR");

            foreach (var message in result.Messages) // Fixed: CS1061 with IValidationResult
            {
                bool isError = message.Contains("missing") || message.Contains("invalid");
                string cleanMessage = message;

                if (message.Contains("file_dir"))
                    cleanMessage = isError ? "file_dir: ERROR NOT FOUND" : $"file_dir: {BatchDir.Instance.FileDir.Split(Path.DirectorySeparatorChar).Last()}";

                _rhinoCommOut.ShowMessage(cleanMessage);
            }

            if (result.IsValid) // Fixed: CS1061 with IValidationResult
            {
                _rhinoCommOut.ShowMessage($"{matchedCount} OF {expectedCount} MATCHED\nSTARTING BATCH EXECUTION");
                return true;
            }
            else
            {
                _rhinoCommOut.ShowError("FILE DIR VALIDATION FAILED.");
                return false;
            }
        }

        public void LogCompletion(bool success)
        {
            if (success)
            {
                _rhinoCommOut.ShowMessage("BATCH PROCESS COMPLETED SUCCESSFULLY.");
                _batchService.CloseAllFiles();
            }
            else
            {
                _rhinoCommOut.ShowError("BATCH PROCESS FAILED.");
                _batchService.CloseAllFiles();
            }
        }
    }
}