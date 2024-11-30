using System;
using System.Collections.Generic;
using Game.Production.Products;
using Game.State;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Interior.Res
{
    public class SingleResourceView: MonoBehaviour
    {
        [SerializeField]
        private Image itemIcon;
        [SerializeField]
        private TMP_Text itemName;
        [SerializeField]
        private TMP_Text itemCount;
        
        [NonSerialized]
        public StateKey Item;

        public void Start()
        {
            var data = ProductRegistry.Instance.GetProductData(Item);
            itemIcon.sprite = data.icon;
            itemName.text = data.name;
            
            GameStateManager.Instance.onProductChanged.AddListener(HandleChangedResource);
            PollChanges();
        }

        private void OnDisable()
        {
            GameStateManager.Instance.onProductChanged.RemoveListener(HandleChangedResource);
        }

        private void PollChanges()
        {
            itemCount.text = GameStateManager.Instance.PlayerProductCount.GetValueOrDefault(Item.Formatted(), 0) + " шт.";
        }

        private void HandleChangedResource(ProductChangedData changed)
        {
            if (changed.Product != Item)
                return;
            PollChanges();
        }
     }
}