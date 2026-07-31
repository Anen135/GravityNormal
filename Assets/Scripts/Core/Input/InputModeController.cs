using System;

namespace Lucy.Core.Input
{
    public sealed class InputModeController : IDisposable
    {
        GlobalController controls;
        bool uiEnabled;
        bool active;
        bool disposed;
        public InputMode CurrentMode { get; set; }
        public bool IsUIEnabled => uiEnabled;
        public event Action<InputMode, InputMode> ModeChanged;

        public InputModeController( GlobalController controls, InputMode initialMode, bool enableUI)
        {
            this.controls = controls ?? throw new ArgumentNullException(nameof(controls));
            CurrentMode = initialMode;
            uiEnabled = enableUI;
        }

        public void Activate()
        {
            if (disposed) ThrowDisposedExpection();
            active = true;
            Apply();
        }

        public void Deactivate()
        {
            if (disposed) return;
            active = false;
            DisableAll();
        }

        public void SetMode(InputMode mode)
        {
            if (disposed) ThrowDisposedExpection();
            if (CurrentMode == mode)
            {
                Apply();
                return;
            }

            InputMode previousMode = CurrentMode;
            CurrentMode = mode;
            Apply();
            ModeChanged?.Invoke(previousMode, mode);
        }

        public void EnterPlayerMode() => SetMode(InputMode.Player);

        public void DisableContextInput() => SetMode(InputMode.Disabled);

        public void SetUIEnabled(bool enabled)
        {
            if (disposed) ThrowDisposedExpection();
            uiEnabled = enabled;
            Apply();
        }

        void Apply()
        {
            if (!active) return;
            controls.Player.Disable();
            if (uiEnabled) controls.UI.Enable();
            else controls.UI.Disable();
            switch (CurrentMode)
            {
                case InputMode.Disabled: break;
                case InputMode.Player: controls.Player.Enable();
                break;
                default: throw new ArgumentOutOfRangeException(nameof(CurrentMode), CurrentMode, "Неизвестный режим управления.");
            }
        }

        void DisableAll()
        {
            controls.Player.Disable();
            controls.UI.Disable();
        }

        public void Dispose()
        {
            if (disposed) return;
            disposed = true;
            active = false;
            DisableAll();
            ModeChanged = null;
        }
        void ThrowDisposedExpection() { throw new ObjectDisposedException(nameof(InputModeController)); }
    }
}