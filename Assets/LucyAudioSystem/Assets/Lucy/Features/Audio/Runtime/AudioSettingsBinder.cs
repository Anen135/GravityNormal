using UnityEngine;
using UnityEngine.UI;
using Lucy.Features.Audio.Runtime;

namespace Lucy.Features.UI.Runtime
{
    public sealed class AudioSettingsBinder : MonoBehaviour
    {
        #region Inspector

        [Header("References Audio Settings Binder")]
        public Slider globalSlider;
        public Slider musicSlider;
        public Slider soundsSlider;
        public Slider uiSlider;
        public Slider ambienceSlider;

        [Header("Settings Audio Settings Binder")]
        public bool refreshValuesOnEnable = true;

        [Header("Debug")]
        public Audio.Runtime.AudioSystem audioSystem;
        public bool listenersBound;

        #endregion

        #region Unity messages

        void Awake()
        {
            audioSystem = Audio.Runtime.AudioSystem.Instance;
            BindListeners();
        }

        void OnEnable()
        {
            audioSystem = Audio.Runtime.AudioSystem.Instance;

            if (refreshValuesOnEnable)
                RefreshFromAudioSystem();
        }

        void OnDestroy()
        {
            UnbindListeners();
        }

        #endregion

        #region Listeners

        void BindListeners()
        {
            if (listenersBound)
                return;

            if (globalSlider != null)
                globalSlider.onValueChanged.AddListener(HandleGlobalChanged);

            if (musicSlider != null)
                musicSlider.onValueChanged.AddListener(HandleMusicChanged);

            if (soundsSlider != null)
                soundsSlider.onValueChanged.AddListener(HandleSoundsChanged);

            if (uiSlider != null)
                uiSlider.onValueChanged.AddListener(HandleUIChanged);

            if (ambienceSlider != null)
                ambienceSlider.onValueChanged.AddListener(HandleAmbienceChanged);

            listenersBound = true;
        }

        void UnbindListeners()
        {
            if (!listenersBound)
                return;

            if (globalSlider != null)
                globalSlider.onValueChanged.RemoveListener(HandleGlobalChanged);

            if (musicSlider != null)
                musicSlider.onValueChanged.RemoveListener(HandleMusicChanged);

            if (soundsSlider != null)
                soundsSlider.onValueChanged.RemoveListener(HandleSoundsChanged);

            if (uiSlider != null)
                uiSlider.onValueChanged.RemoveListener(HandleUIChanged);

            if (ambienceSlider != null)
                ambienceSlider.onValueChanged.RemoveListener(HandleAmbienceChanged);

            listenersBound = false;
        }

        #endregion

        #region Refresh

        public void RefreshFromAudioSystem()
        {
            audioSystem = Audio.Runtime.AudioSystem.Instance;

            if (audioSystem == null)
                return;

            if (globalSlider != null)
                globalSlider.SetValueWithoutNotify(audioSystem.MasterVolume);

            if (musicSlider != null)
                musicSlider.SetValueWithoutNotify(audioSystem.MusicVolume);

            if (soundsSlider != null)
                soundsSlider.SetValueWithoutNotify(audioSystem.SFXVolume);

            if (uiSlider != null)
                uiSlider.SetValueWithoutNotify(audioSystem.UIVolume);

            if (ambienceSlider != null)
                ambienceSlider.SetValueWithoutNotify(audioSystem.AmbienceVolume);
        }

        #endregion

        #region Handlers

        public void HandleGlobalChanged(float value)
        {
            audioSystem = Audio.Runtime.AudioSystem.Instance;

            if (audioSystem == null)
                return;

            audioSystem.SetMasterVolume(value);
        }

        public void HandleMusicChanged(float value)
        {
            audioSystem = Audio.Runtime.AudioSystem.Instance;

            if (audioSystem == null)
                return;

            audioSystem.SetMusicVolume(value);
        }

        public void HandleSoundsChanged(float value)
        {
            audioSystem = Audio.Runtime.AudioSystem.Instance;

            if (audioSystem == null)
                return;

            audioSystem.SetSFXVolume(value);
        }

        public void HandleUIChanged(float value)
        {
            audioSystem = Audio.Runtime.AudioSystem.Instance;

            if (audioSystem == null)
                return;

            audioSystem.SetUIVolume(value);
        }

        public void HandleAmbienceChanged(float value)
        {
            audioSystem = Audio.Runtime.AudioSystem.Instance;

            if (audioSystem == null)
                return;

            audioSystem.SetAmbienceVolume(value);
        }

        #endregion
    }
}