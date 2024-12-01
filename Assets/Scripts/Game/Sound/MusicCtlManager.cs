using Game.DayNight;
using UnityEngine;

namespace Game.Sound
{
    public class MusicCtlManager: MonoBehaviour
    {
        public static MusicCtlManager Instance { get; private set; }

        private float moodFactorDay = 0.3f;
        private float moodFactorNight = 0.8f;
        
        private bool _isPlayingMusic;

        private float _mood;

        public void HandleHourChange()
        {
            if (_isPlayingMusic)
                return;

            _mood += DayCycleManager.Instance.DayTime().Item1;
        }
    }
}