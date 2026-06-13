using System;
using UnityEngine;

public class ScoringSystem : MonoBehaviour
{
    private static readonly int[] LineScores = { 0, 40, 100, 300, 1200 };

    public int Score { get; private set; }
    public int Level { get; private set; }
    public int TotalLines { get; private set; }

    public event Action<int, int, int> OnStatsChanged;

    [SerializeField] private PieceController _pieceController;

    private void Awake()
    {
        Level = 1;
    }

    public void AddLines(int linesCleared)
    {
        if (linesCleared <= 0)
            return;

        int clampedLines = Mathf.Clamp(linesCleared, 1, 4);
        Score += LineScores[clampedLines] * (Level + 1);
        TotalLines += linesCleared;
        Level = Mathf.Max(1, TotalLines / 10 + 1);

        if (_pieceController != null)
            _pieceController.SetLevel(Level);

        OnStatsChanged?.Invoke(Score, Level, TotalLines);
    }

    public void Reset()
    {
        Score = 0;
        Level = 1;
        TotalLines = 0;

        if (_pieceController != null)
            _pieceController.SetLevel(Level);
    }
}
