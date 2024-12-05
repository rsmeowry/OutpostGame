using Game.Citizens;
using Game.Production.POI;
using TMPro;
using UnityEngine;

namespace Tutorial.Impl
{
    public class GetNewCitizenTutorialStep: TutorialStep
    {
        [SerializeField]
        private GameObject prefab;
        
        public override void Activate()
        {
            TutorialCtl.Instance.topBar.GetComponentInChildren<TMP_Text>().text = "Пригласите медведя из системы GALACTION";
        }

        public override void DisplayModal(RectTransform modal)
        {
            var inst = Instantiate(prefab, modal);
            inst.transform.SetSiblingIndex(1);
        }

        public override void ReceiveCitizenInvited()
        {
            MarkDone();
        }
    }
}