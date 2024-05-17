using System;
using System.Collections.Generic;
using System.Linq;
using IJunior.CompositeRoot;
using UnityEngine;

namespace IJunior.ArrowBlocks.Main
{
    public class BlockConstruction : Script, IPlayerCameraTarget, IActivatable, IStartable
    {
        private List<ArrowBlock> _blocks;
        private List<ArrowBlock> _unreleasedBlocks;
        private bool _isFirstBlockActivated;

        public event Action<float> ThroughLineLengthChanged;

        public event Action FirstBlockActivated;

        public event Action AllBlocksReleased;

        public event Action<int> ReleasedBlocksQuantityChanged;

        public Vector3 CenterPointPosition { get; private set; }

        public BlockConstructionCalculations Calculations { get; private set; }

        public int BlocksQuantity => _blocks.Count;

        private int ReleasedBlocksQuantity => _blocks.Count - _unreleasedBlocks.Count;

        public void Initialize(BlockConstructionCalculations calculations, List<ArrowBlock> blocks)
        {
            Calculations = calculations;
            _blocks = blocks;

            ResetInternalValues();
        }

        public void OnActivate()
        {
            foreach (var block in _blocks)
            {
                block.Released += OnBlockReleased;
                block.Activated += OnBlockActivated;
                block.ChangedPosition += OnBlockChangedPosition;
            }
        }

        public void OnDeactivate()
        {
            foreach (var block in _blocks)
            {
                block.Released -= OnBlockReleased;
                block.Activated -= OnBlockActivated;
                block.ChangedPosition -= OnBlockChangedPosition;
            }
        }

        public void OnStart() => SetCenterPointPosition();

        public void ResetAllBlocks()
        {
            foreach (var block in _blocks)
            {
                block.gameObject.SetActive(true);
                block.ResetValues();
            }

            ResetInternalValues();
        }

        public void RemoveBlock(ArrowBlock block)
        {
            block.gameObject.SetActive(false);
        }

        public void OnBlockReleased(ArrowBlock arrowBlock)
        {
            _unreleasedBlocks.Remove(arrowBlock);
            ReleasedBlocksQuantityChanged?.Invoke(ReleasedBlocksQuantity);

            if (_unreleasedBlocks.Count == 0)
                AllBlocksReleased?.Invoke();

            SetCenterPointPosition();
        }

        public void OnBlockActivated()
        {
            if (_isFirstBlockActivated == false)
                FirstBlockActivated?.Invoke();

            _isFirstBlockActivated = true;
        }

        public void OnBlockChangedPosition()
        {
            SetCenterPointPosition();
        }

        private void ResetInternalValues()
        {
            _unreleasedBlocks = new List<ArrowBlock>(_blocks);
            _isFirstBlockActivated = false;

            SetCenterPointPosition();
            ReleasedBlocksQuantityChanged?.Invoke(ReleasedBlocksQuantity);
        }

        private void SetCenterPointPosition()
        {
            if (_unreleasedBlocks.Count == 0)
                return;

            Vector3[] positions = _unreleasedBlocks.Select(x => x.Transform.position).ToArray();
            var extremes = Calculations.GetPositionExtremes(positions);

            CenterPointPosition = Calculations.GetCenterPointPosition(extremes);

            float throughLineLength = Calculations.GetThroughLineLength(extremes);
            ThroughLineLengthChanged?.Invoke(throughLineLength);
        }
    }
}