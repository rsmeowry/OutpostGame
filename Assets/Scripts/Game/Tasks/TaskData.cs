using System;
using System.Collections.Generic;
using Game.State;
using UnityEngine;

namespace Game.Tasks
{
    [Serializable]
    public class StoredTaskReward
    {
        public int currency;
        public string itemReward;
        public int itemCount;
    }
    
    [Serializable]
    public class StoredTaskData
    {
        public string taskId;
        public string taskGiver;
        public string taskGiverBackground;
        public string taskDescription;
        public Dictionary<string, int> RequiredItems;
        public StoredTaskReward reward;
        public int expReward;
    }

    [Serializable]
    public class SerializedTaskInformation
    {
        public List<string> completedTasks;
        public TaskData currentTask;
    }

    [Serializable]
    public class TaskReward
    {
        public int currency;
        public StateKey item;
        public int itemCount;
    }

    [Serializable]
    public class TaskData
    {
        public string taskId;
        public string taskGiver;
        public string taskGiverBackground;
        public string taskDescription;
        public Dictionary<StateKey, int> RequiredItems;
        public TaskReward reward;
        public int experienceReward;
    }
}