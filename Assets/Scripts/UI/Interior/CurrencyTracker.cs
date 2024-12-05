using System;
using DG.Tweening;
using External.Util;
using Game.Production.POI;
using Game.State;
using TMPro;
using UnityEngine;

namespace UI.Interior
{
    public class CurrencyTracker: MonoBehaviour
    {
        [SerializeField]
        private TMP_Text text;

        private Vector3 _basePos;
        private RectTransform _rect;

        
        private void Start()
        {
            _rect = (RectTransform)transform;
            _basePos = _rect.anchoredPosition;
            GameStateManager.Instance.onCurrencyChanged.RemoveListener(CurrencyHandler);
            GameStateManager.Instance.onCurrencyChanged.AddListener(CurrencyHandler);
        }

        private void OnEnable()
        {
            GameStateManager.Instance?.onCurrencyChanged?.AddListener(CurrencyHandler);
            CurrencyHandler();
        }

        private void OnDisable()
        {
            GameStateManager.Instance.onCurrencyChanged.RemoveListener(CurrencyHandler);
        }

        private Tween _shakeTween;
        private void CurrencyHandler()
        {
            if (_rect == null)
            {
                this.Delayed(0.5f, CurrencyHandler);
                return;
            }

            _shakeTween?.Kill();
            _rect.anchoredPosition = _basePos;
            _shakeTween = _rect.DOShakeAnchorPos(0.2f, 5f).Play();
            text.text = $"{GameStateManager.Instance.Currency} ЭМ";
        }
    }
}