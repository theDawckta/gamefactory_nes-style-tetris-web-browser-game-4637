using System;
using UnityEngine;

[RequireComponent(typeof(PlayfieldController))]
public class PieceController : MonoBehaviour
{
    private static readonly int[] GravityFrames =
    {
        48, 43, 38, 33, 28, 23, 18, 13, 8, 6,
        5, 5, 5, 4, 4, 4, 3, 3, 3,
        2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 1
    };

    private const int DasDelay = 16;
    private const int DasRepeat = 6;
    private const int LockDelay = 30;
    private const float FrameTime = 1f / 60f;

    private PlayfieldController _playfield;
    private PieceDefinition _piece;
    private int _rotationIndex;
    private Vector2Int _position;
    private int _gravityLevel;
    private float _accumulator;

    private int _gravityCounter;

    private bool _dasLeftHeld;
    private bool _dasRightHeld;
    private int _dasLeftCounter;
    private int _dasRightCounter;

    private bool _isGrounded;
    private int _lockCounter;

    private bool _inputLeft;
    private bool _inputRight;
    private bool _inputRotateLeft;
    private bool _inputRotateRight;
    private bool _inputSoftDrop;

    public event Action OnPieceLocked;
    public event Action OnTopOut;

    private void Awake()
    {
        _playfield = GetComponent<PlayfieldController>();
    }

    public void SpawnPiece(PieceDefinition piece)
    {
        _piece = piece;
        _rotationIndex = 0;
        _position = new Vector2Int(3, 0);
        _gravityCounter = 0;
        _isGrounded = false;
        _lockCounter = 0;
        _accumulator = 0f;
        _dasLeftHeld = false;
        _dasRightHeld = false;
        _dasLeftCounter = 0;
        _dasRightCounter = 0;

        if (IsColliding(_piece.rotations[_rotationIndex].cells, _position))
            OnTopOut?.Invoke();
    }

    public void SetLevel(int level)
    {
        _gravityLevel = Mathf.Clamp(level - 1, 0, GravityFrames.Length - 1);
    }

    public Vector2Int[] GetCurrentCellPositions()
    {
        if (_piece == null)
            return new Vector2Int[0];
        var cells = _piece.rotations[_rotationIndex].cells;
        var result = new Vector2Int[cells.Length];
        for (int i = 0; i < cells.Length; i++)
            result[i] = _position + cells[i];
        return result;
    }

    public int GetCurrentColorIndex()
    {
        return _piece != null ? _piece.atlasIndex : 0;
    }

    public void ProcessInput(bool moveLeft, bool moveRight, bool rotateLeft, bool rotateRight, bool softDrop)
    {
        _inputLeft = moveLeft;
        _inputRight = moveRight;
        _inputRotateLeft = rotateLeft;
        _inputRotateRight = rotateRight;
        _inputSoftDrop = softDrop;
    }

    private void Update()
    {
        if (_piece == null) return;
        _accumulator += Time.deltaTime;
        while (_accumulator >= FrameTime)
        {
            _accumulator -= FrameTime;
            ProcessFrameStep();
        }
    }

    public void ProcessFrameStep()
    {
        if (_piece == null) return;

        if (_inputRotateLeft)
        {
            TryRotate(-1);
            _inputRotateLeft = false;
        }
        if (_inputRotateRight)
        {
            TryRotate(1);
            _inputRotateRight = false;
        }

        ProcessDas(_inputLeft, ref _dasLeftHeld, ref _dasLeftCounter, -1);
        ProcessDas(_inputRight, ref _dasRightHeld, ref _dasRightCounter, 1);

        if (_inputSoftDrop)
        {
            _gravityCounter = 0;
            if (!TryMove(0, 1))
                SetGrounded();
            else
                _isGrounded = false;
        }
        else
        {
            _gravityCounter++;
            if (_gravityCounter >= GravityFrames[_gravityLevel])
            {
                _gravityCounter = 0;
                if (!TryMove(0, 1))
                    SetGrounded();
                else
                    _isGrounded = false;
            }
        }

        if (_isGrounded)
        {
            _lockCounter++;
            if (_lockCounter >= LockDelay)
                LockCurrentPiece();
        }
    }

    private void ProcessDas(bool held, ref bool dasHeld, ref int dasCounter, int dx)
    {
        if (held)
        {
            if (!dasHeld)
            {
                dasHeld = true;
                dasCounter = 0;
                TryMove(dx, 0);
            }
            else
            {
                dasCounter++;
                if (dasCounter >= DasDelay)
                {
                    int repeat = dasCounter - DasDelay;
                    if (repeat % DasRepeat == 0)
                        TryMove(dx, 0);
                }
            }
        }
        else
        {
            dasHeld = false;
            dasCounter = 0;
        }
    }

    private bool TryMove(int dx, int dy)
    {
        var newPos = _position + new Vector2Int(dx, dy);
        if (IsColliding(_piece.rotations[_rotationIndex].cells, newPos))
            return false;
        _position = newPos;
        if (_isGrounded && dy == 0)
            _lockCounter = 0;
        return true;
    }

    private void TryRotate(int direction)
    {
        int nextIndex = (_rotationIndex + direction + _piece.rotations.Length) % _piece.rotations.Length;
        var cells = _piece.rotations[nextIndex].cells;

        if (!IsColliding(cells, _position))
        {
            _rotationIndex = nextIndex;
            if (_isGrounded)
                _lockCounter = 0;
            return;
        }

        // Wall kick: nudge 1 column toward board center
        int nudge = _position.x < 5 ? 1 : -1;
        var nudgePos = _position + new Vector2Int(nudge, 0);
        if (!IsColliding(cells, nudgePos))
        {
            _position = nudgePos;
            _rotationIndex = nextIndex;
            if (_isGrounded)
                _lockCounter = 0;
        }
        // else rotation is cancelled
    }

    private void SetGrounded()
    {
        if (!_isGrounded)
        {
            _isGrounded = true;
            _lockCounter = 0;
        }
    }

    private bool IsColliding(Vector2Int[] cells, Vector2Int pos)
    {
        foreach (var cell in cells)
        {
            var world = pos + cell;
            if (!_playfield.IsInBounds(world.x, world.y) || _playfield.IsOccupied(world.x, world.y))
                return true;
        }
        return false;
    }

    private void LockCurrentPiece()
    {
        var positions = GetCurrentCellPositions();
        _playfield.LockPiece(positions, _piece.atlasIndex);
        _piece = null;
        OnPieceLocked?.Invoke();
    }
}
