using System.Collections.Generic;
using Interfaces;

namespace Commons.Logging
{
    public static class ConfigValLog
    {
        public static void LogValidationResults(ConfigValidationResults results, IRhinoCommOut rhinoCommOut)
        {
            foreach (var result in results.ValidatorResults)
            {
                if (result.IsValid)
                    rhinoCommOut.ShowMessage($"{result.ValidatorName}: {result.Message}");
                else
                    rhinoCommOut.ShowError($"{result.ValidatorName}: {result.Message}");
            }

            if (results.IsValid)
                rhinoCommOut.ShowMessage("All validations passed.");
            else
                rhinoCommOut.ShowError("Validation failed. Please address the issues above.");
        }
    }
}