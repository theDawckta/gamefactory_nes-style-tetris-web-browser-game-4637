using System;
using UnityEngine;

[Serializable]
public class RotationState
{
    public Vector2Int[] cells;
}

[Serializable]
public class PieceDefinition
{
    public string pieceName;
    public int atlasIndex;
    public RotationState[] rotations;
}

[CreateAssetMenu(fileName = "TetrominoData", menuName = "Tetris/TetrominoData")]
public class TetrominoData : ScriptableObject
{
    public PieceDefinition[] pieces;
    public Sprite[] cellSprites;
}
