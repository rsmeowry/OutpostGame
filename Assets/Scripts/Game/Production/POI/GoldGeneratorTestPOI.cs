using External.Util;
using Game.Citizens;
using Game.Production.Products;

namespace Game.Production.POI
{
    public class GoldGeneratorTestPOI: ResourceContainingPOI
    {
        public override void WorkerTick(CitizenAgent agent)
        {
            agent.Inventory.Increment(ProductRegistry.GoldOre);
        }
    }
}