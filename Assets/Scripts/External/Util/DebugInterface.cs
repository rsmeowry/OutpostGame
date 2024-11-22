using System;
using System.Linq;
using Game.State;
using TMPro;
using UnityEngine;

namespace External.Util
{
    public class DebugInterface: MonoBehaviour
    {
        private bool _doShow;

        private TMP_Text _text;

        private void Start()
        {
            _text = GetComponent<TMP_Text>();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.N))
            {
                _doShow = !_doShow;
                UpdateState();
            }
        }

        private void FixedUpdate()
        {
            if(_doShow)
                _text.text = $@"====== RESOURCES
{GameStateManager.Instance.PlayerProductCount.Select(it => "* " + it.Key.Formatted() + ": " + it.Value).ToLineSeparatedString()}
======
    ";
        }

        private void UpdateState()
        {
            if(!_doShow)
                _text.text = "";
            else
            {
                _text.gameObject.SetActive(true);
            }
        }
    }
}