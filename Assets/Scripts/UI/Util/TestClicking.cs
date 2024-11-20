using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace UI.Util
{
    public class TestClicking: MonoBehaviour
    {
        private void OnMouseDown()
        {
            if(EventSystem.current.IsPointerOverGameObject() && EventSystem.current.currentSelectedGameObject != null)
                Debug.Log($"OVER GAME OBJECT {EventSystem.current.currentSelectedGameObject.name}");
            else
                Debug.Log("NOT OVER GAME OBJECT");
        }
    }
}