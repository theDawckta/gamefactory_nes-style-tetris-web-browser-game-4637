using UnityEngine;
using UnityEngine.UIElements;

namespace Tetris.UI
{
    public class LevelDisplayWidget : MonoBehaviour
    {
        [SerializeField] private GameScreen _gameScreen;
        [SerializeField] private ScoringSystem _scoringSystem;

        private Label _levelValueLabel;

        private void Start()
        {
            if (_gameScreen != null && _gameScreen.HudRegion != null)
                _levelValueLabel = _gameScreen.HudRegion.Q<Label>("level-value");

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
            UpdateLevel(level);
        }

        public void UpdateLevel(int level)
        {
            if (_levelValueLabel != null)
                _levelValueLabel.text = level.ToString();
        }
    }
}
