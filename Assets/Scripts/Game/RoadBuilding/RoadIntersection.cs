using System;
using UI.Util;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Game.RoadBuilding
{
    public class RoadIntersection: MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
    {
        private Renderer _rd;
        private BoxCollider _bx;
        private static readonly int HoloColor = Shader.PropertyToID("_Color");

        [SerializeField] [ColorUsage(false, true)]
        private Color goodColor;
        [SerializeField] [ColorUsage(false, true)]
        private Color badColor;

        private bool _isBad;
        
        internal void Init()
        {
            _bx = GetComponent<BoxCollider>();
            _rd = GetComponentInChildren<Renderer>();
            _rd.material.SetColor(HoloColor, goodColor);
        }

        public void Show()
        {
            _rd.enabled = true;
            _bx.enabled = true;
        }

        public void Hide()
        {
            _rd.enabled = false;
            _bx.enabled = false;
        }

        public void MarkState(bool bad)
        {
            if (!_rd.enabled)
                return;
            var wasBad = _isBad;
            _isBad = bad;
            if (wasBad && !bad)
            {
                _rd.material.SetColor(HoloColor, goodColor);
            } else if (!wasBad && bad)
            {
                _rd.material.SetColor(HoloColor, badColor);
            }
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            TooltipCtl.Instance.Show("Дорога", "Нажмите чтобы начать строить дорогу из этой точки", 0.2f);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            TooltipCtl.Instance.Hide();
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if(eventData.button == PointerEventData.InputButton.Left)
                RoadManager.Instance.HandleClick(this);
        }
    }
}