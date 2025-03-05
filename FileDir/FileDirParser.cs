using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Interfaces;

namespace FileDir
{
    public class FileDirParser : IFileDirParser
    {
        private readonly IRhinoCommOut _rhinoCommOut;
        private readonly ICommonsDataService _commonsDataService;

        public FileDirParser(IRhinoCommOut rhinoCommOut, ICommonsDataService commonsDataService)
        {
            _rhinoCommOut = rhinoCommOut ?? throw new ArgumentNullException(nameof(rhinoCommOut));
            _commonsDataService = commonsDataService ?? throw new ArgumentNullException(nameof(commonsDataService));
        }

        public async Task<(IFileNameList Data, IFileNameValResults Val)> ParseFileDirAsync(string fileDir, IReadOnlyList<string> uniqueIds, IConfigDataResults config)
        {
            _rhinoCommOut.ShowMessage($"DEBUG: Starting ParseFileDirAsync for {fileDir} at {DateTime.Now}");
            try
            {
                if (string.IsNullOrEmpty(fileDir))
                {
                    _rhinoCommOut.ShowMessage($"DEBUG: File dir empty at {DateTime.Now}");
                    return (null, new FileNameValResults(null, fileDir, new List<string> { "file_dir: missing or invalid" }));
                }

                if (!Directory.Exists(fileDir))
                {
                    _rhinoCommOut.ShowMessage($"DEBUG: File dir {fileDir} not found at {DateTime.Now}");
                    return (null, new FileNameValResults(null, fileDir, new List<string> { $"file_dir '{fileDir}': missing or invalid" }));
                }

                _rhinoCommOut.ShowMessage($"DEBUG: Scanning directories at {DateTime.Now}");
                var matchedFiles = new List<string>();
                var subdirs = Directory.GetDirectories(fileDir, "*", SearchOption.TopDirectoryOnly).Concat(new[] { fileDir }).ToList();
                _rhinoCommOut.ShowMessage($"DEBUG: Found {subdirs.Count} directories at {DateTime.Now}");

                var allFiles = new List<string>();
                foreach (var subdir in subdirs)
                {
                    _rhinoCommOut.ShowMessage($"DEBUG: Scanning {subdir} at {DateTime.Now}");
                    var files = await Task.Run(() => Directory.GetFiles(subdir, "*.3dm").ToList());
                    allFiles.AddRange(files);
                    _rhinoCommOut.ShowMessage($"DEBUG: Found {files.Count} .3dm files in {subdir} at {DateTime.Now}");
                }

                _rhinoCommOut.ShowMessage($"DEBUG: Matching {allFiles.Count} files against {uniqueIds.Count} unique IDs at {DateTime.Now}");
                foreach (var file in allFiles)
                {
                    string fileName = Path.GetFileName(file);
                    foreach (var id in uniqueIds)
                    {
                        if (id.EndsWith(".3dm") && !id.Contains("-") && id != ".3dm")
                        {
                            string keyword = id.Replace(".3dm", "");
                            if (IsKeywordInFilename(fileName, keyword))
                            {
                                matchedFiles.Add(file);
                                break;
                            }
                        }
                        else
                        {
                            Regex pattern = GetPatternForUniqueId(id);
                            if (pattern.IsMatch(fileName))
                            {
                                matchedFiles.Add(file);
                                break;
                            }
                        }
                    }
                }

                var distinctMatches = matchedFiles.Distinct().ToList();
                _rhinoCommOut.ShowMessage($"DEBUG: Found {distinctMatches.Count} distinct matches at {DateTime.Now}");

                var data = new FileNameList(distinctMatches.AsReadOnly());
                var val = new FileNameValResults(data, fileDir);
                _rhinoCommOut.ShowMessage($"DEBUG: Updating commons data at {DateTime.Now}");
                _commonsDataService.UpdateFromFileDir(data, val);
                _rhinoCommOut.ShowMessage($"DEBUG: ParseFileDirAsync completed at {DateTime.Now}");
                return (data, val);
            }
            catch (Exception ex)
            {
                _rhinoCommOut.ShowError($"DEBUG: File directory parsing failed at {DateTime.Now}: {ex.Message}");
                return (null, new FileNameValResults(null, fileDir, new List<string> { $"File directory parsing failed: {ex.Message}" }));
            }
        }

        private static Regex GetPatternForUniqueId(string id)
        {
            var options = RegexOptions.IgnoreCase | RegexOptions.Compiled;
            if (id == ".3dm") return new Regex(@".*\.3dm$", options);
            if (id.Contains("*"))
            {
                var parts = id.Split(new[] { '*' }, StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length == 2 && parts[0].EndsWith("-") && parts[1].StartsWith("-"))
                {
                    string prefix = Regex.Escape(parts[0].TrimEnd('-'));
                    string suffix = Regex.Escape(parts[1].TrimStart('-').Replace(".3dm", ""));
                    return new Regex($"^{prefix}\\d*-.*-{suffix}\\.3dm$", options);
                }
            }
            else if (id.EndsWith(".3dm"))
            {
                var parts = id.Replace(".3dm", "").Split('-');
                if (parts.Length == 3)
                {
                    string prefix = Regex.Escape(parts[0]);
                    string keyword = Regex.Escape(parts[1]);
                    string suffix = Regex.Escape(parts[2]);
                    return new Regex($"^{prefix}\\d*-{keyword}(?:-[a-zA-Z0-9]+)?-{suffix}\\.3dm$", options);
                }
            }
            throw new ArgumentException($"Invalid unique ID format: {id}");
        }

        private static bool IsKeywordInFilename(string filename, string keyword)
        {
            if (string.IsNullOrWhiteSpace(filename) || string.IsNullOrWhiteSpace(keyword))
                return false;
            string boundedKeyword = $"-{keyword}-";
            return filename.Contains(boundedKeyword, StringComparison.OrdinalIgnoreCase) ||
                   filename.StartsWith(keyword + "-", StringComparison.OrdinalIgnoreCase) ||
                   filename.EndsWith("-" + keyword, StringComparison.OrdinalIgnoreCase);
        }
    }
}