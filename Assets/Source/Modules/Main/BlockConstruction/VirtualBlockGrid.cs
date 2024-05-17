using System;
using System.Collections.Generic;
using System.Linq;
using IJunior.CompositeRoot;
using UnityEngine;

namespace IJunior.ArrowBlocks.Main
{
    public class VirtualBlockGrid : Script
    {
        [HideInInspector][SerializeField] private Vector3 _cellSize;
        [HideInInspector][SerializeField] private List<ArrowBlock> _blocks;
        [HideInInspector][SerializeField] private List<ArrowBlock> _blocksOffGrid;

        public Vector3 CellSize => _cellSize;

        public void Initialize()
        {
            SetBlocksFromChildElements();
            SetBlocksOffGrid();

            CheckForMatchedBlocks();

            if (_blocksOffGrid.Count > 0)
                throw new Exception("There are blocks outside the grid.");
        }

        public List<ArrowBlock> TakeBlocks()
        {
            List<ArrowBlock> blocks = _blocks;
            _blocks = _blocksOffGrid = null;

            return blocks;
        }

        public void SetBlocksFromChildElements()
        {
            _blocks = GetComponentsInChildren<ArrowBlock>().ToList();
        }

        public void SetBlocksOffGrid()
        {
            _blocksOffGrid.Clear();

            foreach (var block in _blocks)
            {
                Vector3 position = block.transform.position;

                if (position.DivideByModulus(_cellSize) != Vector3.zero)
                    _blocksOffGrid.Add(block);
            }
        }

        public void CheckForMatchedBlocks()
        {
            for (int i = 0; i < _blocks.Count; i++)
            {
                for (int j = 0; j < i; j++)
                {
                    if (_blocks[i].transform.position == _blocks[j].transform.position)
                        throw new Exception($"{_blocks[i]} matched with {_blocks[j]}");
                }
            }
        }

        public void FixAllBlocksOffGrid()
        {
            foreach (var block in _blocksOffGrid)
            {
                FixBlockPosition(block.transform);
            }

            _blocksOffGrid.Clear();
        }

        private void FixBlockPosition(Transform block)
        {
            block.position = block.position.PerformFunctionForCoordinates(_cellSize, GetFixedBlockCoordinate);
        }

        private float GetFixedBlockCoordinate(float value, float step)
        {
            return (float)(Math.Round(value / step) * step);
        }
    }
}