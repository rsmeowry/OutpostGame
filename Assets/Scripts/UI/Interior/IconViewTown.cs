using System.Collections;
using DG.Tweening;
using External.Util;
using Game.Controllers;
using Game.Stocks;
using Inside;
using UI.BottomRow;
using UI.Util;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UI.Interior
{
    public class IconViewTown: MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
    {
        [SerializeField]
        private TownCameraController cameraController;

        [SerializeField]
        private InsideCameraController inside;

        [SerializeField]
        private GameObject room;

        [SerializeField]
        private Image black;

        public void OnPointerEnter(PointerEventData eventData)
        {
            TooltipCtl.Instance.Show("Просмотреть поселение", "Наблюдайте за поселением, стройте здания и развивайте его!");
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            TooltipCtl.Instance.Hide();
        }

        private bool _busy;
        public void OnPointerClick(PointerEventData eventData)
        {
            if (_busy)
                return;
            _busy = true;
            ScreenFocus.Instance.DoExit();
            ViewSwitchCtl.Instance.StartCoroutine(Transition().Callback(() => TooltipCtl.Instance.Hide()));
        }   

        private IEnumerator Transition()
        {
            yield return ViewSwitchCtl.Instance.SwitchToGlobal();
            inside.gameObject.SetActive(false);
            cameraController.gameObject.SetActive(true);
            _busy = false;
            TooltipCtl.Instance.Hide();
            yield return black.DOFade(0f, 0.5f).Play().WaitForCompletion();
            room.SetActive(false);
        }
    }
}