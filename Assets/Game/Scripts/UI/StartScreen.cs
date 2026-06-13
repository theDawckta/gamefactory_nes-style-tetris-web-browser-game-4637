using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace Tetris.UI
{
    public class StartScreen : BaseScreen
    {
        public event Action<ScoreEntry[]> OnLeaderboardFetched;

        public VisualElement LeaderboardRegion => GetElement("leaderboard-region");

        private InputHandler _inputHandler;

        protected override void Awake()
        {
            base.Awake();
        }

        private void Start()
        {
            if (GameStateManager.Instance != null)
            {
                GameStateManager.Instance.OnEnterStart += OnEnterStartHandler;
                GameStateManager.Instance.OnEnterPlaying += Hide;
                GameStateManager.Instance.OnEnterGameOver += Hide;
            }

            _inputHandler = UnityEngine.Object.FindFirstObjectByType<InputHandler>();
            if (_inputHandler != null)
                _inputHandler.OnActionDown += OnActionDown;

            gameObject.SetActive(false);
        }

        private void OnDestroy()
        {
            if (GameStateManager.Instance != null)
            {
                GameStateManager.Instance.OnEnterStart -= OnEnterStartHandler;
                GameStateManager.Instance.OnEnterPlaying -= Hide;
                GameStateManager.Instance.OnEnterGameOver -= Hide;
            }

            if (_inputHandler != null)
                _inputHandler.OnActionDown -= OnActionDown;
        }

        private void OnEnterStartHandler()
        {
            base.Show();
            if (LeaderboardService.Instance != null)
                LeaderboardService.Instance.FetchScores(
                    scores => OnLeaderboardFetched?.Invoke(scores),
                    err => Debug.LogWarning("StartScreen: leaderboard fetch failed: " + err)
                );
        }

        private void OnActionDown(string actionName)
        {
            if (!gameObject.activeSelf)
                return;
            if (actionName == "NavDown")
                GameStateManager.Instance?.StartGame();
        }
    }
}
