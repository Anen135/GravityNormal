using UnityEngine;
using UnityEngine.EventSystems;

namespace Lucy.Features.UI.Runtime
{
    [RequireComponent(typeof(RectTransform))]
    public sealed class ButtonHoverEffect : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        #region Inspector

        [Header("Settings Button Hover Effect")]
        public float hoverDistance = 0.01f;
        public float transitionSpeed = 12f;

        [Header("Debug")]
        public RectTransform rectTransform;
        public bool isHovered;
        public Vector3 defaultPosition;

        #endregion

        public void Init()
        {
            rectTransform = GetComponent<RectTransform>();
            defaultPosition = rectTransform.localPosition;
        }

        void Update()
        {
            Vector3 targetPosition = isHovered
                ? defaultPosition + Vector3.back * hoverDistance
                : defaultPosition;

            rectTransform.localPosition = Vector3.Lerp(
                rectTransform.localPosition,
                targetPosition,
                transitionSpeed * Time.unscaledDeltaTime
            );
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            isHovered = true;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            isHovered = false;
        }

        void OnDisable()
        {
            isHovered = false;

            if (rectTransform != null)
                rectTransform.localPosition = defaultPosition;
        }
    }
}