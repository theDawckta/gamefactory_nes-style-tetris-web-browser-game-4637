using System;

public class PieceBag
{
    private readonly TetrominoData _data;
    private readonly Random _random;
    private int _previousIndex;
    private int _nextIndex;

    public PieceBag(TetrominoData data)
    {
        _data = data;
        _random = new Random();
        _previousIndex = -1;
        _nextIndex = DrawIndex();
    }

    public PieceDefinition Next()
    {
        var piece = _data.pieces[_nextIndex];
        _previousIndex = _nextIndex;
        _nextIndex = DrawIndex();
        return piece;
    }

    public PieceDefinition Peek()
    {
        return _data.pieces[_nextIndex];
    }

    private int DrawIndex()
    {
        int count = _data.pieces.Length;
        // Roll 0 to count inclusive (count is the re-roll sentinel slot)
        int result = _random.Next(0, count + 1);
        // Re-roll once if result matches previous piece or hits the sentinel slot
        if (result == _previousIndex || result == count)
            result = _random.Next(0, count + 1);
        // Wrap into valid range if sentinel still came up after re-roll
        if (result >= count)
            result %= count;
        return result;
    }
}
