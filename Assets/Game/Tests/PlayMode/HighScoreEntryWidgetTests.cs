using System.Collections;
using NUnit.Framework;
using Tetris.UI;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.TestTools;

public class HighScoreEntryWidgetTests : InputTestFixture
{
    private Keyboard _keyboard;
    private GameObject _gsmGo;
    private GameStateManager _gsm;
    private GameObject _inputGo;
    private InputHandler _inputHandler;
    private GameObject _widgetGo;
    private HighScoreEntryWidget _widget;

    [SetUp]
    public override void Setup()
    {
        base.Setup();
        _keyboard = InputSystem.AddDevice<Keyboard>();

        _gsmGo = new GameObject("GameStateManager");
        _gsm = _gsmGo.AddComponent<GameStateManager>();

        _inputGo = new GameObject("InputHandler");
        _inputHandler = _inputGo.AddComponent<InputHandler>();

        _widgetGo = new GameObject("HighScoreEntryWidget");
        _widget = _widgetGo.AddComponent<HighScoreEntryWidget>();
    }

    [TearDown]
    public override void TearDown()
    {
        if (_widgetGo != null) Object.DestroyImmediate(_widgetGo);
        if (_inputGo != null) Object.DestroyImmediate(_inputGo);
        if (_gsmGo != null) Object.DestroyImmediate(_gsmGo);
        base.TearDown();
    }

    [UnityTest]
    public IEnumerator Activate_SetsWidgetActive()
    {
        yield return null;
        _widget.Activate(1000);
        Assert.IsTrue(_widget.IsActive);
    }

    [UnityTest]
    public IEnumerator Activate_ResetsAllSlotsToSpace()
    {
        yield return null;
        _widget.Activate(500);
        Assert.AreEqual(' ', _widget.GetSlotChar(0));
        Assert.AreEqual(' ', _widget.GetSlotChar(1));
        Assert.AreEqual(' ', _widget.GetSlotChar(2));
    }

    [UnityTest]
    public IEnumerator Deactivate_SetsWidgetInactive()
    {
        yield return null;
        _widget.Activate(1000);
        _widget.Deactivate();
        Assert.IsFalse(_widget.IsActive);
    }

    [UnityTest]
    public IEnumerator NavRight_CyclesCurrentSlotFromSpaceToA()
    {
        yield return null;
        _widget.Activate(500);

        Press(_keyboard.rightArrowKey);
        yield return null;

        Assert.AreEqual('A', _widget.GetSlotChar(0));

        Release(_keyboard.rightArrowKey);
        yield return null;
    }

    [UnityTest]
    public IEnumerator NavLeft_WrapsFromSpaceToNine()
    {
        yield return null;
        _widget.Activate(500);

        Press(_keyboard.leftArrowKey);
        yield return null;

        Assert.AreEqual('9', _widget.GetSlotChar(0));

        Release(_keyboard.leftArrowKey);
        yield return null;
    }

    [UnityTest]
    public IEnumerator NavRight_WrapsFromNineBackToSpace()
    {
        yield return null;
        _widget.Activate(500);

        // Press left once to reach '9' (wraps from space)
        Press(_keyboard.leftArrowKey);
        yield return null;
        Release(_keyboard.leftArrowKey);
        yield return null;

        // Press right once to wrap back to space
        Press(_keyboard.rightArrowKey);
        yield return null;

        Assert.AreEqual(' ', _widget.GetSlotChar(0));

        Release(_keyboard.rightArrowKey);
        yield return null;
    }

    [UnityTest]
    public IEnumerator NavUp_ClearsCurrentSlotToSpace()
    {
        yield return null;
        _widget.Activate(500);

        // Set slot 0 to 'A'
        Press(_keyboard.rightArrowKey);
        yield return null;
        Release(_keyboard.rightArrowKey);
        yield return null;

        // Clear with NavUp
        Press(_keyboard.upArrowKey);
        yield return null;

        Assert.AreEqual(' ', _widget.GetSlotChar(0));

        Release(_keyboard.upArrowKey);
        yield return null;
    }

    [UnityTest]
    public IEnumerator NavDown_AdvancesActiveSlotToNext()
    {
        yield return null;
        _widget.Activate(500);

        // Confirm slot 0 and advance to slot 1
        Press(_keyboard.downArrowKey);
        yield return null;
        Release(_keyboard.downArrowKey);
        yield return null;

        // NavRight now affects slot 1, not slot 0
        Press(_keyboard.rightArrowKey);
        yield return null;

        Assert.AreEqual(' ', _widget.GetSlotChar(0), "Slot 0 should remain unchanged after advancing");
        Assert.AreEqual('A', _widget.GetSlotChar(1), "Slot 1 should cycle to A after NavRight");

        Release(_keyboard.rightArrowKey);
        yield return null;
    }

    [UnityTest]
    public IEnumerator NavDown_OnSlot2_EntersConfirmMode_WidgetRemainsActive()
    {
        yield return null;
        _widget.Activate(500);

        // Confirm slots 0, 1, 2 with three NavDowns
        for (int i = 0; i < 3; i++)
        {
            Press(_keyboard.downArrowKey);
            yield return null;
            Release(_keyboard.downArrowKey);
            yield return null;
        }

        Assert.IsTrue(_widget.IsActive, "Widget should stay active while awaiting CONFIRM press");
    }

    [UnityTest]
    public IEnumerator NavDown_OnConfirmWithNoLeaderboardService_TransitionsToStart()
    {
        yield return null;
        _widget.Activate(500);

        // Three NavDowns to reach confirm state
        for (int i = 0; i < 3; i++)
        {
            Press(_keyboard.downArrowKey);
            yield return null;
            Release(_keyboard.downArrowKey);
            yield return null;
        }

        // Fourth NavDown on CONFIRM -- no LeaderboardService so GoToStart fires directly
        Press(_keyboard.downArrowKey);
        yield return null;

        Assert.AreEqual("Start", _gsm.CurrentState, "NavDown on CONFIRM should navigate to Start state");

        Release(_keyboard.downArrowKey);
        yield return null;
    }

    [UnityTest]
    public IEnumerator InactiveWidget_IgnoresNavInput()
    {
        yield return null;
        // Widget never activated -- NavRight should not change slot 0

        Press(_keyboard.rightArrowKey);
        yield return null;

        Assert.AreEqual(' ', _widget.GetSlotChar(0), "Inactive widget should ignore input");

        Release(_keyboard.rightArrowKey);
        yield return null;
    }

    [UnityTest]
    public IEnumerator Activate_TwiceResets_AllSlotsToSpace()
    {
        yield return null;
        _widget.Activate(100);

        // Advance slot 0 to 'A'
        Press(_keyboard.rightArrowKey);
        yield return null;
        Release(_keyboard.rightArrowKey);
        yield return null;

        // Re-activate should reset
        _widget.Activate(200);

        Assert.AreEqual(' ', _widget.GetSlotChar(0), "Re-activating should reset slot 0 to space");
        Assert.AreEqual(' ', _widget.GetSlotChar(1));
        Assert.AreEqual(' ', _widget.GetSlotChar(2));
    }

    [UnityTest]
    public IEnumerator GetSlotChar_OutOfRange_ReturnsSpace()
    {
        yield return null;
        _widget.Activate(100);
        Assert.AreEqual(' ', _widget.GetSlotChar(-1));
        Assert.AreEqual(' ', _widget.GetSlotChar(3));
    }
}
