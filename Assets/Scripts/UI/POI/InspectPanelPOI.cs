using System;
using Game.POI;
using Game.Production.POI;
using TMPro;
using UnityEngine;

namespace UI.POI
{
    public class InspectPanelPOI: MonoBehaviour
    {
        public CitizenSelectorParent selectorParentPrefab;

        public PointOfInterest poi;
        internal ResourceContainingPOI rcPoi;
        
        private TMP_Text _title;
        private TMP_Text _description;

        private CitizenSelectorParent _citizenSelectorParent;
        
        public void Start()
        {
            _title = transform.GetChild(0).GetComponent<TMP_Text>();
            _description = transform.GetChild(1).GetComponent<TMP_Text>();

            _title.SetText(poi.data.title);
            _description.SetText(poi.data.description);
        }

        public void InitForResourcePOI()
        {
            _citizenSelectorParent = Instantiate(selectorParentPrefab, transform);
            rcPoi = (ResourceContainingPOI) poi;
            _citizenSelectorParent.parentPanel = this;
            _citizenSelectorParent.acceptedProfessions = rcPoi.acceptedProfessions;
            
            _citizenSelectorParent.Init();
        }
    }
}