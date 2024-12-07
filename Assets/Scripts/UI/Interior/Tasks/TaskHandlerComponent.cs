using System;
using System.Collections.Generic;
using System.Linq;
using Game.Production.Products;
using Game.Sound;
using Game.State;
using Game.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Interior.Tasks
{
    public class TaskHandlerComponent: MonoBehaviour
    {
        [SerializeField]
        private Sprite chargedHoney;
        
        [SerializeField]
        private TMP_Text nameText;
        [SerializeField]
        private TMP_Text bgText;
        [SerializeField]
        private TMP_Text taskText;

        [SerializeField]
        private TMP_Text rewardText;
        [SerializeField]
        private Image rewardIcon;
        [SerializeField]
        private TMP_Text experienceText;
        
        [SerializeField]
        private Button sendButton;

        public void Start()
        {
            var task = TaskManager.Instance.CurrentTask;
            GameStateManager.Instance.onProductChanged.AddListener(HandleResourceChange);
            
            nameText.SetText(task.taskGiver);
            bgText.SetText(task.taskGiverBackground);
            taskText.SetText(task.taskDescription);

            if (task.reward.currency != -1)
            {
                // currency
                rewardIcon.sprite = chargedHoney;
                rewardText.SetText($"+{task.reward.currency} ЭМ");
            }
            else
            {
                // else item
                var dat = ProductRegistry.Instance.GetProductData(task.reward.item);
                rewardText.SetText($"{dat.name} ({task.reward.itemCount} шт.)");
                rewardIcon.sprite = dat.icon;
            }
            
            experienceText.SetText($"Опыт: +{task.experienceReward}");
            
            sendButton.onClick.AddListener(() =>
            {
                TaskManager.Instance.CompleteTask();
                SoundManager.Instance.PlaySound2D(SoundBank.Instance.GetSound("ui.mouse_click"), 0.8f);
                var window = transform.parent.parent.GetComponent<PCWindowDisplay>();
                window.ShowLoading("GALACTION - Поиск заданий", window.taskPrefab, _ => { });
            });
            
            RecalculateTaskPossibility(task);
        }

        private void OnDisable()
        {
            GameStateManager.Instance.onProductChanged.RemoveListener(HandleResourceChange);
        }

        private void HandleResourceChange(ProductChangedData changedData)
        {
            if (!TaskManager.Instance.CurrentTask.RequiredItems.ContainsKey(changedData.Product))
                return;
            RecalculateTaskPossibility(TaskManager.Instance.CurrentTask);
        }

        private void RecalculateTaskPossibility(TaskData task)
        {
            var possible = task.RequiredItems.All(it =>
            {
                Debug.Log("CHECKING " + GameStateManager.Instance.PlayerProductCount.GetValueOrDefault(it.Key.Formatted(), 0) + " " + it.Value);
                return GameStateManager.Instance.PlayerProductCount.GetValueOrDefault(it.Key.Formatted(), 0) >=
                       it.Value;
            });
            Debug.Log($"POSSIBLE? {possible}");
            sendButton.interactable = possible;
        }
    }
}