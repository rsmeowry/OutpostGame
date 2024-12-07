using System;
using External.Achievement;
using Game.Citizens;
using Game.Production.POI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Interior
{
    public class AchievementViewComponent: MonoBehaviour
    {
        [SerializeField]
        private GameObject singlePrefab;
        [SerializeField]
        private RectTransform container;
        [SerializeField]
        private Sprite lockedIcon;

        public void Start()
        {
            foreach (var (id, ach) in AchievementManager.Instance.Datas)
            {
                var o = Instantiate(singlePrefab, container);
                var unlocked = AchievementManager.Instance.HasUnlocked(id);
                var icon = o.transform.GetChild(0).GetComponent<Image>();
                icon.sprite = unlocked ? ach.icon : lockedIcon;
                var title = o.transform.GetChild(1).GetChild(0).GetComponent<TMP_Text>();
                title.text = unlocked ? ach.name : "???";
                var desc = o.transform.GetChild(1).GetChild(1).GetComponent<TMP_Text>();
                desc.text = ach.description;
            }
            
            var anch = container.anchoredPosition;
            anch.y -= container.rect.height;
            container.anchoredPosition = anch;
        }
    }
}