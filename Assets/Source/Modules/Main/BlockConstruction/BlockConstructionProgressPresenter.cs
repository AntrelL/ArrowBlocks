using System;
using IJunior.CompositeRoot;
using IJunior.UI;

namespace IJunior.ArrowBlocks.Main
{
    public class BlockConstructionProgressPresenter : IProgressBarInfo, IActivatable
    {
        private readonly BlockConstruction BlockConstruction;

        public BlockConstructionProgressPresenter(BlockConstruction blockConstruction)
        {
            BlockConstruction = blockConstruction;
        }

        public event Action<float> ValueChanged;

        public void OnActivate()
        {
            BlockConstruction.ReleasedBlocksQuantityChanged += OnReleasedBlocksQuantityChanged;
        }

        public void OnDeactivate()
        {
            BlockConstruction.ReleasedBlocksQuantityChanged -= OnReleasedBlocksQuantityChanged;
        }

        private void OnReleasedBlocksQuantityChanged(int total)
        {
            float ratio = CalculateRatio(total, BlockConstruction.BlocksQuantity);
            ValueChanged?.Invoke(ratio);
        }

        private float CalculateRatio(float value, float maxValue) => value / maxValue;
    }
}