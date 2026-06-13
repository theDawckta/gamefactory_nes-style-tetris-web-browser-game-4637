using System.Collections.Generic;
using UnityEngine;

namespace Tetris.UI
{
    public class NextPiecePreview : MonoBehaviour
    {
        private const int GridSize = 4;

        [SerializeField] private TetrominoData _tetrominoData;

        private SpriteRenderer[,] _cells;

        private void Start()
        {
            CreatePreviewCells();
        }

        private void CreatePreviewCells()
        {
            _cells = new SpriteRenderer[GridSize, GridSize];
            for (int col = 0; col < GridSize; col++)
            {
                for (int row = 0; row < GridSize; row++)
                {
                    var go = new GameObject($"PreviewCell_{col}_{row}");
                    go.transform.SetParent(transform, false);
                    go.transform.localPosition = CellLocalPosition(col, row);
                    var sr = go.AddComponent<SpriteRenderer>();
                    sr.sortingOrder = 1;
                    sr.enabled = false;
                    _cells[col, row] = sr;
                }
            }
        }

        public void ShowNextPiece(PieceDefinition piece)
        {
            if (_cells == null) return;

            var activeCells = new HashSet<Vector2Int>();
            foreach (var cell in piece.rotations[0].cells)
                activeCells.Add(cell);

            Sprite sprite = GetSprite(piece.atlasIndex);

            for (int col = 0; col < GridSize; col++)
            {
                for (int row = 0; row < GridSize; row++)
                {
                    bool active = activeCells.Contains(new Vector2Int(col, row));
                    _cells[col, row].enabled = active;
                    if (active)
                        _cells[col, row].sprite = sprite;
                }
            }
        }

        private static Vector3 CellLocalPosition(int col, int row)
        {
            return new Vector3(col - (GridSize - 1) * 0.5f, (GridSize - 1) * 0.5f - row, 0f);
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
}
