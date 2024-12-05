using TMPro;
using UnityEngine;

namespace Tutorial
{
    public abstract class TextTutorialStep: TutorialStep
    {
        [SerializeField]
        private GameObject textPrefab;
        [SerializeField]
        [TextArea(2, 4)]
        private string shownText;
        [SerializeField]
        private string title;

        public override void DisplayModal(RectTransform modal)
        {
            var txt = Instantiate(textPrefab, modal);
            txt.GetComponentInChildren<TMP_Text>().text = shownText;
            txt.transform.SetSiblingIndex(1);
        }

        public override void Activate()
        {
            TutorialCtl.Instance.topBar.GetComponentInChildren<TMP_Text>().text = title;
        }
    }
}