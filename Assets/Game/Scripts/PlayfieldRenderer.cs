using UnityEngine;

public class PlayfieldRenderer : MonoBehaviour
{
    private const int Cols = 10;
    private const int Rows = 20;

    [SerializeField] private TetrominoData _tetrominoData;

    private SpriteRenderer[,] _boardCells;
    private SpriteRenderer[] _pieceCells;

    private void Start()
    {
        CreateBoardCells();
        CreatePieceCells();
    }

    private void CreateBoardCells()
    {
        _boardCells = new SpriteRenderer[Cols, Rows];
        Sprite emptySprite = GetSprite(0);
        for (int col = 0; col < Cols; col++)
        {
            for (int row = 0; row < Rows; row++)
            {
                var go = new GameObject($"Cell_{col}_{row}");
                go.transform.SetParent(transform, false);
                go.transform.localPosition = CellLocalPosition(col, row);
                var sr = go.AddComponent<SpriteRenderer>();
                sr.sprite = emptySprite;
                _boardCells[col, row] = sr;
            }
        }
    }

    private void CreatePieceCells()
    {
        _pieceCells = new SpriteRenderer[4];
        for (int i = 0; i < 4; i++)
        {
            var go = new GameObject($"PieceCell_{i}");
            go.transform.SetParent(transform, false);
            var sr = go.AddComponent<SpriteRenderer>();
            sr.sortingOrder = 1;
            sr.enabled = false;
            _pieceCells[i] = sr;
        }
    }

    public void RefreshBoard(int[,] boardState)
    {
        if (_boardCells == null) return;
        for (int col = 0; col < Cols; col++)
        {
            for (int row = 0; row < Rows; row++)
            {
                _boardCells[col, row].sprite = GetSprite(boardState[col, row]);
            }
        }
    }

    public void SetActivePiece(Vector2Int[] positions, int colorIndex)
    {
        if (_pieceCells == null) return;
        Sprite sprite = GetSprite(colorIndex);
        for (int i = 0; i < _pieceCells.Length; i++)
        {
            if (i < positions.Length)
            {
                _pieceCells[i].enabled = true;
                _pieceCells[i].transform.localPosition = CellLocalPosition(positions[i].x, positions[i].y);
                _pieceCells[i].sprite = sprite;
            }
            else
            {
                _pieceCells[i].enabled = false;
            }
        }
    }

    public void ClearActivePiece()
    {
        if (_pieceCells == null) return;
        foreach (var cell in _pieceCells)
            cell.enabled = false;
    }

    private static Vector3 CellLocalPosition(int col, int row)
    {
        // Board centered at origin; col 0 = left, row 0 = top
        return new Vector3(col - (Cols - 1) * 0.5f, (Rows - 1) * 0.5f - row, 0f);
    }

    private Sprite GetSprite(int colorIndex)
    {
        if (_tetrominoData == null || _tetrominoData.cellSprites == null || _tetrominoData.cellSprites.Length == 0)
            return null;
        if (colorIndex < 0 || colorIndex >= _tetrominoData.cellSprites.Length)
            return _tetrominoData.cellSprites[0];
        return _tetrominoData.cellSprites[colorIndex];
    }
}
