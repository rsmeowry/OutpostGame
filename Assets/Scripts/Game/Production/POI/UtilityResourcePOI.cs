using System.Collections;
using System.Collections.Generic;
using External.Util;
using Game.Citizens;

namespace Game.Production.POI
{
    public abstract class UtilityResourcePOI: ResourceContainingPOI
    {
        private Dictionary<int, int> _ticks = new();

        protected bool ShouldSubtick(CitizenAgent agent, int subtickCount)
        {
            var t = _ticks.Increment(agent.citizenId);
            if (t % subtickCount == 0)
            {
                _ticks[agent.citizenId] = 0;
                return true;
            }

            return false;
        }

        public override IEnumerator LeaveWorkPlace(CitizenAgent agent)
        {
            yield return base.LeaveWorkPlace(agent);
            _ticks[agent.citizenId] = 0;
        }
    }
}