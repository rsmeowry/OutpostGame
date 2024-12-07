using System;
using System.Collections;
using External.Util;
using Game.Citizens;
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
        private Terrain toUnrenderTerrain;
        [SerializeField]
        private Camera cameraTerrain;

        [SerializeField]
        private float rotationTime;
        
        private float _rotationY;
        private float _rotationX;

        public bool doMove = true;

        public void Update()
        {
            
        }

        private void OnEnable()
        {
            rot = transform.rotation;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            toUnrenderTerrain.enabled = false;
            cameraTerrain.GetComponent<GrassBlendRenderer>().enabled = false;
            
            this.Delayed(2f, () =>
            {
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
            });

        }

        private void OnDisable()
        {
            toUnrenderTerrain.enabled = true;
            cameraTerrain.GetComponent<GrassBlendRenderer>().enabled = true;

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
                var agent = rd.GetComponentInParent<CitizenAgent>();
                if(agent.StateMachine.CurrentState != agent.SleepState)
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