using System.Collections;
using NUnit.Framework;
using Tetris.UI;
using UnityEngine;
using UnityEngine.TestTools;

public class GameScreenTests
{
    private GameObject _gsmGo;
    private GameStateManager _gsm;
    private GameObject _screenGo;
    private GameScreen _screen;

    [SetUp]
    public void SetUp()
    {
        _gsmGo = new GameObject("GameStateManager");
        _gsm = _gsmGo.AddComponent<GameStateManager>();

        _screenGo = new GameObject("GameScreen");
        _screen = _screenGo.AddComponent<GameScreen>();
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
    public IEnumerator Show_ActivatesGameObject()
    {
        yield return null;
        _screen.Show();
        Assert.IsTrue(_screenGo.activeSelf);
    }

    [UnityTest]
    public IEnumerator Hide_DeactivatesGameObject()
    {
        yield return null;
        _screen.Show();
        _screen.Hide();
        Assert.IsFalse(_screenGo.activeSelf);
    }

    [UnityTest]
    public IEnumerator OnEnterPlaying_ShowsScreen()
    {
        yield return null;
        _gsm.StartGame();
        Assert.IsTrue(_screenGo.activeSelf);
    }

    [UnityTest]
    public IEnumerator OnEnterStart_HidesScreen()
    {
        yield return null;
        _screen.Show();
        _gsm.GoToStart();
        Assert.IsFalse(_screenGo.activeSelf);
    }

    [UnityTest]
    public IEnumerator OnEnterGameOver_HidesScreen()
    {
        yield return null;
        _screen.Show();
        _gsm.GoToGameOver();
        Assert.IsFalse(_screenGo.activeSelf);
    }

    [UnityTest]
    public IEnumerator HudRegion_WithoutUIDocument_ReturnsNull()
    {
        yield return null;
        Assert.IsNull(_screen.HudRegion);
    }
}
