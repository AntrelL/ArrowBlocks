namespace IJunior.ArrowBlocks.Main
{
    public interface IReadOnlyLevelData
    {
        public LevelState State { get; }

        public float RecordTime { get; }
    }
}