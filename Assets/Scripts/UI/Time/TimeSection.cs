using System;
using DG.Tweening;
using Game.Tick;
using TMPro;
using UnityEngine;

namespace UI.Time
{
    public class TimeSection: MonoBehaviour
    {
        private TMP_Text _descText;
        private float[] _sliderPositions = new float[4] { 34.5f, 105f, 175.5f, 246f };
        [SerializeField]
        private RectTransform slider;

        private int _selectedSpeedBoost = 1;
        
        public void Start()
        {
            _selectedSpeedBoost = TimeManager.Instance.gameSpeed;
            _descText = GetComponentInChildren<TMP_Text>();
            _descText.SetText("Скорость времени: " + (_selectedSpeedBoost == 0 ? "на паузе" : _selectedSpeedBoost.ToString()));
            MoveSlider(_selectedSpeedBoost);
        }

        public void ChangeSpeed(int newSpeed)
        {
            _selectedSpeedBoost = newSpeed;
            _descText.SetText("Скорость времени: " + (_selectedSpeedBoost == 0 ? "на паузе" : _selectedSpeedBoost.ToString()));
            TimeManager.Instance.ChangeGameSpeed(newSpeed);
            MoveSlider(newSpeed);
        }

        private Tween _sliderMovement;
        private void MoveSlider(int newBoost)
        {
            if (_sliderMovement != null)
            {
                _sliderMovement.Kill();
                _sliderMovement = null;
            }

            _sliderMovement = slider.DOAnchorPosX(_sliderPositions[newBoost], 0.5f).SetEase(Ease.OutExpo).Play();
        }
    }
}