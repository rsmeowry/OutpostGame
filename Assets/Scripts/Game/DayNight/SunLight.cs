using System;
using UnityEngine;

namespace Game.DayNight
{
    public class SunLight: MonoBehaviour
    {
        private static readonly int SunDirection = Shader.PropertyToID("_SunDirection");

        private void Update()
        {
            Shader.SetGlobalVector(SunDirection, transform.forward);
        }
    }
}