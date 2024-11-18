using System;
using System.Collections;
using Game.Citizens.Navigation;
using Game.Controllers;
using UI;
using UnityEngine;

namespace Game.POI
{
    public abstract class PointOfInterest: MonoBehaviour
    {
        public AudioClip clickedSound;
        public POIData data;
        
        public virtual (float, float) SoundPitchRange => (0.7f, 1.3f);

        public float cameraZoomAmt = 30f;
        public Transform focusPos;
        public bool shouldDepthOfField = false;
        
        public abstract QueuePosition EntrancePos { get; }

        private IEnumerator OnMouseDown()
        {
            TownCameraController.Instance.FocusedPOI = this;
            yield return TownCameraController.Instance.StateMachine.SwitchState(TownCameraController.Instance.FocusedState);
            var poi = Instantiate(UIManager.Instance.prefabInspectPoi, UIManager.Instance.transform);
            poi.poi = this;
            poi.InitForResourcePOI();
        }
    }
}