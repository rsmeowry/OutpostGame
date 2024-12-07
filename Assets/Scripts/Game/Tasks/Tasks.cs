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
            400, 300, false
        );

        public static TaskData GatherOre => TaskManager.CreateTaskData(
            "gather_ore",
            "Старпом",
            "Ваш верный, опытный помощник. Очень дальновиден, чем заслужил авторитет на корабле.",
            "Добудь немного руды, и я раскрою тебе потенциал металлургии.",
            new[] { (ProductRegistry.IronOre, 20), (ProductRegistry.CopperOre, 20) },
            800, 250
        );
        
        public static TaskData SapsanTask => TaskManager.CreateTaskData(
            "oleg",
            "Сапсан",
            "Главный программист на орбитальной станции BRZ. Попал в передрягу и теперь нуждается в помощи.",
            "<i>*ПОМЕХИ*</i> -Ало? Галактион? Мне срочно нужно 30 железных пластин, у нас пробоина!",
            new[] { (ProductRegistry.IronPlate, 40) },
            ProductRegistry.Honey, 100, 300
        );
        
        public static TaskData KozlowTask => TaskManager.CreateTaskData(
            "kozlow",
            "Олег",
            "Главный конструктор на орбитальной станции BRZ. Крайне уважаем за свои исследования в сфере кибернетики.",
            "Итак, шпингалеты из Галактиона, разместите запрос так: МНЕ. НУЖНО. 100. ШЕСТЕРЕНОК. Ни больше ни меньше. ПРИЕМ!",
            new[] { (ProductRegistry.Cog, 100) },
            ProductRegistry.Honey, 100, 500
        );

        public static TaskData DonTask => TaskManager.CreateTaskData(
            "don_1",
            "Дониил",
            "Широко известный в узких кругах творец. Создал и случайно разбил на миллион осколков мозаику закрытия Утраты.",
            "Здравствуйте? Эта связь работает? Для медовых красок мне нужно 20 единиц меда и 20 единиц сырой меди. Буду благодарен.",
            new[] { (ProductRegistry.Honey, 20), (ProductRegistry.CopperOre, 20) },
            ProductRegistry.Steel, 5, 150
        );

        public static TaskData DisaTask => TaskManager.CreateTaskData(
            "disa_1",
            "Диса",
            "Самый юный биоинженер в кванториуме \"Талант академия\"",
            "Для выполнения исследования мне нужно 50 шт. обычного меда как катализатор. Прием.",
            new[] { (ProductRegistry.Honey, 50) },
            ProductRegistry.Wood, 20, 300
        );
     
        public static TaskData BulatTask => TaskManager.CreateTaskData(
            "bulat_1",
            "Булат",
            "Строгий, но добродушный пасечник с планеты Умарталык, возглавляющий оборону от кибер-ос.",
            "Задание: здравия желаю, товарищи! Нам срочно нужно 50 цемента и 50 железный пластин для защитных систем и оборонительных укреплений. Рассчитываем на вас, в долгу не останемся!",
            new[] { (ProductRegistry.Concrete, 50), (ProductRegistry.IronPlate, 50) },
            ProductRegistry.Honey, 400, 200
        );
        
        public static TaskData SaidaTask => TaskManager.CreateTaskData(
            "saida_1",
            "Сайда",
            "Легендарная представительница первопроходцев, наставница многих великих капитанов. Некогда коллега главного героя.",
            "Здравствуй, ИО, давно не общались. Дошла информация, что в вашем поселении есть 50 шестеренок и 100 проводов, необходимые для строительства наших кораблей. Я предлагаю сотрудничество.",
            new[] { (ProductRegistry.Cog, 50), (ProductRegistry.CopperWires, 100) },
            ProductRegistry.Concrete, 250, 200
        );
        
        public static TaskData AtaiTask => TaskManager.CreateTaskData(
            "atai_1",
            "Атай",
            "Храбрый и ответственный пасечник, один из первых, кто отважился на устранение последствий Ошибки.",
            "Приветствую, друзья! Буду краток и перейду сразу к делу: мы восстанавливаем дома медведей, пострадавших от нападений кибер-ос и нам очень пригодилось бы 500 древесины. От нас - честное вознаграждение.",
            new[] { (ProductRegistry.Wood, 500) },
            1500, 1000
        );
        
        public static TaskData NextTask(string taskId)
        {
            return taskId switch
            {
                "begin" => GatherOre,
                _ => Rng.Choice(new[] { SapsanTask, KozlowTask, DonTask, AtaiTask, SaidaTask, BulatTask, DisaTask, RandomTask() }.Where(it => !TaskManager.Instance.completedTasks.Contains(it.taskId)).ToList())
            };
        }

        public static TaskData RandomTask()
        {
            var resourceCount = DayCycleManager.Instance.days > 15 ? 3 : DayCycleManager.Instance.days > 12 ? 2 : 1;
            var items = new Dictionary<StateKey, int>();
            for (var i = 0; i < resourceCount; i++)
            {
                items[ProductRegistry.Instance.RandomItem()] =
                    Mathf.RoundToInt(Random.Range(10, 30) * (1 + 0.08f * DayCycleManager.Instance.days));
            }

            var reward = Rng.Bool()
                ? new TaskReward
                {
                    currency = Mathf.RoundToInt(Random.Range(50, 80) * (1 + 0.6f * DayCycleManager.Instance.days)),
                }
                : new TaskReward()
                {
                    currency = -1,
                    item = ProductRegistry.Instance.RandomItem(),
                    itemCount = Mathf.RoundToInt(Random.Range(40, 70) * 0.8f * (DayCycleManager.Instance.days + 1))
                };


            return new TaskData
            {
                experienceReward = Mathf.RoundToInt(Random.Range(200, 400) * 0.35f * (DayCycleManager.Instance.days + 1)),
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