using System;
using System.Collections;
using System.Reflection;
using NUnit.Framework;
using Tetris.UI;
using UnityEngine;
using UnityEngine.TestTools;

public class GameplayControllerTests
{
    private GameObject _gsmGo;
    private GameStateManager _gsm;
    private GameObject _systemsGo;
    private PlayfieldController _playfieldController;
    private PieceController _pieceController;
    private ScoringSystem _scoringSystem;
    private GameObject _rendererGo;
    private PlayfieldRenderer _playfieldRenderer;
    private GameObject _nextPieceGo;
    private NextPiecePreview _nextPiecePreview;
    private GameObject _gameOverGo;
    private GameOverScreen _gameOverScreen;
    private GameObject _gameScreenGo;
    private GameScreen _gameScreen;
    private GameObject _inputGo;
    private InputHandler _inputHandler;
    private GameObject _controllerGo;
    private GameplayController _controller;
    private TetrominoData _tetrominoData;
    private Texture2D[] _textures;

    [SetUp]
    public void SetUp()
    {
        _gsmGo = new GameObject("GameStateManager");
        _gsm = _gsmGo.AddComponent<GameStateManager>();

        _tetrominoData = CreateTetrominoData();

        _systemsGo = new GameObject("Systems");
        _playfieldController = _systemsGo.AddComponent<PlayfieldController>();
        _pieceController = _systemsGo.AddComponent<PieceController>();
        _pieceController.enabled = false;
        _scoringSystem = _systemsGo.AddComponent<ScoringSystem>();

        _rendererGo = new GameObject("PlayfieldRenderer");
        _playfieldRenderer = _rendererGo.AddComponent<PlayfieldRenderer>();
        SetField(_playfieldRenderer, "_tetrominoData", _tetrominoData);

        _nextPieceGo = new GameObject("NextPiecePreview");
        _nextPiecePreview = _nextPieceGo.AddComponent<NextPiecePreview>();
        SetField(_nextPiecePreview, "_tetrominoData", _tetrominoData);

        _gameOverGo = new GameObject("GameOverScreen");
        _gameOverScreen = _gameOverGo.AddComponent<GameOverScreen>();

        _gameScreenGo = new GameObject("GameScreen");
        _gameScreen = _gameScreenGo.AddComponent<GameScreen>();

        _inputGo = new GameObject("InputHandler");
        _inputHandler = _inputGo.AddComponent<InputHandler>();

        _controllerGo = new GameObject("GameplayController");
        _controller = _controllerGo.AddComponent<GameplayController>();

        SetField(_controller, "_playfieldController", _playfieldController);
        SetField(_controller, "_pieceController", _pieceController);
        SetField(_controller, "_scoringSystem", _scoringSystem);
        SetField(_controller, "_playfieldRenderer", _playfieldRenderer);
        SetField(_controller, "_nextPiecePreview", _nextPiecePreview);
        SetField(_controller, "_gameOverScreen", _gameOverScreen);
        SetField(_controller, "_gameScreen", _gameScreen);
        SetField(_controller, "_inputHandler", _inputHandler);
        SetField(_controller, "_tetrominoData", _tetrominoData);
    }

    [TearDown]
    public void TearDown()
    {
        UnityEngine.Object.DestroyImmediate(_controllerGo);
        UnityEngine.Object.DestroyImmediate(_inputGo);
        UnityEngine.Object.DestroyImmediate(_gameScreenGo);
        UnityEngine.Object.DestroyImmediate(_gameOverGo);
        UnityEngine.Object.DestroyImmediate(_nextPieceGo);
        UnityEngine.Object.DestroyImmediate(_rendererGo);
        UnityEngine.Object.DestroyImmediate(_systemsGo);
        UnityEngine.Object.DestroyImmediate(_gsmGo);
        UnityEngine.Object.DestroyImmediate(_tetrominoData);
        if (_textures != null)
            foreach (var t in _textures)
                UnityEngine.Object.DestroyImmediate(t);
    }

    private TetrominoData CreateTetrominoData()
    {
        var data = ScriptableObject.CreateInstance<TetrominoData>();
        _textures = new Texture2D[8];
        data.cellSprites = new Sprite[8];
        for (int i = 0; i < 8; i++)
        {
            _textures[i] = new Texture2D(24, 24);
            data.cellSprites[i] = Sprite.Create(_textures[i], new Rect(0, 0, 24, 24), Vector2.one * 0.5f, 24f);
        }
        data.pieces = new[]
        {
            MakePiece("A", 0, new[] { Vector2Int.zero }),
            MakePiece("B", 1, new[] { Vector2Int.zero }),
        };
        return data;
    }

    private static PieceDefinition MakePiece(string name, int atlasIndex, Vector2Int[] cells)
    {
        var def = new PieceDefinition();
        def.pieceName = name;
        def.atlasIndex = atlasIndex;
        def.rotations = new[] { new RotationState { cells = cells } };
        return def;
    }

    private static void SetField(object target, string fieldName, object value)
    {
        var type = target.GetType();
        while (type != null)
        {
            var field = type.GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Instance);
            if (field != null) { field.SetValue(target, value); return; }
            type = type.BaseType;
        }
    }

    private static object GetField(object target, string fieldName)
    {
        var type = target.GetType();
        while (type != null)
        {
            var field = type.GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Instance);
            if (field != null) return field.GetValue(target);
            type = type.BaseType;
        }
        return null;
    }

    private static void FireActionEvent(object target, string eventName)
    {
        var type = target.GetType();
        while (type != null)
        {
            var field = type.GetField(eventName, BindingFlags.NonPublic | BindingFlags.Instance);
            if (field != null)
            {
                ((Action)field.GetValue(target))?.Invoke();
                return;
            }
            type = type.BaseType;
        }
    }

    [UnityTest]
    public IEnumerator OnEnterPlaying_ResetsScoringSystem()
    {
        yield return null;
        _scoringSystem.AddLines(2);
        Assert.Greater(_scoringSystem.Score, 0, "Precondition: score should be non-zero");

        _gsm.StartGame();

        Assert.AreEqual(0, _scoringSystem.Score);
    }

    [UnityTest]
    public IEnumerator OnEnterPlaying_SpawnsPiece()
    {
        yield return null;
        _gsm.StartGame();

        var positions = _pieceController.GetCurrentCellPositions();
        Assert.Greater(positions.Length, 0);
    }

    [UnityTest]
    public IEnumerator OnEnterPlaying_ShowsGameScreen()
    {
        yield return null;
        _gsm.StartGame();

        Assert.IsTrue(_gameScreenGo.activeSelf);
    }

    [UnityTest]
    public IEnumerator OnEnterGameOver_ClearsActivePiece()
    {
        yield return null;
        _gsm.StartGame();

        var positions = _pieceController.GetCurrentCellPositions();
        _playfieldRenderer.SetActivePiece(positions, _pieceController.GetCurrentColorIndex());

        bool anyEnabled = false;
        foreach (Transform child in _rendererGo.transform)
            if (child.name.StartsWith("PieceCell_") && child.GetComponent<SpriteRenderer>().enabled)
                anyEnabled = true;
        Assert.IsTrue(anyEnabled, "Precondition: at least one piece cell should be visible");

        _gsm.GoToGameOver();

        foreach (Transform child in _rendererGo.transform)
            if (child.name.StartsWith("PieceCell_"))
                Assert.IsFalse(child.GetComponent<SpriteRenderer>().enabled, $"{child.name} should be hidden after game over");
    }

    [UnityTest]
    public IEnumerator OnEnterStart_ClearsActivePiece()
    {
        yield return null;
        _gsm.StartGame();

        var positions = _pieceController.GetCurrentCellPositions();
        _playfieldRenderer.SetActivePiece(positions, _pieceController.GetCurrentColorIndex());

        _gsm.GoToStart();

        foreach (Transform child in _rendererGo.transform)
            if (child.name.StartsWith("PieceCell_"))
                Assert.IsFalse(child.GetComponent<SpriteRenderer>().enabled, $"{child.name} should be hidden after going to start");
    }

    [UnityTest]
    public IEnumerator HandleTopOut_TransitionsToGameOver()
    {
        yield return null;
        _gsm.StartGame();

        FireActionEvent(_pieceController, "OnTopOut");

        Assert.AreEqual("GameOver", _gsm.CurrentState);
    }

    [UnityTest]
    public IEnumerator HandleTopOut_ShowsGameOverScreen()
    {
        yield return null;
        _gsm.StartGame();

        FireActionEvent(_pieceController, "OnTopOut");

        Assert.IsTrue(_gameOverGo.activeSelf);
    }

    [UnityTest]
    public IEnumerator HandleTopOut_ShowsGameOverScreenWithCachedLeaderboard()
    {
        yield return null;
        var scores = new[] { new ScoreEntry { score = 5000, initials = "ZZZ", rank = 1 } };
        _controller.CacheLeaderboard(scores);
        _gsm.StartGame();

        FireActionEvent(_pieceController, "OnTopOut");

        Assert.IsTrue(_gameOverGo.activeSelf);
        Assert.AreEqual("GameOver", _gsm.CurrentState);
        var cached = (ScoreEntry[])GetField(_controller, "_lastFetchedLeaderboard");
        Assert.AreEqual(1, cached.Length);
        Assert.AreEqual(5000, cached[0].score);
    }

    [UnityTest]
    public IEnumerator HandlePieceLocked_SpawnsNewPiece()
    {
        yield return null;
        _gsm.StartGame();

        FireActionEvent(_pieceController, "OnPieceLocked");

        var positions = _pieceController.GetCurrentCellPositions();
        Assert.Greater(positions.Length, 0);
    }

    [UnityTest]
    public IEnumerator HandlePieceLocked_WithFullRow_UpdatesScore()
    {
        yield return null;
        _gsm.StartGame();

        for (int col = 0; col < 10; col++)
            _playfieldController.LockPiece(new[] { new Vector2Int(col, 19) }, 1);

        FireActionEvent(_pieceController, "OnPieceLocked");

        Assert.Greater(_scoringSystem.Score, 0);
    }

    [UnityTest]
    public IEnumerator CacheLeaderboard_StoresScores()
    {
        yield return null;
        var scores = new[]
        {
            new ScoreEntry { score = 1000, initials = "AAA", rank = 1 },
            new ScoreEntry { score = 500,  initials = "BBB", rank = 2 },
        };

        _controller.CacheLeaderboard(scores);

        var cached = (ScoreEntry[])GetField(_controller, "_lastFetchedLeaderboard");
        Assert.AreEqual(2, cached.Length);
        Assert.AreEqual(1000, cached[0].score);
        Assert.AreEqual(500, cached[1].score);
    }

    [UnityTest]
    public IEnumerator CacheLeaderboard_NullInput_StoresEmptyArray()
    {
        yield return null;
        _controller.CacheLeaderboard(null);

        var cached = (ScoreEntry[])GetField(_controller, "_lastFetchedLeaderboard");
        Assert.IsNotNull(cached);
        Assert.AreEqual(0, cached.Length);
    }
}
