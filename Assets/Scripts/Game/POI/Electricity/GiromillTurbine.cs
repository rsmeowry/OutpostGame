namespace Game.POI.Electricity
{
    public class GiromillTurbine: SimpleElectricityProducer
    {
        // 25kW, pretty good but still not perfect
        public override float MaxProduction => 25_000;
    }
}