using System.Collections.Generic;
using External.Util;
using Game.Production.Products;
using Game.State;
using UI.POI;
using UnityEngine;

namespace Game.POI.Produce
{
    public class OffshorePump: IndependantProductionPOI
    {
        [SerializeField]
        private Sprite waterSprite;

        [SerializeField]
        private AudioClip waterClip;
        
        private bool _works;
        public override int SecondsPerTick => 1;
        public override bool IsWorking => _works;
        public override void Tick()
        {
            _works = GameStateManager.Instance.FluidCount.GetValueOrDefault(ProductRegistry.Water, 0) <
                     GameStateManager.Instance.FluidLimits[ProductRegistry.Water];
            if (_works)
            {
                AudioSource.PlayOneShot(waterClip);
                GameStateManager.Instance.FluidCount.Increment(ProductRegistry.Water);
            }
        }

        protected override void LoadForInspect(PanelViewPOI panel)
        {
            panel.AddSoloProduction("Вода", waterSprite, "1/с");
            // TODO: electricity
        }
    }
}