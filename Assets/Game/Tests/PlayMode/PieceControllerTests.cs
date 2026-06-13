using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class PieceControllerTests
{
    private GameObject _go;
    private PlayfieldController _playfield;
    private PieceController _controller;

    [SetUp]
    public void SetUp()
    {
        _go = new GameObject("PieceController");
        _playfield = _go.AddComponent<PlayfieldController>();
        _controller = _go.AddComponent<PieceController>();
        _controller.enabled = false; // prevent Update() from driving frame steps during tests
    }

    [TearDown]
    public void TearDown()
    {
        Object.Destroy(_go);
    }

    private static PieceDefinition MakePiece(int atlasIndex, params Vector2Int[][] rotations)
    {
        var def = new PieceDefinition();
        def.atlasIndex = atlasIndex;
        def.pieceName = "Test";
        def.rotations = new RotationState[rotations.Length];
        for (int i = 0; i < rotations.Length; i++)
        {
            def.rotations[i] = new RotationState();
            def.rotations[i].cells = rotations[i];
        }
        return def;
    }

    private static Vector2Int[] Single() => new[] { Vector2Int.zero };
    private static Vector2Int[] Horizontal() => new[] { Vector2Int.zero, new Vector2Int(1, 0) };
    private static Vector2Int[] Vertical() => new[] { Vector2Int.zero, new Vector2Int(0, 1) };

    private void NoInput() => _controller.ProcessInput(false, false, false, false, false);
    private void Step() => _controller.ProcessFrameStep();

    [UnityTest]
    public IEnumerator SpawnPiece_PlacesPieceAtCol3Row0()
    {
        yield return null;
        _controller.SpawnPiece(MakePiece(2, Single()));
        var pos = _controller.GetCurrentCellPositions();
        Assert.AreEqual(1, pos.Length);
        Assert.AreEqual(new Vector2Int(3, 0), pos[0]);
    }

    [UnityTest]
    public IEnumerator SpawnPiece_FiresOnTopOut_WhenSpawnPositionOccupied()
    {
        yield return null;
        bool fired = false;
        _controller.OnTopOut += () => fired = true;
        _playfield.LockPiece(new[] { new Vector2Int(3, 0) }, 1);
        _controller.SpawnPiece(MakePiece(2, Single()));
        Assert.IsTrue(fired);
    }

    [UnityTest]
    public IEnumerator SpawnPiece_DoesNotFireOnTopOut_WhenPositionClear()
    {
        yield return null;
        bool fired = false;
        _controller.OnTopOut += () => fired = true;
        _controller.SpawnPiece(MakePiece(2, Single()));
        Assert.IsFalse(fired);
    }

    [UnityTest]
    public IEnumerator GetCurrentCellPositions_ReturnsWorldPositionsForMultiCellPiece()
    {
        yield return null;
        _controller.SpawnPiece(MakePiece(3, Horizontal()));
        var pos = _controller.GetCurrentCellPositions();
        Assert.AreEqual(2, pos.Length);
        Assert.AreEqual(new Vector2Int(3, 0), pos[0]);
        Assert.AreEqual(new Vector2Int(4, 0), pos[1]);
    }

    [UnityTest]
    public IEnumerator GetCurrentColorIndex_ReturnsAtlasIndex()
    {
        yield return null;
        _controller.SpawnPiece(MakePiece(5, Single()));
        Assert.AreEqual(5, _controller.GetCurrentColorIndex());
    }

    [UnityTest]
    public IEnumerator GetCurrentCellPositions_ReturnsEmptyArrayBeforeSpawn()
    {
        yield return null;
        var pos = _controller.GetCurrentCellPositions();
        Assert.AreEqual(0, pos.Length);
    }

    [UnityTest]
    public IEnumerator Gravity_Default_DoesNotDropBefore48FrameSteps()
    {
        yield return null;
        _controller.SpawnPiece(MakePiece(1, Single()));
        for (int i = 0; i < 47; i++)
        {
            NoInput();
            Step();
        }
        Assert.AreEqual(0, _controller.GetCurrentCellPositions()[0].y);
    }

    [UnityTest]
    public IEnumerator Gravity_Default_DropsOnFrame48()
    {
        yield return null;
        _controller.SpawnPiece(MakePiece(1, Single()));
        for (int i = 0; i < 48; i++)
        {
            NoInput();
            Step();
        }
        Assert.AreEqual(1, _controller.GetCurrentCellPositions()[0].y);
    }

    [UnityTest]
    public IEnumerator SetLevel_AffectsGravityRate()
    {
        yield return null;
        _controller.SpawnPiece(MakePiece(1, Single()));
        _controller.SetLevel(10); // level 10 -> index 9 -> 6 frames/row
        for (int i = 0; i < 6; i++)
        {
            NoInput();
            Step();
        }
        Assert.AreEqual(1, _controller.GetCurrentCellPositions()[0].y);
    }

    [UnityTest]
    public IEnumerator SoftDrop_MovesPieceDownEachFrameStep()
    {
        yield return null;
        _controller.SpawnPiece(MakePiece(1, Single()));
        _controller.ProcessInput(false, false, false, false, true);
        Step();
        Assert.AreEqual(1, _controller.GetCurrentCellPositions()[0].y);
        Step();
        Assert.AreEqual(2, _controller.GetCurrentCellPositions()[0].y);
    }

    [UnityTest]
    public IEnumerator DAS_Left_MovesImmediatelyOnFirstPress()
    {
        yield return null;
        _controller.SpawnPiece(MakePiece(1, Single()));
        _controller.ProcessInput(true, false, false, false, false);
        Step();
        Assert.AreEqual(2, _controller.GetCurrentCellPositions()[0].x);
    }

    [UnityTest]
    public IEnumerator DAS_Left_DoesNotRepeatBefore16FrameDelay()
    {
        yield return null;
        _controller.SpawnPiece(MakePiece(1, Single()));
        _controller.ProcessInput(true, false, false, false, false);
        // Step 1: immediate move to col 2, dasCounter=0
        Step();
        Assert.AreEqual(2, _controller.GetCurrentCellPositions()[0].x);
        // Steps 2-16: counter goes 1..15, no repeat yet
        for (int i = 1; i < 16; i++)
            Step();
        Assert.AreEqual(2, _controller.GetCurrentCellPositions()[0].x);
    }

    [UnityTest]
    public IEnumerator DAS_Left_RepeatsMoveAtFrame16ThenEvery6Frames()
    {
        yield return null;
        _controller.SpawnPiece(MakePiece(1, Single()));
        _controller.ProcessInput(true, false, false, false, false);
        // Step 1: immediate move (col 3->2), dasCounter=0
        Step();
        // Steps 2-17 (counter 1->16): move fires at counter==16 (col 2->1)
        for (int i = 1; i <= 16; i++)
            Step();
        Assert.AreEqual(1, _controller.GetCurrentCellPositions()[0].x);
        // Steps 18-23 (counter 17->22): move fires at counter==22 i.e. repeat=6 (col 1->0)
        for (int i = 17; i <= 22; i++)
            Step();
        Assert.AreEqual(0, _controller.GetCurrentCellPositions()[0].x);
    }

    [UnityTest]
    public IEnumerator DAS_ResetsOnRelease_AllowsImmediateMoveOnNextPress()
    {
        yield return null;
        _controller.SpawnPiece(MakePiece(1, Single()));
        _controller.ProcessInput(true, false, false, false, false);
        Step(); // col 3->2
        _controller.ProcessInput(false, false, false, false, false);
        Step(); // released, DAS resets
        _controller.ProcessInput(true, false, false, false, false);
        Step(); // new press: col 2->1
        Assert.AreEqual(1, _controller.GetCurrentCellPositions()[0].x);
    }

    [UnityTest]
    public IEnumerator OnPieceLocked_FiresAfter30GroundedFrameSteps()
    {
        yield return null;
        _controller.SpawnPiece(MakePiece(1, Single()));
        bool locked = false;
        _controller.OnPieceLocked += () => locked = true;

        // Soft-drop to bottom row (row 19)
        _controller.ProcessInput(false, false, false, false, true);
        for (int i = 0; i < 19; i++)
            Step();

        // Ground the piece (row 20 is out of bounds)
        Step(); // lockCounter -> 1
        Assert.IsFalse(locked);

        // 28 more grounded steps -> lockCounter 2..29
        NoInput();
        for (int i = 1; i < 29; i++)
            Step();
        Assert.IsFalse(locked);

        // 30th grounded step -> lock
        Step();
        Assert.IsTrue(locked);
    }

    [UnityTest]
    public IEnumerator LockTimer_ResetsOnSuccessfulLateralMoveWhileGrounded()
    {
        yield return null;
        _controller.SpawnPiece(MakePiece(1, Single()));
        bool locked = false;
        _controller.OnPieceLocked += () => locked = true;

        // Drop to bottom
        _controller.ProcessInput(false, false, false, false, true);
        for (int i = 0; i < 19; i++)
            Step();
        Step(); // ground, lockCounter -> 1

        // Move right while grounded: resets lockCounter to 0, then increments to 1
        _controller.ProcessInput(false, true, false, false, false);
        Step();
        Assert.IsFalse(locked);

        // 28 more steps -> lockCounter 2..29
        NoInput();
        for (int i = 0; i < 28; i++)
            Step();
        Assert.IsFalse(locked);

        // 30th grounded step from reset -> lock
        Step();
        Assert.IsTrue(locked);
    }

    [UnityTest]
    public IEnumerator Rotation_SucceedsWhenClear()
    {
        yield return null;
        _controller.SpawnPiece(MakePiece(1, Horizontal(), Vertical()));
        _controller.ProcessInput(false, false, false, true, false);
        Step();
        var pos = _controller.GetCurrentCellPositions();
        Assert.AreEqual(new Vector2Int(3, 0), pos[0]);
        Assert.AreEqual(new Vector2Int(3, 1), pos[1]);
    }

    [UnityTest]
    public IEnumerator WallKick_NudgesPieceTowardCenter_OnLeftWallCollision()
    {
        yield return null;
        // Rotation 0: (0,0). Rotation 1: (-1,0),(0,0) -- extends left.
        // At col 0, rotation 1 cell (-1,0) is OOB. Nudge +1 to col 1 succeeds.
        var piece = MakePiece(1,
            new[] { Vector2Int.zero },
            new[] { new Vector2Int(-1, 0), Vector2Int.zero });
        _controller.SpawnPiece(piece); // at col 3

        // Move piece to left wall (col 0)
        _controller.ProcessInput(true, false, false, false, false);
        for (int i = 0; i < 3; i++)
        {
            Step();
            _controller.ProcessInput(false, false, false, false, false);
            Step();
            _controller.ProcessInput(true, false, false, false, false);
        }
        Assert.AreEqual(0, _controller.GetCurrentCellPositions()[0].x);

        // Rotate: initial blocked, nudge +1 to col 1 succeeds
        _controller.ProcessInput(false, false, false, true, false);
        Step();
        var pos = _controller.GetCurrentCellPositions();
        Assert.AreEqual(2, pos.Length);
        Assert.AreEqual(0, pos[0].x);
        Assert.AreEqual(1, pos[1].x);
    }

    [UnityTest]
    public IEnumerator Rotation_CancelledWhenBothOriginalAndNudgeAreBlocked()
    {
        yield return null;
        // Rotation 0: (0,0). Rotation 1: (1,0).
        // At col 3: rotation 1 needs col 4 -> block. Nudge +1 to col 4 needs col 5 -> block.
        var piece = MakePiece(1,
            new[] { Vector2Int.zero },
            new[] { new Vector2Int(1, 0) });
        _controller.SpawnPiece(piece);
        _playfield.LockPiece(new[] { new Vector2Int(4, 0) }, 2);
        _playfield.LockPiece(new[] { new Vector2Int(5, 0) }, 2);

        _controller.ProcessInput(false, false, false, true, false);
        Step();

        var pos = _controller.GetCurrentCellPositions();
        Assert.AreEqual(1, pos.Length);
        Assert.AreEqual(new Vector2Int(3, 0), pos[0]);
    }
}
