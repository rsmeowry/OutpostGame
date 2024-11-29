using System;
using DG.Tweening;
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
            CurrencyHandler();
            GameStateManager.Instance.currencyIncreaseEvent.AddListener(CurrencyHandler);
        }

        private void OnDisable()
        {
            GameStateManager.Instance.currencyIncreaseEvent.RemoveListener(CurrencyHandler);
        }

        private Tween _shakeTween;
        private void CurrencyHandler()
        {
            _shakeTween?.Kill();
            _rect.anchoredPosition = _basePos;
            _shakeTween = _rect.DOShakeAnchorPos(0.2f, 5f).Play();
            text.text = $"{GameStateManager.Instance.Currency:##,###} ЭМ";
        }
    }
}