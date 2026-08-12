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
        public FloatInputBinder floats = new();
        public AxisInputBinder axis = new();

        [Header("Input Bindings User Interface")]
        public ButtonInputBinder uibuttons = new();
        public Vector2InputBinder uivector2 = new();
        public FloatInputBinder uifloats = new();
        public AxisInputBinder uiaxis = new();

        #endregion

        DisposableGroup subscriptions;

        #region Unity Messages

        protected virtual void OnEnable()
        {

            subscriptions = new DisposableGroup();
            buttons.Bind(InputProvider.Global.Controls.Player, subscriptions);
            vector2.Bind(InputProvider.Global.Controls.Player, subscriptions);
            floats.Bind(InputProvider.Global.Controls.Player, subscriptions);
            axis.Bind(InputProvider.Global.Controls.Player, subscriptions);

            uibuttons.Bind(InputProvider.Global.Controls.UI, subscriptions);
            uivector2.Bind(InputProvider.Global.Controls.UI, subscriptions);
            uifloats.Bind(InputProvider.Global.Controls.UI, subscriptions);
            uiaxis.Bind(InputProvider.Global.Controls.UI, subscriptions);
        }

        protected virtual void OnDisable()
        {
            subscriptions?.Dispose();
            subscriptions = null;
        }

        #endregion
    }
}