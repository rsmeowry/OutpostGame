using System;
using System.Collections;
using Game.State;
using UnityEngine;
using Object = UnityEngine.Object;

namespace External.Util
{
    public static class Coroutines
    {
        public static void Delayed<T>(this T self, float seconds, Action action, bool unscaled = false) where T: MonoBehaviour
        {
            self.StartCoroutine(DelayRoutine(seconds, action, unscaled));
        }

        private static IEnumerator DelayRoutine(float seconds, Action action, bool unscaled)
        {
            yield return unscaled ? new WaitForSecondsRealtime(seconds) : new WaitForSeconds(seconds);
            action();
        }

        public static IEnumerator Callback(this IEnumerator selfRoutine, Action callback)
        {
            yield return selfRoutine;
            callback();
        }
    }
}