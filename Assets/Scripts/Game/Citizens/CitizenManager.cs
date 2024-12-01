using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using DG.Tweening.Plugins;
using External.Storage;
using External.Util;
using Game.POI;
using Game.POI.Housing;
using Game.Production;
using Game.Production.POI;
using Game.State;
using Game.Storage;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Windows;
using Random = UnityEngine.Random;

namespace Game.Citizens
{
    public class CitizenManager: MonoBehaviour
    {
        public static CitizenManager Instance { get; private set; }

        private int citizenIdTracker = 0;
        
        [SerializeField]
        private CitizenAgent constructorPrefab;
        [SerializeField]
        private CitizenAgent beekeeperPrefab;
        [SerializeField]
        private CitizenAgent creatorPrefab;
        [SerializeField]
        private CitizenAgent explorerPrefab;
        
        [SerializeField]
        public GameObject boxPrefab;
        
        public Dictionary<int, CitizenAgent> Citizens = new();
        [NonSerialized]
        public List<HousePOI> Houses = new();
        private Dictionary<int, StoredCitizenData> _intermediate = new();

        public void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            
        }
        
        // LOADING DATA

        public void LoadSavedData()
        {
            if (!FileManager.Instance.Storage.FileExists("agents.dat", true))
                return;

            using var data = FileManager.Instance.Storage.ReadFileBytes("agents.dat", true);
            var fmt = new BinaryFormatter();
            
            var dat = (StoredCitizenDatabase) fmt.Deserialize(data.BaseStream);
            citizenIdTracker = dat.citizenIdTracker;
            foreach (var citizen in dat.storedCitizens)
            {
                var prefab = citizen.persistentData.caste switch
                {
                    CitizenCaste.Creator => creatorPrefab,
                    CitizenCaste.Explorer => explorerPrefab,
                    CitizenCaste.Beekeeper => beekeeperPrefab,
                    CitizenCaste.Engineer => constructorPrefab,
                    _ => throw new ArgumentOutOfRangeException()
                };
                var ctz = Instantiate(prefab, transform);
                ctz.loadedFromData = true;
                ctz.Inventory = citizen.Inventory.Select(it => (StateKey.FromString(it.Key), it.Value)).ToDictionary(it => it.Item1, it => it.Value);
                ctz.citizenId = citizen.citizenId;
                ctz.inventoryCapacity = citizen.baseInventoryCapacity;
                ctz.wanderAnchor = citizen.wanderAnchor.ToVec3();
                ctz.PersistentData = new PersistentCitizenData()
                {
                    Awards = citizen.persistentData.awards,
                    Name = citizen.persistentData.name,
                    Profession = citizen.persistentData.caste
                };
                ctz.Load();
                Citizens[ctz.citizenId] = ctz;
                _intermediate[ctz.citizenId] = citizen;
            }
        }

        public bool CanSpawnCitizen()
        {
            return Houses.Sum(it => it.houseSize) >= Citizens.Count + 1;
        }

        public void BeginJobs()
        {
            foreach (var citizenKv in Citizens)
            {
                var citizen = citizenKv.Value;
                var dat = _intermediate[citizen.citizenId];
                if (dat.BuildingGuid == Guid.Empty)
                {
                    StartCoroutine(citizen.StateMachine.Init(citizen.WanderState));
                }
                else
                {
                    var workPlace = (ICitizenWorkPlace)POIManager.Instance.LoadedPois[dat.BuildingGuid];
                    var home = (HousePOI)POIManager.Instance.LoadedPois[dat.BuildingGuid];
                    var rcpoi = (ResourceContainingPOI)workPlace;
                    if(rcpoi.AssignedAgents.Count + 1 < rcpoi.capacity)
                        rcpoi.AssignedAgents.Add(citizen);
                    citizen.WorkPlace = workPlace;
                    citizen.Home = home;
                    home.Tenants.Add(citizen);
                    
                    StartCoroutine(citizen.StateMachine.Init(dat.state switch
                    {
                        "carry" => citizen.CarryResourcesState,
                        "wander" => citizen.WanderState,
                        "gowork" => citizen.GoWorkState,
                        "work" => citizen.GoWorkState,
                        "movetospot" => citizen.GoWorkState,
                        "gohome" => citizen.GoHomeState,
                        "sleep" => citizen.SleepState,
                        _ => citizen.WanderState
                    }));
                }
            }
        }

        public void RecalculateAllStates()
        {
            
        }
        
        /////////

        [SerializeField]
        private ResourceContainingPOI poi;
        
        public UnityEvent onCitizenFired;
        
        public CitizenAgent SpawnCitizen(Vector3 position, PersistentCitizenData data)
        {
            var citizen = Instantiate(data.Profession switch
            {
                CitizenCaste.Creator => creatorPrefab,
                CitizenCaste.Explorer => explorerPrefab,
                CitizenCaste.Beekeeper => beekeeperPrefab,
                CitizenCaste.Engineer => constructorPrefab,
                _ => throw new ArgumentOutOfRangeException()
            });
            citizen.transform.position = position;
            citizen.citizenId = citizenIdTracker++;
            citizen.PersistentData = data;
            citizen.Home = Houses.First(it => it.Tenants.Count < it.houseSize);
            citizen.Home.Tenants.Add(citizen);
            Citizens[citizen.citizenId] = citizen;
            return citizen;
        }

        [ContextMenu("Test/Debug Citizen Names")]
        public void __TestDebDebugCitizenNames()
        {
            foreach (var citizen in Citizens.Values)
            {
                Debug.Log(citizen.PersistentData.Name);
            }
        }

        [ContextMenu("Test/Spawn Citizen Raw")]
        public CitizenAgent __TestSpawnCitizen()
        {
            SpawnCitizen(new Vector3(180, 3, 110f), new PersistentCitizenData()
            {
                Name = CitizenNames.RandomMascName(),
                Profession = CitizenCaste.Creator
            });
            return null;
        }

        // TODO: using an event here could be useful in case any race conditions arise (shouldn't happen though)
        public bool AnyFree(CitizenCaste caste)
        {
            return Citizens.Any(it => it.Value.PersistentData.Profession == caste && it.Value.IsUnoccupied());
        }

        public CitizenAgent FindUnassignedCitizen(CitizenCaste caste)
        {
            return Citizens.Select(it => it.Value).FirstOrDefault(it => it.PersistentData.Profession == caste && it.IsUnoccupied());
        }

        public List<CitizenAgent> FindUnassignedCitizens(CitizenCaste caste, int amount)
        {
            return Citizens.Select(it => it.Value).Where(it => it.PersistentData.Profession == caste && it.IsUnoccupied()).Take(amount).ToList();
        }

        public void Save()
        {
            var db = new StoredCitizenDatabase()
            {
                citizenIdTracker = citizenIdTracker,
                storedCitizens = Citizens.Values.Select(it => it.Serialize()).ToList()
            };
            
            using var memStream = new MemoryStream();
            var fmt = new BinaryFormatter();
            fmt.Serialize(memStream, db);
            FileManager.Instance.Storage.SaveBytes("agents.dat", memStream.GetBuffer(), true);
        }

        [Serializable]
        public class StoredCitizenDatabase
        {
            public int citizenIdTracker;
            public List<StoredCitizenData> storedCitizens;
        }
    }
}