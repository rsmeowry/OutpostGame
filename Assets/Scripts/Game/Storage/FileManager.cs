using System;
using External.Storage;
using UnityEngine;

namespace Game.Storage
{
    public class FileManager: MonoBehaviour
    {
        public static FileManager Instance { get; private set; }
        
        public IStorage Storage { get; private set; }

        public void Awake()
        {
            Instance = this;

            Storage = new LocalStorage();
        }
    }
}