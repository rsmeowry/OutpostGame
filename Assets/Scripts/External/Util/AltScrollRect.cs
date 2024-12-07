using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace External.Util
{
    public class AltScrollRect: ScrollRect
    {
        public override void OnScroll(PointerEventData data)
        {
            RectTransformUtility.ScreenPointToWorldPointInRectangle(_thisTransform, Input.mousePosition,
                Camera.main, out var localPoint);
            Debug.Log(new Vector3(localPoint.x, localPoint.y, 0).Formatted());
            var delta = data.scrollDelta.y;

            switch (delta)
            {
                case > 0 when _scale.x < maximumScale: //zoom in

                    _scale.Set(_scale.x + scaleIncrement, _scale.y + scaleIncrement, 1f);
                    _thisTransform.localScale = _scale;
                    _thisTransform.anchoredPosition -= 0.5f * ((Vector2) localPoint * scaleIncrement);
                    break;
                case < 0 when _scale.x > minimumScale:
                {
                    //zoom out
                    var scalex = Mathf.Clamp(_scale.x - scaleIncrement, minimumScale, maximumScale);
                    var scaley = Mathf.Clamp(_scale.y - scaleIncrement, minimumScale, maximumScale);
                    _scale.Set(scalex, scaley, 1f);
                    _thisTransform.localScale = _scale;
                    _thisTransform.anchoredPosition += 0.5f * ((Vector2) localPoint * scaleIncrement);
                    break;
                }
            }
        }
        
        //Make sure these values are evenly divisible by scaleIncrement
        [FormerlySerializedAs("_minimumScale")] [SerializeField] private float minimumScale = 0.5f;
        [FormerlySerializedAs("_initialScale")] [SerializeField] private float initialScale = 1f;
        [FormerlySerializedAs("_maximumScale")] [SerializeField] private float maximumScale = 1.5f;
        /////////////////////////////////////////////
        [FormerlySerializedAs("_scaleIncrement")] [SerializeField] private float scaleIncrement = .05f;
        /////////////////////////////////////////////

        [HideInInspector] Vector3 _scale;

        RectTransform _thisTransform;

        private void Start()
        {
            base.Awake();
            
            _thisTransform = (RectTransform) transform.GetChild(0);

            _scale.Set(initialScale, initialScale, 1f);
            _thisTransform.localScale = _scale;
        }

    }
}