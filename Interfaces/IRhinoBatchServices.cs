namespace Interfaces
{
    public interface IRhinoBatchServices
    {
        bool OpenFile(string filePath);
        void CloseFile();
        /// <summary>
        /// Closes all open Rhino documents—ensures no files remain open after batch completion.
        /// </summary>
        void CloseAllFiles();
    }
}