using System.Collections;
using UnityEngine;
using Unity.Cinemachine;
using UnityEngine.InputSystem;

namespace Lucy.Shared.Basic
{
    public class TutorialManager : MonoBehaviour
    {
        [Header("UI Components")]
        public GameObject panel;
        public TextTyper typer;
        public Animator panelAnimator;

        [Header("Cinemachine Camera Settings")]
        public CinemachineCamera tutorialCamera;
        public float cameraBlendTime = 1.5f;
        public Transform[] followTargets;
        public Transform[] lookAtTargets;

        [Header("Tutorial Flows")]
        [TextArea(3, 5)] 
        public string[] tutorialTexts;
        
        [Header("External Systems Trigger")]
        [Tooltip("Активируется сразу после завершения туториала")]
        public GameObject zombieControlManagerObject;

        int currentStep = 0;
        bool tutorialActive = true;
        bool isBusy = false;

        void Awake()
        {
            if (PlayerPrefs.GetInt("TutorialDone", 0) == 1)
            {
                if (zombieControlManagerObject != null) zombieControlManagerObject.SetActive(true);

                gameObject.SetActive(false);
                return;
            }
        }

        void Start()
        {
            if (panelAnimator == null && panel != null) panelAnimator = panel.GetComponent<Animator>();
            if (ValidateDependencies()) StartCoroutine(IntroSequence());
        }

        void Update()
        {
            if (!tutorialActive) return;

            bool skipOrNextPressed = Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame || Keyboard.current != null && Keyboard.current.enterKey.wasPressedThisFrame;
            if (skipOrNextPressed) HandleInputTrigger();
        }

        void HandleInputTrigger()
        {
            if (typer.IsTyping) typer.Skip();
            else if (!isBusy) StartCoroutine(NextStepSequence());
        }

        IEnumerator IntroSequence()
        {
            panel.SetActive(true);

            if (followTargets.Length > 0 && followTargets[0] != null) tutorialCamera.Follow = followTargets[0];
            
            if (lookAtTargets.Length > 0 && lookAtTargets[0] != null) tutorialCamera.LookAt = lookAtTargets[0];

            yield return ShowTextRoutine(tutorialTexts[0]);
        }

        IEnumerator NextStepSequence()
        {
            isBusy = true;

            if (panelAnimator != null)
            {
                panelAnimator.SetTrigger("Hide");
                yield return new WaitForSeconds(0.5f); 
            }

            currentStep++;

            if (currentStep >= tutorialTexts.Length)
            {
                EndTutorial();
                yield break;
            }

            if (currentStep < followTargets.Length && currentStep < lookAtTargets.Length)
            {
                yield return SmoothChangeCameraTargets(followTargets[currentStep], lookAtTargets[currentStep]);
            }

            if (panelAnimator != null)
            {
                panelAnimator.SetTrigger("Show");
                yield return new WaitForSeconds(0.5f);
            }

            yield return ShowTextRoutine(tutorialTexts[currentStep]);

            isBusy = false;
        }

        IEnumerator SmoothChangeCameraTargets(Transform newFollow, Transform newLookAt)
        {
            if (newFollow == null || newLookAt == null) yield break;

            tutorialCamera.Follow = newFollow;
            tutorialCamera.LookAt = newLookAt;
            
            yield return new WaitForSeconds(cameraBlendTime);
        }

        IEnumerator ShowTextRoutine(string text)
        {
            typer.StartTyping(text);
            
            while (typer.IsTyping)
            {
                yield return null;
            }
        }

        void EndTutorial()
        {
            tutorialActive = false;
            
            if (panel != null)
                panel.SetActive(false);

            if (zombieControlManagerObject != null)
                zombieControlManagerObject.SetActive(true);

            PlayerPrefs.SetInt("TutorialDone", 1);
            PlayerPrefs.Save(); 

            gameObject.SetActive(false);
        }

        bool ValidateDependencies()
        {
            if (panel == null || typer == null || tutorialCamera == null)
            {
                Debug.LogError($"[{nameof(TutorialManager)}] Критические зависимости не назначены в инспекторе!", this);
                tutorialActive = false;
                return false;
            }

            if (tutorialTexts == null || tutorialTexts.Length == 0)
            {
                Debug.LogWarning($"[{nameof(TutorialManager)}] Массив текстов Tutorial Texts пуст!", this);
                tutorialActive = false;
                return false;
            }

            return true;
        }
    }
}