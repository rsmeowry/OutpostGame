using System.Collections;
using DG.Tweening;
using UI;
using UI.BottomRow;
using UnityEngine;

namespace Game.Controllers.States
{
    public class CameraFocusedState: CameraState
    {
        private float _previousZoom;
        private Vector3 _originalAnchor;
        
        public CameraFocusedState(CameraStateMachine stateMachine, TownCameraController cameraController) : base(stateMachine, cameraController)
        {
        }

        public override IEnumerator ExitState()
        {
            BottomRowCtl.Instance.StartCoroutine(BottomRowCtl.Instance.ShowTopRow());
            UIManager.Instance.HidePanel();
            
            CameraController.transform.DOMove(_originalAnchor, 1.5f).Play();
            CameraController.GetComponentInChildren<Camera>().DOFieldOfView(40f, 1.5f).SetEase(Ease.Flash).Play();
            CameraController.cameraTransform.DOLocalRotate(new Vector3(45f, 0f, 0f), 1.5f).Play();
            yield return CameraController.cameraTransform.DOLocalMove(new Vector3(0f, _previousZoom, -_previousZoom), 1.5f).SetEase(Ease.Flash).Play()
                .WaitForCompletion();
        }

        public override IEnumerator EnterState()
        {
            BottomRowCtl.Instance.StartCoroutine(BottomRowCtl.Instance.HideTopRow());
            yield return DoFocus();
        }

        private float _animTime = 0.8f;
        private Ease _ease = Ease.InOutSine;
        private IEnumerator DoFocus()
        {
            _originalAnchor = CameraController.transform.position;
            _previousZoom = CameraController.cameraTransform.localPosition.y;
            CameraController.GetComponentInChildren<Camera>().DOFieldOfView(60f, _animTime).SetEase(_ease).Play();
            var lookPos = CameraController.FocusedPOI.lookPos != null
                ? CameraController.FocusedPOI.lookPos.position
                : CameraController.FocusedPOI.transform.position + Vector3.up * 4f;
            CameraController.cameraTransform.DODynamicLookAt(lookPos, _animTime * 2f).SetEase(_ease).Play();
            CameraController.transform.DOMove(CameraController.FocusedPOI.focusPos.transform.position, 1.5f).SetEase(_ease)
                .Play();
            yield return CameraController.cameraTransform.DOLocalMove(Vector3.zero, 1.5f).SetEase(_ease).Play()
                .WaitForCompletion();
        }

        public override void LateFrameUpdate()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                CameraController.StartCoroutine(StateMachine.SwitchState(CameraController.FreeMoveState));
            }
        }
    }
}