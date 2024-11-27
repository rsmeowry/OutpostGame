using DG.Tweening;
using UI.Util;
using UnityEngine;
using UnityEngine.EventSystems;

namespace UI.Interior
{
    public class IconTrashBin: MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField]
        private GameObject unrealPrefab;

        private bool _anim;
        public void OnPointerClick(PointerEventData eventData)
        {
            if (_anim)
                return;
            _anim = true;
            var unreal = (RectTransform) Instantiate(unrealPrefab, transform).transform;
            unreal.SetSiblingIndex(0);
            var seq = DOTween.Sequence();
            seq.Join(unreal.DOBlendableRotateBy(new Vector3(0f, 0f, 360f), 1f, RotateMode.FastBeyond360));
            seq.Join(unreal.DOAnchorPosY(unreal.anchoredPosition.y + 100f, 0.5f));
            seq.Insert(0.5f, unreal.DOAnchorPosY(unreal.anchoredPosition.y, 0.5f));
            seq.OnComplete(() =>
            {
                _anim = false;
                Destroy(unreal.gameObject);
            });
            seq.Play();
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            TooltipCtl.Instance.Show("Корзина", "", 0.4f);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            TooltipCtl.Instance.Hide();
        }
    }
}