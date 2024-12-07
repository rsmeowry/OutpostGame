using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using External.Util;
using Game.Citizens.Navigation;
using Game.Sound;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

namespace UI.Util
{
    public class ToastManager: MonoBehaviour
    {
        public static ToastManager Instance { get; private set; }

        [SerializeField]
        private GameObject toastPrefab;

        public List<string> NotificationCenter;

        private int _toastCount = 0;

        public void Awake()
        {
            Instance = this;
        }

        [ContextMenu("Test/Single Toast")]
        private void __TestSingleToast()
        {
            ShowToast("ДОН ВАС НАЙДЕТ!");
            this.Delayed(0.5f, () => ShowToast("Дон все никак от вас не отстанет"), true);
        }

        [ContextMenu("Test/Toasts")]
        private void __TestToasts()
        {
            ShowToast("Всем привет от дона!!");
            this.Delayed(1f, () => ShowToast("Дон передает еще один привет"), true);
            this.Delayed(2.5f, () => ShowToast("Дон все никак от вас не отстанет"), true);
        }

        public void ShowToast(string text, float duration = 3f)
        {
            // TODO: do smth about toasts that appear over 4
            NotificationCenter.Add(text);
            if (_toastCount > 4)
                return;
            _toastCount++;
            
            SoundManager.Instance.PlaySound2D(SoundBank.Instance.GetSound("ui.notification"), 0.6f, Random.Range(0.8f, 1.2f));

            
            var parentToast = Instantiate(toastPrefab, transform);
            StartCoroutine(SingleToastRoutine((RectTransform)parentToast.transform.GetChild(0), text, duration));
        }

        private IEnumerator SingleToastRoutine(RectTransform toast, string text, float duration)
        {
            toast.GetComponentInChildren<TMP_Text>().SetText(text);
            yield return null;
            yield return toast.DOAnchorPosX(150, 1f).SetEase(Ease.InOutQuart).Play().WaitForCompletion();
            yield return new WaitForSecondsRealtime(duration);
            yield return toast.DOAnchorPosX(-150, 0.75f).SetEase(Ease.InOutQuart).Play().WaitForCompletion();
            Destroy(toast.parent.gameObject);
            _toastCount--;
        }
    }
}