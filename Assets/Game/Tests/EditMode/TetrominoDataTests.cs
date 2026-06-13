using NUnit.Framework;
using UnityEngine;
using UnityEditor;

[TestFixture]
public class TetrominoDataTests
{
    private TetrominoData _data;

    [SetUp]
    public void SetUp()
    {
        _data = AssetDatabase.LoadAssetAtPath<TetrominoData>("Assets/Game/Data/TetrominoData.asset");
    }

    [Test]
    public void Asset_Exists()
    {
        Assert.IsNotNull(_data, "TetrominoData.asset not found at Assets/Game/Data/TetrominoData.asset");
    }

    [Test]
    public void Pieces_HasSevenEntries()
    {
        Assert.IsNotNull(_data, "TetrominoData.asset not found");
        Assert.AreEqual(7, _data.pieces.Length);
    }

    [Test]
    public void CellSprites_HasEightEntries()
    {
        Assert.IsNotNull(_data, "TetrominoData.asset not found");
        Assert.AreEqual(8, _data.cellSprites.Length);
    }

    [Test]
    public void CellSprites_AllNonNull()
    {
        Assert.IsNotNull(_data, "TetrominoData.asset not found");
        for (int i = 0; i < _data.cellSprites.Length; i++)
            Assert.IsNotNull(_data.cellSprites[i], $"cellSprites[{i}] is null");
    }

    [Test]
    public void Pieces_NamesMatchExpected()
    {
        Assert.IsNotNull(_data, "TetrominoData.asset not found");
        string[] expected = { "I", "O", "T", "S", "Z", "J", "L" };
        for (int i = 0; i < expected.Length; i++)
            Assert.AreEqual(expected[i], _data.pieces[i].pieceName, $"pieces[{i}].pieceName mismatch");
    }

    [Test]
    public void Pieces_AtlasIndicesMatchExpected()
    {
        Assert.IsNotNull(_data, "TetrominoData.asset not found");
        for (int i = 0; i < _data.pieces.Length; i++)
            Assert.AreEqual(i + 1, _data.pieces[i].atlasIndex, $"pieces[{i}].atlasIndex expected {i + 1}");
    }

    [Test]
    public void IPiece_HasTwoRotationStates()
    {
        Assert.IsNotNull(_data, "TetrominoData.asset not found");
        Assert.AreEqual(2, _data.pieces[0].rotations.Length);
    }

    [Test]
    public void OPiece_HasOneRotationState()
    {
        Assert.IsNotNull(_data, "TetrominoData.asset not found");
        Assert.AreEqual(1, _data.pieces[1].rotations.Length);
    }

    [Test]
    public void TPiece_HasFourRotationStates()
    {
        Assert.IsNotNull(_data, "TetrominoData.asset not found");
        Assert.AreEqual(4, _data.pieces[2].rotations.Length);
    }

    [Test]
    public void SPiece_HasFourRotationStates()
    {
        Assert.IsNotNull(_data, "TetrominoData.asset not found");
        Assert.AreEqual(4, _data.pieces[3].rotations.Length);
    }

    [Test]
    public void ZPiece_HasFourRotationStates()
    {
        Assert.IsNotNull(_data, "TetrominoData.asset not found");
        Assert.AreEqual(4, _data.pieces[4].rotations.Length);
    }

    [Test]
    public void JPiece_HasFourRotationStates()
    {
        Assert.IsNotNull(_data, "TetrominoData.asset not found");
        Assert.AreEqual(4, _data.pieces[5].rotations.Length);
    }

    [Test]
    public void LPiece_HasFourRotationStates()
    {
        Assert.IsNotNull(_data, "TetrominoData.asset not found");
        Assert.AreEqual(4, _data.pieces[6].rotations.Length);
    }

    [Test]
    public void AllRotations_HaveFourCells()
    {
        Assert.IsNotNull(_data, "TetrominoData.asset not found");
        foreach (var piece in _data.pieces)
        {
            Assert.IsNotNull(piece.rotations, $"Piece {piece.pieceName} has null rotations");
            foreach (var rotation in piece.rotations)
            {
                Assert.IsNotNull(rotation, $"Piece {piece.pieceName} has a null RotationState");
                Assert.AreEqual(4, rotation.cells.Length,
                    $"Piece {piece.pieceName} has a rotation with {rotation.cells.Length} cells, expected 4");
            }
        }
    }
}
