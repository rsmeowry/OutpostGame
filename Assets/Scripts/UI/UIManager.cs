using System;
using DG.Tweening;
using UI.POI;
using UnityEngine;
using UnityEngine.UI;

namespace UI {
    public class UIManager: MonoBehaviour
    {
        public static UIManager Instance { get; private set; }

        public PanelViewPOI prefabInspectPoi;

        [SerializeField]
        private Image black;

        public void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            black.color = Color.black;
            black.DOFade(0f, 1f).Play();
        }

        private bool _openingView;
        private bool _closingView;
        private Tween _panelTween;
        private PanelViewPOI _activePanel;
        public PanelViewPOI OpenPanel()
        {
            if (_openingView)
                return null;
            _openingView = true;
            _closingView = false;
            _panelTween?.Kill();
            _activePanel = Instantiate(prefabInspectPoi, transform);
            var rect = (RectTransform)_activePanel.transform;
            rect.localScale = Vector3.zero;
            _panelTween = rect.DOScale(Vector3.one, 0.4f).SetEase(Ease.OutBack).OnComplete(() => _openingView = false).Play();
            return _activePanel;
        }

        public void HidePanel()
        {
            if (_closingView)
                return;
            _closingView = true;
            _openingView = false;
            _panelTween?.Kill();
            if (_activePanel == null)
                return;
            var lc = transform.GetChild(transform.childCount - 1);
            if(lc.name.Contains("Recipe"))
                lc.DOScale(Vector3.zero, 0.2f).SetEase(Ease.InBack).OnComplete(() =>
                {
                    Destroy(lc.gameObject);
                }).Play();
            var rect = (RectTransform) _activePanel.transform;
            rect.DOScale(Vector3.zero, 0.4f).SetEase(Ease.InBack).OnComplete(() =>
            {
                _closingView = false;
                Destroy(_activePanel.gameObject);
                _activePanel = null;
            }).Play();
            
        }
    }
}

