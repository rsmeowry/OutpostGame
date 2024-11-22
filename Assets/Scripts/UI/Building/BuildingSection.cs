using System;
using System.Collections;
using DG.Tweening;
using UI.Building.Tabs;
using UnityEngine;
using UnityEngine.Serialization;

namespace UI.Building
{
    public class BuildingSection: MonoBehaviour
    {
        [SerializeField]
        private RectTransform selectorArea;

        [SerializeField]
        private BuildingTabSelector firstTab;

        [SerializeField]
        private SingleBuildingChoice singleButtonPrefab;
        
        public BuildingTabSelector selectedTab;

        public void Start()
        {
            InitTab(firstTab);
        }

        private void InitTab(BuildingTabSelector tab)
        {
            selectedTab = tab;
            foreach (var building in tab.storedBuildings.buildings)
            {
                var btn = Instantiate(singleButtonPrefab, selectorArea);
                btn.buildingData = building;
                btn.Init();
            }
        }

        private bool _busy;

        public IEnumerator SwitchTab(BuildingTabSelector newTab)
        {
            if (_busy)
                yield break;
            _busy = true;
            var seq = DOTween.Sequence();
            for (var i = 0; i < selectorArea.childCount; i++)
            {
                var child = selectorArea.GetChild(i);
                seq.Join(child.DOScale(Vector3.zero, 0.25f).SetEase(Ease.OutExpo).OnComplete(() => Destroy(child.gameObject)));
            }

            seq.Join(selectedTab.transform.DORotate(Vector3.zero, 0.25f).SetEase(Ease.OutExpo));
            

            yield return seq.Play().WaitForCompletion();

            selectedTab = newTab;
            var seq2 = DOTween.Sequence();

            foreach (var building in newTab.storedBuildings.buildings)
            {
                var btn = Instantiate(singleButtonPrefab, selectorArea);
                btn.buildingData = building;
                btn.Init();
                btn.transform.localScale = Vector3.zero;
                seq2.Join(btn.transform.DOScale(Vector3.one, 0.25f).SetEase(Ease.OutExpo));
            }
            
            seq2.Join(selectedTab.transform.DORotate(new Vector3(0f, 0f, 45f), 0.25f).SetEase(Ease.OutExpo));
            
            yield return seq2.Play().WaitForCompletion();

            _busy = false;
        }
    }
}