using Game.Sound;
using UnityEngine;
using UnityEngine.EventSystems;

namespace UI.Util
{
    public class SoundClick: MonoBehaviour, IPointerClickHandler
    {
        [SerializeField]
        private string soundEvent;
        [SerializeField]
        private Vector2 pitchRange = new Vector2(0.8f, 1.2f);
        
        public void OnPointerClick(PointerEventData eventData)
        {
            SoundManager.Instance.PlaySound2D(SoundBank.Instance.GetSound(soundEvent), 0.8f, Random.Range(pitchRange.x, pitchRange.y));
        }
    }
}