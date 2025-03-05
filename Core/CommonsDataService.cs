using System;
using System.Threading;
using Interfaces;
using Commons.Params;

namespace Core
{
    public class CommonsDataService : ICommonsDataService
    {
        private readonly object _lock = new object();
        private readonly IRhinoCommOut _rhinoCommOut;

        public CommonsDataService(IRhinoCommOut rhinoCommOut)
        {
            _rhinoCommOut = rhinoCommOut ?? throw new ArgumentNullException(nameof(rhinoCommOut));
        }

        public void UpdateFromConfig(IConfigDataResults config, IConfigValResults valResults)
        {
            lock (_lock)
            {
                BatchDir.Instance.SetDirectories(config);
                PIDList.Instance.CompileIds(config);
                Reprocess.Instance.SetReprocess(config);
                ScriptPath.Instance.SetScriptPath(config);
                TimeOutMin.Instance.SetMinutes(config);
            }
        }

        public void UpdateFromFileDir(IFileNameList fileList, IFileNameValResults valResults)
        {
            lock (_lock)
            {
                if (fileList == null || fileList.MatchedFiles == null)
                {
                    _rhinoCommOut.ShowError($"DEBUG: UpdateFromFileDir received null or empty fileList at {DateTime.Now}");
                    return;
                }

                _rhinoCommOut.ShowMessage($"DEBUG: Updating RhinoFileNameList at {DateTime.Now} with {fileList.MatchedFiles.Count} matched files: {string.Join(", ", fileList.MatchedFiles.Select(Path.GetFileName))}");
                RhinoFileNameList.Instance.SetFiles(fileList);
                _rhinoCommOut.ShowMessage($"DEBUG: RhinoFileNameList updated at {DateTime.Now}. Current matched files: {string.Join(", ", RhinoFileNameList.Instance.GetMatchedFiles().Select(Path.GetFileName))}");
            }
        }
    }
}