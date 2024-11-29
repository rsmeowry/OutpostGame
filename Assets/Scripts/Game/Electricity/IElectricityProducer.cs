namespace Game.Electricity
{
    public interface IElectricityProducer: IElectrical
    {
        public float MaxProduction { get; }
        
        public float ProductionTick();
    }
}