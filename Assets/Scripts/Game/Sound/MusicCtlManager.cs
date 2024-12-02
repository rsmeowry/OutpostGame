using System;
using System.Collections.Generic;
using System.Linq;
using External.Util;
using Game.Citizens;
using Game.Controllers;
using Game.DayNight;
using Game.State;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Game.Sound
{
    public class MusicCtlManager: MonoBehaviour
    {
        public static MusicCtlManager Instance { get; private set; }

        [SerializeField]
        private MusicFadingChannel musicChannel;

        [SerializeField]
        private List<AudioClip> outsideNight;
        [SerializeField]
        private List<AudioClip> outsideDay;
        [SerializeField]
        private List<AudioClip> insideNight;
        [SerializeField]
        private List<AudioClip> insideDay;

        private float moodFactorDay = 0.2f;
        private float moodFactorNight = 0.5f;
        
        private bool _isPlayingMusic;
        
        [SerializeField]
        private float mood;

        private static readonly float ESqr = Mathf.Pow((float)Math.E, 2);

        private void Start()
        {
            DayCycleManager.Instance.onHourPassed.AddListener(HandleHourChange);
        }

        public void HandleHourChange()
        {
            if (_isPlayingMusic)
                return;

            // why is it happening??
            if (float.IsNaN(mood))
                mood = -0.5f;

            var factorRandom = Random.Range(0.6f, 1.2f);
            var factorProductivity = 1 + Mathf.Log(5 * (GameStateManager.Instance.PlayerProductDelta.Values.Sum() + 2)) / ESqr;
            var factorPopulation = 1 + Mathf.Sqrt(CitizenManager.Instance.Citizens.Count + 1) / 20f;
            var time = DayCycleManager.Instance.DayTime();
            mood += (time.Item1 > 18
                ? moodFactorNight
                : moodFactorDay) * factorRandom * factorProductivity * factorPopulation;

            if (mood >= 1f)
            {
                mood = -Random.Range(1.3f, 2.5f);
                _isPlayingMusic = true;
                var isInside = !TownCameraController.Instance.gameObject.activeSelf;
                var isNight = time.Item1 > 18;
                var track = (isInside, isNight) switch
                {
                    (false, false) => Rng.Choice(outsideDay),
                    (true, false) => Rng.Choice(insideDay),
                    (false, true) => Rng.Choice(outsideNight),
                    (true, true) => Rng.Choice(insideNight)
                };
                musicChannel.QueueCrossfade(track);
                Invoke(nameof(ResetPlaying), track.length);
            }
        }

        private void ResetPlaying()
        {
            _isPlayingMusic = false;
        }
    }
}