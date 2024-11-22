using System.Collections.Generic;
using External.Util;
using Game.Citizens;
using Game.Production.Products;

namespace Game.Production.POI
{
    public class StoneQuarryRawPOI: UtilityResourcePOI
    {
        private Dictionary<CitizenAgent, int> _ticks;
        public override void WorkerTick(CitizenAgent agent)
        {
            if (ShouldSubtick(agent, 2))
            {
                agent.Inventory.Increment(ProductRegistry.Stone); 
            }
        }
    }
}