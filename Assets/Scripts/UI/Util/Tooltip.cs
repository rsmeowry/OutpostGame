using System;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace UI.Util
{
    [ExecuteInEditMode]
    public class Tooltip: MonoBehaviour
    {
        [SerializeField]
        public TMP_Text header;
        [SerializeField]
        public TMP_Text body;
        
        private LayoutElement _layout;
        private Image _img;
        
        public CanvasGroup canvasGroup;

        [FormerlySerializedAs("_hidden")] public bool hidden = true;

        private void Start()
        {
            _layout = GetComponent<LayoutElement>();
            canvasGroup = GetComponent<CanvasGroup>();
            _img = GetComponent<Image>();
        }

        public void SetText(string vHeader, string vBody)
        {
            header.text = vHeader;
            body.text = vBody;
            
            _layout.enabled = header.preferredWidth > _layout.preferredWidth ||
                              body.preferredWidth > _layout.preferredWidth;
        }

        private Tween _showTween;

        public void DelayedShow(float delay = 0.5f)
        {
            hidden = false;
            _showTween = transform.DOScale(Vector3.one, 0.2f).SetDelay(delay).Play();
        }

        public void Hide()
        {
            if (_showTween != null)
            {
                _showTween.Kill();
                _showTween = null;
            }
            transform.DOScale(Vector3.zero, 0.2f).OnComplete(() =>
            {
                hidden = true;
            }).Play();
        }
    }
}