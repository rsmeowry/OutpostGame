using System;
using System.Collections;
using DG.Tweening;
using Game.Sound;
using UnityEngine;
using Random = UnityEngine.Random;

namespace UI.Interior
{
    public class LoadingComponentHandler: MonoBehaviour
    {
        public GameObject next;
        public string nextTitle;
        public Action<GameObject> Callback;
        public PCWindowDisplay window;
        
        private void OnEnable()
        {
            StartCoroutine(Handler());
        }

        private IEnumerator Handler()
        {
            SoundManager.Instance.PlaySound2D(SoundBank.Instance.GetSound("ui.galaction_load"), 0.8f);

            var spinnerOrbit = transform.GetChild(0).GetChild(0).GetChild(0);
            yield return spinnerOrbit.DOLocalRotate(new Vector3(0f, 0f, 180f), 0.8f * Random.Range(0.6f, 1f), RotateMode.FastBeyond360).SetEase(Ease.Linear)
                .Play().WaitForCompletion();

            Callback(window.ShowWithData(nextTitle, next));
        }
    }
}