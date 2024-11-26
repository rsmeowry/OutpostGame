﻿using System.Collections;
using DG.Tweening;
using Game.Controllers;
using UI.Util;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UI.BottomRow
{
    public class GoHomeButton: MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
    {
        [SerializeField]
        private Image black;

        [SerializeField]
        private GameObject controlRoom;
        
        public void OnPointerEnter(PointerEventData eventData)
        {
            TooltipCtl.Instance.Show("Перейти в будку", "Перемещает вас обратно в контрольную будку", 0.2f);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            TooltipCtl.Instance.Hide();
        }

        private bool _transition;
        public void OnPointerClick(PointerEventData eventData)
        {
            if (TownCameraController.Instance.interactionFilter == CameraInteractionFilter.None || _transition)
                return;

            StartCoroutine(DoClick());
        }

        private IEnumerator DoClick()
        {
            _transition = true;
            yield return black.DOFade(1f, 0.5f).Play().WaitForCompletion();
            // TODO: disable rendering of overworld elements
            yield return BottomRowCtl.Instance.HideTopRow();
            TownCameraController.Instance.gameObject.SetActive(false);
            controlRoom.SetActive(true);
            yield return black.DOFade(0f, 0.5f).Play().WaitForCompletion();
            _transition = false;
        }
    }
}