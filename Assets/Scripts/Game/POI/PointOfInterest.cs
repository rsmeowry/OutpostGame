using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Game.Citizens.Navigation;
using Game.Controllers;
using NUnit.Framework;
using UI;
using UI.Util;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Game.POI
{
    public abstract class PointOfInterest: MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
    {
        public AudioClip clickedSound;
        public POIData data;
        
        public virtual (float, float) SoundPitchRange => (0.7f, 1.3f);

        public float cameraZoomAmt = 30f;
        public Transform focusPos;
        public bool shouldDepthOfField = false;
        
        public abstract QueuePosition EntrancePos { get; }


        private bool _clickTransition;

        public IEnumerator DoClick()
        {
            if (_clickTransition || TownCameraController.Instance.interactionFilter != CameraInteractionFilter.ProductionAndCitizens)
                yield break;
            // Debug.Log(EventSystem.current.IsPointerOverGameObject());
            // if (EventSystem.current.IsPointerOverGameObject())
            //     yield break;
            _clickTransition = true;
            TownCameraController.Instance.FocusedPOI = this;
            yield return TownCameraController.Instance.StateMachine.SwitchState(TownCameraController.Instance.FocusedState);
            var poi = Instantiate(UIManager.Instance.prefabInspectPoi, UIManager.Instance.transform);
            poi.poi = this;
            poi.InitForResourcePOI();
            _clickTransition = false;
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            TooltipCtl.Instance.Show(data.title, "дадада");
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            TooltipCtl.Instance.Hide();
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (eventData.button != PointerEventData.InputButton.Left)
                return;
            // var list = new List<RaycastResult>();
            // EventSystem.current.RaycastAll(eventData, list);
            StartCoroutine(DoClick());
        }
    }
}