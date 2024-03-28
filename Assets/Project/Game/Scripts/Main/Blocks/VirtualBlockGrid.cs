using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine;

public class VirtualBlockGrid : MonoBehaviour, IProgressBarInfo
{
    [SerializeField] private Vector3 _cellSize;
    [SerializeField] private ProgressBar _progressBar;
    [SerializeField] private Transform _centerPoint;
    [SerializeField] private PlayerCamera _playerCamera;

    [HideInInspector] [SerializeField] private List<ArrowBlock> _blocks;
    [HideInInspector] [SerializeField] private List<ArrowBlock> _blocksOffGrid;

    private int _releasedBlocks;

    public event Action<float> ValueChanged;
    public event Action AllBlocksReleased;

    private void Awake()
    {
        SetBlocksFromChild();
        SetBlocksOffGrid();
        SetCenterPointPosition();
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
        SetCenterPointPosition();
    }

    public void OnBlockReleased()
    {
        UpdateProgressBar(++_releasedBlocks);

        if (_releasedBlocks == _blocks.Count)
            AllBlocksReleased?.Invoke();

        SetCenterPointPosition();
    }

    public void OnBlockTouchedOther()
    {
        SetCenterPointPosition();
    }

    public void RemoveBlock(ArrowBlock block)
    {
        block.gameObject.SetActive(false);
    }

    public Vector3 GetCoordinatesOfNeighboringCell(Vector3 cellPosition, Vector3 directionToCell)
    {
        return cellPosition + Vector3.Scale(-directionToCell, _cellSize);
    }

    public void SetCenterPointPosition()
    {
        if (_blocks.Count - _releasedBlocks == 0)
            return;

        Vector3 maxPositionValues = _centerPoint.position;
        Vector3 minPositionValues = maxPositionValues;
        int accountedBlocksQuantity = 0;

        foreach (var block in _blocks)
        {
            if (block.IsReleased)
                continue;

            if (accountedBlocksQuantity++ == 0)
            {
                maxPositionValues = minPositionValues = block.transform.position;
            }
            else
            {
                maxPositionValues = Vector3.Max(block.transform.position, maxPositionValues);
                minPositionValues = Vector3.Min(block.transform.position, minPositionValues);
            }
        }

        if (accountedBlocksQuantity == 1)
            _centerPoint.position = maxPositionValues;
        else
            _centerPoint.position = (maxPositionValues + minPositionValues) / 2f;

        _playerCamera.OffsetDistance = Vector3.Distance(maxPositionValues, minPositionValues);
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