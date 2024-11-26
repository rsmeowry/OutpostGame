using System;
using System.Collections;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Serialization;

namespace Inside
{
    public class InsideCameraController: MonoBehaviour
    {
        [SerializeField]
        private float sensitivity = 2;
        [SerializeField]
        private float maxX;
        [SerializeField]
        private float maxY;

        [SerializeField]
        private Transform toUnrenderPoi;
        [SerializeField]
        private Transform toUnrenderPoi2;
        [SerializeField]
        private Transform toUnrenderUnits;

        [SerializeField]
        private float rotationTime;
        
        private float _rotationY;
        private float _rotationX;

        public bool doMove = true;

        public void Update()
        {
            
        }

        private IEnumerator Start()
        {
            rot = transform.rotation;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            yield return new WaitForSeconds(2f);
            foreach (var rd in toUnrenderPoi.GetComponentsInChildren<Renderer>())
            {
                rd.enabled = false;
            }
            foreach (var rd in toUnrenderPoi2.GetComponentsInChildren<Renderer>())
            {
                rd.enabled = false;
            }
            foreach (var rd in toUnrenderUnits.GetComponentsInChildren<Renderer>())
            {
                rd.enabled = false;
            }
        }

        private void OnDisable()
        {
            foreach (var rd in toUnrenderPoi.GetComponentsInChildren<Renderer>())
            {
                rd.enabled = true;
            }
            foreach (var rd in toUnrenderPoi2.GetComponentsInChildren<Renderer>())
            {
                rd.enabled = true;
            }
            foreach (var rd in toUnrenderUnits.GetComponentsInChildren<Renderer>())
            {
                rd.enabled = true;
            }
        }

        private void LateUpdate()
        {
            if(doMove)
                PollRotation();
        }

        [FormerlySerializedAs("_rot")] public Quaternion rot;
        private void PollRotation()
        {
            _rotationX += Input.GetAxisRaw("Mouse X") * sensitivity;
            _rotationY += Input.GetAxisRaw("Mouse Y") * sensitivity;
            _rotationX = Mathf.Clamp(_rotationX, -maxX, maxX);
            _rotationY = Mathf.Clamp(_rotationY, -maxY, maxY);

            rot = Quaternion.Lerp(rot, Quaternion.Euler(new Vector3(-_rotationY, _rotationX)),
                rotationTime * Time.deltaTime);
            transform.rotation = rot;
        }
    }
}