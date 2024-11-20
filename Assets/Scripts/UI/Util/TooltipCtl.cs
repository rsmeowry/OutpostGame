using System;
using UnityEngine;

namespace UI.Util
{
    public class TooltipCtl: MonoBehaviour
    {
        public static TooltipCtl Instance { get; private set; }

        private Tooltip _tooltip;
        private RectTransform _rectTransform;

        private void Start()
        {
            _rectTransform = (RectTransform)transform;
            _tooltip = GetComponent<Tooltip>();
            _tooltip.transform.localScale = Vector3.zero;
            // _tooltip.gameObject.SetActive(false);
            Instance = this;
            
        }

        public void Show(string header, string body, float delay = 0.5f)
        {
            // _tooltip.gameObject.SetActive(true);
            _tooltip.SetText(header, body);
            _tooltip.DelayedShow(delay);
        }

        public void Hide()
        {
            _tooltip.Hide();
        }

        private Vector2 CalculatePivot(Vector2 normalizedPosition)
        {
            var pivotTopLeft = new Vector2(-0.05f, 1.05f);
            var pivotTopRight = new Vector2(1.05f, 1.05f);
            var pivotBottomLeft = new Vector2(-0.05f, -0.05f);
            var pivotBottomRight = new Vector2(1.05f, -0.05f);
            
            if (normalizedPosition.x < 0.5f && normalizedPosition.y >= 0.5f)
            {
                return pivotTopLeft;
            }

            else if (normalizedPosition.x > 0.5f && normalizedPosition.y >= 0.5f)
            {
                return pivotTopRight;
            }

            else if (normalizedPosition.x <= 0.5f && normalizedPosition.y < 0.5f)
            {
                return pivotBottomLeft;
            }

            else
            {
                return pivotBottomRight;
            }
        }

        private void Update()
        {
            // if (_tooltip.hidden)
                // return;
            var position = Input.mousePosition;
            var normalizedPosition = new Vector2(position.x / Screen.width, position.y / Screen.height);
            var pivot = CalculatePivot(normalizedPosition);
            _rectTransform.pivot = pivot;
            transform.position = position;
        }
    }
}