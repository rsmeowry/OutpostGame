using System;
using System.Numerics;
using DG.Tweening;
using TMPro;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

namespace UI.Interior
{
    public class PCWindowDisplay: MonoBehaviour
    {
        [SerializeField]
        private Transform contentContainer;

        [SerializeField]
        private TMP_Text title;
        
        private RectTransform _window;

        private bool _isHidden = true;
        private Tween _scaleAnim;
        private GameObject _activeObj;

        private void Start()
        {
            _window = (RectTransform) transform;
            _window.localScale = Vector3.zero;
        }

        public void ShowWithData(string windowTitle, GameObject data)
        {
            ShowTab(data);
            title.text = windowTitle;
        }

        public void Show()
        {
            if (!_isHidden)
                return;
            _scaleAnim?.Kill();
            _isHidden = false;
            _scaleAnim = _window.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutExpo).Play();
        }

        public void Hide()
        {
            if (_isHidden)
                return;
            _scaleAnim?.Kill();
            _isHidden = true;
            _scaleAnim = _window.DOScale(Vector3.zero, 0.5f).SetEase(Ease.OutExpo).Play().OnComplete(() => { Destroy(_activeObj); _activeObj = null; });
        }

        public GameObject ShowTab(GameObject prefab)
        {
            if(_activeObj != null)
                Destroy(_activeObj);
            _activeObj = Instantiate(prefab, contentContainer);
            Show();
            return _activeObj;
        }
    }
}