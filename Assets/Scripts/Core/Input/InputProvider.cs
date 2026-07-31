using System;
using Lucy.Core.Input;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Lucy.Core
{

    [DefaultExecutionOrder(-10000)]
    public sealed class InputProvider : MonoBehaviour
    {
        static InputProvider instance;
        static bool applicationIsQuitting;
        [Header("Startup")]
        public InputMode initialMode = InputMode.Player;
        public bool enableUI = true;
        InputSubscriptionRegistry subscriptionRegistry;
        InputModeController modeController;
        bool initialized;
        bool destroying;
        public GlobalController Controls { get; set; }
        public InputMode CurrentMode => modeController?.CurrentMode ?? InputMode.Disabled;
        public bool IsInitialized => initialized && !destroying && Controls != null;
        public bool IsDestroying => destroying;

        public event Action<InputMode, InputMode> ModeChanged
        {
            add
            {
                EnsureAvailable();
                modeController.ModeChanged += value;
            }
            remove
            {
                if (modeController != null) modeController.ModeChanged -= value;
            }
        }

        public static InputProvider Global
        {
            get
            {
                if (instance != null) return instance;
                if (!Application.isPlaying) throw new InvalidOperationException("InputProvider.Global is only available in Play Mode.");
                if (applicationIsQuitting) throw new InvalidOperationException("You cannot create an InputProvider during the exit.");
                return CreateLazy();
            }
        }

        public static bool HasInstance => instance != null && instance.IsInitialized;

        public static bool TryGetExisting(out InputProvider provider)
        {
            provider = instance;
            return provider != null && provider.IsInitialized;
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        static void ResetStatics()
        {
            instance = null;
            applicationIsQuitting = false;
        }

        static InputProvider CreateLazy()
        {
            var providerObject = new GameObject(nameof(InputProvider));
            var provider = providerObject.AddComponent<InputProvider>();
            if (provider.IsInitialized) return provider;
            Destroy(providerObject);
            throw new InvalidOperationException("Не удалось инициализировать InputProvider.");
        }

        void Awake()
        {
            if (instance != null && instance != this)
            {
                Debug.LogError($"В сцене уже существует {nameof(InputProvider)}. " + $"Объект '{name}' будет уничтожен.", this);
                Destroy(gameObject);
                return;
            }
            instance = this;
            Initialize();
        }

        void Initialize()
        {
            if (initialized) return;
            Controls = new GlobalController();
            subscriptionRegistry = new InputSubscriptionRegistry(Controls.asset);
            modeController = new InputModeController(Controls, initialMode, enableUI);
            initialized = true;
        }

        void OnEnable()
        {
            if (!IsInitialized) return;
            modeController.Activate();
        }

        void OnDisable() => modeController?.Deactivate();
        void OnApplicationQuit() => applicationIsQuitting = true;
        void OnDestroy()
        {
            if (destroying) return;
            destroying = true;
            if (instance == this) instance = null;
            subscriptionRegistry?.Dispose();
            subscriptionRegistry = null;
            modeController?.Dispose();
            modeController = null;
            Controls?.Dispose();
            Controls = null;
            initialized = false;
        }

        #region Modes

        public void SetMode(InputMode mode)
        {
            EnsureAvailable();
            modeController.SetMode(mode);
        }

        public void EnterPlayerMode()
        {
            EnsureAvailable();
            modeController.EnterPlayerMode();
        }

        public void DisableContextInput()
        {
            EnsureAvailable();
            modeController.DisableContextInput();
        }

        public void SetUIEnabled(bool enabled)
        {
            EnsureAvailable();
            modeController.SetUIEnabled(enabled);
        }

        #endregion

        #region Subscriptions

        public IDisposable SubscribePerformed(InputAction action, Action<InputAction.CallbackContext> callback)
        {
            EnsureAvailable();
            return subscriptionRegistry.SubscribePerformed(action, callback);
        }

        public IDisposable SubscribeStarted( InputAction action, Action<InputAction.CallbackContext> callback)
        {
            EnsureAvailable();
            return subscriptionRegistry.SubscribeStarted( action, callback);
        }

        public IDisposable SubscribeCanceled( InputAction action, Action<InputAction.CallbackContext> callback)
        {
            EnsureAvailable();
            return subscriptionRegistry.SubscribeCanceled( action, callback);
        }

        #endregion

        void EnsureAvailable()
        {
            if (!initialized || Controls == null) throw new InvalidOperationException("InputProvider ещё не инициализирован.");
            if (destroying) throw new ObjectDisposedException(nameof(InputProvider), "InputProvider уже уничтожается.");
        }
    }
}