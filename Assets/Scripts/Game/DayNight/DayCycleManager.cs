using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using External.Network;
using External.Util;
using Game.Citizens.States;
using Game.Storage;
using UnityEngine;
using UnityEngine.Events;

namespace Game.DayNight
{
    public class DayCycleManager: MonoBehaviour
    {
        public static DayCycleManager Instance { get; private set; }
        
        [SerializeField]
        [Range(0, 1)]
        private float time;

        [SerializeField]
        private float dayDuration = 10f; // night duration is two times shorter

        [SerializeField]
        private AnimationCurve timeSpeed;
        [SerializeField]
        private AnimationCurve sunIntensity;

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

        [SerializeField]
        private Light colorLight;

        private Light _activeLight;

        private static readonly int HorizonColor = Shader.PropertyToID("_HorizonColor");
        private static readonly int SkyColor = Shader.PropertyToID("_SkyColor");

        private float _tickBuffer = 0;

        public float bufferSeconds = 1f;
        public bool doBuffer = true;
        private static readonly int StarBrightness = Shader.PropertyToID("_StarBrightness");

        public int days;

        private void Awake()
        {
            Instance = this;
        }

        private float _oldTime;
        private int _oldHour;

        public UnityEvent onDayChanged = new();
        public UnityEvent onHourPassed = new();

        public void Update()
        {
            _tickBuffer += Time.deltaTime;
            if (!(_tickBuffer > bufferSeconds || !doBuffer)) return;
            _tickBuffer = 0f;
            
            var bufferFactor = doBuffer ? bufferSeconds : Time.deltaTime;
            
            var cycleTime = dayDuration * 1.5f;
            _tickedTime += bufferFactor / cycleTime;
            _tickedTime %= 0.75f;
            var passageSpeed = time % 1f > 0.5f ? 2f : 1f;
            _secondsTime = (_secondsTime + passageSpeed * bufferFactor) % cycleTime;
            time = _secondsTime / cycleTime;
            if (_oldTime > time)
            {
                // new day!
                onDayChanged.Invoke();
                days += 1;
            }

            var newHour = Mathf.RoundToInt(time * 24);
            if (newHour != _oldHour)
            {
                onHourPassed.Invoke();
                _oldHour = newHour;
            }

            _oldTime = time;

            var lc = lightColor.Evaluate(time);
            mainLight.color = lightColor.Evaluate(time);
            mainLight.transform.rotation = Quaternion.Euler(new Vector3(360f * time, -45f, 0f));
            mainLight.intensity = sunIntensity.Evaluate(time);
            
            RenderSettings.ambientLight = lc;
            
            // TODO: set shader material color shit
            skyboxMaterial.SetColor(HorizonColor, horizonColor.Evaluate(time));
            skyboxMaterial.SetColor(SkyColor, skyColor.Evaluate(time));
            skyboxMaterial.SetFloat(StarBrightness, Mathf.Lerp(0f, 2f, Mathf.Clamp(time - 0.5f, 0f, 0.5f) * 4f));
        }

        public int DayTimeMinutes()
        {
            return Mathf.RoundToInt(time * 24 * 60);
        }

        public (int, int) DayTime()
        {
            var minutes = DayTimeMinutes();
            if (time > 0.5f)
            {
                var remappedTime = Mathu.Remap(time, 0.5f, 1f, 0f, 0.5f);
                var operatedMinutes = Mathf.RoundToInt(minutes * 1.5f - minutes * remappedTime);
                return (operatedMinutes / 60, operatedMinutes % 60);
            }
            else
            {
                var operatedMinutes = Mathf.RoundToInt(minutes * 1.5f);
                return (operatedMinutes / 60, operatedMinutes % 60);
            }
        }

        public void Load()
        {
            if (!FileManager.Instance.Storage.FileExists("env.dat"))
                return;

            var str = FileManager.Instance.Storage.ReadFileBytes("env.dat");
            var fmt = new BinaryFormatter();
            var data = (StoredEnvData) fmt.Deserialize(str.BaseStream);
            _secondsTime = data.secondsTime;
            _tickedTime = data.tickedTime;
            _tickBuffer = data.tickBuffer;
            days = data.days;
        }

        public void Save()
        {
            var fmt = new BinaryFormatter();
            using var memStream = new MemoryStream();
            fmt.Serialize(memStream, new StoredEnvData
            {
                tickBuffer = _tickBuffer,
                tickedTime = _tickedTime,
                secondsTime = _secondsTime,
                days = days
            });
            FileManager.Instance.Storage.SaveBytes("env.dat", memStream.GetBuffer(), true);
        }
    }

    [Serializable]
    public class StoredEnvData
    {
        public float tickBuffer;
        public float tickedTime;
        public float secondsTime;
        public int days;
    }
}