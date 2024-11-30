using System;
using Game.Citizens.Navigation;
using Game.POI;
using Game.Production.Products;
using Game.State;
using UI.POI;
using UnityEngine;

namespace Game.Electricity
{
    public class TestElectricityConsumer: IndependantTickingPOI, IElectricityConsumer
    {
        public override int SecondsPerTick => 1;
        
        public override bool IsWorking => false;
        public bool IsCovered { get; set; }
        // 20 kW/t
        public float MaxConsumption => 20_000;

        public override void Tick()
        {
            var working = ((IElectricityConsumer)this).IsConnectedAndWorking();
            Debug.Log($"ATTEMTPING TO TICK {IsCovered} {working}");
            if ((working))
            {
                GameStateManager.Instance.ChangeCurrency(10, "бе", false);
            }
        }
        
        protected override void LoadForInspect(PanelViewPOI panel)
        {
            panel.AddElectricityConsumption();
        }
    }
}