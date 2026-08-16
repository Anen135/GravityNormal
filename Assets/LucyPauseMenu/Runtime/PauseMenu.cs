using System.IO;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
#endif

namespace Lucy.Features.UI.Runtime
{
    public sealed class PauseMenu : MonoBehaviour
    {
        #region Inspector

        [Header("References Pause Menu")]
        public GameObject pauseRoot;
        public GameObject mainPanel;
        public GameObject settingsPanel;
        public GameObject HUD;
        public Button resumeButton;
        public Button settingsButton;
        public Button mainMenuButton;
        public Button closeSettingsButton;

        [Header("Settings Pause Menu")]
        public string mainMenuSceneName = "MainMenu";
        public bool listenForEscape = true;
        public bool lockCursorWhilePlaying = true;
        public bool restoreTimeScaleOnDisable = true;

        [Header("Debug")]
        public bool isPaused;
        public float previousTimeScale = 1f;

        #endregion

        void Awake()
        {
            EnsureEventSystem();
            BindButtons();

            if (pauseRoot != null)
                pauseRoot.SetActive(false);

            isPaused = false;
        }

        void OnDestroy()
        {
            UnbindButtons();

            if (isPaused && restoreTimeScaleOnDisable)
                Time.timeScale = previousTimeScale > 0f ? previousTimeScale : 1f;
        }

        void Update()
        {
            if (!listenForEscape || !PausePressed())
                return;

            if (!isPaused)
            {
                Pause();
                return;
            }

            if (settingsPanel != null && settingsPanel.activeSelf)
            {
                ShowMainPanel();
                return;
            }

            Resume();
        }

        public void TogglePause()
        {
            if (isPaused)
                Resume();
            else
                Pause();
        }

        public void Pause()
        {
            if (isPaused)
                return;

            previousTimeScale = Time.timeScale > 0f ? Time.timeScale : 1f;
            Time.timeScale = 0f;
            isPaused = true;

            if (pauseRoot != null)
                pauseRoot.SetActive(true);
            HUD.SetActive(false);
            ShowMainPanel();
            SetPauseCursor();
        }

        public void Resume()
        {
            if (!isPaused)
                return;

            Time.timeScale = previousTimeScale > 0f ? previousTimeScale : 1f;
            isPaused = false;

            if (pauseRoot != null)
                pauseRoot.SetActive(false);
            HUD.SetActive(true);
            SetGameplayCursor();
        }

        public void OpenSettings()
        {
            if (mainPanel != null)
                mainPanel.SetActive(false);

            if (settingsPanel != null)
                settingsPanel.SetActive(true);
        }

        public void ShowMainPanel()
        {
            if (settingsPanel != null)
                settingsPanel.SetActive(false);

            if (mainPanel != null)
                mainPanel.SetActive(true);
        }

        public void LoadMainMenu()
        {
            Time.timeScale = 1f;
            isPaused = false;

            string sceneToLoad = ResolveMainMenuScene();

            if (string.IsNullOrEmpty(sceneToLoad))
                return;

            SceneManager.LoadScene(sceneToLoad);
        }

        void BindButtons()
        {
            if (resumeButton != null)
                resumeButton.onClick.AddListener(Resume);

            if (settingsButton != null)
                settingsButton.onClick.AddListener(OpenSettings);

            if (mainMenuButton != null)
                mainMenuButton.onClick.AddListener(LoadMainMenu);

            if (closeSettingsButton != null)
                closeSettingsButton.onClick.AddListener(ShowMainPanel);
        }

        void UnbindButtons()
        {
            if (resumeButton != null)
                resumeButton.onClick.RemoveListener(Resume);

            if (settingsButton != null)
                settingsButton.onClick.RemoveListener(OpenSettings);

            if (mainMenuButton != null)
                mainMenuButton.onClick.RemoveListener(LoadMainMenu);

            if (closeSettingsButton != null)
                closeSettingsButton.onClick.RemoveListener(ShowMainPanel);
        }

        bool PausePressed()
        {
#if ENABLE_INPUT_SYSTEM
            return Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame;
#else
            return Input.GetKeyDown(KeyCode.Escape);
#endif
        }

        void SetPauseCursor()
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }

        void SetGameplayCursor()
        {
            if (!lockCursorWhilePlaying)
                return;

            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }

        string ResolveMainMenuScene()
        {
            if (SceneExistsInBuild(mainMenuSceneName))
                return mainMenuSceneName;

            if (SceneExistsInBuild("MainMenu"))
                return "MainMenu";

            if (SceneExistsInBuild("Menu"))
                return "Menu";

            return string.Empty;
        }

        bool SceneExistsInBuild(string sceneName)
        {
            if (string.IsNullOrWhiteSpace(sceneName))
                return false;

            for (int i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
            {
                string path = SceneUtility.GetScenePathByBuildIndex(i);
                string name = Path.GetFileNameWithoutExtension(path);

                if (name == sceneName)
                    return true;
            }

            return false;
        }

        void EnsureEventSystem()
        {
            if (EventSystem.current != null)
                return;

            GameObject eventSystemObject = new GameObject("EventSystem");
            eventSystemObject.AddComponent<EventSystem>();

#if ENABLE_INPUT_SYSTEM
            eventSystemObject.AddComponent<InputSystemUIInputModule>();
#else
            eventSystemObject.AddComponent<StandaloneInputModule>();
#endif
        }
    }
}
