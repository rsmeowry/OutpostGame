using Game.Citizens.Navigation;
using Game.POI;
using UnityEngine;

namespace Game.Citizens
{
    public interface IOrderTarget
    {
        public bool Accept(CitizenAgent agent);
        public void WorkerTick(CitizenAgent agent);
        public void Release(CitizenAgent agent);
        public void Free(CitizenAgent agent);
        
        public QueuePosition EntrancePos { get; }
        public PointOfInterest GatheringPost { get; }
    }
}