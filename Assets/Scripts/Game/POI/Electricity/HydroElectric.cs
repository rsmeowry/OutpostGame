using External.Achievement;
using External.Util;
using Game.POI.Deco;
using Game.Sound;
using UI.Interior;
using UI.Interior.Stocks;
using UnityEngine;

namespace Game.POI.Electricity
{
    public class HydroElectric: SimpleElectricityProducer
    {
        // 8kW => SUPER LOW
        public override float MaxProduction => 64_000;

        private bool _startedPlaying;
        public override float ProductionTick()
        {
            return base.ProductionTick();
        }
        
        public override void OnBuilt()
        {
            base.OnBuilt();

            GetComponentInChildren<ParticleSystem>().Play();
            
            AchievementManager.Instance.GiveAchievement(Achievements.Hydroelectricity);
        }
    }
}