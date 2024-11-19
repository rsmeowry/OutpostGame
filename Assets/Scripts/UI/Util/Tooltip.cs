using System;
using DG.Tweening;
using TMPro;
using UnityEngine;
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
        [SerializeField]
        private int characterWrapLimit = 80;
        
        private LayoutElement _layout;
        
        public CanvasGroup canvasGroup;

        private void Start()
        {
            _layout = GetComponent<LayoutElement>();
            canvasGroup = GetComponent<CanvasGroup>();
        }

        public void SetText(string vHeader, string vBody)
        {
            _layout.enabled = header.preferredWidth > _layout.preferredWidth ||
                              body.preferredWidth > _layout.preferredWidth;
                
            header.text = vHeader;
            body.text = vBody;
        }

        private Tween showTween;

        public void DelayedShow()
        {
            showTween = transform.DOScale(Vector3.one, 0.2f).SetDelay(0.5f).Play();
        }

        public void Hide()
        {
            if (showTween != null)
            {
                showTween.Kill();
                showTween = null;
            }
            transform.DOScale(Vector3.zero, 0.2f).Play();
        }
    }
}