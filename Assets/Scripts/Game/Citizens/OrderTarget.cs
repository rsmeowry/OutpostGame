using System;
using System.Collections;
using Game.Citizens.Navigation;
using Game.POI;
using UnityEngine;

namespace Game.Citizens
{
    public interface ICitizenWorkPlace
    {
        public IEnumerator EnterWorkPlace(CitizenAgent agent, Action<WorkPlaceEnterResult> callback);
        public void WorkerTick(CitizenAgent agent);
        public IEnumerator LeaveWorkPlace(CitizenAgent agent);
        public void Fire(CitizenAgent agent);
        public bool IsCurrentlyWorking(CitizenAgent agent);
        public Vector3 DesignatedWorkingSpot(CitizenAgent agent);
        
        public QueuePosition EntrancePos { get; }
        public PointOfInterest GatheringPost { get; }
    }

    public enum WorkPlaceEnterResult
    {
        Accepted,
        Declined,
        NeedToMoveToSpot
    }
}