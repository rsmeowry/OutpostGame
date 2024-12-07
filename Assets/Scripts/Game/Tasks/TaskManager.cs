using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using External.Data;
using Game.DayNight;
using Game.POI;
using Game.State;
using Game.Storage;
using Game.Upgrades;
using UnityEngine;
using UnityEngine.Serialization;

namespace Game.Tasks
{
    public class TaskManager: MonoBehaviour
    {
        public static TaskManager Instance { get; private set; }

        [FormerlySerializedAs("_completedTasks")] public List<string> completedTasks = new();
        [NonSerialized]
        public TaskData CurrentTask = Tasks.BeginningTask;

        private void Awake()
        {
            Instance = this;
        }

        public static TaskData CreateTaskData(string id, string giver, string giverBg, string desc, (StateKey, int)[] requiredItems, StateKey reward, int rewardCount, int experience, bool useExpBoost = true)
        {
            return new TaskData()
            {
                taskId = id,
                taskGiver = giver,
                taskGiverBackground = giverBg,
                taskDescription = desc,
                RequiredItems = (requiredItems).ToDictionary(it => it.Item1, it => it.Item2),
                reward = new TaskReward
                {
                    currency = -1,
                    item = reward,
                    itemCount = rewardCount
                },
                experienceReward = useExpBoost ? Mathf.RoundToInt(experience * (1 + UpgradeTreeManager.Instance.Upgrades.GetValueOrDefault(Upgrades.Upgrades.IncreasedExperience) * 0.25f)) : experience
            };
        }
        
        public static TaskData CreateTaskData(string id, string giver, string giverBg, string desc, (StateKey, int)[] requiredItems, int currencyReward, int experience, bool useExpBoost = true)
        {
            return new TaskData
            {
                taskId = id,
                taskGiver = giver,
                taskGiverBackground = giverBg,
                taskDescription = desc,
                RequiredItems = (requiredItems).ToDictionary(it => it.Item1, it => it.Item2),
                reward = new TaskReward
                {
                    currency = currencyReward
                },
                experienceReward = useExpBoost ? Mathf.RoundToInt(experience * (1 + UpgradeTreeManager.Instance.Upgrades.GetValueOrDefault(Upgrades.Upgrades.IncreasedExperience) * 0.25f)) : experience
            };
        }

        public void CompleteTask()
        {
            if (CurrentTask.reward.currency != -1)
            {
                GameStateManager.Instance.ChangeCurrency(CurrentTask.reward.currency, "Task completed", true);
            }
            else
            {
                GameStateManager.Instance.IncreaseProduct(CurrentTask.reward.item, CurrentTask.reward.itemCount);
            }

            foreach (var req in CurrentTask.RequiredItems)
            {
                GameStateManager.Instance.IncreaseProduct(req.Key, -req.Value);
            }

            MiscSavedData.Instance.Data.Experience += CurrentTask.experienceReward;
            
            completedTasks.Add(CurrentTask.taskId);
            CurrentTask = Tasks.NextTask(CurrentTask.taskId);
        }

        public void LoadData()
        {
            if (!FileManager.Instance.Storage.FileExists("tasks.dat", true))
            {
                SaveData();
                return;
            }
            
            var fmt = new BinaryFormatter();
            using var stream = FileManager.Instance.Storage.ReadFileBytes("tasks.dat", true);
            var db = (SerializedTaskInformation) fmt.Deserialize(stream.BaseStream);
            CurrentTask = db.currentTask;
            completedTasks = db.completedTasks;
        }

        public void SaveData()
        {
            var serialized = new SerializedTaskInformation
            {
                completedTasks = completedTasks,
                currentTask = CurrentTask
            };
            
            using var memStream = new MemoryStream();
            var fmt = new BinaryFormatter();
            fmt.Serialize(memStream, serialized);
            FileManager.Instance.Storage.SaveBytes("tasks.dat", memStream.GetBuffer(), true);
        }
    }
}