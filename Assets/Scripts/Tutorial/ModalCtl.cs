using System;
using DG.Tweening;
using External.Data;
using External.Util;
using Game.Controllers;
using UnityEngine;
using UnityEngine.UI;

namespace Tutorial
{
    public class ModalCtl: MonoBehaviour
    {
        public RectTransform rect;

        [SerializeField]
        private Button closeBtn;

        private void Awake()
        {
            rect = (RectTransform)transform;
        }

        public void Start()
        {
            closeBtn.onClick.AddListener(Hide);
        }

        private Tween _displayTween;
        public void Show()
        {
            _displayTween?.Kill();
            _displayTween = rect.DOScale(Vector3.one, 0.4f).SetEase(Ease.OutBack).Play();
            TownCameraController.Instance.interactionFilter = CameraInteractionFilter.None;
        }
        
        public void Hide()
        {
            _displayTween?.Kill();
            _displayTween = rect.DOScale(Vector3.zero, 0.4f).SetEase(Ease.InBack).Play();
            TutorialCtl.Instance.ActiveStep?.ReceiveModalClose();
            TownCameraController.Instance.interactionFilter = CameraInteractionFilter.ProductionAndCitizens;
        }
    }
}