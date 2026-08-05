using System.Collections;
using Lucy.Core.Basic; // Подключаем пространство имен нашего тайпера
using UnityEngine;

public class InspectText : MonoBehaviour
{

    [Header("Settings")]
    [TextArea(3, 10)]
    public string inspectText;
    [Tooltip("Сколько текст будет висеть на экране после завершения печати")]
    public float stayDuration = 3f;

    [Header("References")]
    [Tooltip("Ссылка на компонент TextTyper, который будет печатать этот текст")]
    public TextTyper textTyper;

    Coroutine displayLifecycleCoroutine;

    public void ShowText()
    {
        if (textTyper == null)
        {
            Debug.LogWarning($"[{nameof(InspectText)}] TextTyper reference is missing!", this);
            return;
        }

        if (displayLifecycleCoroutine != null) StopCoroutine(displayLifecycleCoroutine);

        displayLifecycleCoroutine = StartCoroutine(DisplayLifecycle());
    }

    public void ClearText()
    {
        if (displayLifecycleCoroutine != null)
        {
            StopCoroutine(displayLifecycleCoroutine);
            displayLifecycleCoroutine = null;
        }

        if (textTyper != null) textTyper.Clear();
    }

    private IEnumerator DisplayLifecycle()
    {
        textTyper.StartTyping(inspectText);

        while (textTyper.IsTyping) yield return null; 

        yield return new WaitForSeconds(stayDuration);

        textTyper.Clear();
        displayLifecycleCoroutine = null;
    }
}