using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Lucy.UI
{
    public sealed class SpaceMenuView : MonoBehaviour
    {
        #region Inspector

        [Header("References Space Menu View")]
        public Canvas canvas;
        public Image backgroundImage;
        public Image starfieldImage;
        public Image sunImage;
        public Image panelImage;

        public Button playButton;
        public Button settingsButton;
        public Button exitButton;

        [Header("Settings Space Menu View")]
        public bool autoFocusPlayButton = true;
        [Min(0f)] public float sunScale = 1f;

        [Header("Debug")]
        public bool debugEnabled;
        public bool gizmosEnabled;
        public bool isInteractable = true;

        #endregion

        #region Unity messages

        void Awake()
        {
            ApplySunScale();
        }

        void Start()
        {
            if (autoFocusPlayButton && playButton != null)
                playButton.Select();
        }

        void OnValidate()
        {
            ApplySunScale();
        }

        #endregion

        #region Public methods

        public void SetInteractable(bool value)
        {
            isInteractable = value;

            if (playButton != null) playButton.interactable = value;
            if (settingsButton != null) settingsButton.interactable = value;
            if (exitButton != null) exitButton.interactable = value;
        }

        public void Bind(
            UnityAction onPlay,
            UnityAction onSettings,
            UnityAction onExit)
        {
            if (playButton != null)
            {
                playButton.onClick.RemoveAllListeners();
                if (onPlay != null) playButton.onClick.AddListener(onPlay);
            }

            if (settingsButton != null)
            {
                settingsButton.onClick.RemoveAllListeners();
                if (onSettings != null) settingsButton.onClick.AddListener(onSettings);
            }

            if (exitButton != null)
            {
                exitButton.onClick.RemoveAllListeners();
                if (onExit != null) exitButton.onClick.AddListener(onExit);
            }
        }

        public void SetButtonsActive(bool play, bool settings, bool exit)
        {
            if (playButton != null) playButton.gameObject.SetActive(play);
            if (settingsButton != null) settingsButton.gameObject.SetActive(settings);
            if (exitButton != null) exitButton.gameObject.SetActive(exit);
        }

        public void LogDebug(string message)
        {
            if (!debugEnabled) return;
            Debug.Log($"[SpaceMenuView] {message}", this);
        }

        #endregion

        #region Private methods

        void ApplySunScale()
        {
            if (sunImage == null) return;
            sunImage.rectTransform.localScale = Vector3.one * Mathf.Max(0.01f, sunScale);
        }

        #endregion
    }
}
