using UnityEngine;
using System.Collections;

public class CanvasFadeController : MonoBehaviour
{
    public enum FadeMode
    {
        FadeIn,
        FadeOut
    }

    public enum TriggerMode
    {
        Distance,
        ExternalCall
    }

    [Header("References")]
    public CanvasGroup canvasGroup;
    public Transform player;

    [Header("Settings")]
    public FadeMode fadeMode = FadeMode.FadeOut;
    public TriggerMode triggerMode = TriggerMode.Distance;

    [Tooltip("Дистанция срабатывания")]
    public float triggerDistance = 2f;

    [Tooltip("Скорость появления/исчезновения")]
    public float fadeDuration = 1f;

    [Header("Options")]
    public bool disableAfterFadeOut = true;
    public bool useUnscaledTime = false;

    private bool triggered = false;
    private Coroutine fadeRoutine;

    private void Start()
    {
        if (canvasGroup == null)
        {
            Debug.LogWarning($"{name}: CanvasGroup missing!");
            enabled = false;
            return;
        }

        // Стартовое состояние
        if (fadeMode == FadeMode.FadeIn)
            canvasGroup.alpha = 0f;
        else
            canvasGroup.alpha = 1f;
    }

    private void Update()
    {
        if (triggered)
            return;

        if (triggerMode == TriggerMode.Distance)
        {
            if (player == null)
                return;

            float distance = Vector3.Distance(player.position, transform.position);

            if (distance <= triggerDistance)
            {
                TriggerFade();
            }
        }
    }

    // Внешний вызов
    public void TriggerFade()
    {
        if (triggered)
            return;

        triggered = true;

        if (fadeRoutine != null)
            StopCoroutine(fadeRoutine);

        fadeRoutine = StartCoroutine(FadeCoroutine());
    }

    IEnumerator FadeCoroutine()
    {
        float startAlpha = canvasGroup.alpha;
        float targetAlpha = fadeMode == FadeMode.FadeIn ? 1f : 0f;

        float time = 0f;

        while (time < fadeDuration)
        {
            time += useUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime;

            canvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, time / fadeDuration);

            yield return null;
        }

        canvasGroup.alpha = targetAlpha;

        // Выключение объекта после исчезновения
        if (fadeMode == FadeMode.FadeOut && disableAfterFadeOut)
        {
            gameObject.SetActive(false);
        }
    }

    // Сброс состояния
    public void ResetFade()
    {
        triggered = false;

        if (fadeMode == FadeMode.FadeIn)
            canvasGroup.alpha = 0f;
        else
            canvasGroup.alpha = 1f;

        gameObject.SetActive(true);
    }
}