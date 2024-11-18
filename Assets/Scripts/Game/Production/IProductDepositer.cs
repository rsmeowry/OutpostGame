using System.Collections.Generic;
using Game.State;

namespace Game.Production
{
    public interface IProductDepositer
    {
        public void DepositInventory(Dictionary<StateKey, int> inventory);
    }
}