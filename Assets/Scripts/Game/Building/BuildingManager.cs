using System;
using System.Collections;
using DG.Tweening;
using External.Util;
using Game.Controllers;
using UnityEngine;
using UnityEngine.EventSystems;
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
        [FormerlySerializedAs("_isBuilding")] public bool isBuilding;
        public bool isReady = true;
        private static readonly int BaseColor = Shader.PropertyToID("_BaseColor");
        private static readonly int FadeFactor = Shader.PropertyToID("_FadeFactor");

        private void Start()
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
            isReady = false;
            
            isBuilding = true;
            currentBuildingData = data;
            currentBuilding = Instantiate(currentBuildingData.buildingPrefab, transform).transform;
            currentBuilding.position = SnapToGrid(TownCameraController.Instance.MouseToWorld());
            // todo: handle more than one material?
            var rd = currentBuilding.GetComponentInChildren<Renderer>();
            _buildingMaterial = new Material(rd.material);
            // Debug.Log(_buildingMaterial.shader);
            rd.material = holographicMaterial;
            Instantiate(gridPlanePrefab, currentBuilding);
        }

        public void CancelBuilding()
        {
            isBuilding = false;
            Destroy(currentBuilding.gameObject);
            currentBuildingData = null;
            isReady = true;
        }

        public IEnumerator PlaceBuilding()
        {
            if (!isBuilding || _isRotating)
                yield break;

            // TODO: check several preconditions: aligned to road, near water, etc.
            var buildingItself = currentBuilding.GetChild(0).GetChild(0);
            
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
            
            // then we fade out the grid
            var buildingRenderer = currentBuilding.transform.GetChild(1).GetComponent<Renderer>().material.DOFloat(40f, FadeFactor, 0.3f)
                .SetEase(Ease.OutExpo).OnComplete(() =>
                {
                    Destroy(currentBuilding.transform.GetChild(1).gameObject);
                }).Play();
            
            // then we tween the scale
            var originalScale = currentBuilding.localScale;
            currentBuilding.localScale = new Vector3(originalScale.x, 0f, originalScale.z);
            yield return currentBuilding.DOScale(originalScale, 0.5f).SetEase(Ease.OutBack).SetDelay(1.5f).OnStart(() =>
            {
                rd.enabled = true;
            }).OnComplete(() =>
            {
                particles.Stop();
                Destroy(particles.gameObject, 1.5f);
                buildingItself.SetParent(null);
                Destroy(currentBuilding.gameObject);
                currentBuilding = null;
                isBuilding = false;
                _buildingMaterial = null;
                isReady = true;
            }).Play().WaitForCompletion();
            
        }

        public void Rotate(bool counterClockwise = false)
        {
            if (_isRotating)
                return;

            _isRotating = true;
            var pivot = currentBuilding.GetChild(0);
            pivot.DOLocalRotate(pivot.rotation.eulerAngles + new Vector3(0f, 90 * Mathu.BSign(counterClockwise), 0f),
                0.25f, RotateMode.FastBeyond360).OnComplete(() => _isRotating = false).SetEase(Ease.OutExpo).Play();
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
}