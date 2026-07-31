using System;
using System.Collections.Generic;
using UnityEngine;
using Lucy.SO;

[RequireComponent(typeof(AudioSource))]
public class DialogueSpeaker : MonoBehaviour
{
    [Serializable]
    public class VoiceLine
    {
        [Tooltip("Значение SubtitleEntry.audioKey")]
        public string audioKey;

        public AudioClip audioClip;
    }

    [Header("Voice Settings")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private List<VoiceLine> voiceLines = new();

    private Dictionary<string, AudioClip> voiceDictionary;

    private void Awake()
    {
        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();

        audioSource.playOnAwake = false;

        CreateVoiceDictionary();
    }

    public void PlayVoiceLine(SubtitleEntry entry)
    {
        if (entry == null || string.IsNullOrWhiteSpace(entry.audioKey))
            return;

        if (!voiceDictionary.TryGetValue(entry.audioKey, out AudioClip clip))
        {
            Debug.LogWarning(
                $"Не найден аудиофайл для ключа: {entry.audioKey}",
                this
            );

            return;
        }

        audioSource.Stop();
        audioSource.clip = clip;
        audioSource.Play();
    }

    public void StopVoiceLine()
    {
        audioSource.Stop();
        audioSource.clip = null;
    }

    private void CreateVoiceDictionary()
    {
        voiceDictionary = new Dictionary<string, AudioClip>();

        foreach (VoiceLine voiceLine in voiceLines)
        {
            if (voiceLine == null ||
                string.IsNullOrWhiteSpace(voiceLine.audioKey) ||
                voiceLine.audioClip == null)
            {
                continue;
            }

            if (!voiceDictionary.TryAdd(
                    voiceLine.audioKey,
                    voiceLine.audioClip))
            {
                Debug.LogWarning(
                    $"Повторяющийся аудиоключ: {voiceLine.audioKey}",
                    this
                );
            }
        }
    }

    private void OnDestroy()
    {
        if (audioSource != null)
            audioSource.Stop();
    }
}