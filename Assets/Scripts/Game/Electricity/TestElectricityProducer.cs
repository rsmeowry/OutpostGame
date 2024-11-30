using Game.POI;
using UI.POI;
using UnityEngine;

namespace Game.Electricity
{
    public class TestElectricityProducer: IndependantTickingPOI, IElectricityProducer
    {
        protected override void LoadForInspect(PanelViewPOI panel)
        {
            panel.AddElectricityProduction();
        }

        public override int SecondsPerTick => 1;
        public override bool IsWorking => true;
        public override void Tick()
        {
            // noop
        }

        public bool IsCovered { get; set; }
        // 30kW/t
        public float MaxProduction => 30_000;
        public float ProductionTick()
        {
            return MaxProduction;
        }
    }
}