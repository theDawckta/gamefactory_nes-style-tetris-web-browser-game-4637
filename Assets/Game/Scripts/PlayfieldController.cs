using UnityEngine;

public class PlayfieldController : MonoBehaviour
{
    private const int Cols = 10;
    private const int Rows = 20;

    private int[,] _grid = new int[Cols, Rows];

    public bool IsOccupied(int col, int row)
    {
        return IsInBounds(col, row) && _grid[col, row] != 0;
    }

    public bool IsInBounds(int col, int row)
    {
        return col >= 0 && col < Cols && row >= 0 && row < Rows;
    }

    public void LockPiece(Vector2Int[] cells, int colorIndex)
    {
        foreach (var cell in cells)
        {
            if (IsInBounds(cell.x, cell.y))
                _grid[cell.x, cell.y] = colorIndex;
        }
    }

    public int ClearLines()
    {
        int cleared = 0;
        for (int row = Rows - 1; row >= 0; row--)
        {
            if (IsRowFull(row))
            {
                ShiftRowsDown(row);
                row++;
                cleared++;
            }
        }
        return cleared;
    }

    public void GetBoardState(out int[,] state)
    {
        state = (int[,])_grid.Clone();
    }

    public void Reset()
    {
        _grid = new int[Cols, Rows];
    }

    public bool IsTopRowOccupied()
    {
        for (int col = 0; col < Cols; col++)
        {
            if (_grid[col, 0] != 0)
                return true;
        }
        return false;
    }

    private bool IsRowFull(int row)
    {
        for (int col = 0; col < Cols; col++)
        {
            if (_grid[col, row] == 0)
                return false;
        }
        return true;
    }

    private void ShiftRowsDown(int clearedRow)
    {
        for (int row = clearedRow; row > 0; row--)
        {
            for (int col = 0; col < Cols; col++)
                _grid[col, row] = _grid[col, row - 1];
        }
        for (int col = 0; col < Cols; col++)
            _grid[col, 0] = 0;
    }
}
