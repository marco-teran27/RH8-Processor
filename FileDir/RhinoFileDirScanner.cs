using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Commons;
using Commons.Interfaces;
using Commons.Logging;
using Commons.Params;

namespace FileDir
{
    public class RhinoFileDirScanner : IRhinoFileDirScanner
    {
        private readonly RhinoFileDirValidator _validator;

        public RhinoFileDirScanner(RhinoFileDirValidator validator)
        {
            _validator = validator ?? throw new ArgumentNullException(nameof(validator));
        }

        public ValidationResult Validate()
        {
            return _validator.Validate();
        }

        public async Task ScanAsync(CancellationToken ct)
        {
            try
            {
                var uniqueIds = PIDList.Instance.GetUniqueIds();
                var subdirs = Directory.GetDirectories(BatchDir.Instance.FileDir);

                var idStatuses = uniqueIds.ToDictionary(id => id, id => false);
                var matchedFiles = new List<string>();

                foreach (var subdir in subdirs)
                {
                    var allFiles = Directory.GetFiles(subdir, "*.3dm");

                    foreach (var file in allFiles)
                    {
                        string fileName = Path.GetFileName(file);

                        if (uniqueIds.Any(id => fileName.Contains(id)))
                        {
                            matchedFiles.Add(file);
                            foreach (var id in uniqueIds.Where(id => fileName.Contains(id)))
                            {
                                idStatuses[id] = true;
                            }
                        }
                    }
                }

                RhinoFileNameList.Instance.SetFiles(matchedFiles);
                RhinoFileNameListLog.Instance.SetFiles(idStatuses.Select(kvp => new RhinoFileStatus(kvp.Key, kvp.Value)));
            }
            catch (Exception ex)
            {
                // Error moved to RhinoFileDirValComm
                throw new Exception($"Pre-parsing failed: {ex.Message}");
            }
            await Task.CompletedTask;
        }
    }
}