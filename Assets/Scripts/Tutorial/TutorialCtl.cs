using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using External.Achievement;
using External.Data;
using External.Util;
using Game.State;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Tutorial
{
    public class TutorialCtl: MonoBehaviour
    {
        public static TutorialCtl Instance { get; private set; }
        
        [SerializeField]
        public ModalCtl modal;

        [SerializeField]
        public RectTransform topBar;
        [SerializeField]
        public Image topBarImage;

        [SerializeField]
        private List<TutorialStep> steps;
        
        private Queue<TutorialStep> _steps = new();

        private bool _disabled;

        public TutorialStep ActiveStep => _steps.Count == 0 ? null : _steps.Peek();

        public void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            if (MiscSavedData.Instance.Data.TutorialStep == -1)
            {
                Destroy(modal.gameObject);
                Destroy(topBar.gameObject);
                _disabled = true;
                return;
            }

            modal.rect.localScale = Vector3.zero;
            SetupTutorial();
        }

        public void SendMovementData(Vector3 newPos)
        {
            if (_disabled)
                return;
            _steps.Peek().ReceiveMovementData(newPos);
        }

        private void HandleProductChange(ProductChangedData delta)
        {
            if (_disabled)
            {
                GameStateManager.Instance.onProductChanged.RemoveListener(HandleProductChange);
                return;
            }
            _steps.Peek().ReceiveResourceData(delta.Product, delta.Delta);
        }

        private int _previousCurrency = 0;
        private void HandleCurrencyChange()
        {
            if (_disabled)
            {
                GameStateManager.Instance.onCurrencyChanged.RemoveListener(HandleCurrencyChange);
                return;
            }

            var delta = GameStateManager.Instance.Currency - _previousCurrency;
            _previousCurrency = GameStateManager.Instance.Currency;
            _steps.Peek().ReceiveCurrencyData(delta);
        }

        private void SetupTutorial()
        {
            _previousCurrency = GameStateManager.Instance.Currency;

            _steps = new Queue<TutorialStep>(steps.Skip(MiscSavedData.Instance.Data.TutorialStep));
            
            GameStateManager.Instance.onCurrencyChanged.AddListener(HandleCurrencyChange);
            GameStateManager.Instance.onProductChanged.AddListener(HandleProductChange);
            
            _steps.Peek().Activate();
            _steps.Peek().DisplayModal(modal.rect);
            modal.Show();
        }

        public void StepCompleted()
        {
            _steps.Dequeue();
            MiscSavedData.Instance.Data.TutorialStep += 1;
            topBarImage.fillAmount = 0;
            if (_steps.Count == 0)
            {
                MiscSavedData.Instance.Data.TutorialStep = -1;
                _disabled = true;
                topBar.DOAnchorPosY(50, 0.5f).OnComplete(() => Destroy(topBar.gameObject)).SetEase(Ease.InBack).Play();
                modal.Hide();
                this.Delayed(1.5f, () =>
                {
                    AchievementManager.Instance.GiveAchievement(Achievements.Tutorial);
                    Destroy(modal.gameObject);
                });
                return;
            }
            _steps.Peek().Activate();
            
            Destroy(modal.rect.GetChild(1).gameObject);
            _steps.Peek().DisplayModal(modal.rect);
            modal.Show();
        }
    }
}