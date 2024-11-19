using DG.Tweening;
using UnityEngine;

namespace Game.Tick
{
    public class TimeManager: MonoBehaviour
    {
        public static TimeManager Instance { get; private set; }

        public void Start()
        {
            Instance = this;

            DOTween.Init();
            DOTween.defaultTimeScaleIndependent = true;
        }

        [ContextMenu("Test/Speedup")]
        private void __TestChangeSpeed()
        {
            Time.timeScale = 4f;
        }
        
        [ContextMenu("Test/Slowdown")]
        private void __TestSlowDown()
        {
            Time.timeScale = 1f;
        }

    }
}