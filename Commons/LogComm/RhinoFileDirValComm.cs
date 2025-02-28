using System.Linq;
using Interfaces;
using FileDir;
using System.ComponentModel.DataAnnotations;

namespace Commons.LogComm
{
    public class RhinoFileDirValComm
    {
        private readonly IRhinoCommOut _rhinoCommOut;

        public RhinoFileDirValComm(IRhinoCommOut rhinoCommOut)
        {
            _rhinoCommOut = rhinoCommOut;
        }

        public void LogValidationResults(ValidationResult result, int matchedCount, int expectedCount)
        {
            _rhinoCommOut.ShowMessage("\nRHINO FILE DIR");

            foreach (var message in result.Messages)
            {
                bool isError = message.Contains("missing") || message.Contains("invalid");
                string cleanMessage = message;

                if (message.Contains("file_dir"))
                    cleanMessage = isError ? "file_dir: ERROR NOT FOUND" : $"file_dir: {Commons.Params.BatchDir.Instance.FileDir.Split(Path.DirectorySeparatorChar).Last()}";

                _rhinoCommOut.ShowMessage(cleanMessage);
            }

            if (result.IsValid)
                _rhinoCommOut.ShowMessage($"{matchedCount} OF {expectedCount} MATCHED\nSTARTING BATCH EXECUTION");
            else
                _rhinoCommOut.ShowError("FILE DIR VALIDATION FAILED.");
        }
    }
}