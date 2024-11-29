using UnityEngine;

namespace Game.Electricity
{
    public interface IElectrical
    {
        public bool IsCovered { get; set; }
        public void InitElectricity(Transform self)
        {
            IsCovered = ElectricityManager.Instance.IsCovering(self);
        }
    }
}