using System;
using UnityEngine;

namespace External.Util
{
    public class Persistent: MonoBehaviour
    {
        public void Awake()
        {
            DontDestroyOnLoad(gameObject);
        }
    }
}