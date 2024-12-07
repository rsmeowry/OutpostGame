using System;
using System.Collections.Generic;
using DG.Tweening;
using External.Data;
using Game.Sound;
using Game.State;
using Game.Tasks;
using Game.Upgrades;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Interior.Upgrades
{
    public class SingleUpgradeChoice: MonoBehaviour
    {
        [SerializeField]
        private TMP_Text titleText;
        [SerializeField]
        private TMP_Text descriptionText;
        [SerializeField]
        private TMP_Text costText;
        [SerializeField]
        private GameObject infiniteIcon;

        [SerializeField]
        private Image hideImage;

        [SerializeField]
        private Button obtainButton;
        
        public UpgradeData data;

        private StateKey _key;

        private void Start()
        {
            _key = StateKey.FromString(data.id);

            var count = UpgradeTreeManager.Instance.Upgrades.GetValueOrDefault(_key, 0);
            var cntTxt = count == 0 ? "" : count.ToString();
            titleText.text = data.isInfinite ? $"{data.title} {cntTxt}" : data.title;
            
            infiniteIcon.SetActive(data.isInfinite);

            descriptionText.text = data.description;
            var experienceCost = data.isInfinite ? data.baseExperienceCost * (int)Mathf.Pow(2, UpgradeTreeManager.Instance.Upgrades.GetValueOrDefault(_key)) : data.baseExperienceCost;
            costText.text = $"Опыт: {experienceCost} XP";

            if (UpgradeTreeManager.Instance.ShouldBeShown(data))
            {
                hideImage.enabled = false;
                hideImage.transform.GetChild(0).GetComponent<Image>().enabled = false;
            }
            else
            {
                hideImage.enabled = true;
                hideImage.transform.GetChild(0).GetComponent<Image>().enabled = true;
            }

            var hasUpg = UpgradeTreeManager.Instance.Has(_key);
            if (hasUpg)
            {
                obtainButton.GetComponentInChildren<TMP_Text>().text = data.isInfinite ? "Открыть" : "Открыто!";
                obtainButton.interactable = data.isInfinite && MiscSavedData.Instance.Data.Experience >= experienceCost;
            } else
                obtainButton.interactable = MiscSavedData.Instance.Data.Experience >= experienceCost;
            
            obtainButton.onClick.AddListener(() =>
            {
                ((RectTransform)transform).DOShakeAnchorPos(0.1f, 20f).Play();
                SoundManager.Instance.PlaySound2D(SoundBank.Instance.GetSound("ui.upgrade"), 0.8f);
                hideImage.transform.GetChild(0).GetComponent<Image>().enabled = false;
                UpgradeTreeManager.Instance.UnlockUpgrade(data);
            });
        }

        public void PollChanges()
        {
            if (UpgradeTreeManager.Instance.ShouldBeShown(data))
            {
                hideImage.enabled = false;
                hideImage.transform.GetChild(0).GetComponent<Image>().enabled = false;
                
                var count = UpgradeTreeManager.Instance.Upgrades.GetValueOrDefault(_key, 0);
                var cntTxt = count == 0 ? "" : count.ToString();
                titleText.text = data.isInfinite ? $"{data.title} {cntTxt}" : data.title;

                var experienceCost = data.isInfinite ? data.baseExperienceCost * (int)Mathf.Pow(2, UpgradeTreeManager.Instance.Upgrades.GetValueOrDefault(_key)) : data.baseExperienceCost;
                var hasUpg = UpgradeTreeManager.Instance.Has(_key);
                if (hasUpg)
                {
                    obtainButton.GetComponentInChildren<TMP_Text>().text = "Открыто!";
                    obtainButton.interactable = data.isInfinite && MiscSavedData.Instance.Data.Experience >= experienceCost;
                } else
                    obtainButton.interactable = MiscSavedData.Instance.Data.Experience >= experienceCost;
                costText.text = $"Опыт: {experienceCost} XP";
            }
            else
            {
                hideImage.enabled = true;
                hideImage.GetComponentInChildren<Image>().enabled = true;
            }
        }
    }
}