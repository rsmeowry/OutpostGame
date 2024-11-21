using System;
using System.Collections;
using DG.Tweening;
using UnityEngine;

namespace UI.BottomRow
{
    public class BottomRowCtl: MonoBehaviour
    {
        public BottomRowTabSelector activeTab;

        private RectTransform _self;
        private RectTransform _btmRow;

        private bool _busy;

        private void Start()
        {
            _self = (RectTransform)transform;
            _btmRow = (RectTransform) transform.GetChild(1).GetChild(0);
        }

        public IEnumerator SwitchTab(BottomRowTabSelector newTab)
        {
            if (_busy)
                yield break;
            _busy = true;
            
            var seq = DOTween.Sequence();
            seq.Join(activeTab.rectTransform.DORotate(Vector3.zero, .25f).SetEase(Ease.OutExpo));
            seq.Join(activeTab.rectTransform.DOScale(Vector3.one, .25f).SetEase(Ease.OutExpo));
            seq.Join(_btmRow.DOAnchorPosY(-150f, .25f).SetEase(Ease.OutExpo));
            yield return seq.WaitForCompletion();
            
            
            Destroy(_btmRow.GetChild(0).gameObject);
            
            activeTab = newTab;
            Instantiate(newTab.bottomTabPrefab, _btmRow);
            
            var seq2 = DOTween.Sequence();
            seq2.Join(_btmRow.DOAnchorPosY(0, .25f).SetEase(Ease.OutExpo));
            seq2.Join(activeTab.rectTransform.DORotate(new Vector3(0f, 0f, 45f), .25f).SetEase(Ease.OutExpo));
            seq2.Join(activeTab.rectTransform.DOScale(new Vector3(1.25f, 1.25f, 1.25f), .25f).SetEase(Ease.OutExpo));
            yield return seq2.Play().WaitForCompletion();
            
            _busy = false;
        }

        public IEnumerator CloseTab()
        {
            if (_busy)
                yield break;
            _busy = true;
            yield return HideTab();
            activeTab = null;
            Destroy(_btmRow.GetChild(0).gameObject);
            _busy = false;
        }

        public IEnumerator DisplayTab(BottomRowTabSelector newTab)
        {
            if (_busy)
                yield break;
            _busy = true;
            activeTab = newTab;
            Instantiate(newTab.bottomTabPrefab, _btmRow);
            yield return ShowTab(0.5f);
            _busy = false;
        }

        private IEnumerator ShowTab(float timeScale = 1f)
        {
            var seq = DOTween.Sequence();
            seq.Join(_self.DOAnchorPosY(-120, .5f * timeScale).SetEase(Ease.OutExpo));
            seq.Join(activeTab.rectTransform.DORotate(new Vector3(0f, 0f, 45f), .5f * timeScale).SetEase(Ease.OutExpo));
            seq.Join(activeTab.rectTransform.DOScale(new Vector3(1.25f, 1.25f, 1.25f), .5f * timeScale).SetEase(Ease.OutExpo));

            yield return seq.Play().WaitForCompletion();
        }

        private IEnumerator HideTab(float timeScale = 1f)
        {
            var seq = DOTween.Sequence();
            seq.Join(_self.DOAnchorPosY(-230, .5f * timeScale).SetEase(Ease.OutExpo));
            seq.Join(activeTab.rectTransform.DORotate(Vector3.zero, .5f * timeScale).SetEase(Ease.OutExpo));
            seq.Join(activeTab.rectTransform.DOScale(Vector3.one, .5f * timeScale).SetEase(Ease.OutExpo));
            yield return seq.Play().WaitForCompletion();
        }
    }
}