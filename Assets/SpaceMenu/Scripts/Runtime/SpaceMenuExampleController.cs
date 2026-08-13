using UnityEngine;

namespace Lucy.UI
{
    public sealed class SpaceMenuExampleController : MonoBehaviour
    {
        #region Inspector

        [Header("References Space Menu Example Controller")]
        public SpaceMenuView menuView;

        [Header("Debug")]
        public bool debugEnabled = true;

        #endregion

        #region Unity messages

        void Start()
        {
            if (menuView == null)
                menuView = FindFirstObjectByType<SpaceMenuView>();

            if (menuView == null)
            {
                if (debugEnabled) Debug.LogWarning("SpaceMenuView not found.", this);
                return;
            }

            menuView.Bind(HandlePlay, HandleSettings, HandleExit);
        }

        #endregion

        #region Private methods

        void HandlePlay()
        {
            if (debugEnabled) Debug.Log("Play clicked.", this);
        }

        void HandleSettings()
        {
            if (debugEnabled) Debug.Log("Settings clicked.", this);
        }

        void HandleExit()
        {
            if (debugEnabled) Debug.Log("Exit clicked.", this);
        }

        #endregion
    }
}
