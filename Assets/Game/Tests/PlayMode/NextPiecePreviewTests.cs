using System.Collections;
using System.Reflection;
using NUnit.Framework;
using Tetris.UI;
using UnityEngine;
using UnityEngine.TestTools;

public class NextPiecePreviewTests
{
    private GameObject _go;
    private NextPiecePreview _preview;
    private TetrominoData _tetrominoData;
    private Texture2D[] _textures;

    // T-piece: cells at (0,1),(1,0),(1,1),(2,1)
    private PieceDefinition MakePiece(int atlasIndex, Vector2Int[] cells)
    {
        return new PieceDefinition
        {
            pieceName = "Test",
            atlasIndex = atlasIndex,
            rotations = new[] { new RotationState { cells = cells } }
        };
    }

    [SetUp]
    public void SetUp()
    {
        _tetrominoData = ScriptableObject.CreateInstance<TetrominoData>();
        _textures = new Texture2D[8];
        _tetrominoData.cellSprites = new Sprite[8];
        for (int i = 0; i < 8; i++)
        {
            _textures[i] = new Texture2D(24, 24);
            _tetrominoData.cellSprites[i] = Sprite.Create(_textures[i], new Rect(0, 0, 24, 24), Vector2.one * 0.5f, 24f);
        }

        _go = new GameObject("NextPiecePreview");
        _preview = _go.AddComponent<NextPiecePreview>();

        typeof(NextPiecePreview)
            .GetField("_tetrominoData", BindingFlags.NonPublic | BindingFlags.Instance)
            .SetValue(_preview, _tetrominoData);
    }

    [TearDown]
    public void TearDown()
    {
        Object.Destroy(_go);
        for (int i = 0; i < _textures.Length; i++)
            Object.Destroy(_textures[i]);
        Object.Destroy(_tetrominoData);
    }

    [UnityTest]
    public IEnumerator Start_Creates16PreviewCells()
    {
        yield return null;

        int count = 0;
        foreach (Transform child in _go.transform)
            if (child.name.StartsWith("PreviewCell_"))
                count++;

        Assert.AreEqual(16, count);
    }

    [UnityTest]
    public IEnumerator PreviewCells_InitiallyHidden()
    {
        yield return null;

        foreach (Transform child in _go.transform)
            if (child.name.StartsWith("PreviewCell_"))
                Assert.IsFalse(child.GetComponent<SpriteRenderer>().enabled, $"{child.name} should start hidden");
    }

    [UnityTest]
    public IEnumerator ShowNextPiece_Enables4Cells()
    {
        yield return null;

        var piece = MakePiece(1, new[] { new Vector2Int(0, 1), new Vector2Int(1, 0), new Vector2Int(1, 1), new Vector2Int(2, 1) });
        _preview.ShowNextPiece(piece);

        int enabled = 0;
        foreach (Transform child in _go.transform)
            if (child.name.StartsWith("PreviewCell_") && child.GetComponent<SpriteRenderer>().enabled)
                enabled++;

        Assert.AreEqual(4, enabled);
    }

    [UnityTest]
    public IEnumerator ShowNextPiece_DisablesNonPieceCells()
    {
        yield return null;

        var piece = MakePiece(1, new[] { new Vector2Int(0, 1), new Vector2Int(1, 0), new Vector2Int(1, 1), new Vector2Int(2, 1) });
        _preview.ShowNextPiece(piece);

        int disabled = 0;
        foreach (Transform child in _go.transform)
            if (child.name.StartsWith("PreviewCell_") && !child.GetComponent<SpriteRenderer>().enabled)
                disabled++;

        Assert.AreEqual(12, disabled);
    }

    [UnityTest]
    public IEnumerator ShowNextPiece_AssignsCorrectSprite()
    {
        yield return null;

        int atlasIndex = 3;
        var piece = MakePiece(atlasIndex, new[] { new Vector2Int(0, 0), new Vector2Int(1, 0), new Vector2Int(2, 0), new Vector2Int(3, 0) });
        _preview.ShowNextPiece(piece);

        var cell00 = _go.transform.Find("PreviewCell_0_0").GetComponent<SpriteRenderer>();
        Assert.AreEqual(_tetrominoData.cellSprites[atlasIndex], cell00.sprite);
    }

    [UnityTest]
    public IEnumerator ShowNextPiece_EnablesCorrectNamedCells()
    {
        yield return null;

        // I-piece: horizontal row at y=1
        var piece = MakePiece(1, new[] { new Vector2Int(0, 1), new Vector2Int(1, 1), new Vector2Int(2, 1), new Vector2Int(3, 1) });
        _preview.ShowNextPiece(piece);

        Assert.IsTrue(_go.transform.Find("PreviewCell_0_1").GetComponent<SpriteRenderer>().enabled);
        Assert.IsTrue(_go.transform.Find("PreviewCell_1_1").GetComponent<SpriteRenderer>().enabled);
        Assert.IsTrue(_go.transform.Find("PreviewCell_2_1").GetComponent<SpriteRenderer>().enabled);
        Assert.IsTrue(_go.transform.Find("PreviewCell_3_1").GetComponent<SpriteRenderer>().enabled);
        Assert.IsFalse(_go.transform.Find("PreviewCell_0_0").GetComponent<SpriteRenderer>().enabled);
    }

    [UnityTest]
    public IEnumerator ShowNextPiece_PositionsCellsAtGridCoordinates()
    {
        yield return null;

        var piece = MakePiece(1, new[] { new Vector2Int(0, 0), new Vector2Int(1, 0), new Vector2Int(2, 0), new Vector2Int(3, 0) });
        _preview.ShowNextPiece(piece);

        // CellLocalPosition(col, row) = (col - 1.5, 1.5 - row)
        Assert.AreEqual(new Vector3(-1.5f, 1.5f, 0f), _go.transform.Find("PreviewCell_0_0").localPosition);
        Assert.AreEqual(new Vector3(1.5f, -1.5f, 0f), _go.transform.Find("PreviewCell_3_3").localPosition);
    }

    [UnityTest]
    public IEnumerator ShowNextPiece_UpdatesOnSecondCall()
    {
        yield return null;

        var pieceA = MakePiece(1, new[] { new Vector2Int(0, 0), new Vector2Int(1, 0), new Vector2Int(2, 0), new Vector2Int(3, 0) });
        var pieceB = MakePiece(2, new[] { new Vector2Int(1, 0), new Vector2Int(2, 0), new Vector2Int(1, 1), new Vector2Int(2, 1) });

        _preview.ShowNextPiece(pieceA);
        _preview.ShowNextPiece(pieceB);

        // Only O-piece cells should be enabled after second call
        Assert.IsTrue(_go.transform.Find("PreviewCell_1_0").GetComponent<SpriteRenderer>().enabled);
        Assert.IsTrue(_go.transform.Find("PreviewCell_2_0").GetComponent<SpriteRenderer>().enabled);
        Assert.IsTrue(_go.transform.Find("PreviewCell_1_1").GetComponent<SpriteRenderer>().enabled);
        Assert.IsTrue(_go.transform.Find("PreviewCell_2_1").GetComponent<SpriteRenderer>().enabled);
        // Cells from first call that are not in second piece should be disabled
        Assert.IsFalse(_go.transform.Find("PreviewCell_0_0").GetComponent<SpriteRenderer>().enabled);
        Assert.IsFalse(_go.transform.Find("PreviewCell_3_0").GetComponent<SpriteRenderer>().enabled);
    }
}
