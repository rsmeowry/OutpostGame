using System;
using DG.Tweening;
using External.Data;
using Game.Sound;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

namespace UI.Interior
{
    public class NotesComponentHandler: MonoBehaviour
    {
        [SerializeField]
        private TMP_InputField textArea;

        [SerializeField]
        private float shakeIntensity;

        private RectTransform _rect;

        private void OnDestroy()
        {
            MiscSavedData.Instance.Data.NotesData = textArea.text;
        }

        private Tween _shaker;
        private void Start()
        {
            textArea.text = MiscSavedData.Instance.Data.NotesData;
            var rect = (RectTransform)textArea.transform;
            var staticPivot = rect.pivot;
            textArea.onValueChanged.AddListener(val =>
            {
                _shaker?.Kill();
                rect.pivot = staticPivot;
                _shaker = rect.DOShakeAnchorPos(0.1f, strength: shakeIntensity).Play();
                SoundManager.Instance.PlaySound2D(SoundBank.Instance.GetSound("ui.keyboard"), 0.6f, Random.Range(0.8f, 1.2f));
            });
        }
    }
}