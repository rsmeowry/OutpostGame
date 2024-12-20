﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using External.Util;
using Game.Controllers;
using Game.POI;
using Game.POI.Deco;
using Game.Production.POI;
using Game.Production.Products;
using Game.Sound;
using Game.State;
using Newtonsoft.Json;
using Tutorial;
using UI.POI;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

namespace Game.Building
{
    public class BuildingManager: MonoBehaviour
    {
        public static BuildingManager Instance { get; private set; }
        
        [SerializeField]
        public LayerMask terrainLayer;

        [SerializeField]
        private Camera cam;

        [SerializeField]
        private float lerpedSnappingTime = 5f;

        [ColorUsage(false, true)]
        [SerializeField]
        private Color canPlaceColor;
        [ColorUsage(false, true)]
        [SerializeField]
        private Color cantPlaceColor;

        [SerializeField]
        private GameObject gridPlanePrefab;

        [SerializeField]
        private Material holographicMaterial;

        [SerializeField]
        private ParticleSystem buildingParticlesPrefab;

        [SerializeField]
        private GameObject targetBuildingContainer;

        [SerializeField]
        private LayerMask waterLayer;

        [SerializeField]
        private GameObject electricityGridPlanePrefab;
        
        private Grid _grid;

        public Transform currentBuilding;
        public BuildingData currentBuildingData;
        private Dictionary<Transform, Material> _buildingMaterials = new();
        private bool _isRotating;
        private bool _hasCollisions;
        private bool _isConstructing;
        private Vector3Int _currentCellSpot;
        [FormerlySerializedAs("_isBuilding")] public bool isBuilding; 
        private static readonly int BaseColor = Shader.PropertyToID("_BaseColor");
        private static readonly int GridColor = Shader.PropertyToID("_GridColor");
        private static readonly int HoloColor = Shader.PropertyToID("_Color");
        
        // serialized data below
        public Dictionary<Vector3Int, BuiltObject> BuiltObjects = new();

        public BuiltObject ObjectAt(Vector3 pos)
        {
            return BuiltObjects[_grid.WorldToCell(pos)];
        }

        private void Awake()
        {
            Instance = this;
            _grid = GetComponent<Grid>();
        }
        
        public Vector3 SnapToGrid(Vector3 pos)
        {
            return _grid.CellToWorld(_grid.WorldToCell(pos));
        }

        public Vector3 LerpedSnap(Vector3 self, Vector3 pos)
        {
            return Vector3.Lerp(self, SnapToGrid(pos), Time.unscaledDeltaTime * lerpedSnappingTime);
        }

        public void BeginBuilding(BuildingData data)
        {
            isBuilding = true;
            currentBuildingData = data;
            currentBuilding = Instantiate(currentBuildingData.buildingPrefab, transform).transform;
            currentBuilding.GetChild(0).GetChild(0).transform.localScale *= 1.1f;
            if (currentBuilding.GetChild(0).GetChild(0).TryGetComponent<PointOfInterest>(out var poi))
            {
                poi.enabled = false;
            }
            currentBuilding.position = SnapToGrid(TownCameraController.Instance.MouseToWorld());
            var rds = currentBuilding.GetComponentsInChildren<Renderer>();
            foreach (var rd in rds)
            {
                _buildingMaterials[rd.transform] = new Material(rd.material);
                rd.material = holographicMaterial;
            }
            Instantiate(data.keyId == "substation" ? electricityGridPlanePrefab : gridPlanePrefab, currentBuilding);

            _collisionState = CollisionState.Unchecked;
            CheckPlaceableConditions();
        }

        public void CancelBuilding()
        {
            isBuilding = false;
            Destroy(currentBuilding.gameObject);
            currentBuildingData = null;
        }

        public void MoveBuilding(Vector3 newPos)
        {
            if (_isConstructing)
                return;
            newPos.y = Mathf.Max(-3f, newPos.y);
            currentBuilding.position = newPos;
        }

        public void CheckPlaceableConditions()
        {
            var newCellSpot = _grid.WorldToCell(TownCameraController.Instance.BuildingState.mouseHitPos);
            if (newCellSpot != _currentCellSpot)
            {
                _currentCellSpot = newCellSpot;
                DoCheckCollisions(newCellSpot);
            }
        }

        private CollisionState _collisionState = CollisionState.Unchecked;
        private void DoCheckCollisions(Vector3Int cellSpot)
        {
            if (currentBuilding.GetChild(0).GetChild(0).TryGetComponent<IBuildingWithConditions>(out var cond) &&
                !cond.CanBePlacedAt(_grid.CellToWorld(cellSpot)))
            {
                _hasCollisions = true;
            }
            else
            {
                _hasCollisions = HasCollisions(cellSpot) || HasWaterCollisions(cellSpot);
            }

            try
            {
                if (_collisionState != CollisionState.NoCollision && !_hasCollisions)
                {
                    // dont have collisions anymore, restore materials
                    var buildRd = currentBuilding.GetChild(0).GetChild(0).GetComponentInChildren<Renderer>();
                    buildRd.material.SetColor(HoloColor, canPlaceColor);
                    var childCount = currentBuilding.childCount;
                    var gridRd = currentBuilding.GetChild(1).GetComponent<Renderer>();
                    if(currentBuildingData.keyId != "substation")
                        gridRd.material.SetColor(GridColor, canPlaceColor);
                    _collisionState = CollisionState.NoCollision;
                }
                else if (_collisionState != CollisionState.HadCollision && _hasCollisions)
                {
                    // we got a collision! change materials
                    var buildRd = currentBuilding.GetChild(0).GetChild(0).GetComponentInChildren<Renderer>();
                    buildRd.material.SetColor(HoloColor, cantPlaceColor);
                    var gridRd = currentBuilding.GetChild(1).GetComponent<Renderer>();
                    if(currentBuildingData.keyId != "substation")
                        gridRd.material.SetColor(GridColor, canPlaceColor);
                    _collisionState = CollisionState.HadCollision;
                }
            }
            catch (UnityException _)
            {
                // Nothing bad happened, that just means the grid is already destroyed
            }
            // otherwise nothing changed
        }

        public Transform InstantBuild(BuildingData data, Vector3 position, Vector3 rotation)
        {
            currentBuildingData = data;
            currentBuilding = Instantiate(currentBuildingData.buildingPrefab, transform).transform;
            currentBuilding.transform.position = position;
            var pivot = currentBuilding.GetChild(0);
            pivot.rotation = Quaternion.Euler(rotation);

            var buildingItself = currentBuilding.GetChild(0).GetChild(0);
            var gridPos = _grid.WorldToCell(currentBuilding.position);
            gridPos.y = 0;
            var rSize = RotatedSize();
            BuiltObjects[gridPos] = new BuiltObject
            {
                Id = 0,
                CollisionArea = (rSize.x, rSize.y),
                Rotation = (int) rotation.y,
                BuildingType = currentBuildingData.Id.Formatted()
            };
            if (buildingItself.TryGetComponent<PointOfInterest>(out var poi))
            {
                poi.buildingData = BuiltObjects[gridPos];
                poi.pointId = Guid.NewGuid().ToString();
                poi.OnBuilt();
            }
            buildingItself.SetParent(targetBuildingContainer.transform);
            
            var lPos = buildingItself.localPosition;
            var hit = new RaycastHit[1];
            Physics.RaycastNonAlloc(lPos + Vector3.up * 10, Vector3.down, hit, 50);
            var y = hit[0].point.y;
            lPos.y = Mathf.Max(-3f, y);
            buildingItself.localPosition = lPos;
            
            Destroy(currentBuilding.gameObject);
            currentBuilding = null;
            currentBuildingData = null;
            return buildingItself;
        }

        public IEnumerator PlaceBuilding(Action<bool> callback)
        {
            if (!isBuilding || _isRotating || _isConstructing)
            {
                callback(false);
                yield break;
            }
            
            // TODO: maybe like a pulse effect to show you cant build?
            if (_hasCollisions)
            {
                callback(false);
                yield break;
            }

            // mark that construction began
            _isConstructing = true;

            var buildingItself = currentBuilding.GetChild(0).GetChild(0);

            // scale it down
            buildingItself.localScale /= 1.1f;
            
            currentBuilding.position = SnapToGrid(TownCameraController.Instance.MouseToWorld());

            // hide the building first
            foreach (var rd in buildingItself.GetComponentsInChildren<Renderer>())
            {
                rd.material = _buildingMaterials[rd.transform];
                rd.enabled = false;
            }
            _buildingMaterials.Clear();
            
            // placing animation
            // TODO: sounds
            // first do particles
            var particles = Instantiate(buildingParticlesPrefab, transform);
            var particlesPos = buildingItself.position;
            particlesPos.y = currentBuilding.position.y;
            particles.transform.position = particlesPos;
            particles.Play();
            particles.GetComponent<ParticleSystemRenderer>().material.SetColor(BaseColor, currentBuildingData.prominentColor);
            
            // then we remove the grid
            Destroy(currentBuilding.GetChild(1).gameObject);
            
            // then we tween the scale
            var originalScale = currentBuilding.localScale;
            currentBuilding.localScale = new Vector3(originalScale.x, 0f, originalScale.z);

            var gridPos = _grid.WorldToCell(currentBuilding.position);
            gridPos.y = 0;
            
            // take the resources
            foreach (var req in currentBuildingData.requirements)
            {
                GameStateManager.Instance.IncreaseProduct(StateKey.FromString(req.key), -req.count);
            }
            
            // play the sound
            SoundManager.Instance.PlaySoundAt(SoundBank.Instance.GetSound("building.build"), currentBuilding.transform.position, 1f, Random.Range(0.8f, 1.2f));
            
            yield return currentBuilding.DOScale(originalScale, 0.5f).SetEase(Ease.OutBack).SetDelay(1.5f).OnStart(() =>
            {
                foreach (var rd in buildingItself.GetComponentsInChildren<Renderer>())
                {
                    rd.enabled = true;
                }
            }).OnComplete(() =>
            {
                var rSize = RotatedSize();
                BuiltObjects[gridPos] = new BuiltObject
                {
                    Id = 0,
                    CollisionArea = (rSize.x, rSize.y),
                    Rotation = Mathf.RoundToInt(buildingItself.localRotation.eulerAngles.y),
                    BuildingType = currentBuildingData.Id.Formatted()
                };
                if (buildingItself.TryGetComponent<PointOfInterest>(out var poi))
                {
                    poi.buildingData = BuiltObjects[gridPos];

                    var poiId = Guid.NewGuid();
                    poi.pointId = poiId.ToString();
                    POIManager.Instance.LoadedPois[poiId] = poi;
                    poi.enabled = true;
                    poi.OnBuilt();
                    
                    TutorialCtl.Instance.ActiveStep?.ReceiveBuildingBuilt(currentBuildingData);

                    if (poi is GatheringPost)
                    {
                        POIManager.Instance.RecalculateAllGatheringPosts();
                    }
                }

                particles.Stop();
                Destroy(particles.gameObject, 1.5f);
                buildingItself.SetParent(targetBuildingContainer.transform);
                
                var lPos = buildingItself.localPosition;
                var hit = new RaycastHit[1];
                Physics.RaycastNonAlloc(lPos + Vector3.up * 100, Vector3.down, hit, 150, terrainLayer);
                var y = hit[0].point.y;
                lPos.y = Mathf.Max(-3f, y);
                buildingItself.localPosition = lPos;

                Destroy(currentBuilding.gameObject);
                currentBuilding = null;
                isBuilding = false; 
                callback(true);
                _isConstructing = false;
                _hasCollisions = true;
                _currentCellSpot.x = -10000; // shouldnt really occur, so we are fine
            }).Play().WaitForCompletion();
        }
        
        public void Rotate(bool counterClockwise = false)
        {
            if (_isRotating)
                return;

            _isRotating = true;
            var pivot = currentBuilding.GetChild(0);
            pivot.DOLocalRotate(pivot.rotation.eulerAngles + new Vector3(0f, 90 * Mathu.BSign(counterClockwise), 0f),
                0.25f, RotateMode.FastBeyond360).OnComplete(() =>
            {
                _isRotating = false;
                DoCheckCollisions(_currentCellSpot);
            }).SetEase(Ease.OutExpo).Play();
        }

        private bool HasWaterCollisions(Vector3 snappedPos)
        {
            var pos = new Vector3Int((int) snappedPos.x, (int) snappedPos.y, (int) snappedPos.z);
            var rotatedSize = RotatedSize();
            var signX = Math.Sign(rotatedSize.x);
            var signY = Math.Sign(rotatedSize.y);
            
            HashSet<Vector3Int> requiredWaterSpots = new();
            if (currentBuildingData.requiresWater)
            {
                // rotation pivot -> (1) water test positions
                var ot = currentBuilding.transform.GetChild(0);
                if (ot.childCount >= 2)
                {
                    var waterTestPositions = ot.GetChild(1);
                    for (var i = 0; i < waterTestPositions.childCount; i++)
                    {
                        var posDelta = waterTestPositions.GetChild(i).position - currentBuilding.position;
                        posDelta.x = Mathf.Floor(posDelta.x / 10);
                        posDelta.z = Mathf.Floor(posDelta.z / 10);
                        var ps = pos + posDelta;
                        ps.y = 0;
                        requiredWaterSpots.Add(new Vector3Int((int)ps.x, (int)ps.y, (int)ps.z));
                    }
                }
            }
            
            foreach (var x in Enumerable.Range(0, Math.Abs(rotatedSize.x)))
            {
                foreach (var y in Enumerable.Range(0, Math.Abs(rotatedSize.y)))
                {
                    var delta = new Vector3Int(signX * x, 0, signY * y);
                    var realPos = pos + delta;
                    var waterPos = realPos;
                    waterPos.y = 0;
                    var hitWater = IsWater(_grid.CellToWorld(realPos));
                    if (hitWater)
                    {
                        // if we are supposed to be over water its fine
                        if (requiredWaterSpots.Contains(waterPos))
                            continue;
                        
                        return true;
                    } else if (requiredWaterSpots.Contains(waterPos))
                        // if we are supposed to be over water and we arent, thats bad
                        return true;
                }
            }

            return false;
        }

        [ContextMenu("Test/HELP")]
        private void Test()
        {
            Debug.Log(_grid.WorldToCell(new Vector3(10000, 10, 1000)).ToString());
            Debug.Log(_grid.WorldToCell(new Vector3(1000, 10, 1000)).ToString());
            Debug.Log(_grid.WorldToCell(new Vector3(960, 10, 950)).ToString());
        }

        private bool IsObstacle(Vector3 worldPos)
        {
            var wp = worldPos;
            wp.y = 20f;
            wp += new Vector3(5, 0, 5);
            var hits = new RaycastHit[4];
            Physics.RaycastNonAlloc(wp, Vector3.down, hits, 23f);
            return hits.Any(it => it.collider && it.collider.CompareTag("Obstacle"));
        }

        private bool IsWater(Vector3 worldPos)
        {
            Debug.Log($"RAYCASTING WORLD {worldPos}");
            var wp = worldPos;
            wp.y = 20f;
            wp += new Vector3(5, 0, 5);
            // centering it
            var hits = new RaycastHit[4];
            Physics.RaycastNonAlloc(wp, Vector3.down, hits, 23f);
            Debug.Log($"HITS: {hits.Select(it => it.collider == null ? "null" : it.collider.name).ToCommaSeparatedString()}");
            return hits.Any(it => it.collider && it.collider.CompareTag("Water")) && !hits.Any(it => it.collider && it.collider.name.Contains("Terrain"));
        }

        private bool HasCollisions(Vector3 snappedPos)
        {
            var pos = new Vector3Int((int) snappedPos.x, 0, (int) snappedPos.z);

            // size is (x: 3; y: 2)
            // we check:
            // x --->
            // y
            // |  * * * *
            // |  # A * *
            // |  # # * *
            // V  * * * *
            // where A is anchor
            // A + 0; 0
            // A + 0; 1
            // A + -1; 0
            // A + -1; 1
            var rotatedSize = RotatedSize();
            var signX = Math.Sign(rotatedSize.x);
            var signY = Math.Sign(rotatedSize.y);

            var occupied = OccupiedSpotsInArea(pos);

            if (occupied.Contains(pos))
                return true;
            
            foreach (var x in Enumerable.Range(0, Math.Abs(rotatedSize.x)))
            {
                foreach (var y in Enumerable.Range(0, Math.Abs(rotatedSize.y)))
                {
                    var delta = new Vector3Int(signX * x, 0, signY * y);
                    var newPos = pos + delta;
                    if (occupied.Contains(newPos) || IsObstacle(_grid.CellToWorld(newPos)))
                        return true;
                }
            }

            return false;
        }

        private HashSet<Vector3Int> OccupiedSpotsInArea(Vector3Int point, int range = 5)
        {
            var minPoint = point - new Vector3Int(range, 0, range);
            var maxPoint = point + new Vector3Int(range, 0, range);
            return BuiltObjects.Where(it => it.Key.GreaterThan(minPoint) && it.Key.LessThan(maxPoint))
                .SelectMany(it => GetCollisionAreaPoints(it.Key, it.Value.CollisionArea)).ToHashSet();
        }

        private HashSet<Vector3Int> GetCollisionAreaPoints(Vector3Int point, (int, int) area)
        {
            var signX = Math.Sign(area.Item1);
            var signY = Math.Sign(area.Item2);

            var acc = new HashSet<Vector3Int>();
            acc.Add(point);
            
            // if the area is 2 by 2
            // we do the offsets
            // 0 0 - 0 1 - 1 0 - 1 1
            // if the area is -2 by 2
            // we do the offsets
            // 0 0 - -1 0 - 0 1 - -1 1
            // if the area is -2 by -2
            // we do the offsets
            // 0 0 - -1 0 - 0 -1 - -1 -1
            foreach (var x in Enumerable.Range(0, Math.Abs(area.Item1)))
            {
                foreach (var y in Enumerable.Range(0, Math.Abs(area.Item2)))
                {
                    var delta = new Vector3Int(signX * x, 0, signY * y);
                    acc.Add(point + delta);
                }
            }

            return acc;
        }

        private Vector2Int RotatedSize()
        {
            if (_isRotating)
                return currentBuildingData.size;
            var pivot = currentBuilding.GetChild(0);
            var normalizedAngle = 360 - Mathf.RoundToInt(pivot.localRotation.eulerAngles.y) % 360;
            int signX;
            int signY;
            
            // if we have w:2 h:3, and rotate it
            // 0 - w:2 h:3
            // 90 - w:-3 h:2
            // 180 - w:-2 h:-3
            // 270 - w:3 h:-2
            // 360 - w:2 h:3

            switch (normalizedAngle)
            {
                case 0:
                    return currentBuildingData.size;
                case 90:
                    return new Vector2Int(-currentBuildingData.size.y, currentBuildingData.size.x);
                case 180:
                    return new Vector2Int(-currentBuildingData.size.x, -currentBuildingData.size.y);
                case 270:
                    return new Vector2Int(currentBuildingData.size.y, -currentBuildingData.size.x);
                case 360:
                    return currentBuildingData.size;
                default:
                    throw new ArgumentOutOfRangeException(nameof(normalizedAngle), normalizedAngle, "bwa");
            }
            return new Vector2Int(currentBuildingData.size.x * signX, currentBuildingData.size.y * signY);
        }

        private void Update()
        {
            if (!isBuilding)
                return;
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                CancelBuilding();
                StartCoroutine(TownCameraController.Instance.StateMachine.SwitchState(TownCameraController.Instance.FreeMoveState));
            }
        }
    }

    internal enum CollisionState
    {
        HadCollision,
        NoCollision,
        Unchecked
    }

    [Serializable]
    public struct BuiltObject
    {
        [JsonProperty("id")]
        public int Id;
        [JsonProperty("collisionArea")]
        public (int, int) CollisionArea;
        [JsonProperty("rotation")]
        public int Rotation;
        [JsonProperty("buildingType")]
        public string BuildingType;
    }
}