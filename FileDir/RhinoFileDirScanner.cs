using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Commons.Interfaces;
using Commons.Logging;
using Commons.Params;
using Commons.Utils;
using Interfaces;

namespace FileDir
{
    public class RhinoFileDirScanner : IRhinoFileDirScanner
    {
        private readonly IRhinoCommOut _rhinoCommOut;

        public RhinoFileDirScanner(IRhinoCommOut rhinoCommOut)
        {
            _rhinoCommOut = rhinoCommOut ?? throw new ArgumentNullException(nameof(rhinoCommOut));
        }

        public async Task ScanAsync(CancellationToken ct)
        {
            try
            {
                string fileDir = BatchDir.Instance.FileDir;
                if (string.IsNullOrEmpty(fileDir) || !Directory.Exists(fileDir))
                {
                    _rhinoCommOut.ShowError($"File directory '{fileDir}' is invalid or inaccessible.");
                    return;
                }

                var uniqueIds = PIDList.Instance.GetUniqueIds();
                if (!uniqueIds.Any())
                {
                    _rhinoCommOut.ShowError("No unique IDs found in PIDList.");
                    return;
                }

                var subdirs = Directory.GetDirectories(fileDir, "*", SearchOption.TopDirectoryOnly);
                var matchedFiles = new List<string>();
                var idStatuses = new Dictionary<string, bool>();

                foreach (var id in uniqueIds)
                    idStatuses[id] = false;

                foreach (var subdir in subdirs)
                {
                    if (ct.IsCancellationRequested) break;

                    var files = Directory.GetFiles(subdir, "*.3dm", SearchOption.TopDirectoryOnly);
                    foreach (var file in files)
                    {
                        string fileName = Path.GetFileName(file);
                        var segments = fileName.Split('-').Select(s => s.Replace(".3dm", "")).ToList();

                        foreach (var id in uniqueIds)
                        {
                            if (id == "*.3dm")
                            {
                                matchedFiles.Add(file);
                                idStatuses[id] = true;
                                break;
                            }

                            var idSegments = id.Split('-').Select(s => s.Replace(".3dm", "")).ToList();
                            bool isMatched = false;

                            if (idSegments.Count == 1) // Keyword only (e.g., "damold")
                            {
                                if (segments.Any(s => s == idSegments[0]))
                                    isMatched = true;
                            }
                            else if (idSegments.Count >= 3 && idSegments[1] == "*") // Wildcard ID (e.g., "300051L-*-R25437")
                            {
                                bool prefixMatch = segments.Any(s => Regex.IsMatch(s, $"^{idSegments[0]}"));
                                bool suffixMatch = segments.Any(s => s == idSegments[2]);
                                if (prefixMatch && suffixMatch)
                                    isMatched = true;
                            }
                            else if (idSegments.Count >= 3) // Full ID (e.g., "300000L-damold-S12345")
                            {
                                bool prefixMatch = segments.Any(s => Regex.IsMatch(s, $"^{idSegments[0]}"));
                                bool keywordMatch = segments.Any(s => s == idSegments[1]);
                                bool suffixMatch = segments.Any(s => s == idSegments[2]);
                                if (prefixMatch && keywordMatch && suffixMatch)
                                    isMatched = true;
                            }

                            if (isMatched)
                            {
                                matchedFiles.Add(file);
                                idStatuses[id] = true;
                            }
                        }
                    }
                }

                // Above: foreach (var subdir in subdirs), file matching logic
                RhinoFileNameList.Instance.SetFiles(matchedFiles);
                RhinoFileNameListLog.Instance.SetFiles(idStatuses.Select(kvp => new RhinoFileStatus(kvp.Key, kvp.Value)));
                /// Updated: Removed parsed output—let TheOrchestrator handle it
                // _rhinoCommOut.ShowMessage($"RHINO FILE DIR\nparsed {matchedFiles.Count} of {(uniqueIds.First() == "*.3dm" ? matchedFiles.Count : uniqueIds.Count)} matched");
            }
            catch (Exception ex)
            {
                _rhinoCommOut.ShowError($"Pre-parsing failed: {ex.Message}");
            }
            await Task.CompletedTask;
        }
    }
}