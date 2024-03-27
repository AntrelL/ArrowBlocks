using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine;

public class VirtualBlockGrid : MonoBehaviour, IProgressBarInfo
{
    [SerializeField] private Vector3 _cellSize;
    [SerializeField] private ProgressBar _progressBar;

    [HideInInspector] [SerializeField] private List<ArrowBlock> _blocks;
    [HideInInspector] [SerializeField] private List<ArrowBlock> _blocksOffGrid;

    private int _releasedBlocks;

    public event Action<float> ValueChanged;
    public event Action AllBlocksReleased;

    private void Awake()
    {
        SetBlocksFromChild();
        SetBlocksOffGrid();
    }

    private void OnEnable()
    {
        _progressBar.Connect(this);
    }

    private void OnDisable()
    {
        _progressBar.Disconnect();
    }

    private void Start()
    {
        if (_blocksOffGrid.Count > 0)
            throw new Exception("There are blocks outside the grid.");

        InitializeAllBlocks();
        UpdateProgressBar(_releasedBlocks);
    }

    public void InitializeAllBlocks()
    {
        foreach (var block in _blocks)
        {
            block.Initialize(this);
        }
    }

    public void ResetAllBlocks()
    {
        foreach (var block in _blocks)
        {
            block.gameObject.SetActive(true);
            block.ResetValues();
        }

        _releasedBlocks = 0;
        UpdateProgressBar(_releasedBlocks);
    }

    public void OnBlockReleased()
    {
        UpdateProgressBar(++_releasedBlocks);

        if (_releasedBlocks == _blocks.Count)
            AllBlocksReleased?.Invoke();
    }

    public void RemoveBlock(ArrowBlock block)
    {
        block.gameObject.SetActive(false);
    }

    public Vector3 GetCoordinatesOfNeighboringCell(Vector3 cellPosition, Vector3 directionToCell)
    {
        return cellPosition + Vector3.Scale(-directionToCell, _cellSize);
    }

    public void SetBlocksFromChild()
    {
        _blocks = GetComponentsInChildren<ArrowBlock>().ToList();
        _releasedBlocks = 0;
    }

    public void SetBlocksOffGrid()
    {
        _blocksOffGrid.Clear();

        foreach (var block in _blocks)
        {
            Vector3 position = block.transform.position;

            if (position.x % _cellSize.x != 0 ||
                position.y % _cellSize.y != 0 ||
                position.z % _cellSize.z != 0)
            {
                _blocksOffGrid.Add(block);
            }
        }
    }

    public void FixAllBlocksOffGrid()
    {
        foreach(var block in _blocksOffGrid)
        {
            FixBlockPosition(block.transform);
        }
    }

    private void FixBlockPosition(Transform block)
    {
        block.position = new Vector3(
            FixBlockCoordinate(block.position.x, _cellSize.x), 
            FixBlockCoordinate(block.position.y, _cellSize.y), 
            FixBlockCoordinate(block.position.z, _cellSize.z));
    }

    private float FixBlockCoordinate(float value, float step)
    {
        return (float)(Math.Round(value / step) * step);
    }

    private float CalculateRatio(float value, float maxValue)
    {
        return value / maxValue;
    }

    private void UpdateProgressBar(float value)
    {
        ValueChanged?.Invoke(CalculateRatio(value, _blocks.Count));
    }
}