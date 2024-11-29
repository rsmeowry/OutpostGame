using System;
using Game.DayNight;
using TMPro;
using UnityEngine;

namespace UI.Interior
{
    public class TimeTracker: MonoBehaviour
    {
        private TMP_Text _text;
        
        private void Start()
        {
            _text = GetComponent<TMP_Text>();
            TimeHandler();
            DayCycleManager.Instance.onHourPassed.AddListener(TimeHandler);
        }

        private void OnDisable()
        {
            DayCycleManager.Instance.onHourPassed.RemoveListener(TimeHandler);
        }

        private void TimeHandler()
        {
            var (hours, minutes) = DayCycleManager.Instance.DayTime();
            _text.text = $"{hours:D2}:{minutes:D2}";
        }
    }
}