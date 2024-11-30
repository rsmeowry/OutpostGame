using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using External.Data;
using Game.State;
using Game.Upgrades;
using TMPro;
using UI.Interior.Stocks;
using UnityEngine;

namespace UI.Interior.Upgrades
{
    public class UpgradeTreeComponent: MonoBehaviour
    {
        [SerializeField]
        private UILineRenderer linesPrefab;

        [SerializeField]
        private TMP_Text experienceCounter;

        [SerializeField]
        private Transform scrollableArea;
        
        private Dictionary<string, SingleUpgradeChoice> _upgrades = new();
        
        private void Start()
        {
            // container -> window
            var parentTf = (RectTransform) transform.parent.parent;

            var containerTf = (RectTransform) transform.parent;

            containerTf.sizeDelta = new Vector2(780f, containerTf.sizeDelta.y);
            
            DOTween.To(() => parentTf.sizeDelta.x, v => parentTf.sizeDelta = new Vector2(v, parentTf.sizeDelta.y), 800,
                0.5f).SetEase(Ease.OutExpo).Play();

            var btm = scrollableArea.GetChild(0);
            var left = scrollableArea.GetChild(1);
            var right = scrollableArea.GetChild(2);
            for(var i = 0; i < btm.childCount; i++)
            {
                var tf = btm.GetChild(i);
                var ch = tf.GetComponent<SingleUpgradeChoice>();
                _upgrades[ch.data.id] = ch;
            }
            for(var i = 0; i < left.childCount; i++)
            {
                var tf = left.GetChild(i);
                var ch = tf.GetComponent<SingleUpgradeChoice>();
                _upgrades[ch.data.id] = ch;
            }
            for(var i = 0; i < right.childCount; i++)
            {
                var tf = right.GetChild(i);
                var ch = tf.GetComponent<SingleUpgradeChoice>();
                _upgrades[ch.data.id] = ch;
            }


            experienceCounter.text = "Опыт: " + MiscSavedData.Instance.Data.Experience + "XP";
            
            CreateLines(_upgrades["harder_tasks"]);
            
            UpgradeTreeManager.Instance.onUpgradeUnlocked.AddListener(OnUpgraded);
        }

        private void OnUpgraded(UpgradeUnlockedData _)
        {
            foreach(var child in _upgrades.Values)
                child.PollChanges();
            experienceCounter.text = "Опыт: " + MiscSavedData.Instance.Data.Experience + "XP";
        }

        private void CreateLines(SingleUpgradeChoice choice)
        {
            var rect = (RectTransform)choice.transform;
            var begin = rect.anchoredPosition;
            foreach (var child in choice.data.unlocks)
            {
                var obj = _upgrades[child.id];
                var endPos = ((RectTransform)obj.transform).anchoredPosition;
                var lines = Instantiate(linesPrefab, scrollableArea);
                lines.points = new[] { begin, endPos };
                lines.transform.SetSiblingIndex(0);
                lines.SetAllDirty();
                CreateLines(obj);
            }
        }
        
        private void OnDisable()
        {
            UpgradeTreeManager.Instance.onUpgradeUnlocked.RemoveListener(OnUpgraded);
            
            var containerTf = (RectTransform) transform.parent;
            containerTf.sizeDelta = new Vector2(540f, containerTf.sizeDelta.y);
            var parentTf = (RectTransform) transform.parent.parent;
            parentTf.sizeDelta = new Vector2(560f, parentTf.sizeDelta.y);
        }
    }
}