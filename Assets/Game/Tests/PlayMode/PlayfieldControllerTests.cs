using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class PlayfieldControllerTests
{
    private GameObject _go;
    private PlayfieldController _controller;

    [SetUp]
    public void SetUp()
    {
        _go = new GameObject("PlayfieldController");
        _controller = _go.AddComponent<PlayfieldController>();
    }

    [TearDown]
    public void TearDown()
    {
        Object.Destroy(_go);
    }

    [UnityTest]
    public IEnumerator Reset_AllCellsAreZero()
    {
        _controller.LockPiece(new[] { new Vector2Int(0, 0), new Vector2Int(5, 10) }, 3);
        _controller.Reset();
        yield return null;

        _controller.GetBoardState(out var state);
        for (int col = 0; col < 10; col++)
            for (int row = 0; row < 20; row++)
                Assert.AreEqual(0, state[col, row], $"Cell [{col},{row}] should be zero after Reset");
    }

    [UnityTest]
    public IEnumerator LockPiece_WritesCorrectColorIndex()
    {
        var cells = new[] { new Vector2Int(2, 5), new Vector2Int(3, 5), new Vector2Int(4, 5) };
        _controller.LockPiece(cells, 4);
        yield return null;

        _controller.GetBoardState(out var state);
        Assert.AreEqual(4, state[2, 5]);
        Assert.AreEqual(4, state[3, 5]);
        Assert.AreEqual(4, state[4, 5]);
        Assert.AreEqual(0, state[0, 0]);
    }

    [UnityTest]
    public IEnumerator IsInBounds_ReturnsTrueForValidCells()
    {
        yield return null;

        Assert.IsTrue(_controller.IsInBounds(0, 0));
        Assert.IsTrue(_controller.IsInBounds(9, 19));
        Assert.IsTrue(_controller.IsInBounds(5, 10));
    }

    [UnityTest]
    public IEnumerator IsInBounds_ReturnsFalseForOutOfRangeCells()
    {
        yield return null;

        Assert.IsFalse(_controller.IsInBounds(-1, 0));
        Assert.IsFalse(_controller.IsInBounds(10, 0));
        Assert.IsFalse(_controller.IsInBounds(0, -1));
        Assert.IsFalse(_controller.IsInBounds(0, 20));
    }

    [UnityTest]
    public IEnumerator IsOccupied_ReturnsTrueAfterLock()
    {
        _controller.LockPiece(new[] { new Vector2Int(1, 1) }, 2);
        yield return null;

        Assert.IsTrue(_controller.IsOccupied(1, 1));
        Assert.IsFalse(_controller.IsOccupied(0, 0));
    }

    [UnityTest]
    public IEnumerator IsOccupied_ReturnsFalseForOutOfBounds()
    {
        yield return null;

        Assert.IsFalse(_controller.IsOccupied(-1, 0));
        Assert.IsFalse(_controller.IsOccupied(10, 0));
    }

    [UnityTest]
    public IEnumerator ClearLines_ReturnsZeroWhenNoFullRows()
    {
        _controller.LockPiece(new[] { new Vector2Int(0, 19) }, 1);
        yield return null;

        int cleared = _controller.ClearLines();
        Assert.AreEqual(0, cleared);
    }

    [UnityTest]
    public IEnumerator ClearLines_RemovesOneFullRow()
    {
        for (int col = 0; col < 10; col++)
            _controller.LockPiece(new[] { new Vector2Int(col, 19) }, 1);

        yield return null;
        int cleared = _controller.ClearLines();
        Assert.AreEqual(1, cleared);

        _controller.GetBoardState(out var state);
        for (int col = 0; col < 10; col++)
            Assert.AreEqual(0, state[col, 19], $"Bottom row should be empty after clear");
    }

    [UnityTest]
    public IEnumerator ClearLines_ShiftsRowsDownAfterClear()
    {
        // Put a marker in row 18, fill row 19
        _controller.LockPiece(new[] { new Vector2Int(0, 18) }, 5);
        for (int col = 0; col < 10; col++)
            _controller.LockPiece(new[] { new Vector2Int(col, 19) }, 1);

        yield return null;
        _controller.ClearLines();

        _controller.GetBoardState(out var state);
        Assert.AreEqual(5, state[0, 19], "Row 18 marker should have shifted down to row 19");
        Assert.AreEqual(0, state[0, 18], "Row 18 should now be empty after shift");
    }

    [UnityTest]
    public IEnumerator ClearLines_ClearsFourRowsSimultaneously()
    {
        // Fill rows 16-19
        for (int row = 16; row <= 19; row++)
            for (int col = 0; col < 10; col++)
                _controller.LockPiece(new[] { new Vector2Int(col, row) }, 1);

        // Put a distinct marker in row 15
        _controller.LockPiece(new[] { new Vector2Int(3, 15) }, 7);

        yield return null;
        int cleared = _controller.ClearLines();
        Assert.AreEqual(4, cleared);

        _controller.GetBoardState(out var state);
        Assert.AreEqual(7, state[3, 19], "Marker from row 15 should shift to row 19 after 4 clears");
        for (int row = 0; row < 19; row++)
            Assert.AreEqual(0, state[3, row], $"Row {row} col 3 should be empty after 4-line clear");
    }

    [UnityTest]
    public IEnumerator IsTopRowOccupied_ReturnsFalseWhenTopRowEmpty()
    {
        yield return null;
        Assert.IsFalse(_controller.IsTopRowOccupied());
    }

    [UnityTest]
    public IEnumerator IsTopRowOccupied_ReturnsTrueWhenTopRowHasCell()
    {
        _controller.LockPiece(new[] { new Vector2Int(4, 0) }, 2);
        yield return null;

        Assert.IsTrue(_controller.IsTopRowOccupied());
    }

    [UnityTest]
    public IEnumerator GetBoardState_ReturnsCopyNotReference()
    {
        _controller.LockPiece(new[] { new Vector2Int(0, 0) }, 3);
        yield return null;

        _controller.GetBoardState(out var state);
        state[0, 0] = 99;

        _controller.GetBoardState(out var stateAfter);
        Assert.AreEqual(3, stateAfter[0, 0], "Mutating the copy should not affect the internal grid");
    }
}
