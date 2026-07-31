using UnityEngine;
using UnityEngine.Playables;
using Unity.VisualScripting;

public class PlayableDirectorAdapter : MonoBehaviour
{
    [Header("Settings")]
    [Tooltip("Имя Custom Event в начале таймлайна")]
    public string startEventName = "StartTimeline";
    [Tooltip("Имя Custom Event в конце таймлайна")]
    public string endEventName = "EndTimeline";
    [Tooltip("На каком объекте запускать событие (по умолчанию - этот)")]
    public GameObject target = null;

    PlayableDirector director;


    void Awake()
    {
        director = GetComponent<PlayableDirector>();
        if (target == null) target = gameObject; 
    }

    void OnEnable()
    {
        if (director != null)
        {
            director.played += OnTimelinePlayed;
            director.stopped += OnTimelineStopped;
        }
    }

    void OnDisable()
    {
        if (director != null)
        {
            director.played -= OnTimelinePlayed;
            director.stopped -= OnTimelineStopped;
        }
    }

    void OnTimelinePlayed(PlayableDirector pd)
    {
        if (!string.IsNullOrEmpty(startEventName)) CustomEvent.Trigger(target, startEventName);
    }

    void OnTimelineStopped(PlayableDirector pd)
    {
        if (!string.IsNullOrEmpty(endEventName)) CustomEvent.Trigger(target, endEventName);
    }
}