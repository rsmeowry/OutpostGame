using System.Collections;
using Game.Citizens;
using Game.Citizens.Navigation;
using Game.Electricity;
using Game.Production.POI;
using UI.POI;

namespace Game.POI.Deco
{
    public class RadioStationPOI: UtilityResourcePOI, IElectricityConsumer
    {
        public override void WorkerTick(CitizenAgent agent)
        {
            if (!ShouldSubtick(agent, 4))
            {
                return;
            }
            
            if (!((IElectricityConsumer)this).IsConnectedAndWorking())
            {
                GlobalBuffs.artStations.Remove(this);
                return;
            }
            
            if(!GlobalBuffs.artStations.Contains(this))
                GlobalBuffs.artStations.Add(this);
        }
        
        public override IEnumerator LeaveWorkPlace(CitizenAgent agent)
        {
            yield return base.LeaveWorkPlace(agent);
            GlobalBuffs.artStations.Remove(this);
        }
        
        protected override void LoadForInspect(PanelViewPOI panel)
        {
            base.LoadForInspect(panel);
            panel.AddElectricityConsumption();
        }

        public bool IsCovered { get; set; }
        public float MaxConsumption => 24_000;
    }
}