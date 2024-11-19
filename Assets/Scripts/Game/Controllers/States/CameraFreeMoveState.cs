using System.Collections;
using UnityEngine;

namespace Game.Controllers.States
{
    public class CameraFreeMoveState: CameraState
    {
        private Vector3 _nextPos;
        private Quaternion _nextRot;
        private Vector3 _nextZoom;
        private Vector3 _zoomVelocity;

        private Vector3 _dragStartPos;
        private Vector3 _dragCurrent;
        private Vector3 _rotStartPos;
        private Vector3 _rotCurrent;
        
        public CameraFreeMoveState(CameraStateMachine stateMachine, TownCameraController cameraController) : base(stateMachine, cameraController)
        {
            
        }

        public override IEnumerator ExitState()
        {
            yield break;
        }

        public override IEnumerator EnterState()
        {
            _nextPos = CameraController.transform.position;
            _nextRot = CameraController.transform.rotation;
            _nextZoom = CameraController.cameraTransform.localPosition;
            yield break;
        }

        public override void LateFrameUpdate()
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

                    _nextPos = CameraController.transform.position + (_dragStartPos - _dragCurrent);
                }

            }
            
            // zoom
            if (Input.mouseScrollDelta.y != 0)
            {
                _nextZoom += -Input.mouseScrollDelta.y * 5f * new Vector3(0f, CameraController.zoomSpeed, -CameraController.zoomSpeed);
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
            var axisVert = Input.GetAxisRaw("Vertical") * CameraController.moveSpeed;
            var axisHorizontal = Input.GetAxisRaw("Horizontal") * CameraController.moveSpeed;
            _nextPos += CameraController.transform.forward * axisVert + CameraController.transform.right * axisHorizontal;

            if (Input.GetKey(KeyCode.Q))
                _nextRot *= Quaternion.Euler(1.5f * CameraController.rotationSpeed * Vector3.up);
            else if(Input.GetKey(KeyCode.E))
                _nextRot *= Quaternion.Euler(1.5f * -CameraController.rotationSpeed * Vector3.up);

            if (Input.GetKey(KeyCode.Minus) || Input.GetKey(KeyCode.KeypadMinus))
                _nextZoom += new Vector3(0f, CameraController.zoomSpeed, -CameraController.zoomSpeed);
            if (Input.GetKey(KeyCode.Plus) || Input.GetKey(KeyCode.KeypadPlus))
                _nextZoom += new Vector3(0f, -CameraController.zoomSpeed, CameraController.zoomSpeed);
        }

        private void CommitMovement()
        {
            CameraController.transform.position = Vector3.Lerp(CameraController.transform.position, _nextPos, Time.unscaledDeltaTime * CameraController.moveTime);
            CameraController.transform.rotation = Quaternion.Lerp(CameraController.transform.rotation, _nextRot, Time.unscaledDeltaTime * CameraController.moveTime);

            var zoomAmt = Mathf.Abs(_nextZoom.y);
            var sign = Mathf.Sign(_nextZoom.y);
            if (zoomAmt > CameraController.zoomMax)
            {
                _nextZoom = Vector3.SmoothDamp(_nextZoom, new Vector3(0f, sign * CameraController.zoomMax, -sign * CameraController.zoomMax),
                    ref _zoomVelocity, CameraController.zoomDampen);
            } else if (zoomAmt < CameraController.zoomMin)
            {
                _nextZoom = Vector3.SmoothDamp(_nextZoom, new Vector3(0f, sign * CameraController.zoomMin, -sign * CameraController.zoomMin),
                    ref _zoomVelocity, CameraController.zoomDampen / 4f);
                if (zoomAmt * sign <= 10f)
                {
                    _nextZoom = new Vector3(0f, 10f, -10f);
                }
            }

            CameraController.cameraTransform.localPosition =
                Vector3.Lerp(CameraController.cameraTransform.localPosition, _nextZoom, Time.unscaledDeltaTime * CameraController.moveTime);
        }
    }
}