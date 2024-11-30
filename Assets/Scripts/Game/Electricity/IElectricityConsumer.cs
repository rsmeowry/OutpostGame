using UnityEngine;

namespace Game.Electricity
{
    public interface IElectricityConsumer: IElectrical
    {
        public float Consumption => MaxConsumption;
        public float MaxConsumption { get; }

        public bool IsConnectedAndWorking()
        {
            return IsCovered && ElectricityManager.Instance.RequestPower(Consumption);
        }
    }
}