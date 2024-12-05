using TMPro;
using UnityEngine;

namespace Tutorial.Impl
{
    public class ChangeViewStateTutorialStep: TutorialStep
    {
        [SerializeField]
        private GameObject prefab;
        
        public override void Activate()
        {
            TutorialCtl.Instance.topBar.GetComponentInChildren<TMP_Text>().text = "Зайдите в контрольную комнату";
        }

        public override void DisplayModal(RectTransform modal)
        {
            var inst = Instantiate(prefab, modal);
            inst.transform.SetSiblingIndex(1);
        }

        public override void ReceiveEnterHome()
        {
            MarkDone();
        }
    }
}