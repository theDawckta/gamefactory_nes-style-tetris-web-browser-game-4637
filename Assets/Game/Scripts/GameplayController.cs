using System;
using System.Collections.Generic;
using UnityEngine;
using Tetris.UI;

public class GameplayController : MonoBehaviour
{
    [SerializeField] private PlayfieldController _playfieldController;
    [SerializeField] private PieceController _pieceController;
    [SerializeField] private ScoringSystem _scoringSystem;
    [SerializeField] private PlayfieldRenderer _playfieldRenderer;
    [SerializeField] private NextPiecePreview _nextPiecePreview;
    [SerializeField] private GameOverScreen _gameOverScreen;
    [SerializeField] private GameScreen _gameScreen;
    [SerializeField] private InputHandler _inputHandler;
    [SerializeField] private TetrominoData _tetrominoData;

    private PieceBag _pieceBag;
    private ScoreEntry[] _lastFetchedLeaderboard = Array.Empty<ScoreEntry>();
    private bool _isPlaying;
    private readonly HashSet<string> _heldActions = new HashSet<string>();

    private Action<string> _onActionDown;
    private Action<string> _onActionUp;

    private void Start()
    {
        GameStateManager.Instance.OnEnterPlaying += OnEnterPlaying;
        GameStateManager.Instance.OnEnterStart += OnLeavePlayingState;
        GameStateManager.Instance.OnEnterGameOver += OnLeavePlayingState;

        _pieceController.OnPieceLocked += HandlePieceLocked;
        _pieceController.OnTopOut += HandleTopOut;

        _onActionDown = a => _heldActions.Add(a);
        _onActionUp = a => _heldActions.Remove(a);
        _inputHandler.OnActionDown += _onActionDown;
        _inputHandler.OnActionUp += _onActionUp;
    }

    private void OnDestroy()
    {
        if (GameStateManager.Instance != null)
        {
            GameStateManager.Instance.OnEnterPlaying -= OnEnterPlaying;
            GameStateManager.Instance.OnEnterStart -= OnLeavePlayingState;
            GameStateManager.Instance.OnEnterGameOver -= OnLeavePlayingState;
        }

        if (_pieceController != null)
        {
            _pieceController.OnPieceLocked -= HandlePieceLocked;
            _pieceController.OnTopOut -= HandleTopOut;
        }

        if (_inputHandler != null)
        {
            _inputHandler.OnActionDown -= _onActionDown;
            _inputHandler.OnActionUp -= _onActionUp;
        }
    }

    private void Update()
    {
        if (!_isPlaying) return;

        _pieceController.ProcessInput(
            _heldActions.Contains("MoveLeft"),
            _heldActions.Contains("MoveRight"),
            _heldActions.Contains("RotateLeft"),
            _heldActions.Contains("RotateRight"),
            _heldActions.Contains("SoftDrop")
        );
    }

    public void CacheLeaderboard(ScoreEntry[] scores)
    {
        _lastFetchedLeaderboard = scores ?? Array.Empty<ScoreEntry>();
    }

    private void OnEnterPlaying()
    {
        _scoringSystem.Reset();
        _playfieldController.Reset();
        _pieceBag = new PieceBag(_tetrominoData);
        _isPlaying = true;
        _gameScreen.Show();
        SpawnNext();
    }

    private void OnLeavePlayingState()
    {
        _isPlaying = false;
        _playfieldRenderer.ClearActivePiece();
    }

    private void SpawnNext()
    {
        var current = _pieceBag.Next();
        _pieceController.SpawnPiece(current);
        var upcoming = _pieceBag.Peek();
        _nextPiecePreview.ShowNextPiece(upcoming);
    }

    private void HandlePieceLocked()
    {
        _playfieldController.GetBoardState(out int[,] boardState);
        _playfieldRenderer.RefreshBoard(boardState);

        int linesCleared = _playfieldController.ClearLines();
        if (linesCleared > 0)
            _scoringSystem.AddLines(linesCleared);

        _playfieldController.GetBoardState(out int[,] updatedState);
        _playfieldRenderer.RefreshBoard(updatedState);

        SpawnNext();
    }

    private void HandleTopOut()
    {
        _isPlaying = false;
        GameStateManager.Instance.GoToGameOver();
        _gameOverScreen.ShowWithScore(_scoringSystem.Score, _lastFetchedLeaderboard);
    }
}
