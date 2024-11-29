using System.Collections.Generic;
using System.Linq;
using External.Util;
using Game.DayNight;
using Game.Production.Products;
using Game.State;
using UnityEngine;

namespace Game.Tasks
{
    public static class Tasks
    {
        public static TaskData BeginningTask => TaskManager.CreateTaskData(
            "begin",
            "Старпом",
            "Ваш верный, опытный помощник. Очень дальновиден, чем заслужил авторитет на корабле.",
            "Стоит собрать базовых ресурсов для начала развития. Принеси мне 30 камня, меда и дерева и мы продолжим.",
            new[] { (ProductRegistry.Honey, 30), (ProductRegistry.Wood, 30), (ProductRegistry.Stone, 30) },
            400, 500
        );

        public static TaskData GatherOre => TaskManager.CreateTaskData(
            "gather_ore",
            "Старпом",
            "Ваш верный, опытный помощник. Очень дальновиден, чем заслужил авторитет на корабле.",
            "Добудь немного руды, и я раскрою тебе потенциал металлургии.",
            new[] { (ProductRegistry.IronOre, 20), (ProductRegistry.CopperOre, 20) },
            800, 1000
        );
        
        public static TaskData SapsanTask => TaskManager.CreateTaskData(
            "oleg",
            "Сапсан",
            "Главный программист на орбитальной станции BRZ. Попал в передрягу и теперь нуждается в помощи.",
            "<i>*ПОМЕХИ*</i> -Ало? Галактион? Мне срочно нужно 30 железных пластин, у нас пробоина!",
            new[] { (ProductRegistry.IronPlate, 40) },
            ProductRegistry.Honey, 100, 1000
        );
        
        public static TaskData KozlowTask => TaskManager.CreateTaskData(
            "kozlow",
            "Олег",
            "Главный конструктор на орбитальной станции BRZ. Крайне уважаем за свои исследования в сфере кибернетики.",
            "Итак, шпингалеты из Галактиона, разместите запрос так: МНЕ. НУЖНО. 100. ШЕСТЕРЕНОК. Ни больше ни меньше. ПРИЕМ!",
            new[] { (ProductRegistry.Cog, 100) },
            ProductRegistry.Honey, 100, 1000
        );

        public static TaskData DonTask => TaskManager.CreateTaskData(
            "don_1",
            "Дониил",
            "Широко известный в узких кругах творец. Создал и случайно разбил на миллион осколков мозаику закрытия Утраты.",
            "Здравствуйте? Эта связь работает? Для медовых красок мне нужно 20 единиц меда и 20 единиц сырой меди. Буду благодарен.",
            new[] { (ProductRegistry.Honey, 20), (ProductRegistry.CopperOre, 20) },
            ProductRegistry.Steel, 5, 2000
        );
        
        public static TaskData NextTask(string taskId)
        {
            return taskId switch
            {
                "begin" => GatherOre,
                _ => Rng.Choice(new[] { SapsanTask, KozlowTask, RandomTask() }.Where(it => !TaskManager.Instance.completedTasks.Contains(it.taskId)).ToList())
            };
        }

        public static TaskData RandomTask()
        {
            var resourceCount = DayCycleManager.Instance.days > 6 ? 3 : DayCycleManager.Instance.days > 4 ? 2 : 1;
            var items = new Dictionary<StateKey, int>();
            for (var i = 0; i < resourceCount; i++)
            {
                items[ProductRegistry.Instance.RandomItem()] =
                    Mathf.RoundToInt(Random.Range(10, 30) * (1 + 0.5f * DayCycleManager.Instance.days));
            }

            var reward = Rng.Bool()
                ? new TaskReward
                {
                    currency = Mathf.RoundToInt(Random.Range(50, 80) * (1 + 0.3f * DayCycleManager.Instance.days)),
                }
                : new TaskReward()
                {
                    currency = -1,
                    item = ProductRegistry.Instance.RandomItem(),
                    itemCount = Mathf.RoundToInt(Random.Range(40, 70) * 0.6f * (DayCycleManager.Instance.days + 1))
                };


            return new TaskData
            {
                experienceReward = Mathf.RoundToInt(Random.Range(100, 500) * 0.3f * (DayCycleManager.Instance.days + 1)),
                RequiredItems = items,
                reward = reward,
                taskDescription = "GALACTION запрашивает у вас:\n" + items
                    .Select(it => ProductRegistry.Instance.GetProductData(it.Key).name + ": " + it.Value)
                    .ToLineSeparatedString(),
                taskGiver = "Запрос GALACTION",
                taskGiverBackground = "GALACTION управляет автоматизированными запросами со всей Галактики",
                taskId = "galaction_" + Random.Range(0, 10000000000)
            };
        }
    }
}