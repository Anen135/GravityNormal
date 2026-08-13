using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Lucy.Features.UI.Runtime
{
    public sealed class AudioSettingsWindow : MonoBehaviour
    {
        #region Inspector

        [Header("References Audio Settings Window")]
        public CanvasGroup windowCanvasGroup;
        public RectTransform revealArea;
        public Button openButton;
        public Button closeButton;
        public Button backgroundButton;

        [Header("Settings Audio Settings Window")]
        [Min(0f)] public float openedHeight = 420f;
        [Min(0.01f)] public float animationDuration = 0.25f;
        public AnimationCurve animationCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);
        public bool closeOnEscape = true;
        public bool startOpened;

        [Header("Debug")]
        public bool isOpen;
        public bool isAnimating;
        public float currentHeight;

        #endregion

        Coroutine animationRoutine;

        void Reset()
        {
            windowCanvasGroup = windowCanvasGroup != null ? windowCanvasGroup : GetComponent<CanvasGroup>();
            revealArea = revealArea != null ? revealArea : GetComponent<RectTransform>();
        }

        void Awake()
        {
            windowCanvasGroup = windowCanvasGroup != null ? windowCanvasGroup : GetComponent<CanvasGroup>();
            revealArea = revealArea != null ? revealArea : GetComponent<RectTransform>();

            if (openedHeight <= 0f && revealArea != null)
                openedHeight = Mathf.Max(revealArea.rect.height, revealArea.sizeDelta.y);

            BindButtons();
        }

        void Start()
        {
            ApplyInstant(startOpened);
        }

        void Update()
        {
            if (closeOnEscape && isOpen && Input.GetKeyDown(KeyCode.Escape))
                Close();
        }

        void OnDestroy()
        {
            UnbindButtons();
        }

        void BindButtons()
        {
            if (openButton != null) openButton.onClick.AddListener(Open);
            if (closeButton != null) closeButton.onClick.AddListener(Close);
            if (backgroundButton != null) backgroundButton.onClick.AddListener(Close);
        }

        void UnbindButtons()
        {
            if (openButton != null) openButton.onClick.RemoveListener(Open);
            if (closeButton != null) closeButton.onClick.RemoveListener(Close);
            if (backgroundButton != null) backgroundButton.onClick.RemoveListener(Close);
        }

        public void Toggle()
        {
            if (isOpen) Close();
            else Open();
        }

        public void Open()
        {
            StartAnimation(true);
        }

        public void Close()
        {
            StartAnimation(false);
        }

        public void ApplyInstant(bool opened)
        {
            isOpen = opened;
            isAnimating = false;

            if (windowCanvasGroup != null)
            {
                windowCanvasGroup.alpha = opened ? 1f : 0f;
                windowCanvasGroup.interactable = opened;
                windowCanvasGroup.blocksRaycasts = opened;
            }

            SetRevealHeight(opened ? openedHeight : 0f);
        }

        void StartAnimation(bool openTarget)
        {
            if (animationRoutine != null)
                StopCoroutine(animationRoutine);

            animationRoutine = StartCoroutine(Animate(openTarget));
        }

        IEnumerator Animate(bool openTarget)
        {
            isAnimating = true;

            if (windowCanvasGroup != null && openTarget)
            {
                windowCanvasGroup.interactable = true;
                windowCanvasGroup.blocksRaycasts = true;
            }

            float startHeight = revealArea != null ? revealArea.sizeDelta.y : 0f;
            float targetHeight = openTarget ? openedHeight : 0f;

            float startAlpha = windowCanvasGroup != null ? windowCanvasGroup.alpha : 0f;
            float targetAlpha = openTarget ? 1f : 0f;

            float time = 0f;

            while (time < animationDuration)
            {
                time += Time.unscaledDeltaTime;
                float progress = Mathf.Clamp01(time / animationDuration);
                float eased = animationCurve != null ? animationCurve.Evaluate(progress) : progress;

                float height = Mathf.Lerp(startHeight, targetHeight, eased);
                float alpha = Mathf.Lerp(startAlpha, targetAlpha, eased);

                SetRevealHeight(height);

                if (windowCanvasGroup != null)
                    windowCanvasGroup.alpha = alpha;

                yield return null;
            }

            SetRevealHeight(targetHeight);

            if (windowCanvasGroup != null)
            {
                windowCanvasGroup.alpha = targetAlpha;
                windowCanvasGroup.interactable = openTarget;
                windowCanvasGroup.blocksRaycasts = openTarget;
            }

            isOpen = openTarget;
            isAnimating = false;
            animationRoutine = null;
        }

        void SetRevealHeight(float height)
        {
            if (revealArea == null) return;

            Vector2 size = revealArea.sizeDelta;
            size.y = height;
            revealArea.sizeDelta = size;
            currentHeight = height;
        }
    }
}