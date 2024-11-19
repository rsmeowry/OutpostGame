using System;
using UnityEngine;

namespace UI.Util
{
    public class SimpleTooltipDisplay: MonoBehaviour
    {
        [SerializeField]
        private string title;
        [SerializeField]
        [TextArea(4, 5)]
        private string body;
        
        private void OnMouseEnter()
        {
            TooltipCtl.Instance.Show(title, body);
        }

        private void OnMouseExit()
        {
            TooltipCtl.Instance.Hide();
        }
    }
}