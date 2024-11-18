using System;
using System.Collections;
using Game.State;
using UnityEngine;
using Object = UnityEngine.Object;

namespace External.Util
{
    public static class Coroutines
    {
        public static void Delayed<T>(this T self, float seconds, Action action) where T: MonoBehaviour
        {
            self.StartCoroutine(DelayRoutine(seconds, action));
        }

        private static IEnumerator DelayRoutine(float seconds, Action action)
        {
            yield return new WaitForSeconds(seconds);
            action();
        }
    }
}