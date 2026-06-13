using NUnit.Framework;
using UnityEngine;

[TestFixture]
public class PieceBagTests
{
    private TetrominoData _data;
    private PieceBag _bag;

    [SetUp]
    public void SetUp()
    {
        _data = ScriptableObject.CreateInstance<TetrominoData>();
        _data.pieces = new PieceDefinition[7];
        for (int i = 0; i < 7; i++)
        {
            _data.pieces[i] = new PieceDefinition
            {
                pieceName = ((char)('A' + i)).ToString(),
                atlasIndex = i + 1,
                rotations = new RotationState[0]
            };
        }
        _bag = new PieceBag(_data);
    }

    [TearDown]
    public void TearDown()
    {
        Object.DestroyImmediate(_data);
    }

    [Test]
    public void Next_NeverReturnsIndexOutside0To6()
    {
        for (int i = 0; i < 1000; i++)
        {
            var piece = _bag.Next();
            bool found = false;
            for (int j = 0; j < _data.pieces.Length; j++)
            {
                if (ReferenceEquals(_data.pieces[j], piece)) { found = true; break; }
            }
            Assert.IsTrue(found, "Next() returned a PieceDefinition not in pieces[0..6]");
        }
    }

    [Test]
    public void Peek_ReturnsSamePieceAsSubsequentNext()
    {
        var peeked = _bag.Peek();
        var next = _bag.Next();
        Assert.AreSame(peeked, next, "Peek() must return the same piece as the next Next() call");
    }

    [Test]
    public void Peek_IsStableBetweenMultipleCalls()
    {
        var peek1 = _bag.Peek();
        var peek2 = _bag.Peek();
        var peek3 = _bag.Peek();
        Assert.AreSame(peek1, peek2, "Peek() must return the same result on repeated calls");
        Assert.AreSame(peek2, peek3, "Peek() must return the same result on repeated calls");
    }

    [Test]
    public void Peek_AfterMultiplePeeks_StillMatchesNext()
    {
        _bag.Peek();
        _bag.Peek();
        var peeked = _bag.Peek();
        var next = _bag.Next();
        Assert.AreSame(peeked, next, "Peek() after repeated calls must still equal subsequent Next()");
    }

    [Test]
    public void Peek_AfterNext_ReflectsNewQueuedPiece()
    {
        var peek1 = _bag.Peek();
        _bag.Next();
        var peek2 = _bag.Peek();
        var next2 = _bag.Next();
        Assert.AreSame(peek2, next2, "Peek() after Next() must match the new next piece");
    }

    [Test]
    public void Next_AllSevenPiecesAppearIn1000Calls()
    {
        var seen = new System.Collections.Generic.HashSet<string>();
        for (int i = 0; i < 1000; i++)
            seen.Add(_bag.Next().pieceName);
        Assert.AreEqual(7, seen.Count, "All 7 piece types must appear in 1000 consecutive Next() calls");
    }

    [Test]
    public void Next_ReturnsNonNullPiece()
    {
        Assert.IsNotNull(_bag.Next());
    }

    [Test]
    public void Peek_ReturnsNonNullPiece()
    {
        Assert.IsNotNull(_bag.Peek());
    }
}
