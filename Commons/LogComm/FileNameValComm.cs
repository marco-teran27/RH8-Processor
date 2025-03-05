using Interfaces;
using Commons.Params;

namespace Commons.LogComm
{
    public class FileNameValComm
    {
        private readonly IRhinoCommOut _rhinoCommOut;
        private readonly IBatchService _batchService;

        public FileNameValComm(IRhinoCommOut rhinoCommOut, IBatchService batchService)
        {
            _rhinoCommOut = rhinoCommOut;
            _batchService = batchService;
        }

        public bool LogValidationAndScanResults(IFileNameValResults result, int matchedCount, int expectedCount)
        {
            _rhinoCommOut.ShowMessage("\nFILE DIR");

            foreach (var message in result.Messages)
            {
                bool isError = message.Contains("missing") || message.Contains("invalid");
                string cleanMessage = message.Contains("file_dir") && !isError ?
                    $"file_dir: {BatchDir.Instance.FileDir.Split(Path.DirectorySeparatorChar).Last()}" :
                    message;
                _rhinoCommOut.ShowMessage(isError ? $"ERROR: {cleanMessage}" : cleanMessage);
            }

            if (result.IsValid)
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