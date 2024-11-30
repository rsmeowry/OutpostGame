using System;
using External.Util;
using Game.Building;
using Game.Citizens.Navigation;
using Game.Electricity;
using Game.POI.Deco;
using UI.POI;
using UnityEngine;

namespace Game.POI.Electricity
{
    public abstract class SimpleElectricityProducer: PointOfInterest, IElectricityProducer
    {
        public override QueuePosition EntrancePos => null;
        public override string PoiDesc => "Подключение к сети: " + (IsCovered ? "Есть" : "Нету");
        protected override void LoadForInspect(PanelViewPOI panel)
        {
            panel.AddElectricityProduction();
        }

        public override void OnBuilt()
        {
            base.OnBuilt();
            
            ((IElectrical) this).InitElectricity(transform);
        }

        public override SerializedPOIData Serialize()
        {
            return new SerializedDecoPoi
            {
                data = buildingData,
                originPrefabId = buildingData.BuildingType,
                position = BuildingManager.Instance.SnapToGrid(transform.position).Ser(),
                rotation = new Vector3(0f, buildingData.Rotation, 0f).Ser(),
                SelfId = Guid.Parse(pointId)
            };
        }

        public bool IsCovered { get; set; }
        public abstract float MaxProduction { get; }

        public float ProductionTick()
        {
            return MaxProduction;
        }
    }
}