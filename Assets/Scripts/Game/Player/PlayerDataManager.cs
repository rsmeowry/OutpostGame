using System;
using UnityEngine;

namespace Game.Player
{
    public class PlayerDataManager: MonoBehaviour
    {
        public static PlayerDataManager Instance { get; private set; }

        public string playerName;

        public void Awake()
        {
            Instance = this;
        }
    }
}