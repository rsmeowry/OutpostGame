using System;
using System.Collections;
using Game.Citizens.Navigation;
using Game.Controllers;
using UI;
using UnityEngine;
using UnityEngine.EventSystems;

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


        private bool _clickTransition;
        
        private IEnumerator OnMouseDown()
        {
            if (_clickTransition)
                yield break;
            if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
                yield break;
            _clickTransition = true;
            TownCameraController.Instance.FocusedPOI = this;
            yield return TownCameraController.Instance.StateMachine.SwitchState(TownCameraController.Instance.FocusedState);
            var poi = Instantiate(UIManager.Instance.prefabInspectPoi, UIManager.Instance.transform);
            poi.poi = this;
            poi.InitForResourcePOI();
            _clickTransition = false;

        } 
    }
}