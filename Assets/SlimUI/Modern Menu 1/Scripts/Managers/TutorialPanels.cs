using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Lucy.Features.UI.Runtime
{
    public sealed class TutorialController : MonoBehaviour
    {
        #region Inspector

        [Header("References Tutorial Controller")]
        public GameObject[] tutorialPanels;
        public GameObject hud;

        [Header("Settings Tutorial Controller")]
        public string playerPrefsKey = "TutorialCompleted";
        public bool showCursor;
        public bool showIfCompleted;

        [Header("Debug")]
        public int currentPanelIndex;
        public bool tutorialActive;
        public bool tutorialCompleted;

        #endregion

        #region Events

        public event Action OnTutorialStarted;
        public event Action OnTutorialCompleted;

        #endregion

        #region Properties

        public bool IsTutorialActive => tutorialActive;
        public bool IsTutorialCompleted => tutorialCompleted;

        #endregion

        void Awake()
        {
            tutorialCompleted = PlayerPrefs.GetInt(playerPrefsKey, 0) != 0;

            if (tutorialCompleted && !showIfCompleted)
            {
                HideAllPanels();
                SetHUDActive(true);
                return;
            }

            StartTutorial();
        }

        void Update()
        {
            if (!tutorialActive)
                return;

            if (Mouse.current == null)
                return;

            if (Mouse.current.leftButton.wasPressedThisFrame)
                NextPanel();
        }

        public void StartTutorial()
        {
            currentPanelIndex = 0;
            tutorialActive = true;
            tutorialCompleted = false;

            SetHUDActive(false);
            SetCursorVisible(showCursor);
            ShowPanel(currentPanelIndex);

            OnTutorialStarted?.Invoke();
        }

        public void NextPanel()
        {
            if (!tutorialActive)
                return;

            currentPanelIndex++;

            if (currentPanelIndex >= tutorialPanels.Length)
            {
                CompleteTutorial();
                return;
            }

            ShowPanel(currentPanelIndex);
        }

        public void CompleteTutorial()
        {
            tutorialActive = false;
            tutorialCompleted = true;

            HideAllPanels();
            SetHUDActive(true);
            SetCursorVisible(false);

            PlayerPrefs.SetInt(playerPrefsKey, 1);
            PlayerPrefs.Save();

            OnTutorialCompleted?.Invoke();
        }

        public void ResetTutorial()
        {
            PlayerPrefs.DeleteKey(playerPrefsKey);
            PlayerPrefs.Save();

            tutorialCompleted = false;
        }

        void ShowPanel(int index)
        {
            for (int i = 0; i < tutorialPanels.Length; i++)
            {
                if (tutorialPanels[i] != null)
                    tutorialPanels[i].SetActive(i == index);
            }
        }

        void HideAllPanels()
        {
            foreach (GameObject panel in tutorialPanels)
            {
                if (panel != null)
                    panel.SetActive(false);
            }
        }

        void SetHUDActive(bool active)
        {
            if (hud != null)
                hud.SetActive(active);
        }

        void SetCursorVisible(bool visible)
        {
            Cursor.visible = visible;
            Cursor.lockState = visible ? CursorLockMode.None : CursorLockMode.Locked;
        }
    }
}