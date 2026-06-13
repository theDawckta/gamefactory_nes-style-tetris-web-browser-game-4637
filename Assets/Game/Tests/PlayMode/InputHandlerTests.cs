using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.TestTools;

public class InputHandlerTests : InputTestFixture
{
    private Keyboard _keyboard;
    private GameObject _go;
    private InputHandler _handler;

    [SetUp]
    public override void Setup()
    {
        base.Setup();
        _keyboard = InputSystem.AddDevice<Keyboard>();
        _go = new GameObject("InputHandler");
        _handler = _go.AddComponent<InputHandler>();
    }

    [TearDown]
    public override void TearDown()
    {
        if (_go != null)
            Object.DestroyImmediate(_go);
        base.TearDown();
    }

    [UnityTest]
    public IEnumerator OnActionDown_FiresOnceWhenKeyFirstPressed()
    {
        yield return null;

        var fired = new List<string>();
        _handler.OnActionDown += action => fired.Add(action);

        Press(_keyboard.zKey);
        yield return null;

        Assert.AreEqual(1, fired.FindAll(a => a == "RotateLeft").Count,
            "OnActionDown should fire exactly once for RotateLeft on Z press");

        fired.Clear();
        yield return null;
        Assert.AreEqual(0, fired.FindAll(a => a == "RotateLeft").Count,
            "OnActionDown should not fire again while key is still held");

        Release(_keyboard.zKey);
        yield return null;
    }

    [UnityTest]
    public IEnumerator OnActionHeld_FiresEveryFrameWhileKeyHeld()
    {
        yield return null;

        int heldCount = 0;
        _handler.OnActionHeld += action => { if (action == "RotateLeft") heldCount++; };

        Press(_keyboard.zKey);
        yield return null;
        yield return null;
        yield return null;

        Assert.GreaterOrEqual(heldCount, 2,
            "OnActionHeld should fire every frame while key is held");

        Release(_keyboard.zKey);
        yield return null;
    }

    [UnityTest]
    public IEnumerator OnActionUp_FiresOnceWhenKeyReleased()
    {
        yield return null;

        int upCount = 0;
        _handler.OnActionUp += action => { if (action == "RotateLeft") upCount++; };

        Press(_keyboard.zKey);
        yield return null;

        Release(_keyboard.zKey);
        yield return null;

        Assert.AreEqual(1, upCount,
            "OnActionUp should fire exactly once when key is released");

        yield return null;
        Assert.AreEqual(1, upCount,
            "OnActionUp should not fire again after key is already released");
    }

    [UnityTest]
    public IEnumerator OnActionDown_SameKeyMultipleActions_AllFire()
    {
        yield return null;

        var fired = new List<string>();
        _handler.OnActionDown += action => fired.Add(action);

        Press(_keyboard.leftArrowKey);
        yield return null;

        Assert.IsTrue(fired.Contains("MoveLeft"),
            "MoveLeft should fire when LeftArrow is pressed");
        Assert.IsTrue(fired.Contains("NavLeft"),
            "NavLeft should fire when LeftArrow is pressed");

        Release(_keyboard.leftArrowKey);
        yield return null;
    }

    [UnityTest]
    public IEnumerator OnActionDown_DoesNotFireWhenNoKeyPressed()
    {
        yield return null;

        int downCount = 0;
        _handler.OnActionDown += _ => downCount++;

        yield return null;
        yield return null;

        Assert.AreEqual(0, downCount,
            "OnActionDown should not fire when no key is pressed");
    }
}
