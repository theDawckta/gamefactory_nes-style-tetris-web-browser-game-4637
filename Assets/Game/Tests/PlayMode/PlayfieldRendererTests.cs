using System.Collections;
using System.Reflection;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class PlayfieldRendererTests
{
    private GameObject _go;
    private PlayfieldRenderer _renderer;
    private TetrominoData _tetrominoData;
    private Texture2D[] _textures;

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

        _go = new GameObject("PlayfieldRenderer");
        _renderer = _go.AddComponent<PlayfieldRenderer>();

        // Set SerializeField before Start() runs
        typeof(PlayfieldRenderer)
            .GetField("_tetrominoData", BindingFlags.NonPublic | BindingFlags.Instance)
            .SetValue(_renderer, _tetrominoData);
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
    public IEnumerator Start_Creates200BoardCells()
    {
        yield return null;

        int count = 0;
        foreach (Transform child in _go.transform)
            if (child.name.StartsWith("Cell_"))
                count++;

        Assert.AreEqual(200, count);
    }

    [UnityTest]
    public IEnumerator Start_Creates4PieceCells()
    {
        yield return null;

        int count = 0;
        foreach (Transform child in _go.transform)
            if (child.name.StartsWith("PieceCell_"))
                count++;

        Assert.AreEqual(4, count);
    }

    [UnityTest]
    public IEnumerator BoardCells_ArePositionedCorrectly()
    {
        yield return null;

        Transform cell00 = _go.transform.Find("Cell_0_0");
        Transform cell919 = _go.transform.Find("Cell_9_19");

        Assert.IsNotNull(cell00, "Cell_0_0 should exist");
        Assert.IsNotNull(cell919, "Cell_9_19 should exist");
        Assert.AreEqual(new Vector3(-4.5f, 9.5f, 0f), cell00.localPosition, "Cell_0_0 position");
        Assert.AreEqual(new Vector3(4.5f, -9.5f, 0f), cell919.localPosition, "Cell_9_19 position");
    }

    [UnityTest]
    public IEnumerator BoardCells_InitiallyShowEmptySprite()
    {
        yield return null;

        var sr = _go.transform.Find("Cell_0_0").GetComponent<SpriteRenderer>();
        Assert.AreEqual(_tetrominoData.cellSprites[0], sr.sprite, "Cell should start with empty sprite");
    }

    [UnityTest]
    public IEnumerator RefreshBoard_EmptyCellShowsZeroIndexSprite()
    {
        yield return null;
        var boardState = new int[10, 20];
        _renderer.RefreshBoard(boardState);

        var sr = _go.transform.Find("Cell_5_10").GetComponent<SpriteRenderer>();
        Assert.AreEqual(_tetrominoData.cellSprites[0], sr.sprite, "Zero cell should use empty sprite");
    }

    [UnityTest]
    public IEnumerator RefreshBoard_OccupiedCellShowsColorSprite()
    {
        yield return null;
        var boardState = new int[10, 20];
        boardState[3, 5] = 2;
        _renderer.RefreshBoard(boardState);

        var sr = _go.transform.Find("Cell_3_5").GetComponent<SpriteRenderer>();
        Assert.AreEqual(_tetrominoData.cellSprites[2], sr.sprite, "Occupied cell should show color sprite at index 2");
    }

    [UnityTest]
    public IEnumerator SetActivePiece_EnablesAllFourCells()
    {
        yield return null;
        var positions = new[]
        {
            new Vector2Int(3, 0), new Vector2Int(4, 0),
            new Vector2Int(5, 0), new Vector2Int(4, 1)
        };
        _renderer.SetActivePiece(positions, 1);

        int enabled = 0;
        foreach (Transform child in _go.transform)
            if (child.name.StartsWith("PieceCell_") && child.GetComponent<SpriteRenderer>().enabled)
                enabled++;

        Assert.AreEqual(4, enabled);
    }

    [UnityTest]
    public IEnumerator SetActivePiece_AssignsCorrectColorSprite()
    {
        yield return null;
        var positions = new[]
        {
            new Vector2Int(0, 0), new Vector2Int(1, 0),
            new Vector2Int(2, 0), new Vector2Int(3, 0)
        };
        _renderer.SetActivePiece(positions, 3);

        var sr = _go.transform.Find("PieceCell_0").GetComponent<SpriteRenderer>();
        Assert.AreEqual(_tetrominoData.cellSprites[3], sr.sprite);
    }

    [UnityTest]
    public IEnumerator SetActivePiece_PositionsCellsCorrectly()
    {
        yield return null;
        var positions = new[]
        {
            new Vector2Int(0, 0), new Vector2Int(1, 0),
            new Vector2Int(2, 0), new Vector2Int(3, 0)
        };
        _renderer.SetActivePiece(positions, 1);

        // col=0, row=0 maps to (-4.5, 9.5)
        Transform pieceCell0 = _go.transform.Find("PieceCell_0");
        Assert.AreEqual(new Vector3(-4.5f, 9.5f, 0f), pieceCell0.localPosition);
    }

    [UnityTest]
    public IEnumerator PieceCells_InitiallyHidden()
    {
        yield return null;

        foreach (Transform child in _go.transform)
            if (child.name.StartsWith("PieceCell_"))
                Assert.IsFalse(child.GetComponent<SpriteRenderer>().enabled, $"{child.name} should start hidden");
    }

    [UnityTest]
    public IEnumerator ClearActivePiece_HidesAllPieceCells()
    {
        yield return null;
        var positions = new[]
        {
            new Vector2Int(0, 0), new Vector2Int(1, 0),
            new Vector2Int(2, 0), new Vector2Int(3, 0)
        };
        _renderer.SetActivePiece(positions, 1);
        _renderer.ClearActivePiece();

        foreach (Transform child in _go.transform)
            if (child.name.StartsWith("PieceCell_"))
                Assert.IsFalse(child.GetComponent<SpriteRenderer>().enabled, $"{child.name} should be hidden after ClearActivePiece");
    }
}
