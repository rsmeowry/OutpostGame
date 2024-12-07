using System;
using DG.Tweening;
using UnityEngine;

namespace Game.Tick
{
    public class TimeManager: MonoBehaviour
    {
        public static TimeManager Instance { get; private set; }

        public int gameSpeed = 1;

        public void Start()
        {
            Instance = this;

            DOTween.Init();
            DOTween.defaultTimeScaleIndependent = true;
        }
        
        public void ChangeGameSpeed(int newSpeed)
        {
            if (gameSpeed != 0 && newSpeed == 0)
            {
                Physics.simulationMode = SimulationMode.Script;
            } else if (gameSpeed == 0 && newSpeed != 0)
            {
                Physics.simulationMode = SimulationMode.FixedUpdate;
            }
            gameSpeed = newSpeed;
            Time.timeScale = gameSpeed;
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

        private void Update()
        {
            if (gameSpeed == 0)
            {
                Physics.Simulate(Time.fixedUnscaledDeltaTime);
            }
        }
    }
}