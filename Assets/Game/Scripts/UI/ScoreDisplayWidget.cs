using UnityEngine;
using UnityEngine.UIElements;

namespace Tetris.UI
{
    public class ScoreDisplayWidget : MonoBehaviour
    {
        [SerializeField] private GameScreen _gameScreen;
        [SerializeField] private ScoringSystem _scoringSystem;

        private Label _scoreValueLabel;

        private void Start()
        {
            if (_gameScreen != null && _gameScreen.HudRegion != null)
                _scoreValueLabel = _gameScreen.HudRegion.Q<Label>("score-value");

            if (_scoringSystem != null)
                _scoringSystem.OnStatsChanged += OnStatsChanged;
        }

        private void OnDestroy()
        {
            if (_scoringSystem != null)
                _scoringSystem.OnStatsChanged -= OnStatsChanged;
        }

        private void OnStatsChanged(int score, int level, int totalLines)
        {
            UpdateScore(score);
        }

        public void UpdateScore(int score)
        {
            if (_scoreValueLabel != null)
                _scoreValueLabel.text = score.ToString("D7");
        }
    }
}
