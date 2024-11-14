using System.Collections;

namespace External.Storage
{
    public interface IStorage
    {
        public void SaveString(string relativePath, string data);
        public string ReadFile(string relativePath);
        public bool DirExists(string relativePath);
        public bool FileExists(string relativePath);
    }
}