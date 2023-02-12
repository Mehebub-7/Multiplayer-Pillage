using System;

public class Player
{
    public delegate void PositionEvent(Position position);
    public event PositionEvent OnPositionChanged;

    public Position position
    {
        get
        {
            return _position;
        }
        set
        {
            if (_position != value)
            {
                _position = value;
                if (OnPositionChanged != null)
                {
                    OnPositionChanged(value);
                }
            }
        }
    }
    private Position _position;
}
