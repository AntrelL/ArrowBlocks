using System;

namespace IJunior.ArrowBlocks.Main
{
    public class LevelData : IReadOnlyLevelData
    {
        private float _recordTime = float.MaxValue;

        public LevelData() : this(LevelState.Blocked, float.MaxValue) { }

        public LevelData(CleanLevelData cleanLevelData) : this(cleanLevelData.State, cleanLevelData.RecordTime) { }

        public LevelData(LevelState levelState, float recordTime)
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

        public void Open(bool ignoreException = false)
        {
            if (State != LevelState.Blocked && ignoreException == false)
                throw new Exception("Level is not blocked.");

            State = LevelState.Opened;
        }

        public void Pass(float time)
        {
            State = LevelState.Passed;
            RecordTime = Math.Min(time, RecordTime);
        }

        public CleanLevelData ConvertToCleanData()
        {
            CleanLevelData cleanLevelData = new()
            {
                State = State,
                RecordTime = RecordTime
            };

            return cleanLevelData;
        }
    }
}