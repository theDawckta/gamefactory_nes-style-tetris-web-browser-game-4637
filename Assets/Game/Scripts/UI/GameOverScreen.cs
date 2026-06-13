using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

namespace Tetris.UI
{
    public class GameOverScreen : BaseScreen
    {
        private Label _scoreLabel;
        private Label _continuePrompt;
        private bool _highScoreEntryActive;
        private HighScoreEntryWidget _widget;

        protected override void Awake()
        {
            base.Awake();
            _scoreLabel = GetElement("score-label") as Label;
            _continuePrompt = GetElement("continue-prompt") as Label;
            _widget = GetComponent<HighScoreEntryWidget>();
        }

        private void Start()
        {
            if (GameStateManager.Instance != null)
            {
                GameStateManager.Instance.OnEnterGameOver += OnEnterGameOverHandler;
                GameStateManager.Instance.OnEnterStart += Hide;
                GameStateManager.Instance.OnEnterPlaying += Hide;
            }
            gameObject.SetActive(false);
        }

        private void OnDestroy()
        {
            if (GameStateManager.Instance != null)
            {
                GameStateManager.Instance.OnEnterGameOver -= OnEnterGameOverHandler;
                GameStateManager.Instance.OnEnterStart -= Hide;
                GameStateManager.Instance.OnEnterPlaying -= Hide;
            }
        }

        private void OnEnterGameOverHandler()
        {
            base.Show();
        }

        private void Update()
        {
            if (_highScoreEntryActive)
                return;

            var keyboard = Keyboard.current;
            if (keyboard != null && keyboard.downArrowKey.wasPressedThisFrame)
                GameStateManager.Instance?.GoToStart();
        }

        public void ShowWithScore(int finalScore, ScoreEntry[] currentLeaderboard)
        {
            if (_scoreLabel != null)
                _scoreLabel.text = "SCORE: " + finalScore.ToString("D7");

            _highScoreEntryActive = QualifiesForTop5(finalScore, currentLeaderboard);

            if (_highScoreEntryActive && _widget != null)
                _widget.Activate(finalScore);
            else
                _widget?.Deactivate();

            if (_continuePrompt != null)
                _continuePrompt.style.display = _highScoreEntryActive ? DisplayStyle.None : DisplayStyle.Flex;

            base.Show();
        }

        private bool QualifiesForTop5(int score, ScoreEntry[] leaderboard)
        {
            if (leaderboard == null || leaderboard.Length < 5)
                return true;

            int lowestScore = int.MaxValue;
            foreach (var entry in leaderboard)
            {
                if (entry.score < lowestScore)
                    lowestScore = entry.score;
            }
            return score > lowestScore;
        }
    }
}
