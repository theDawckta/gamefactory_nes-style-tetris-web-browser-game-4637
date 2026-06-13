using System.Collections;
using NUnit.Framework;
using Tetris.UI;
using UnityEngine;
using UnityEngine.TestTools;

public class GameOverScreenTests
{
    private GameObject _gsmGo;
    private GameStateManager _gsm;
    private GameObject _screenGo;
    private GameOverScreen _screen;

    [SetUp]
    public void SetUp()
    {
        _gsmGo = new GameObject("GameStateManager");
        _gsm = _gsmGo.AddComponent<GameStateManager>();

        _screenGo = new GameObject("GameOverScreen");
        _screen = _screenGo.AddComponent<GameOverScreen>();
    }

    [TearDown]
    public void TearDown()
    {
        if (_screenGo != null) Object.DestroyImmediate(_screenGo);
        if (_gsmGo != null) Object.DestroyImmediate(_gsmGo);
    }

    [UnityTest]
    public IEnumerator Start_ScreenStartsHidden()
    {
        yield return null;
        Assert.IsFalse(_screenGo.activeSelf);
    }

    [UnityTest]
    public IEnumerator ShowWithScore_ActivatesGameObject()
    {
        yield return null;
        _screen.ShowWithScore(1000, new ScoreEntry[0]);
        Assert.IsTrue(_screenGo.activeSelf);
    }

    [UnityTest]
    public IEnumerator Hide_DeactivatesGameObject()
    {
        yield return null;
        _screen.ShowWithScore(1000, new ScoreEntry[0]);
        _screen.Hide();
        Assert.IsFalse(_screenGo.activeSelf);
    }

    [UnityTest]
    public IEnumerator OnEnterStart_HidesScreen()
    {
        yield return null;
        _screen.ShowWithScore(0, new ScoreEntry[0]);
        _gsm.GoToStart();
        Assert.IsFalse(_screenGo.activeSelf);
    }

    [UnityTest]
    public IEnumerator OnEnterPlaying_HidesScreen()
    {
        yield return null;
        _screen.ShowWithScore(0, new ScoreEntry[0]);
        _gsm.StartGame();
        Assert.IsFalse(_screenGo.activeSelf);
    }

    [UnityTest]
    public IEnumerator OnEnterGameOver_ShowsScreen()
    {
        yield return null;
        _gsm.GoToGameOver();
        Assert.IsTrue(_screenGo.activeSelf);
    }

    [UnityTest]
    public IEnumerator ShowWithScore_NullLeaderboard_ActivatesScreen()
    {
        yield return null;
        _screen.ShowWithScore(500, null);
        Assert.IsTrue(_screenGo.activeSelf);
    }

    [UnityTest]
    public IEnumerator ShowWithScore_FullLeaderboardHigherScore_ActivatesScreen()
    {
        yield return null;
        var leaderboard = new ScoreEntry[]
        {
            new ScoreEntry { score = 100 },
            new ScoreEntry { score = 200 },
            new ScoreEntry { score = 300 },
            new ScoreEntry { score = 400 },
            new ScoreEntry { score = 500 },
        };
        _screen.ShowWithScore(600, leaderboard);
        Assert.IsTrue(_screenGo.activeSelf);
    }

    [UnityTest]
    public IEnumerator ShowWithScore_FullLeaderboardLowerScore_ActivatesScreen()
    {
        yield return null;
        var leaderboard = new ScoreEntry[]
        {
            new ScoreEntry { score = 100 },
            new ScoreEntry { score = 200 },
            new ScoreEntry { score = 300 },
            new ScoreEntry { score = 400 },
            new ScoreEntry { score = 500 },
        };
        _screen.ShowWithScore(50, leaderboard);
        Assert.IsTrue(_screenGo.activeSelf);
    }
}
