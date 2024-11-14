using System;
using System.Collections;
using System.IO;
using External.Util;
using UnityEngine;

namespace External.Storage
{
    public class LocalStorage: IStorage
    {
        private static LoggerHandle _log = LoggerHandle.LogHandle<LocalStorage>();
        
        private static readonly string FilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Outpost");
        
        public LocalStorage()
        {
            if (!Directory.Exists(FilePath))
                Directory.CreateDirectory(FilePath);
        }
        
        public void SaveString(string relativePath, string data)
        {
            var path = Path.Combine(FilePath, relativePath);
            var parent = Directory.GetParent(path)!;
            if (!parent.Exists)
                parent.Create();
            using var stream = new StreamWriter(File.Create(path));
            stream.Write(data);
            stream.Flush();
        }

        public string ReadFile(string relativePath)
        {
            var path = Path.Combine(FilePath, relativePath);
            if (!File.Exists(path))
            {
                _log.Warn($"Tried to access a file that does not exist! {relativePath}");
                return null;
            }

            using var stream = new StreamReader(File.OpenRead(path));
            return stream.ReadToEnd();
        }

        public bool FileExists(string relativePath)
        {
            return File.Exists(Path.Combine(FilePath, relativePath));
        }
        
        public bool DirExists(string relativePath)
        {
            return Directory.Exists(Path.Combine(FilePath, relativePath));
        }

    }
}