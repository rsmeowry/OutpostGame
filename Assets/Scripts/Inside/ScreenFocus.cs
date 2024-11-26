using System;
using DG.Tweening;
using External.Util;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Inside
{
    public class ScreenFocus: MonoBehaviour
    {
        private Material _material;

        [SerializeField] [ColorUsage(false, true)]
        private Color overColor;
        [SerializeField] [ColorUsage(false, true)]
        private Color defaultColor;

        [SerializeField]
        private RectTransform canvas;

        [SerializeField]
        private Transform focusPos;

        [SerializeField]
        private float focusFov;
        
        [SerializeField]
        private InsideCameraController cam;

        private float _prevFov;
        private Vector3 _prevPos;
        
        private void Start()
        {
            _material = GetComponentInChildren<MeshRenderer>().material;
        }

        private void OnDrawGizmos()
        {
            if (focusPos != null)
                Gizmos.DrawLine(focusPos.position, focusPos.position + focusPos.forward * 3f);
        }

        private bool _isFocused;
        private bool _inTransition;
        public void OnPointerClick()
        {
            if (_isFocused || _inTransition)
                return;

            _inTransition = true;
            _prevPos = cam.transform.position;
            _prevFov = cam.GetComponent<Camera>().fieldOfView;

            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            canvas.DOScaleY(0.002f, 0.5f).Play();
            cam.transform.DOMove(focusPos.position, 0.5f).Play();
            cam.transform.DOLocalRotateQuaternion(focusPos.rotation, 0.25f).SetDelay(0.5f).Play();
            cam.GetComponent<Camera>().DOFieldOfView(focusFov, 0.5f).Play().OnComplete(() =>
            {
                GetComponent<BoxCollider>().enabled = false;
                cam.doMove = false;
                _isFocused = true;
                _inTransition = false;
            });
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape) && _isFocused && !_inTransition)
            {
                _inTransition = true;

                var seq = DOTween.Sequence();
                seq.Join(canvas.DOScaleY(0f, 0.5f));
                seq.Join(cam.transform.DOMove(_prevPos, 0.5f));
                seq.Join(cam.GetComponent<Camera>().DOFieldOfView(_prevFov, 0.5f));

                seq.OnComplete(() =>
                {
                    GetComponent<BoxCollider>().enabled = true;
                    cam.rot = cam.transform.rotation;
                    this.Delayed(0.2f, () => cam.doMove = true);
                    _isFocused = false;
                    _inTransition = false;
                    Cursor.visible = false;
                    Cursor.lockState = CursorLockMode.Locked;
                });
                seq.Play();
            } else if (Input.GetMouseButton(0) && !_isFocused && !_inTransition)
            {
                var hit = new RaycastHit[1];
                Physics.RaycastNonAlloc(cam.transform.position, cam.transform.forward, hit);
                if (hit[0].collider != null && hit[0].collider.gameObject == gameObject)
                {
                    OnPointerClick();
                }
            }
        }

        private Tween _highlightColor;
        // public void OnPointerEnter(PointerEventData eventData)
        // {
        //     _highlightColor?.Kill();
        //     _highlightColor = _material.DOColor(overColor, 0.2f).Play();
        // }
        //
        // public void OnPointerExit(PointerEventData eventData)
        // {
        //     _highlightColor?.Kill();
        //     _highlightColor = _material.DOColor(defaultColor, 0.2f).Play();
        // }
    }
}