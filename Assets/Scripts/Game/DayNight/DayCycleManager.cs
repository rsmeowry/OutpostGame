﻿using System;
using Game.Citizens.States;
using UnityEngine;

namespace Game.DayNight
{
    public class DayCycleManager: MonoBehaviour
    {
        [SerializeField]
        [Range(0, 1)]
        private float time;

        [SerializeField]
        private float dayDuration = 10f; // night duration is two times shorter

        [SerializeField]
        private AnimationCurve timeSpeed;

        [SerializeField] [GradientUsage(true)] 
        private Gradient horizonColor;
        [SerializeField] [GradientUsage(true)] 
        private Gradient skyColor;

        [SerializeField]
        private Material skyboxMaterial;

        [SerializeField]
        private Gradient lightColor;
        
        // Simple ticker that constantly increments
        private float _tickedTime;
        
        // time of the clock in SECONDS. Can not exceed the cycle duration (day duration + night duration)
        private float _secondsTime;

        [SerializeField]
        private Light mainLight;

        private static readonly int HorizonColor = Shader.PropertyToID("_HorizonColor");
        private static readonly int SkyColor = Shader.PropertyToID("_SkyColor");

        private float _tickBuffer = 0;

        public float bufferSeconds = 1f;
        public bool doBuffer = true;
        private static readonly int StarBrightness = Shader.PropertyToID("_StarBrightness");

        public void Update()
        {
            _tickBuffer += Time.deltaTime;
            if (!(_tickBuffer > bufferSeconds || !doBuffer)) return;
            _tickBuffer = 0f;
            
            var bufferFactor = doBuffer ? bufferSeconds : Time.deltaTime;
            
            var cycleTime = dayDuration * 1.5f;
            _tickedTime += bufferFactor / cycleTime;
            _tickedTime %= 0.75f;
            _secondsTime = (_secondsTime + (1 + timeSpeed.Evaluate(_tickedTime)) * bufferFactor) % cycleTime;
            time = _secondsTime / cycleTime;

            mainLight.color = lightColor.Evaluate(time);
            mainLight.transform.rotation = Quaternion.Euler(new Vector3(360f * time, -45f, 0f));
            
            // TODO: set shader material color shit
            skyboxMaterial.SetColor(HorizonColor, horizonColor.Evaluate(time));
            skyboxMaterial.SetColor(SkyColor, skyColor.Evaluate(time));
            skyboxMaterial.SetFloat(StarBrightness, Mathf.Lerp(0f, 2f, Mathf.Clamp(time - 0.5f, 0f, 0.5f) * 4f));
        }
    }
}