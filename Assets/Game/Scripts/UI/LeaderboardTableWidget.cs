using UnityEngine;
using UnityEngine.UIElements;

namespace Tetris.UI
{
    public class LeaderboardTableWidget : MonoBehaviour
    {
        [SerializeField] private StartScreen _startScreen;

        private Label[] _rankLabels = new Label[5];
        private Label[] _initialsLabels = new Label[5];
        private Label[] _scoreLabels = new Label[5];

        private void Start()
        {
            if (_startScreen != null)
            {
                VisualElement region = _startScreen.LeaderboardRegion;
                if (region != null)
                    BuildTable(region);
                _startScreen.OnLeaderboardFetched += PopulateTable;
            }
        }

        private void OnDestroy()
        {
            if (_startScreen != null)
                _startScreen.OnLeaderboardFetched -= PopulateTable;
        }

        private void BuildTable(VisualElement container)
        {
            container.Clear();

            var table = new VisualElement { name = "leaderboard-table" };
            table.AddToClassList("leaderboard-table");

            var headerRow = new VisualElement { name = "leaderboard-header-row" };
            headerRow.AddToClassList("leaderboard-row");
            headerRow.AddToClassList("leaderboard-header-row");
            headerRow.Add(MakeCell("RANK", "leaderboard-rank"));
            headerRow.Add(MakeCell("NAME", "leaderboard-initials"));
            headerRow.Add(MakeCell("SCORE", "leaderboard-score"));
            table.Add(headerRow);

            for (int i = 0; i < 5; i++)
            {
                var row = new VisualElement { name = "leaderboard-row-" + i };
                row.AddToClassList("leaderboard-row");

                _rankLabels[i] = MakeCell("", "leaderboard-rank");
                _initialsLabels[i] = MakeCell("", "leaderboard-initials");
                _scoreLabels[i] = MakeCell("", "leaderboard-score");

                row.Add(_rankLabels[i]);
                row.Add(_initialsLabels[i]);
                row.Add(_scoreLabels[i]);
                table.Add(row);
            }

            container.Add(table);
        }

        private static Label MakeCell(string text, string cssClass)
        {
            var label = new Label(text);
            label.AddToClassList(cssClass);
            return label;
        }

        public void PopulateTable(ScoreEntry[] scores)
        {
            if (scores == null)
                return;

            for (int i = 0; i < 5; i++)
            {
                if (_rankLabels[i] == null || _initialsLabels[i] == null || _scoreLabels[i] == null)
                    continue;

                if (i >= scores.Length)
                    continue;

                ScoreEntry entry = scores[i];
                bool isPlaceholder = string.IsNullOrEmpty(entry.initials);

                _rankLabels[i].text = entry.rank.ToString();
                _initialsLabels[i].text = isPlaceholder ? "---" : entry.initials;
                _scoreLabels[i].text = isPlaceholder ? "-------" : entry.score.ToString();
            }
        }
    }
}
