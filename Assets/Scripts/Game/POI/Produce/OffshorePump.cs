using System.Collections.Generic;
using External.Util;
using Game.Electricity;
using Game.Production.Products;
using Game.State;
using UI.POI;
using UnityEngine;

namespace Game.POI.Produce
{
    public class OffshorePump: IndependantTickingPOI, IElectricityConsumer
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
            var hasEnoughSpace = GameStateManager.Instance.FluidCount.GetValueOrDefault(ProductRegistry.Water, 0) <
                                 GameStateManager.Instance.FluidLimits[ProductRegistry.Water];
            _works = hasEnoughSpace && ((IElectricityConsumer)this).IsConnectedAndWorking();
            if (_works)
            {
                AudioSource.PlayOneShot(waterClip);
                GameStateManager.Instance.ChangeFluids(ProductRegistry.Water, 1);
            }
        }

        public override void OnBuilt()
        {
            base.OnBuilt();
            ((IElectrical) this).InitElectricity(transform);
        }

        protected override void LoadForInspect(PanelViewPOI panel)
        {
            panel.AddSoloProduction("Вода", waterSprite, "1/с");
            panel.AddElectricityConsumption();
            panel.AddLiquidStats();
        }

        public bool IsCovered { get; set; }
        // 4kW
        public float MaxConsumption => 4_000;
    }
}