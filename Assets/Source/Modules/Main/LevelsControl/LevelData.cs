using System;

namespace IJunior.ArrowBlocks.Main
{
    public class LevelData : IReadOnlyLevelData
    {
        public LevelData()
            : this(LevelState.Blocked, float.MaxValue)
        {
        }

        public LevelData(CleanLevelData cleanLevelData)
            : this(cleanLevelData.State, cleanLevelData.RecordTime)
        {
        }

        public LevelData(LevelState levelState, float recordTime)
        {
            State = levelState;
            SetRecordTime(recordTime);
        }

        public LevelState State { get; private set; }

        public float RecordTime { get; private set; } = float.MaxValue;

        public void Open(bool ignoreException = false)
        {
            if (State != LevelState.Blocked && ignoreException == false)
                throw new Exception("Level is not blocked.");

            State = LevelState.Opened;
        }

        public void Pass(float time)
        {
            State = LevelState.Passed;
            SetRecordTime(Math.Min(time, RecordTime));
        }

        public CleanLevelData ConvertToCleanData()
        {
            var cleanLevelData = new CleanLevelData()
            {
                State = State,
                RecordTime = RecordTime,
            };

            return cleanLevelData;
        }

        private void SetRecordTime(float value)
        {
            if (value < 0)
                throw new Exception("Record time cannot be less than zero.");

            if (RecordTime < value)
                throw new Exception("New record time cannot be greater than previous.");

            RecordTime = value;
        }
    }
}