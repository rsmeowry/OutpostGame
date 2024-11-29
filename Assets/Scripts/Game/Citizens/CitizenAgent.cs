using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using DG.Tweening;
using External.Util;
using Game.Citizens.States;
using Game.POI;
using Game.Production;
using Game.Production.POI;
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
        public CitizenMoveToWorkSpotState MoveToWorkSpotState;

        public Vector3 wanderAnchor = Vector3.zero;
        public float wanderRange = 5f;

        public ICitizenWorkPlace WorkPlace;
        public IProductDepositer ProductDepositer;

        public Transform rightArm;

        [FormerlySerializedAs("LoadedFromData")] public bool loadedFromData = false;
        private static readonly int Velocity = Animator.StringToHash("Velocity");

        private void Start()
        {
            if (loadedFromData)
                return;
            navMeshAgent = GetComponent<NavMeshAgent>();
            _animator = GetComponentInChildren<Animator>();

            StateMachine = new CitizenStateMachine();
            CarryResourcesState = new CitizenCarryResourcesState(this, StateMachine);
            WanderState = new CitizenWanderState(this, StateMachine);
            GoWorkState = new CitizenGoWorkState(this, StateMachine);
            WorkState = new CitizenWorkState(this, StateMachine);
            MoveToWorkSpotState = new CitizenMoveToWorkSpotState(this, StateMachine);

            if (!loadedFromData)
            {
                StartCoroutine(StateMachine.Init(WanderState));
            }
        }

        public void Load()
        {
            navMeshAgent = GetComponent<NavMeshAgent>();
            _animator = GetComponentInChildren<Animator>();

            StateMachine = new CitizenStateMachine();
            CarryResourcesState = new CitizenCarryResourcesState(this, StateMachine);
            WanderState = new CitizenWanderState(this, StateMachine);
            GoWorkState = new CitizenGoWorkState(this, StateMachine);
            WorkState = new CitizenWorkState(this, StateMachine);
            MoveToWorkSpotState = new CitizenMoveToWorkSpotState(this, StateMachine);
        }

        public StoredCitizenData Serialize()
        {
            return new StoredCitizenData
            {
                baseInventoryCapacity = inventoryCapacity,
                BuildingGuid = WorkPlace == null ? Guid.Empty : Guid.Parse(((ResourceContainingPOI)WorkPlace).pointId),
                citizenId = citizenId,
                Inventory = Inventory.ToDictionary(it => it.Key.Formatted(), it => it.Value),
                persistentData = new StoredPersistentCitizenData
                {
                    awards = PersistentData.Awards,
                    caste = PersistentData.Profession,
                    name = PersistentData.Name
                },
                position = transform.position.Ser(),
                state = StateMachine.CurrentState switch
                {
                    CitizenCarryResourcesState => "carry",
                    CitizenGoWorkState => "gowork",
                    CitizenMoveToWorkSpotState => "movetospot",
                    CitizenWanderState => "wander",
                    CitizenWorkState => "work",
                    _ => throw new ArgumentOutOfRangeException()
                },
                wanderAnchor = wanderAnchor.Ser()
            };
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
            
            _animator.SetFloat(Velocity, navMeshAgent.velocity.sqrMagnitude);
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
            WorkPlace.Fire(this);
            WorkPlace = null;
            StartCoroutine(StateMachine.ChangeState(WanderState));
        }

        public void MarkHiredAt(ICitizenWorkPlace workPlace)
        {
            WorkPlace = workPlace;
            Order(GoWorkState);
        }

        public void SetAnimatorBool(int hash, bool value)
        {
            if (_animator == null)
                return;
            _animator.SetBool(hash, value);
        }

        public void SetAnimatorTrigger(int hash)
        {
            if (_animator == null)
                return;
            _animator.SetTrigger(hash);
        }

        public void PlayAnimation(string anim)
        {
            if (_animator == null)
                return;
            _animator.Play(anim);
        }

        public IEnumerator WaitForArrival()
        {
            yield return new WaitUntil(() => navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance);
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

        public IEnumerator SpawnItem(GameObject prefab)
        {
            if (rightArm.childCount > 0)
                yield return RemoveItem();
            var obj = Instantiate(prefab, rightArm);
            var originalScale = obj.transform.localScale;
            obj.transform.localScale = Vector3.zero;
            yield return obj.transform.DOScale(originalScale, 0.35f).SetEase(Ease.OutExpo).Play().WaitForCompletion();
        }

        public IEnumerator RemoveItem()
        {
            if (rightArm.childCount <= 0)
                yield break;
            var tf = rightArm.GetChild(0);
            tf.DOKill();
            yield return tf.DOScale(Vector3.zero, 0.4f).SetEase(Ease.OutExpo)
                .OnComplete(() => Destroy(rightArm.GetChild(0).gameObject)).Play().WaitForCompletion();
        }
    }

    [Serializable]
    public class StoredCitizenData
    {
        [FormerlySerializedAs("PersistentData")] [JsonProperty("persistent")]
        public StoredPersistentCitizenData persistentData;
        public int citizenId;
        [FormerlySerializedAs("Position")] [JsonProperty("position")]
        public SerVec3 position;
        [JsonProperty("inventory")]
        public Dictionary<string, int> Inventory;
        [FormerlySerializedAs("BaseInventoryCapacity")] [JsonProperty("inventoryCapacity")]
        public int baseInventoryCapacity;
        [JsonProperty("assignedBuilding")]
        public Guid BuildingGuid;
        [FormerlySerializedAs("State")] [JsonProperty("state")]
        public string state;
        [FormerlySerializedAs("WanderAnchor")] [JsonProperty("wanderAnchor")]
        public SerVec3 wanderAnchor;
    }
}