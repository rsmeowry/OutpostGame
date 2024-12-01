using System;
using System.Collections.Generic;
using System.Linq;
using Game.Citizens;
using Game.Production.Products;
using Game.State;
using TMPro;
using UI.Util;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Interior.Hire
{
    public class SingleCitizenHireChoice: MonoBehaviour
    {
        private static Dictionary<CitizenCaste, string> _casteColors = new[]
        {
            (CitizenCaste.Creator, "#4575bc"),
            (CitizenCaste.Explorer, "#45bc63"),
            (CitizenCaste.Beekeeper, "#bc9c45"),
            (CitizenCaste.Engineer, "#bc4575")
        }.ToDictionary(it => it.Item1, it => it.Item2);

        [SerializeField]
        private Sprite engineerIcon;
        [SerializeField]
        private Sprite beekeeperIcon;
        [SerializeField]
        private Sprite creatorIcon;
        [SerializeField]
        private Sprite explorerIcon;

        [SerializeField]
        private Sprite allowedIcon;
        [SerializeField]
        private Sprite notAllowedIcon;

        [SerializeField]
        private Sprite chargedHoney;

        [SerializeField]
        private Image casteIcon;
        [SerializeField]
        private TMP_Text title;
        [SerializeField]
        private Transform requestContainer;
        [SerializeField]
        private GameObject singleRequestPrefab;
        [SerializeField]
        private Button hireButton;

        public CareerOffer Offer;
        private HashSet<StateKey> _involvedItems;

        private void RecalculateOfferPossibility()
        {
            var canHire = true;
            for (var i = 0; i < requestContainer.childCount; i++)
            {
                var child = requestContainer.GetChild(i);
                if (i != requestContainer.childCount - 1)
                {
                    var kv = Offer.ResourceRequest.ToArray()[i];
                    var pd = ProductRegistry.Instance.GetProductData(kv.Key);
                    child.GetChild(0).GetComponent<Image>().sprite = pd.icon;
                    child.GetComponentInChildren<TMP_Text>().text = kv.Value.ToString();
                    var cond = GameStateManager.Instance.PlayerProductCount.GetValueOrDefault(kv.Key.Formatted()) >=
                               kv.Value;
                    canHire = canHire && cond;
                    child.GetChild(2).GetComponent<Image>().sprite =
                        cond ? allowedIcon : notAllowedIcon;
                    
                    var tt = child.GetComponent<SimpleTooltipDisplay>();
                    tt.title = pd.name;
                    tt.body = $"Требуется {pd.name} в количестве {kv.Value} шт.";
                }
                else
                {
                    var v = Offer.CurrencyRequest;
                    child.GetChild(0).GetComponent<Image>().sprite = chargedHoney;
                    child.GetComponentInChildren<TMP_Text>().text = $"{v} ЭМ";

                    var cond = GameStateManager.Instance.Currency >= v;
                    canHire = canHire && cond;
                    child.GetChild(2).GetComponent<Image>().sprite =
                        cond ? allowedIcon : notAllowedIcon;

                    var tt = child.GetComponent<SimpleTooltipDisplay>();
                    tt.title = "Энергомед";
                    tt.body = $"Требуется {v} Энергомеда";
                }
            }

            hireButton.interactable = canHire;
        }

        private void HandleProductChange(ProductChangedData data)
        {
            if (!_involvedItems.Contains(data.Product))
                return;
            RecalculateOfferPossibility();
        }

        private void OnEnable()
        {
            GameStateManager.Instance.onProductChanged.AddListener(HandleProductChange);
        }

        private void OnDisable()
        {
            GameStateManager.Instance.onProductChanged.RemoveListener(HandleProductChange);
        }

        private void Start()
        {
            var icon = Offer.CitizenData.Profession switch
            {
                CitizenCaste.Creator => creatorIcon,
                CitizenCaste.Explorer => explorerIcon,
                CitizenCaste.Beekeeper => beekeeperIcon,
                CitizenCaste.Engineer => engineerIcon,
                _ => throw new ArgumentOutOfRangeException()
            };
            casteIcon.sprite = icon;
            title.text = $"{Offer.CitizenData.Name} | <color={_casteColors[Offer.CitizenData.Profession]}>" +
                         Offer.CitizenData.Profession switch
                         {
                             CitizenCaste.Creator => "Творец",
                             CitizenCaste.Explorer => "Первопроходец",
                             CitizenCaste.Beekeeper => "Пасечник",
                             CitizenCaste.Engineer => "Конструктор",
                             _ => throw new ArgumentOutOfRangeException()
                         };
            
            _involvedItems = Offer.ResourceRequest.Keys.ToHashSet();

            foreach (var _ in Offer.ResourceRequest)
            {
                Instantiate(singleRequestPrefab, requestContainer);
            }

            Instantiate(singleRequestPrefab, requestContainer);
            
            RecalculateOfferPossibility();
            
            hireButton.onClick.AddListener(() =>
            {
                if (!CitizenManager.Instance.CanSpawnCitizen())
                {
                    ToastManager.Instance.ShowToast("Недостаточно жилых мест! Постройте дом");
                    return;
                }
                hireButton.interactable = false;
                CareerManager.Instance.Hire(Offer);
            });
        }
    }
}