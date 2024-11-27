using System;
using System.Linq;
using DG.Tweening;
using External.Util;
using Game.News;
using TMPro;
using UnityEngine;

namespace UI.Interior
{
    public class NewsComponentHandler: MonoBehaviour
    {
        [SerializeField]
        private RectTransform newsContainer;

        [SerializeField]
        private GameObject singleNews;
        
        private void OnEnable()
        {
            NewsManager.Instance.newsUpdated.AddListener(UpdateNews);
            foreach (var news in NewsManager.dailyNews.Reverse())
            {
                CreateNewsObject(news);
            }

            var anch = newsContainer.anchoredPosition;
            anch.y -= newsContainer.rect.height / 1.5f;
            newsContainer.anchoredPosition = anch;
        }

        private void OnDisable()
        {
            NewsManager.Instance.newsUpdated.RemoveListener(UpdateNews);
            for (var i = 0; i < newsContainer.childCount; i++)
            {
                var child = newsContainer.GetChild(i);
                Destroy(child.gameObject);
            }
        }

        private void UpdateNews()
        {
            var latest = NewsManager.dailyNews.Peek();
            CreateNewsObject(latest, true);
        }

        private void CreateNewsObject(NewsEntry entry, bool anim = false)
        {
            var obj = Instantiate(singleNews, newsContainer);
            var title = obj.transform.GetChild(0);
            title.GetComponent<TMP_Text>().text = entry.Title;
            var desc = obj.transform.GetChild(1);
            desc.GetComponent<TMP_Text>().text = entry.MoreInfo;
            obj.transform.SetSiblingIndex(0);
            var anchMin = newsContainer.anchorMin;
            anchMin.y -= 0.01f;
            newsContainer.anchorMin = anchMin;
            
            if (!anim) return;
            obj.transform.localScale = Vector3.zero;
            obj.transform.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBack).Play();
        }
    }
}