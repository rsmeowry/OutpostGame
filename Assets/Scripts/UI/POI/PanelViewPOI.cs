using System.Collections.Generic;
using Game.POI;
using Game.Production.POI;
using Game.Production.Products;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

namespace UI.POI
{
    public class PanelViewPOI: MonoBehaviour
    {
        [SerializeField]
        private CitizenSelectorParent selectorParentPrefab;
        [FormerlySerializedAs("soloProductionPrefab")] [SerializeField]
        private ProductionStatistics productionPrefab;

        public PointOfInterest poi;
        
        private TMP_Text _title;
        private TMP_Text _description;

        private List<GameObject> _components = new();
        
        public void Start()
        {
            _title = transform.GetChild(0).GetComponentInChildren<TMP_Text>();
            _description = transform.GetChild(1).GetComponentInChildren<TMP_Text>();

            _title.SetText(poi.data.title);
            _description.SetText(poi.data.description);
        }

        public void AddCitizenHireSelector()
        {
            var parent = Instantiate(selectorParentPrefab, transform);
            // place it right before the close button
            parent.transform.SetSiblingIndex(transform.childCount - 2);
            parent.parentPanel = this;
            parent.acceptedProfessions = ((ResourceContainingPOI) poi).acceptedProfessions;
            
            parent.Init();
            _components.Add(parent.gameObject);
        }

        public void AddSoloProduction(string itemName, Sprite icon, string production)
        {
            var prod = Instantiate(productionPrefab, transform);
            // place it right before the close button
            prod.transform.SetSiblingIndex(transform.childCount - 2);
            prod.itemName = itemName;
            prod.itemIcon = icon;
            prod.productionLevel = production;
            prod.title = "Производство";
            prod.helpTitle = "Собственное производство ресурсов";
            prod.helpDescription = "Этот объект самостоятельно производит ресурсы без вмешательства медведей";
            
            _components.Add(prod.gameObject);
        }
        
        public void AddResourceProduction(string itemName, Sprite icon, string production)
        {
            var prod = Instantiate(productionPrefab, transform);
            // place it right before the close button
            prod.transform.SetSiblingIndex(transform.childCount - 2);
            prod.itemName = itemName;
            prod.itemIcon = icon;
            prod.productionLevel = production;
            prod.title = "Источник";
            prod.helpTitle = "Залежи ресурсов";
            prod.helpDescription = "На этот объект можно отправить медведей определенных сословий, чтобы они добывали ресурсы";
            
            _components.Add(prod.gameObject);
        }
    }
}