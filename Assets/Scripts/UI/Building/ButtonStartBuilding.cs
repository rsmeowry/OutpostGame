using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Building
{
    public class ButtonStartBuilding: MonoBehaviour
    {
        private RectTransform _parent;
        private RectTransform _self;

        private bool _isShown;

        public void Start()
        {
            _parent = (RectTransform) transform.parent.parent;
            _self = (RectTransform) transform;
            GetComponent<Button>().onClick.AddListener(ClickHandler);
        }

        private void ClickHandler()
        {
            if (_isShown)
            {
                var seq = DOTween.Sequence();
                seq.Join(_parent.DOAnchorPosY(-210, .5f).SetEase(Ease.OutExpo));
                // seq.Join(_self.DORotate(Vector3.zero, 0.5f).SetEase(Ease.OutExpo));
                seq.Play();
            }
            else
            {
                var seq = DOTween.Sequence();
                seq.Join(_parent.DOAnchorPosY(-120, .5f).SetEase(Ease.OutExpo));
                // seq.Join(_self.DORotate(new Vector3(0f, 0f, 45f), 0.5f).SetEase(Ease.OutExpo));
                seq.Play();
            }
            _isShown = !_isShown;
        }
    }
}