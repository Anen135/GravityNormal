using Lucy.Core;
using Lucy.Core.Input;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace Lucy.Features.PlayerController.Runtime
{
    public class PlayerController : MonoBehaviour
    {
        [Header("Events")]
        public UnityEvent<Vector2> onMove;
        public UnityEvent onSprintStart;
        public UnityEvent onSprintEnd;
        public UnityEvent onCrouchStart;
        public UnityEvent onCrouchEnd;
        public UnityEvent onAttackStart;
        public UnityEvent onAttackEnd;
        public UnityEvent onInteractStart;
        public UnityEvent onInteractEnd;
        public UnityEvent onUse;
        public UnityEvent onDrop;
        public UnityEvent onJump;
        
        DisposableGroup subscriptions;
        protected virtual void OnEnable()
        {
            subscriptions = new DisposableGroup();
            subscriptions.Add(InputProvider.Global.SubscribePerformed(InputProvider.Global.Controls.Player.Move, OnMovePerformed));
            subscriptions.Add(InputProvider.Global.SubscribeCanceled(InputProvider.Global.Controls.Player.Move, OnMoveCanceled));
            subscriptions.Add(InputProvider.Global.SubscribePerformed(InputProvider.Global.Controls.Player.Sprint, OnSprintPerformed));
            subscriptions.Add(InputProvider.Global.SubscribeCanceled(InputProvider.Global.Controls.Player.Sprint, OnSprintCanceled));
            subscriptions.Add(InputProvider.Global.SubscribePerformed(InputProvider.Global.Controls.Player.Crouch, OnCrouchPerformed));
            subscriptions.Add(InputProvider.Global.SubscribeCanceled(InputProvider.Global.Controls.Player.Crouch, OnCrouchCanceled));
            subscriptions.Add(InputProvider.Global.SubscribePerformed(InputProvider.Global.Controls.Player.Jump, OnJumpPerformed));
            subscriptions.Add(InputProvider.Global.SubscribePerformed(InputProvider.Global.Controls.Player.Attack, OnAttackPerformed));
            subscriptions.Add(InputProvider.Global.SubscribeCanceled(InputProvider.Global.Controls.Player.Attack, OnAttackCanceled));
            subscriptions.Add(InputProvider.Global.SubscribePerformed(InputProvider.Global.Controls.Player.Interact, OnInteractPerformed));
            subscriptions.Add(InputProvider.Global.SubscribeCanceled(InputProvider.Global.Controls.Player.Interact, OnInteractCanceled));
            // subscriptions.Add( InputProvider.Global.SubscribePerformed( player.Use, OnUsePerformed));
            // subscriptions.Add( InputProvider.Global.SubscribePerformed( player.Drop, OnDropPerformed));
        }
        protected virtual void OnDisable() => subscriptions?.Dispose();

        void OnMovePerformed(InputAction.CallbackContext context) => onMove?.Invoke(context.ReadValue<Vector2>());
        void OnMoveCanceled(InputAction.CallbackContext context) => onMove?.Invoke(Vector2.zero);
        void OnSprintPerformed(InputAction.CallbackContext context) => onSprintStart?.Invoke();
        void OnSprintCanceled(InputAction.CallbackContext context) => onSprintEnd?.Invoke();
        void OnCrouchPerformed(InputAction.CallbackContext context) => onCrouchStart?.Invoke();
        void OnCrouchCanceled(InputAction.CallbackContext context) => onCrouchEnd?.Invoke();
        void OnJumpPerformed(InputAction.CallbackContext context) => onJump?.Invoke();
        void OnAttackPerformed(InputAction.CallbackContext context) => onAttackStart?.Invoke();
        void OnAttackCanceled(InputAction.CallbackContext context) => onAttackEnd?.Invoke();
        void OnInteractPerformed(InputAction.CallbackContext context) => onInteractStart?.Invoke();
        void OnInteractCanceled(InputAction.CallbackContext context) => onInteractEnd?.Invoke();
        void OnUsePerformed(InputAction.CallbackContext context) => onUse?.Invoke();
        void OnDropPerformed(InputAction.CallbackContext context) => onDrop?.Invoke();
    }
}