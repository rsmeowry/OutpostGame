using System;
using System.Collections.Generic;
using System.Linq;
using External.Util;
using Game.Citizens;
using Game.DayNight;
using Game.State;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

namespace Game.News
{
    public class NewsManager: MonoBehaviour
    {
        public static NewsManager Instance { get; private set; }

        private float _accumulatedProbability = 0.0125f;
        private void Start()
        {
            Instance = this;
            DayCycleManager.Instance.onDayChanged.AddListener(() =>
            {
                dailyNews.Clear();
            });
            DayCycleManager.Instance.onHourPassed.AddListener(() =>
            {
                var happened = Random.Range(0f, 1f) < _accumulatedProbability;
                if (!happened)
                    _accumulatedProbability *= 2f;
                else
                {
                    PushRandomNews();
                    _accumulatedProbability = 0.0125f;
                }
            });
        }

        public static Stack<NewsEntry> dailyNews = new();
        public UnityEvent newsUpdated = new();

        public void PushNews(NewsEntry entry)
        {
            dailyNews.Push(entry);
            newsUpdated.Invoke();
        }

        public void PushRandomNews()
        {
            var shouldBeAboutBear = Random.Range(0f, 1f) < 0.25f;
            if (shouldBeAboutBear)
            {
                var bear = Rng.Choice(CitizenManager.Instance.Citizens.Values.ToList());
                var text = Rng.Choice(NewsLines.citizenLines);
                var more = text.Item2.Replace("%s", bear.PersistentData.Name);
                dailyNews.Push(new NewsEntry
                {
                    Title = text.Item1,
                    MoreInfo = more
                });
            }
            else
            {
                var text = Rng.Choice(NewsLines.randomLines);
                dailyNews.Push(new NewsEntry
                {
                    Title = text.Item1,
                    MoreInfo = text.Item2
                });
            }
            newsUpdated.Invoke();
        }

        public void PushResourceSoar(StateKey key)
        {
            var text = Rng.Choice(NewsLines.marketSoar[key]);
            dailyNews.Push(new NewsEntry
            {
                Title = text.Item1,
                MoreInfo = text.Item2
            });
            newsUpdated.Invoke();
        }
        
        public void PushResourceCrash(StateKey key)
        {
            var text = Rng.Choice(NewsLines.marketCrash[key]);
            dailyNews.Push(new NewsEntry
            {
                Title = text.Item1,
                MoreInfo = text.Item2
            });
            newsUpdated.Invoke();
        }
    }

    public struct NewsEntry
    {
        public string Title;
        public string MoreInfo;
    }
}