using Lucy.Core;
using Lucy.Core.Input;
using Lucy.Features.PlayerController.Bindings;
using UnityEngine;

namespace Lucy.Features.PlayerController
{
    public class PlayerController : MonoBehaviour
    {
        #region Inspector

        [Header("Input Bindings PlayerController")]
        public ButtonInputBinder buttons = new();
        public Vector2InputBinder vector2 = new();

        #endregion

        DisposableGroup subscriptions;

        #region Unity Messages

        protected virtual void OnEnable()
        {

            subscriptions = new DisposableGroup();
            buttons.Bind(InputProvider.Global.Controls.Player, subscriptions);
            vector2.Bind(InputProvider.Global.Controls.Player, subscriptions);
        }

        protected virtual void OnDisable()
        {
            subscriptions?.Dispose();
            subscriptions = null;
        }

        #endregion
    }
}