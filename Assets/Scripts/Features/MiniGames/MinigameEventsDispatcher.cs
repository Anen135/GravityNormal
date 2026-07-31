using UnityEngine;
using UnityEngine.Events;

namespace Lucy.Features.Minigames
{
    [RequireComponent(typeof(MiniGame))]
    public class MinigameEventsDispatcher : MonoBehaviour
    {
        [Header("Unity Events for Inspector")]
        public UnityEvent OnGameStarted;
        public UnityEvent OnGameSuccess;
        public UnityEvent OnGameFailed;

        private MiniGame _miniGame;

        private void Awake()
        {
            _miniGame = GetComponent<MiniGame>();
        }

        private void OnEnable()
        {
            // Подписываемся на чистые C# события
            _miniGame.OnStarted += HandleStarted;
            _miniGame.OnFinished += HandleFinished;
        }

        private void OnDisable()
        {
            _miniGame.OnStarted -= HandleStarted;
            _miniGame.OnFinished -= HandleFinished;
        }

        private void HandleStarted() => OnGameStarted?.Invoke();

        private void HandleFinished(MiniGame.Result result)
        {
            if (result == MiniGame.Result.Success) OnGameSuccess?.Invoke();
            else if (result == MiniGame.Result.Failure) OnGameFailed?.Invoke();
        }
    }
}