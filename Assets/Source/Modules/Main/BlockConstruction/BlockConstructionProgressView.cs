using IJunior.CompositeRoot;
using IJunior.UI;
using System;

namespace IJunior.ArrowBlocks.Main
{
    public class BlockConstructionProgressView : IProgressBarInfo, IActivatable
    {
        private BlockConstruction _blockConstruction;

        public BlockConstructionProgressView(BlockConstruction blockConstruction)
        {
            _blockConstruction = blockConstruction;
        }

        public event Action<float> ValueChanged;

        public void OnActivate()
        {
            _blockConstruction.ReleasedBlocksQuantityChanged += OnReleasedBlocksQuantityChanged;
        }

        public void OnDeactivate()
        {
            _blockConstruction.ReleasedBlocksQuantityChanged -= OnReleasedBlocksQuantityChanged;
        }

        private void OnReleasedBlocksQuantityChanged(int total)
        {
            float ratio = CalculateRatio(total, _blockConstruction.BlocksQuantity);
            ValueChanged?.Invoke(ratio);
        }

        private float CalculateRatio(float value, float maxValue) => value / maxValue;
    }
}