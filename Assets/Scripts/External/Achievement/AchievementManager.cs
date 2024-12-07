using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using External.Data;
using External.Util;
using Game.Sound;
using Game.State;
using Menu;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace External.Achievement
{
    public class AchievementManager: MonoBehaviour
    {
        public static AchievementManager Instance { get; private set; }

        [SerializeField]
        private AchievementDatabase db;

        private List<StateKey> _unlockedAchievements = new();
        public Dictionary<StateKey, SingleAchievementData> Datas = new();

        private RectTransform _container;
        private TMP_Text _title;
        private TMP_Text _description;
        private Image _icon;

        private void Awake()
        {
            Instance = this;
        }

        public bool HasUnlocked(StateKey ach)
        {
            return _unlockedAchievements.Contains(ach);
        }

        private void Start()
        {
            _container = (RectTransform) transform;
            _container.localScale = Vector3.zero;
            _icon = _container.GetChild(0).GetComponent<Image>();
            _title = _container.GetChild(1).GetChild(0).GetComponent<TMP_Text>();
            _description = _container.GetChild(1).GetChild(1).GetComponent<TMP_Text>();
            _unlockedAchievements.AddRange(MiscSavedData.Instance.Data.Achievements.Select(StateKey.FromString));

            Datas = db.achievements.ToDictionary(it => StateKey.FromString(it.id), it => it);
        }
        
        public void GiveAchievement(StateKey id)
        {
            // TODO: two achievements at once cant happen, but just in case need to handle it
            if (_unlockedAchievements.Contains(id))
                return;
            var data = Datas[id];
            MiscSavedData.Instance.Data.Achievements.Add(id.Formatted());
            _unlockedAchievements.Add(id);
            _icon.sprite = data.icon;
            _title.SetText($"Достижение - {data.name}");
            _description.SetText(data.description);
            var seq = DOTween.Sequence();
            seq.Append(_container.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBack));
            seq.Append(_container.DOScale(Vector3.zero, 0.5f).SetEase(Ease.InBack).SetDelay(2f));
            seq.Play();
            SoundManager.Instance.PlaySound2D(SoundBank.Instance.GetSound("ui.achievement"), 1f);
        }
    }
}