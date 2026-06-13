using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class GameStateManagerTests
{
    private GameObject _go;
    private GameStateManager _manager;

    [SetUp]
    public void SetUp()
    {
        _go = new GameObject("GameStateManager");
        _manager = _go.AddComponent<GameStateManager>();
    }

    [TearDown]
    public void TearDown()
    {
        if (_go != null)
            Object.DestroyImmediate(_go);
    }

    [UnityTest]
    public IEnumerator Awake_SetsInstance()
    {
        yield return null;
        Assert.IsNotNull(GameStateManager.Instance);
        Assert.AreEqual(_manager, GameStateManager.Instance);
    }

    [UnityTest]
    public IEnumerator GoToStart_SetsCurrentState()
    {
        yield return null;
        _manager.GoToStart();
        Assert.AreEqual("Start", _manager.CurrentState);
    }

    [UnityTest]
    public IEnumerator StartGame_SetsCurrentState()
    {
        yield return null;
        _manager.StartGame();
        Assert.AreEqual("Playing", _manager.CurrentState);
    }

    [UnityTest]
    public IEnumerator GoToGameOver_SetsCurrentState()
    {
        yield return null;
        _manager.GoToGameOver();
        Assert.AreEqual("GameOver", _manager.CurrentState);
    }

    [UnityTest]
    public IEnumerator IsInState_ReturnsTrueForCurrentState()
    {
        yield return null;
        _manager.GoToStart();
        Assert.IsTrue(_manager.IsInState("Start"));
        Assert.IsFalse(_manager.IsInState("Playing"));
    }

    [UnityTest]
    public IEnumerator OnEnterStart_FiredOnGoToStart()
    {
        yield return null;
        bool fired = false;
        _manager.OnEnterStart += () => fired = true;
        _manager.GoToStart();
        Assert.IsTrue(fired);
    }

    [UnityTest]
    public IEnumerator OnEnterPlaying_FiredOnStartGame()
    {
        yield return null;
        bool fired = false;
        _manager.OnEnterPlaying += () => fired = true;
        _manager.StartGame();
        Assert.IsTrue(fired);
    }

    [UnityTest]
    public IEnumerator OnEnterGameOver_FiredOnGoToGameOver()
    {
        yield return null;
        bool fired = false;
        _manager.OnEnterGameOver += () => fired = true;
        _manager.GoToGameOver();
        Assert.IsTrue(fired);
    }

    [UnityTest]
    public IEnumerator SecondInstance_IsDestroyedBySingleton()
    {
        yield return null;
        Assert.IsNotNull(GameStateManager.Instance);

        var go2 = new GameObject("GameStateManager2");
        go2.AddComponent<GameStateManager>();

        yield return null;

        Assert.IsTrue(go2 == null, "Second GameStateManager GameObject should be destroyed by singleton logic");
        Assert.AreEqual(_manager, GameStateManager.Instance, "Original instance should remain");
    }
}
