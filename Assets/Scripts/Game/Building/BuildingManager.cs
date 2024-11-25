using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using External.Util;
using Game.Controllers;
using Game.POI;
using Game.State;
using Newtonsoft.Json;
using UI.POI;
using UnityEngine;
using UnityEngine.Serialization;

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
        
        private Grid _grid;

        public Transform currentBuilding;
        public BuildingData currentBuildingData;
        private Material _buildingMaterial;
        private bool _isRotating;
        private bool _hasCollisions;
        private bool _isConstructing;
        private Vector3Int _currentCellSpot;
        [FormerlySerializedAs("_isBuilding")] public bool isBuilding; 
        private static readonly int BaseColor = Shader.PropertyToID("_BaseColor");
        private static readonly int GridColor = Shader.PropertyToID("_GridColor");
        private static readonly int HoloColor = Shader.PropertyToID("_Color");
        
        // serialized data below
        // TODO: read them from save data
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
            currentBuilding.position = SnapToGrid(TownCameraController.Instance.MouseToWorld());
            // todo: handle more than one material?
            var rd = currentBuilding.GetComponentInChildren<Renderer>();
            _buildingMaterial = new Material(rd.material);
            rd.material = holographicMaterial;
            Instantiate(gridPlanePrefab, currentBuilding);
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
            currentBuilding.position = newPos;
        }

        public void CheckCollisionsIfNeeded()
        {
            var newCellSpot = _grid.WorldToCell(TownCameraController.Instance.BuildingState.mouseHitPos);
            if (newCellSpot != _currentCellSpot)
            {
                _currentCellSpot = newCellSpot;
                DoCheckCollisions(newCellSpot);
            }
        }

        private void DoCheckCollisions(Vector3Int cellSpot)
        {
            var hadCollisionsBefore = _hasCollisions;
            _hasCollisions = HasCollisions(cellSpot);
            try
            {
                if (hadCollisionsBefore && !_hasCollisions)
                {
                    // dont have collisions anymore, restore materials
                    var buildRd = currentBuilding.GetChild(0).GetChild(0).GetComponentInChildren<Renderer>();
                    buildRd.material.SetColor(HoloColor, canPlaceColor);
                    var gridRd = currentBuilding.GetChild(1).GetComponent<Renderer>();
                    gridRd.material.SetColor(GridColor, canPlaceColor);
                }
                else if (!hadCollisionsBefore && _hasCollisions)
                {
                    // we got a collision! change materials
                    var buildRd = currentBuilding.GetChild(0).GetChild(0).GetComponentInChildren<Renderer>();
                    buildRd.material.SetColor(HoloColor, cantPlaceColor);
                    var gridRd = currentBuilding.GetChild(1).GetComponent<Renderer>();
                    gridRd.material.SetColor(GridColor, cantPlaceColor);
                }
            }
            catch (UnityException e)
            {
                // Nothing bad happened, that just means the grid is already destroyed
            }
            // otherwise nothing changed
        }

        public Transform InstantBuild(BuildingData data, Vector3 position, Vector3 rotation)
        {
            currentBuildingData = data;
            currentBuilding = Instantiate(currentBuildingData.buildingPrefab, transform).transform;
            Debug.Log(_grid);
            currentBuilding.transform.position = position;
            var pivot = currentBuilding.GetChild(0);
            pivot.rotation = Quaternion.Euler(rotation);

            var buildingItself = currentBuilding.GetChild(0).GetChild(0);
            var gridPos = _grid.WorldToCell(currentBuilding.position);
            var rSize = RotatedSize();
            BuiltObjects[gridPos] = new BuiltObject
            {
                Id = 0,
                CollisionArea = (rSize.x, rSize.y),
                Rotation = Mathf.RoundToInt(buildingItself.rotation.eulerAngles.y),
                BuildingType = currentBuildingData.Id.Formatted()
            };
            if (buildingItself.TryGetComponent<PointOfInterest>(out var poi))
            {
                poi.buildingData = BuiltObjects[gridPos];
                poi.pointId = Guid.NewGuid().ToString();
            }
            buildingItself.SetParent(null);
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

            
            // TODO: check several preconditions: aligned to road, near water, etc.
            var buildingItself = currentBuilding.GetChild(0).GetChild(0);

            // scale it down
            buildingItself.localScale /= 1.1f;
            
            currentBuilding.position = SnapToGrid(TownCameraController.Instance.MouseToWorld());

            // hide the building first
            var rd = buildingItself.TryGetComponent<Renderer>(out var cmp)
                ? cmp
                : buildingItself.GetComponentInChildren<Renderer>();
            rd.material = _buildingMaterial;
            rd.enabled = false;
            
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
            
            yield return currentBuilding.DOScale(originalScale, 0.5f).SetEase(Ease.OutBack).SetDelay(1.5f).OnStart(() =>
            {
                rd.enabled = true;
            }).OnComplete(() =>
            {
                var rSize = RotatedSize();
                BuiltObjects[gridPos] = new BuiltObject
                {
                    Id = 0,
                    CollisionArea = (rSize.x, rSize.y),
                    Rotation = Mathf.RoundToInt(buildingItself.rotation.eulerAngles.y),
                    BuildingType = currentBuildingData.Id.Formatted()
                };
                if (buildingItself.TryGetComponent<PointOfInterest>(out var poi))
                {
                    poi.buildingData = BuiltObjects[gridPos];

                    var poiId = Guid.NewGuid();
                    poi.pointId = poiId.ToString();
                    POIManager.Instance.LoadedPois[poiId] = poi;
                }
                particles.Stop();
                Destroy(particles.gameObject, 1.5f);
                buildingItself.SetParent(null);
                Destroy(currentBuilding.gameObject);
                currentBuilding = null;
                isBuilding = false;
                _buildingMaterial = null;
                callback(true);
                _isConstructing = false;
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

        private bool HasCollisions(Vector3 snappedPos)
        {
            var pos = new Vector3Int((int) snappedPos.x, (int) snappedPos.y, (int) snappedPos.z);

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
                    if (occupied.Contains(newPos))
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