using System;
using Game.DayNight;
using UnityEngine;

namespace Inside
{
    public class InsideLamp: MonoBehaviour
    {
        [SerializeField]
        private Light light;

        private void OnEnable()
        {
            if(DayCycleManager.Instance == null)
                Invoke(nameof(Register), 2);
            else Register();
        }

        private void OnDisable()
        {
            DayCycleManager.Instance.onHourPassed.RemoveListener(HandleHour);
        }

        private void Register()
        {
            DayCycleManager.Instance.onHourPassed.AddListener(HandleHour);
        }

        private void HandleHour()
        {
            var time = DayCycleManager.Instance.DayTime();
            light.gameObject.SetActive(time.Item1 > 16 || time.Item1 < 2);
        }
    }
}