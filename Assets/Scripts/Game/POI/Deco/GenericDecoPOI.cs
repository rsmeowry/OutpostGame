using System;
using System.Collections.Generic;
using External.Achievement;
using External.Util;
using Game.Building;
using Game.Citizens;
using Game.Citizens.Navigation;
using Game.Citizens.States;
using Game.Sound;
using UI.POI;
using UnityEngine;

namespace Game.POI.Deco
{
    public class GenericDecoPoi: PointOfInterest
    {
        public QueuePosition entrancePos;

        public override QueuePosition EntrancePos => entrancePos;
        protected override void LoadForInspect(PanelViewPOI panel)
        {
            
        }

        public override void OnBuilt()
        {
            base.OnBuilt();
            if (data.keyId.Contains("moyai"))
            {
                SoundManager.Instance.PlaySoundAt(SoundBank.Instance.GetSound("building.moyai"), transform.position);
                AchievementManager.Instance.GiveAchievement(Achievements.Final);
            }
        }

        public override SerializedPOIData Serialize()
        {
            return new SerializedDecoPoi()
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
    public class SerializedDecoPoi: SerializedPOIData
    {
        
    }
}