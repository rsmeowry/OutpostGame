using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace Game.Controllers
{
    public class TownCameraController: MonoBehaviour
    {
        [SerializeField]
        private float moveSpeed = 2f;
        [SerializeField]
        private float moveTime = 2f;
        [FormerlySerializedAs("rotationTime")] [SerializeField]
        private float rotationSpeed = 0.6f;
        [SerializeField] 
        private float zoomSpeed = 0.6f;

        [SerializeField]
        private float zoomMin = 10f;

        [SerializeField]
        private float zoomMax = 30f;
        
        [SerializeField]
        private float zoomDampen = 2f;

        [SerializeField]
        private Transform cameraTransform;

        private Vector3 _nextPos;
        private Quaternion _nextRot;
        private Vector3 _nextZoom;
        private Vector3 _zoomVelocity;

        private Vector3 _dragStartPos;
        private Vector3 _dragCurrent;
        private Vector3 _rotStartPos;
        private Vector3 _rotCurrent;

        private void Start()
        {
            _nextPos = transform.position;
            _nextRot = transform.rotation;
            _nextZoom = cameraTransform.localPosition;
        }

        private void LateUpdate()
        {
            ClickHandler();
            MouseMovement();
            KeyboardMovement();
            CommitMovement();
        }

        private void ClickHandler()
        {
            if (Input.GetMouseButtonDown(0))
            {
                var ray = Camera.main!.ScreenPointToRay(Input.mousePosition);
                var h = Physics.SphereCast(ray, 0.3f, out var hit);
                if (h && hit.transform.TryGetComponent<ICameraClickable>(out var clickable))
                {
                    clickable.OnClick();
                }
            }
        }

        private void MouseMovement()
        {
            // move
            if (Input.GetMouseButtonDown(2))
            {
                var plane = new Plane(Vector3.up, Vector3.zero);
                var ray = Camera.main!.ScreenPointToRay(Input.mousePosition);

                float entry;

                if (plane.Raycast(ray, out entry))
                {
                    _dragStartPos = ray.GetPoint(entry);
                }
                // used to be just if 
            } else if (Input.GetMouseButton(2))
            {
                var plane = new Plane(Vector3.up, Vector3.zero);
                var ray = Camera.main!.ScreenPointToRay(Input.mousePosition);

                float entry;

                if (plane.Raycast(ray, out entry))
                {
                    _dragCurrent = ray.GetPoint(entry);

                    _nextPos = transform.position + (_dragStartPos - _dragCurrent);
                }

            }
            
            // zoom
            if (Input.mouseScrollDelta.y != 0)
            {
                _nextZoom += -Input.mouseScrollDelta.y * 5f * new Vector3(0f, zoomSpeed, -zoomSpeed);
            }
            
            // rotate
            if (Input.GetMouseButtonDown(1))
            {
                _rotStartPos = Input.mousePosition;
            } else if (Input.GetMouseButton(1))
            {
                _rotCurrent = Input.mousePosition;

                var diff = _rotStartPos - _rotCurrent;
                _rotStartPos = _rotCurrent;
                
                _nextRot *= Quaternion.Euler(Vector3.up * (-diff.x / 10f));
            }

        }

        private void KeyboardMovement()
        {
            var axisVert = Input.GetAxisRaw("Vertical") * moveSpeed;
            var axisHorizontal = Input.GetAxisRaw("Horizontal") * moveSpeed;
            _nextPos += transform.forward * axisVert + transform.right * axisHorizontal;

            if (Input.GetKey(KeyCode.Q))
                _nextRot *= Quaternion.Euler(Vector3.up * rotationSpeed);
            else if(Input.GetKey(KeyCode.E))
                _nextRot *= Quaternion.Euler(Vector3.up * -rotationSpeed);

            if (Input.GetKey(KeyCode.Plus) || Input.GetKey(KeyCode.KeypadPlus))
                _nextZoom += new Vector3(0f, zoomSpeed, -zoomSpeed);
            if (Input.GetKey(KeyCode.Minus) || Input.GetKey(KeyCode.KeypadMinus))
                _nextZoom += new Vector3(0f, -zoomSpeed, zoomSpeed);
        }

        private void CommitMovement()
        {
            transform.position = Vector3.Lerp(transform.position, _nextPos, Time.deltaTime * moveTime);
            transform.rotation = Quaternion.Lerp(transform.rotation, _nextRot, Time.deltaTime * moveTime);

            var zoomAmt = Mathf.Abs(_nextZoom.y);
            var sign = Mathf.Sign(_nextZoom.y);
            if (zoomAmt > zoomMax)
            {
                _nextZoom = Vector3.SmoothDamp(_nextZoom, new Vector3(0f, sign * zoomMax, -sign * zoomMax),
                    ref _zoomVelocity, zoomDampen);
            } else if (zoomAmt < zoomMin)
            {
                _nextZoom = Vector3.SmoothDamp(_nextZoom, new Vector3(0f, sign * zoomMin, -sign * zoomMin),
                    ref _zoomVelocity, zoomDampen / 4f);
            }

            cameraTransform.localPosition =
                Vector3.Lerp(cameraTransform.localPosition, _nextZoom, Time.deltaTime * moveTime);
        }
    }
}
