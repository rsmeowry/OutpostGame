using System;
using DG.Tweening;
using Game.Controllers.States;
using Game.Sound;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Menu
{
    public class WizardButton: MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        private AudioSource _as;

        [SerializeField]
        private AudioClip wizard;

        private void Start()
        {
            _as = GetComponentInParent<AudioSource>();
        }

        private Tween _tween;
        public void OnPointerEnter(PointerEventData eventData)
        {
            _as.PlayOneShot(wizard);
            _tween.Kill();
            _tween = transform.DOScale(Vector3.one * 1.1f, 0.2f).Play();
        }
        
        public void OnPointerExit(PointerEventData eventData)
        {
            _tween.Kill();
            _tween = transform.DOScale(Vector3.one, 0.2f).Play();
        }
    }
}