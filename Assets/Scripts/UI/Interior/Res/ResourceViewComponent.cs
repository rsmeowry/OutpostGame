using System;
using Game.Production.Products;
using UnityEngine;

namespace UI.Interior.Res
{
    public class ResourceViewComponent: MonoBehaviour
    {
        [SerializeField]
        private SingleResourceView singlePrefab;
        [SerializeField]
        private RectTransform container;

        public void Start()
        {
            foreach (var res in ProductRegistry.Instance.AllItems())
            {
                var o = Instantiate(singlePrefab, container);
                o.Item = res;
            }
            
            var anch = container.anchoredPosition;
            anch.y -= container.rect.height;
            container.anchoredPosition = anch;
        }
    }
}