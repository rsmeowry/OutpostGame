using System;
using Game.Citizens;
using UnityEngine;

namespace UI.Interior
{
    public class CitizenWatchComponent: MonoBehaviour
    {
        [SerializeField]
        private SingleCitizenWatch singlePrefab;
        [SerializeField]
        private RectTransform container;

        public void Start()
        {
            foreach (var citizen in CitizenManager.Instance.Citizens.Values)
            {
                var o = Instantiate(singlePrefab, container);
                o.Agent = citizen;
            }
            
            var anch = container.anchoredPosition;
            anch.y -= container.rect.height;
            container.anchoredPosition = anch;
        }
    }
}