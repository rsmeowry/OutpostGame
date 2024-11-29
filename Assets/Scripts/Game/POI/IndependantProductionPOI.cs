using System;
using External.Util;
using Game.Building;
using Game.Citizens.Navigation;
using UnityEngine;

namespace Game.POI
{
    public abstract class IndependantProductionPOI: PointOfInterest
    {
        [SerializeField]
        private string desc;
        
        public override QueuePosition EntrancePos => null;

        public override string PoiDesc => desc;

        public abstract int SecondsPerTick { get; }
        public abstract bool IsWorking { get; }

        public abstract void Tick();

        private float _ticker;
        private bool _isWorking;
        public void Update()
        {
            if (!IsBuilt)
                return;
            _ticker += Time.deltaTime;
            if (_ticker > SecondsPerTick)
            {
                _ticker = 0f;
                Tick();
                // _anim.SetBool(Working, IsWorking);
                if (!IsWorking && _isWorking)
                {
                    _anim.SetBool(Working, false);
                } else if (IsWorking && !_isWorking)
                {
                    _anim.SetBool(Working, true);
                }

                _isWorking = IsWorking;
            }
        }

        private Animator _anim;
        private static readonly int Working = Animator.StringToHash("Working");

        public override void OnBuilt()
        {
            base.OnBuilt();
            _anim = GetComponentInChildren<Animator>();
        }

        public override SerializedPOIData Serialize()
        { 
            return new SerializedIndependantPOI
            {
                data = buildingData,
                originPrefabId = buildingData.BuildingType,
                position = BuildingManager.Instance.SnapToGrid(transform.position).Ser(),
                rotation = new Vector3(0f, buildingData.Rotation, 0f).Ser(),
                SelfId = Guid.Parse(pointId)
            };
        }
    }

    [Serializable]
    public class SerializedIndependantPOI: SerializedPOIData
    {
        
    }
}