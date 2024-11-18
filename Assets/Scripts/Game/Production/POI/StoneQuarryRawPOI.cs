using External.Util;
using Game.Citizens;
using Game.POI;
using Game.Production.Products;
using UnityEngine;

namespace Game.Production.POI
{
    public class StoneQuarryRawPOI: ResourceContainingPOI
    {
        public override void WorkerTick(CitizenAgent agent)
        {
            agent.Inventory.Increment(ProductRegistry.Stone);
        }
    }
}