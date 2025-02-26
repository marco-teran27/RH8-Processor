namespace Commons.Params
{
    public class BatchDir
    {
        private static readonly BatchDir _instance = new BatchDir();
        private string _fileDir;
        private string _outputDir;

        private BatchDir()
        {
            _fileDir = string.Empty;
            _outputDir = string.Empty;
        }

        public static BatchDir Instance => _instance;

        public void SetDirectories(Interfaces.IDirectorySettings directories)
        {
            _fileDir = directories.FileDir;
            _outputDir = directories.OutputDir;
        }

        public string FileDir => _fileDir;
        public string OutputDir => _outputDir;
    }
}