using System;
using System.Collections;
using DG.Tweening;
using External.Util;
using Game.Controllers;
using UI.BottomRow;
using UnityEngine;
using UnityEngine.UI;

namespace Inside
{
    public class ViewSwitchCtl: MonoBehaviour
    {
        [SerializeField]
        private Image black;
        
        public static ViewSwitchCtl Instance { get; private set; }

        public void Awake()
        {
            Instance = this;
        }

        public IEnumerator SwitchToGlobal()
        {
            yield return black.DOFade(1f, 0.3f).Play().WaitForCompletion();
            GC.Collect();
            yield return BottomRowCtl.Instance.ShowTopRow();
        }

        public IEnumerator SwitchToLocal()
        {
            yield return TownCameraController.Instance.StateMachine.SwitchState(TownCameraController.Instance
                .FreeMoveState); 
            
            GC.Collect();
            yield return BottomRowCtl.Instance.HideTopRow();
        }
    }
}