using System;

public class Level : IReadOnlyLevel
{
    private float _recordTime = float.MaxValue;

    public Level(LevelState levelState, float recordTime)
    {
        State = levelState;
        RecordTime = recordTime;
    }

    public LevelState State { get; private set; }
    public float RecordTime
    {
        get => _recordTime;
        private set
        {
            if (value < 0)
                throw new Exception("Record time cannot be less than zero.");

            if (_recordTime < value)
                throw new Exception("New record time cannot be greater than previous.");

            _recordTime = value;
        }
    }

    public void Pass(float time)
    {
        State = LevelState.Passed;
        RecordTime = Math.Min(time, RecordTime);
    }

    public void Open()
    {
        if (State != LevelState.Blocked)
            throw new Exception("Level is not blocked.");

        State = LevelState.Opened;
    }
}