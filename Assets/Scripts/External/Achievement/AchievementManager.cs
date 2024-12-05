using System;
using System.Collections.Generic;
using DG.Tweening;
using External.Data;
using External.Util;
using Game.State;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace External.Achievement
{
    public class AchievementManager: MonoBehaviour
    {
        public static AchievementManager Instance { get; private set; }

        private List<StateKey> _unlockedAchievements = new();

        private RectTransform _container;
        private TMP_Text _title;
        private TMP_Text _description;
        private Image _icon;

        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            _container = (RectTransform) transform;
            _container.localScale = Vector3.zero;
            _title = _container.GetChild(1).GetChild(0).GetComponent<TMP_Text>();
            _description = _container.GetChild(1).GetChild(1).GetComponent<TMP_Text>();
        }

        [ContextMenu("Test/Achievement")]
        private void __TestAchievement()
        {
            this.Delayed(0.5f, () =>
            {
                GiveAchievement("Ротару 3", "Произведите 300.000 роторов", new StateKey("test"));
            }); 
        }

        public void GiveAchievement(string name, string description, StateKey id)
        {
            // TODO: two achievements at once cant happen, but just in case need to handle it
            if (_unlockedAchievements.Contains(id))
                return;
            MiscSavedData.Instance.Data.Achievements.Add(id.Formatted());
            _unlockedAchievements.Add(id);
            _title.SetText($"Достижение - {name}");
            _description.SetText(description);
            var seq = DOTween.Sequence();
            seq.Append(_container.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBack));
            seq.Append(_container.DOScale(Vector3.zero, 0.5f).SetEase(Ease.InBack).SetDelay(2f));
            seq.Play();
            // TODO: achievement sounds!
        }
    }
}