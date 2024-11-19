using System;
using Game.Controllers;
using UnityEngine;

namespace Game.Building
{
    public class BuildingController: MonoBehaviour
    {
        public int gridX;
        public int gridY;

        public Transform selectedObject;
        public Transform anchor;
        public LayerMask terrainLayer;

        private Camera _camera;

        public void Start()
        {
            _camera = Camera.main;
            anchor = selectedObject.GetChild(0);
            nextPos = selectedObject.position;

            intendedPosition = anchor.position;
        }

        private Vector3 nextPos;
        private Vector3 intendedPosition;
        [SerializeField]
        private float updateSpeed = 30f;

        public void FixedUpdate()
        {
            var ray = _camera.ScreenPointToRay(Input.mousePosition);
            var hit = new RaycastHit[1];
            var hitCnt = Physics.RaycastNonAlloc(ray, hit, 50000, terrainLayer);
            if (hitCnt == 0)
                return;

            var h = hit[0];
            var dPos = h.point;
            dPos.y = 2.5f;
            // snap to grid
            dPos.x -= dPos.x % gridX;
            dPos.z -= dPos.z % gridY;
            var anchPos = anchor.position;
            anchPos.x -= anchPos.x % gridX;
            anchPos.y -= anchPos.z % gridY;
            intendedPosition = anchPos;
            selectedObject.position = dPos;
            nextPos =
                Vector3.Lerp(nextPos, anchor.position, updateSpeed * Time.deltaTime);
            selectedObject.position = nextPos;
        }

        public void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                Instantiate(selectedObject);

                var dPos = selectedObject.position;
                dPos.x = Mathf.Floor(dPos.x - dPos.x % gridX);
                dPos.z = Mathf.Floor(dPos.z - dPos.z % gridY);
                dPos.y = 2.5f;
                selectedObject.position = dPos;
                selectedObject.position = anchor.position;
            }
        }
    }
}