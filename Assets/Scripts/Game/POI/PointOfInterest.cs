using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using External.Achievement;
using Game.Building;
using Game.Citizens;
using Game.Citizens.Navigation;
using Game.Controllers;
using Game.Sound;
using NUnit.Framework;
using UI;
using UI.POI;
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

        public bool doNotSerialize = false;
        public float cameraZoomAmt = 30f;
        public Transform focusPos;
        public Transform lookPos;
        public bool shouldDepthOfField = false;
        public string pointId;
        public BuiltObject buildingData;

        public virtual string PoiTitle => data.title;
        public virtual string PoiDesc => "";
        
        public abstract QueuePosition EntrancePos { get; }


        private bool _clickTransition;

        protected bool IsBuilt;

        protected AudioSource AudioSource;
        
        public virtual void OnBuilt()
        {
            IsBuilt = true;
            AudioSource = SoundManager.Instance.AddAudioSource(transform);
        }

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
            var poi = UIManager.Instance.OpenPanel();
            poi.poi = this;
            // poi.InitForResourcePOI();
            LoadForInspect(poi);
            _clickTransition = false;
        }

        protected abstract void LoadForInspect(PanelViewPOI panel);

        public void OnPointerEnter(PointerEventData eventData)
        {
            TooltipCtl.Instance.Show(PoiTitle, PoiDesc);
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

        public abstract SerializedPOIData Serialize();

        public virtual void LoadData(SerializedPOIData pl)
        {
            
        }
    }
    
    [Serializable]
    public abstract class SerializedPOIData
    {
        public string originPrefabId;
        public BuiltObject data;
        public SerVec3 position;
        public SerVec3 rotation;
        public Guid SelfId;
    }

    [Serializable]
    public struct SerVec3
    {
        public float X;
        public float Y;
        public float Z;
        
        public Vector3 ToVec3()
        {
            return new Vector3(X, Y, Z);
        }
    }
}