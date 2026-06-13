using System.Collections;
using NUnit.Framework;
using Tetris.UI;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.TestTools;

public class StartScreenTests : InputTestFixture
{
    private GameObject _gsmGo;
    private GameStateManager _gsm;
    private GameObject _inputGo;
    private InputHandler _inputHandler;
    private GameObject _screenGo;
    private StartScreen _screen;
    private Keyboard _keyboard;

    [SetUp]
    public override void Setup()
    {
        base.Setup();
        _keyboard = InputSystem.AddDevice<Keyboard>();

        _gsmGo = new GameObject("GameStateManager");
        _gsm = _gsmGo.AddComponent<GameStateManager>();

        _inputGo = new GameObject("InputHandler");
        _inputHandler = _inputGo.AddComponent<InputHandler>();

        _screenGo = new GameObject("StartScreen");
        _screen = _screenGo.AddComponent<StartScreen>();
    }

    [TearDown]
    public override void TearDown()
    {
        if (_screenGo != null) Object.DestroyImmediate(_screenGo);
        if (_inputGo != null) Object.DestroyImmediate(_inputGo);
        if (_gsmGo != null) Object.DestroyImmediate(_gsmGo);
        base.TearDown();
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
    public IEnumerator OnEnterStart_ShowsScreen()
    {
        yield return null;
        _gsm.GoToStart();
        Assert.IsTrue(_screenGo.activeSelf);
    }

    [UnityTest]
    public IEnumerator OnEnterPlaying_HidesScreen()
    {
        yield return null;
        _gsm.GoToStart();
        _gsm.StartGame();
        Assert.IsFalse(_screenGo.activeSelf);
    }

    [UnityTest]
    public IEnumerator OnEnterGameOver_HidesScreen()
    {
        yield return null;
        _gsm.GoToStart();
        _gsm.GoToGameOver();
        Assert.IsFalse(_screenGo.activeSelf);
    }

    [UnityTest]
    public IEnumerator NavDown_WhenVisible_TransitionsToPlaying()
    {
        yield return null;
        _gsm.GoToStart();
        Assert.IsTrue(_screenGo.activeSelf, "Screen should be visible before input");

        Press(_keyboard.downArrowKey);
        yield return null;

        Assert.AreEqual("Playing", _gsm.CurrentState, "NavDown should call StartGame");

        Release(_keyboard.downArrowKey);
        yield return null;
    }

    [UnityTest]
    public IEnumerator NavDown_WhenHidden_DoesNotTransitionToPlaying()
    {
        yield return null;
        Assert.IsFalse(_screenGo.activeSelf, "Screen should be hidden initially");

        Press(_keyboard.downArrowKey);
        yield return null;

        Assert.AreNotEqual("Playing", _gsm.CurrentState, "NavDown should not fire when screen is hidden");

        Release(_keyboard.downArrowKey);
        yield return null;
    }
}
