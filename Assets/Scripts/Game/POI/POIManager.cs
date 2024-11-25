using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using Game.Building;
using Game.Citizens;
using Game.Production.POI;
using Game.Storage;
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
            LoadAllPointsOfInterest();
            
            PreloadPointsOfInterest();
            
            CitizenManager.Instance.LoadSavedData();
            
            InitPointsOfInterest();
            
            CitizenManager.Instance.BeginJobs();
        }
        

        private void InitPointsOfInterest()
        {
            // this is only needed to assign citizens, we are doing it in a different spot
        }

        private void PreloadPointsOfInterest()
        {
            foreach (var pl in _toPreload)
            {
                var found = buildingDb.buildings.First(it => it.Id.Formatted() == pl.originPrefabId);
                var obj = BuildingManager.Instance.InstantBuild(found, pl.position.ToVec3(), pl.rotation.ToVec3());
                obj.GetComponent<PointOfInterest>().pointId = pl.SelfId.ToString();
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
        }
    }

    [Serializable]
    public class SerializedPOIDatabase
    {
        public List<SerializedPOIData> pointsOfInterest;
    }
}