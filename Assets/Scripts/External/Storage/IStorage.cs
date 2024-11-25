using System.Collections;
using System.IO;

namespace External.Storage
{
    public interface IStorage
    {
        public void SaveString(string relativePath, string data, bool localToPlayer = false);
        public void SaveBytes(string relativePath, byte[] data, bool localToPlayer = false);
        public string ReadFile(string relativePath, bool localToPlayer = false);
        public StreamReader ReadFileBytes(string relativePath, bool localToPlayer = false);
        public bool DirExists(string relativePath, bool localToPlayer = false);
        public bool FileExists(string relativePath, bool localToPlayer = false);
        public void CreateDir(string relativePath, bool localToPlayer = false);
    }
}