using System;
using System.Collections.Generic;
using Game.Production.Products;
using Game.State;
using TMPro;
using UnityEngine;

namespace UI.POI
{
    public class LiquidStatistics: MonoBehaviour
    {
        [SerializeField]
        private TMP_Text waterText;
        
        private void Start()
        {
            GameStateManager.Instance.onFluidsChanged.AddListener(HandleFluidsChanged);
            HandleFluidsChanged();
        }

        private void OnDisable()
        {
            GameStateManager.Instance.onFluidsChanged.RemoveListener(HandleFluidsChanged);
        }

        private void HandleFluidsChanged()
        {
            var waterCount = GameStateManager.Instance.FluidCount.GetValueOrDefault(ProductRegistry.Water, 0);
            var waterLimit = GameStateManager.Instance.FluidLimits.GetValueOrDefault(ProductRegistry.Water, 10);
            waterText.text = $"{waterCount}/{waterLimit}";
        }
    }
}