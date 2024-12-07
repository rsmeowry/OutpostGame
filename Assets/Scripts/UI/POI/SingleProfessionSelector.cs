using System;
using System.Linq;
using DG.Tweening;
using Game.Building;
using Game.Citizens;
using Game.Production.POI;
using Game.Sound;
using TMPro;
using UI.Util;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UI.POI
{
    public class SingleProfessionSelector: MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        private Button _buttonRemove;
        private Button _buttonAdd;
        private TMP_Text _counter;
        private TMP_Text _professionName;
        
        public CitizenSelectorParent parent;
        public CitizenCaste caste;
        
        public void PreInit()
        {
            _buttonRemove = transform.GetChild(0).GetComponent<Button>();
            _counter = transform.GetChild(1).GetComponent<TMP_Text>();
            _buttonAdd = transform.GetChild(2).GetComponent<Button>();
            _professionName = transform.GetChild(3).GetComponent<TMP_Text>();
            
            _buttonAdd.onClick.AddListener(() =>
            {
                SoundManager.Instance.PlaySound2D(SoundBank.Instance.GetSound("ui.hire_citizen"), 0.8f);
                parent.AssignCitizen(caste);
            });
            _buttonRemove.onClick.AddListener(() =>
            {
                SoundManager.Instance.PlaySound2D(SoundBank.Instance.GetSound("ui.fire_citizen"), 0.8f);
                parent.RemoveCitizen(caste);
            });
        }

        private Tween _tween;
        public void PollChanges()
        {
            _professionName.SetText(caste switch
            {
                CitizenCaste.Creator => "Творцы",
                CitizenCaste.Explorer => "Первопроходцы",
                CitizenCaste.Beekeeper => "Пасечники",
                CitizenCaste.Engineer => "Конструкторы",
                _ => throw new ArgumentOutOfRangeException()
            });

            _buttonAdd.interactable = parent.CanAssign(caste);
            _buttonRemove.interactable = parent.CanRemove(caste);

            var txt = ((ResourceContainingPOI)parent.parentPanel.poi).AssignedAgents
                .Count(it => it.PersistentData.Profession == caste).ToString();

            if (txt != _counter.text)
            {
                _tween?.Kill();
                _tween = _counter.rectTransform.DOPunchScale(Vector3.one * 1.05f, 0.2f, 10, 0.3f).OnComplete(() => _counter.rectTransform.localScale = Vector3.one).Play();
                _counter.SetText(txt);
            }
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            TooltipCtl.Instance.Show("Отправить " + caste switch
            {
                CitizenCaste.Creator => "творцов",
                CitizenCaste.Explorer => "первопроходцов",
                CitizenCaste.Beekeeper => "пасечников",
                CitizenCaste.Engineer => "конструкторов",
                _ => throw new ArgumentOutOfRangeException()
            }, "Изменяет количество медведей этого сословия в этом месте работы", 0.6f);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            TooltipCtl.Instance.Hide();
        }
    }
}