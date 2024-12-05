using System.Collections.Generic;
using Game.Electricity;
using Game.POI;
using Game.POI.Housing;
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
        [SerializeField]
        private POIElectricityStatistics electricityStatsPrefab;

        [SerializeField]
        private ElectricityProdReqs electricityProdPrefab;
        [SerializeField]
        private ElectricityConsReqs electricityConsPrefab;

        [SerializeField]
        private RecipeSelectorParent recipeSelectorPrefab;

        [SerializeField]
        private LiquidStatistics liquidStatsPrefab;

        [SerializeField]
        private HouseStatistics tenantsViewPrefab;

        [SerializeField]
        private CameraFollowComponent screenshottingPrefab;

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

        public void AddGlobalElectricityStats()
        {
            var stat = Instantiate(electricityStatsPrefab, transform);
            stat.transform.SetSiblingIndex(transform.childCount - 2);

            stat.Stats = ElectricityManager.Instance.GetStatistics();
        }
        
        public void AddElectricityProduction()
        {
            var req = Instantiate(electricityProdPrefab, transform);
            req.transform.SetSiblingIndex(transform.childCount - 2);

            req.Producer = poi.GetComponent<IElectricityProducer>();
        }
        
        public void AddElectricityConsumption()
        {
            var req = Instantiate(electricityConsPrefab, transform);
            req.transform.SetSiblingIndex(transform.childCount - 2);

            req.Consumer = poi.GetComponent<IElectricityConsumer>();
        }

        public void AddRecipeSelector()
        {
            var rec = Instantiate(recipeSelectorPrefab, transform);
            rec.transform.SetSiblingIndex(transform.childCount - 2);
            
            rec.RecipeContainer = poi.GetComponent<IRecipeContainer>();
        }

        public void AddLiquidStats()
        {
            var pref = Instantiate(liquidStatsPrefab, transform);
            pref.transform.SetSiblingIndex(transform.childCount - 2);
        }

        public void AddTenantsView()
        {
            var view = Instantiate(tenantsViewPrefab, transform);
            view.transform.SetSiblingIndex(transform.childCount - 2);

            view.House = poi.GetComponent<HousePOI>();
        }

        public void AddScreenshotting()
        {
            var view = Instantiate(screenshottingPrefab, transform);
            view.transform.SetSiblingIndex(transform.childCount - 2);
        }
    }
}