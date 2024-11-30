using UnityEngine;

namespace Game.POI.Electricity
{
    public class BasicWindmill: SimpleElectricityProducer
    {
        // 8kW => SUPER LOW
        public override float MaxProduction => 8_000;
        

        public override void OnBuilt()
        {
            base.OnBuilt();

            GetComponentInChildren<Animator>().Play("windmill_spin");
        }
    }
}