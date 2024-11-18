using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using External.Util;
using Game.Citizens.States;
using Game.Production;
using Game.State;
using Newtonsoft.Json;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Serialization;

namespace Game.Citizens
{
    [RequireComponent(typeof(NavMeshAgent))]
    public class CitizenAgent : MonoBehaviour
    {
        [FormerlySerializedAs("NavMeshAgent")] public NavMeshAgent navMeshAgent;

        private Animator _animator;

        public Dictionary<StateKey, int> Inventory = new();
        public PersistentCitizenData PersistentData;
        public int citizenId;
        public int inventoryCapacity = 3;

        public CitizenStateMachine StateMachine;
        public CitizenCarryResourcesState CarryResourcesState;
        public CitizenWanderState WanderState;
        public CitizenGoWorkState GoWorkState;
        public CitizenWorkState WorkState;

        public Vector3 wanderAnchor = Vector3.zero;
        public float wanderRange = 5f;

        public IOrderTarget WorkPlace;
        public IProductDepositer ProductDepositer;

        public bool LoadedFromData = false;
        
        private void Start()
        {
            navMeshAgent = GetComponent<NavMeshAgent>();
            _animator = GetComponentInChildren<Animator>();

            StateMachine = new CitizenStateMachine();
            CarryResourcesState = new CitizenCarryResourcesState(this, StateMachine);
            WanderState = new CitizenWanderState(this, StateMachine);
            GoWorkState = new CitizenGoWorkState(this, StateMachine);
            WorkState = new CitizenWorkState(this, StateMachine);

            PersistentData = new PersistentCitizenData
            {
                Profession = CitizenCaste.Beekeeper
            };
            if(!LoadedFromData)
                StartCoroutine(StateMachine.Init(WanderState));
        }
        
        [ContextMenu("Debug/Log data")]
        private void __TestDebugData()
        {
            Debug.Log(StateMachine.CurrentState);
        }

        [ContextMenu("Test/Go Wander")]
        private void __TestGoWander()
        {
            StartCoroutine(StateMachine.ChangeState(WanderState));
        }

        public void Order(CitizenState newState)
        {
            StartCoroutine(StateMachine.ChangeState(newState));
        }

        private void Update()
        {
            StateMachine.FrameUpdate();
        }

        public bool InventoryFull()
        {
            return Inventory.Values.Sum() >= inventoryCapacity;
        }

        private void FixedUpdate()
        {
            StateMachine.PhysicsUpdate();
        }

        public bool IsUnoccupied()
        {
            return WorkPlace == null;
        }

        public void Free()
        {
            WorkPlace.Free(this);
            StartCoroutine(StateMachine.ChangeState(WanderState).Callback(() => WorkPlace = null));
        }

        public void Assign(IOrderTarget workPlace)
        {
            WorkPlace = workPlace;
            Order(GoWorkState);
        }

        public void PlayAnimation(string anim)
        {
            if (_animator == null)
                return;
            _animator.Play(anim);
        }

        public void HideSelf()
        {
            // TODO: something else???
            // GetComponent<Renderer>().enabled = false;
            foreach (var rd in GetComponentsInChildren<Renderer>())
            {
                rd.enabled = false;
            }
            GetComponent<Collider>().enabled = false;
        }

        public void ShowSelf()
        {
            // GetComponent<Renderer>().enabled = true;
            foreach (var rd in GetComponentsInChildren<Renderer>())
            {
                rd.enabled = true;
            }
            GetComponent<Collider>().enabled = true;
        }

        public void Renavigate()
        {
            StateMachine.Renavigate();
        }
    }

    [Serializable]
    public struct StoredCitizenData
    {
        [FormerlySerializedAs("PersistentData")] [JsonProperty("persistent")]
        public StoredPersistentCitizenData persistentData;
        [FormerlySerializedAs("Position")] [JsonProperty("position")]
        public Vector3 position;
        [JsonProperty("inventory")]
        public Dictionary<string, int> Inventory;
        [FormerlySerializedAs("BaseInventoryCapacity")] [JsonProperty("inventoryCapacity")]
        public int baseInventoryCapacity;
        [FormerlySerializedAs("BuildingGuid")] [JsonProperty("assignedBuilding")]
        public GUID buildingGuid;
        [JsonProperty("Depositer")]
        public GUID depositer;
        [FormerlySerializedAs("State")] [JsonProperty("state")]
        public string state;
        [FormerlySerializedAs("WanderAnchor")] [JsonProperty("wanderAnchor")]
        public Vector3 wanderAnchor;
    }
}