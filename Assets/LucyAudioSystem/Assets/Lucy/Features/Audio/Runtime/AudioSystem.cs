using System.Collections;
using UnityEngine;
using UnityEngine.Audio;

namespace Lucy.Features.Audio.Runtime
{
    [DisallowMultipleComponent]
    public sealed class AudioSystem : MonoBehaviour
    {
        #region Constants

        const string MasterVolumeParameter = "MasterVolume";
        const string MusicVolumeParameter = "MusicVolume";
        const string SFXVolumeParameter = "SFXVolume";
        const string UIVolumeParameter = "UIVolume";
        const string AmbienceVolumeParameter = "AmbienceVolume";

        const string MasterVolumeKey = "Audio.MasterVolume";
        const string MusicVolumeKey = "Audio.MusicVolume";
        const string SFXVolumeKey = "Audio.SFXVolume";
        const string UIVolumeKey = "Audio.UIVolume";
        const string AmbienceVolumeKey = "Audio.AmbienceVolume";

        const float MinimumDb = -80f;

        #endregion

        #region Inspector

        [Header("References Audio System")]
        public AudioMixer mixer;
        public AudioMixerGroup musicGroup;
        public AudioMixerGroup sfxGroup;
        public AudioMixerGroup uiGroup;
        public AudioMixerGroup ambienceGroup;

        [Header("Settings Audio System")]
        public bool dontDestroyOnLoad = true;
        public bool saveVolumes = true;
        [Min(1)] public int sfxPoolSize = 16;
        [Min(0f)] public float defaultCrossfadeDuration = 0.6f;
        [Min(0.01f)] public float spatialMinDistance = 1f;
        [Min(0.01f)] public float spatialMaxDistance = 40f;

        [Header("Settings Audio System Default Volumes")]
        [Range(0f, 1f)] public float defaultMasterVolume = 1f;
        [Range(0f, 1f)] public float defaultMusicVolume = 1f;
        [Range(0f, 1f)] public float defaultSFXVolume = 1f;
        [Range(0f, 1f)] public float defaultUIVolume = 1f;
        [Range(0f, 1f)] public float defaultAmbienceVolume = 1f;

        [Header("Debug")]
        public bool debugEnabled;
        public bool initialized;
        public AudioClip currentMusic;
        public AudioClip currentAmbience;
        [Range(0f, 1f)] public float masterVolume = 1f;
        [Range(0f, 1f)] public float musicVolume = 1f;
        [Range(0f, 1f)] public float sfxVolume = 1f;
        [Range(0f, 1f)] public float uiVolume = 1f;
        [Range(0f, 1f)] public float ambienceVolume = 1f;
        public int activeSFXSources;

        #endregion

        #region Properties

        public static AudioSystem Instance { get; private set; }
        public float MasterVolume => masterVolume;
        public float MusicVolume => musicVolume;
        public float SFXVolume => sfxVolume;
        public float UIVolume => uiVolume;
        public float AmbienceVolume => ambienceVolume;

        #endregion

        AudioSource musicSourceA;
        AudioSource musicSourceB;
        AudioSource ambienceSourceA;
        AudioSource ambienceSourceB;
        AudioSource uiSource;
        AudioSource[] sfxSources;

        bool musicUsesA = true;
        bool ambienceUsesA = true;
        int sfxCursor;

        Coroutine musicRoutine;
        Coroutine ambienceRoutine;

        #region Unity messages

        void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;

            if (dontDestroyOnLoad)
                DontDestroyOnLoad(gameObject);

            EnsureSources();
        }

        void Start()
        {
            LoadVolumes();
            ApplyAllVolumes();
            initialized = true;

            if (debugEnabled)
                Debug.Log("AudioSystem initialized.", this);
        }

        void Update()
        {
            if (!debugEnabled || sfxSources == null)
                return;

            int count = 0;
            foreach (AudioSource source in sfxSources)
                if (source != null && source.isPlaying)
                    count++;

            activeSFXSources = count;
        }

        void OnValidate()
        {
            sfxPoolSize = Mathf.Max(1, sfxPoolSize);
            spatialMinDistance = Mathf.Max(0.01f, spatialMinDistance);
            spatialMaxDistance = Mathf.Max(spatialMinDistance, spatialMaxDistance);
        }

        void OnDestroy()
        {
            if (Instance == this)
                Instance = null;
        }

        #endregion

        #region Volume

        public void SetMasterVolume(float value) => SetVolume(AudioBus.Master, value);
        public void SetMusicVolume(float value) => SetVolume(AudioBus.Music, value);
        public void SetSFXVolume(float value) => SetVolume(AudioBus.SFX, value);
        public void SetUIVolume(float value) => SetVolume(AudioBus.UI, value);
        public void SetAmbienceVolume(float value) => SetVolume(AudioBus.Ambience, value);

        public void SetVolume(AudioBus bus, float value)
        {
            value = Mathf.Clamp01(value);

            switch (bus)
            {
                case AudioBus.Master:
                    masterVolume = value;
                    SetMixerVolume(MasterVolumeParameter, value);
                    SaveVolume(MasterVolumeKey, value);
                    break;

                case AudioBus.Music:
                    musicVolume = value;
                    SetMixerVolume(MusicVolumeParameter, value);
                    SaveVolume(MusicVolumeKey, value);
                    break;

                case AudioBus.SFX:
                    sfxVolume = value;
                    SetMixerVolume(SFXVolumeParameter, value);
                    SaveVolume(SFXVolumeKey, value);
                    break;

                case AudioBus.UI:
                    uiVolume = value;
                    SetMixerVolume(UIVolumeParameter, value);
                    SaveVolume(UIVolumeKey, value);
                    break;

                case AudioBus.Ambience:
                    ambienceVolume = value;
                    SetMixerVolume(AmbienceVolumeParameter, value);
                    SaveVolume(AmbienceVolumeKey, value);
                    break;
            }
        }

        public float GetVolume(AudioBus bus)
        {
            return bus switch
            {
                AudioBus.Master => masterVolume,
                AudioBus.Music => musicVolume,
                AudioBus.SFX => sfxVolume,
                AudioBus.UI => uiVolume,
                AudioBus.Ambience => ambienceVolume,
                _ => 1f
            };
        }

        public void ResetVolumes()
        {
            SetMasterVolume(defaultMasterVolume);
            SetMusicVolume(defaultMusicVolume);
            SetSFXVolume(defaultSFXVolume);
            SetUIVolume(defaultUIVolume);
            SetAmbienceVolume(defaultAmbienceVolume);
        }

        void LoadVolumes()
        {
            masterVolume = LoadVolume(MasterVolumeKey, defaultMasterVolume);
            musicVolume = LoadVolume(MusicVolumeKey, defaultMusicVolume);
            sfxVolume = LoadVolume(SFXVolumeKey, defaultSFXVolume);
            uiVolume = LoadVolume(UIVolumeKey, defaultUIVolume);
            ambienceVolume = LoadVolume(AmbienceVolumeKey, defaultAmbienceVolume);
        }

        void ApplyAllVolumes()
        {
            SetMixerVolume(MasterVolumeParameter, masterVolume);
            SetMixerVolume(MusicVolumeParameter, musicVolume);
            SetMixerVolume(SFXVolumeParameter, sfxVolume);
            SetMixerVolume(UIVolumeParameter, uiVolume);
            SetMixerVolume(AmbienceVolumeParameter, ambienceVolume);
        }

        void SetMixerVolume(string parameter, float linearVolume)
        {
            if (mixer == null)
                return;

            float db = LinearToDb(linearVolume);
            bool success = mixer.SetFloat(parameter, db);

            if (!success && debugEnabled)
                Debug.LogWarning($"AudioMixer parameter '{parameter}' was not found.", this);
        }

        void SaveVolume(string key, float value)
        {
            if (!saveVolumes)
                return;

            PlayerPrefs.SetFloat(key, value);
            PlayerPrefs.Save();
        }

        float LoadVolume(string key, float fallback)
        {
            return saveVolumes ? PlayerPrefs.GetFloat(key, fallback) : fallback;
        }

        static float LinearToDb(float value)
        {
            return value <= 0.0001f ? MinimumDb : Mathf.Log10(value) * 20f;
        }

        #endregion

        #region Music

        public void PlayMusic(AudioClip clip, bool loop = true, float fadeDuration = -1f, float volume = 1f)
        {
            if (clip == null)
                return;

            EnsureSources();

            AudioSource from = musicUsesA ? musicSourceA : musicSourceB;
            AudioSource to = musicUsesA ? musicSourceB : musicSourceA;

            if (from.isPlaying && from.clip == clip)
                return;

            if (musicRoutine != null)
                StopCoroutine(musicRoutine);

            PreparePersistentSource(to, clip, musicGroup, loop, Mathf.Clamp01(volume));
            musicUsesA = !musicUsesA;
            currentMusic = clip;

            float duration = fadeDuration < 0f ? defaultCrossfadeDuration : fadeDuration;
            musicRoutine = StartCoroutine(Crossfade(from, to, Mathf.Clamp01(volume), duration));
        }

        public void StopMusic(float fadeDuration = -1f)
        {
            AudioSource source = musicUsesA ? musicSourceA : musicSourceB;
            if (source == null || !source.isPlaying)
                return;

            if (musicRoutine != null)
                StopCoroutine(musicRoutine);

            float duration = fadeDuration < 0f ? defaultCrossfadeDuration : fadeDuration;
            musicRoutine = StartCoroutine(FadeOut(source, duration));
            currentMusic = null;
        }

        #endregion

        #region Ambience

        public void PlayAmbience(AudioClip clip, bool loop = true, float fadeDuration = -1f, float volume = 1f)
        {
            if (clip == null)
                return;

            EnsureSources();

            AudioSource from = ambienceUsesA ? ambienceSourceA : ambienceSourceB;
            AudioSource to = ambienceUsesA ? ambienceSourceB : ambienceSourceA;

            if (from.isPlaying && from.clip == clip)
                return;

            if (ambienceRoutine != null)
                StopCoroutine(ambienceRoutine);

            PreparePersistentSource(to, clip, ambienceGroup, loop, Mathf.Clamp01(volume));
            ambienceUsesA = !ambienceUsesA;
            currentAmbience = clip;

            float duration = fadeDuration < 0f ? defaultCrossfadeDuration : fadeDuration;
            ambienceRoutine = StartCoroutine(Crossfade(from, to, Mathf.Clamp01(volume), duration));
        }

        public void StopAmbience(float fadeDuration = -1f)
        {
            AudioSource source = ambienceUsesA ? ambienceSourceA : ambienceSourceB;
            if (source == null || !source.isPlaying)
                return;

            if (ambienceRoutine != null)
                StopCoroutine(ambienceRoutine);

            float duration = fadeDuration < 0f ? defaultCrossfadeDuration : fadeDuration;
            ambienceRoutine = StartCoroutine(FadeOut(source, duration));
            currentAmbience = null;
        }

        #endregion

        #region SFX and UI

        public AudioSource PlaySFX(AudioClip clip, float volume = 1f, float pitch = 1f)
        {
            return PlaySFXAt(clip, transform.position, false, volume, pitch);
        }

        public AudioSource PlaySFXAt(AudioClip clip, Vector3 position, float volume = 1f, float pitch = 1f)
        {
            return PlaySFXAt(clip, position, true, volume, pitch);
        }

        public void PlayUI(AudioClip clip, float volume = 1f)
        {
            if (clip == null)
                return;

            EnsureSources();
            uiSource.PlayOneShot(clip, Mathf.Clamp01(volume));
        }

        AudioSource PlaySFXAt(AudioClip clip, Vector3 position, bool spatial, float volume, float pitch)
        {
            if (clip == null)
                return null;

            EnsureSources();
            AudioSource source = GetSFXSource();

            source.Stop();
            source.transform.position = position;
            source.outputAudioMixerGroup = sfxGroup;
            source.clip = clip;
            source.loop = false;
            source.playOnAwake = false;
            source.volume = Mathf.Clamp01(volume);
            source.pitch = Mathf.Clamp(pitch, -3f, 3f);
            source.spatialBlend = spatial ? 1f : 0f;
            source.rolloffMode = AudioRolloffMode.Logarithmic;
            source.minDistance = spatialMinDistance;
            source.maxDistance = spatialMaxDistance;
            source.Play();

            return source;
        }

        AudioSource GetSFXSource()
        {
            for (int i = 0; i < sfxSources.Length; i++)
            {
                int index = (sfxCursor + i) % sfxSources.Length;
                if (!sfxSources[index].isPlaying)
                {
                    sfxCursor = (index + 1) % sfxSources.Length;
                    return sfxSources[index];
                }
            }

            AudioSource fallback = sfxSources[sfxCursor];
            sfxCursor = (sfxCursor + 1) % sfxSources.Length;
            return fallback;
        }

        #endregion

        #region Sources

        void EnsureSources()
        {
            musicSourceA = EnsureSource(musicSourceA, "Music A", musicGroup, false);
            musicSourceB = EnsureSource(musicSourceB, "Music B", musicGroup, false);
            ambienceSourceA = EnsureSource(ambienceSourceA, "Ambience A", ambienceGroup, false);
            ambienceSourceB = EnsureSource(ambienceSourceB, "Ambience B", ambienceGroup, false);
            uiSource = EnsureSource(uiSource, "UI", uiGroup, false);

            if (sfxSources == null || sfxSources.Length != sfxPoolSize)
            {
                Transform oldPool = transform.Find("SFX Pool");
                if (oldPool != null)
                    Destroy(oldPool.gameObject);

                GameObject poolObject = new GameObject("SFX Pool");
                poolObject.transform.SetParent(transform, false);

                sfxSources = new AudioSource[sfxPoolSize];
                for (int i = 0; i < sfxPoolSize; i++)
                {
                    GameObject sourceObject = new GameObject($"SFX {i:00}");
                    sourceObject.transform.SetParent(poolObject.transform, false);

                    AudioSource source = sourceObject.AddComponent<AudioSource>();
                    source.outputAudioMixerGroup = sfxGroup;
                    source.playOnAwake = false;
                    source.loop = false;
                    source.spatialBlend = 0f;
                    sfxSources[i] = source;
                }
            }
        }

        AudioSource EnsureSource(AudioSource source, string sourceName, AudioMixerGroup group, bool loop)
        {
            if (source == null)
            {
                Transform child = transform.Find(sourceName);
                if (child == null)
                {
                    GameObject childObject = new GameObject(sourceName);
                    childObject.transform.SetParent(transform, false);
                    child = childObject.transform;
                }

                source = child.GetComponent<AudioSource>();
                if (source == null)
                    source = child.gameObject.AddComponent<AudioSource>();
            }

            source.outputAudioMixerGroup = group;
            source.playOnAwake = false;
            source.loop = loop;
            source.spatialBlend = 0f;
            return source;
        }

        void PreparePersistentSource(AudioSource source, AudioClip clip, AudioMixerGroup group, bool loop, float targetVolume)
        {
            source.Stop();
            source.clip = clip;
            source.outputAudioMixerGroup = group;
            source.loop = loop;
            source.spatialBlend = 0f;
            source.volume = 0f;
            source.Play();
        }

        IEnumerator Crossfade(AudioSource from, AudioSource to, float targetVolume, float duration)
        {
            float fromStartVolume = from != null ? from.volume : 0f;

            if (duration <= 0f)
            {
                if (from != null)
                {
                    from.Stop();
                    from.volume = 1f;
                }

                to.volume = targetVolume;
                yield break;
            }

            float time = 0f;
            while (time < duration)
            {
                time += Time.unscaledDeltaTime;
                float t = Mathf.Clamp01(time / duration);

                if (from != null)
                    from.volume = Mathf.Lerp(fromStartVolume, 0f, t);

                to.volume = Mathf.Lerp(0f, targetVolume, t);
                yield return null;
            }

            if (from != null)
            {
                from.Stop();
                from.volume = 1f;
            }

            to.volume = targetVolume;
        }

        IEnumerator FadeOut(AudioSource source, float duration)
        {
            if (duration <= 0f)
            {
                source.Stop();
                source.volume = 1f;
                yield break;
            }

            float startVolume = source.volume;
            float time = 0f;

            while (time < duration)
            {
                time += Time.unscaledDeltaTime;
                source.volume = Mathf.Lerp(startVolume, 0f, Mathf.Clamp01(time / duration));
                yield return null;
            }

            source.Stop();
            source.volume = 1f;
        }

        #endregion
    }
}
