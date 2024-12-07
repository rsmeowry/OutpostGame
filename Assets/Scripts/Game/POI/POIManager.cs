using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;
using External.Util;
using Game.Building;
using Game.Citizens;
using Game.Electricity;
using Game.POI.Housing;
using Game.Production.POI;
using Game.Storage;
using Game.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Game.POI
{
    public class POIManager: MonoBehaviour
    {
        [SerializeField]
        private BuildingDatabase buildingDb;
        
        public Dictionary<Guid, PointOfInterest> LoadedPois = new();
        private List<SerializedPOIData> _toPreload = new();
        
        public static POIManager Instance { get; private set; }

        private void Awake()
        {
            Instance = this;
        }

        public void LoadData()
        {
            TaskManager.Instance.LoadData();
            
            LoadAllPointsOfInterest();
            
            PreloadPointsOfInterest();
            
            CitizenManager.Instance.LoadSavedData();
            
            InitPointsOfInterest();
            
            CitizenManager.Instance.BeginJobs();
            
            ElectricityManager.Instance.BakeNetwork();
        }
        
        private void InitPointsOfInterest()
        {
        }

        public void RecalculateAllGatheringPosts()
        {
            foreach (var poi in LoadedPois.Values.Where(it => it is ResourceContainingPOI))
            {
                ((ResourceContainingPOI) poi).RecalculateNearestGatheringPost();
            }
        }

        private void PreloadPointsOfInterest()
        {
            foreach (var pl in _toPreload)
            {
                var found = buildingDb.buildings.First(it => it.Id.Formatted() == pl.originPrefabId);
                var obj = BuildingManager.Instance.InstantBuild(found, pl.position.ToVec3(), pl.rotation.ToVec3());
                obj.GetComponent<PointOfInterest>().pointId = pl.SelfId.ToString();
                obj.GetComponent<PointOfInterest>().LoadData(pl);
                LoadedPois[pl.SelfId] = obj.GetComponent<PointOfInterest>();
            }
        }

        private void LoadAllPointsOfInterest()
        {
            LoadExistingPointsOfInterest();

            if (!FileManager.Instance.Storage.FileExists("poi.dat", true))
                return;

            var fmt = new BinaryFormatter();
            using var stream = FileManager.Instance.Storage.ReadFileBytes("poi.dat", true);
            var db = (SerializedPOIDatabase) fmt.Deserialize(stream.BaseStream);
            _toPreload.AddRange(db.pointsOfInterest);
        }

        private void LoadExistingPointsOfInterest()
        {
            var pois = SceneManager.GetActiveScene().GetRootGameObjects().First(it => it.name == "POI");
            for (var i = 0; i < pois.transform.childCount; i++)
            {
                var poi = pois.transform.GetChild(i).GetComponent<PointOfInterest>();
                LoadedPois[Guid.Parse(poi.pointId)] = poi;
                if (poi.TryGetComponent<HousePOI>(out var house))
                {
                    if(house.doNotSerialize)
                        CitizenManager.Instance.Houses.Add(house);
                }
            }
        }

        public void SaveData()
        {
            
            var db = (from poi in LoadedPois where !poi.Value.doNotSerialize select poi.Value.Serialize()).ToList();
            
            using var memStream = new MemoryStream();
            var fmt = new BinaryFormatter();
            fmt.Serialize(memStream, new SerializedPOIDatabase { pointsOfInterest = db });
            FileManager.Instance.Storage.SaveBytes("poi.dat", memStream.GetBuffer(), true);
            
            CitizenManager.Instance.Save();
            
            TaskManager.Instance.SaveData();
        }
    }

    [Serializable]
    public class SerializedPOIDatabase
    {
        public List<SerializedPOIData> pointsOfInterest;
    }
}