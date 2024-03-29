using System;
using System.Collections;
using UnityEngine;
using DG.Tweening;

public class ArrowBlock : MonoBehaviour
{
    [SerializeField] private float _movementSpeed;
    [SerializeField] private float _flightTimeIntoVoid;
    [SerializeField] private TrailRenderer _trail;
    [SerializeField] private float _minTrailDistance;
    [SerializeField] private float _pushAnimationDistance;

    private bool _isMoving = false;
    private bool _isAnimated = false;

    private Vector3 _basePosition;
    private Vector3 _targetPosition;
    private Transform _transform;
    private VirtualBlockGrid _virtualBlockGrid;
    private Coroutine _submitRemovalRequester;
    private ArrowBlock _obstructingBlock;

    public bool IsReleased { get; private set; }

    private void Awake()
    {
        _transform = transform;
        _basePosition = _targetPosition = _transform.position;
        _trail.enabled = false;

        IsReleased = false;
    }

    private void FixedUpdate()
    {
        if (_isMoving == false)
            return;

        _transform.position = Vector3.MoveTowards(_transform.position,
            _targetPosition, _movementSpeed * Time.fixedDeltaTime);

        if (_transform.position == _targetPosition)
        {
            if (_obstructingBlock != null && _isMoving == true)
            {
                _virtualBlockGrid.OnBlockTouchedOther();
                PerformChainPushAnimation(_transform.forward);
                _obstructingBlock = null;
            }

            _isMoving = false;
            _trail.enabled = false;
        }
    }

    public void Initialize(VirtualBlockGrid virtualBlockGrid)
    {
        _virtualBlockGrid = virtualBlockGrid;   
    }

    public void TryActivate()
    {
        if (_isMoving || _isAnimated || IsReleased)
            return;

        _virtualBlockGrid.OnBlockActivated();

        if (Physics.Raycast(_transform.position, _transform.forward, out RaycastHit hit) == false)
        {
            _targetPosition = _transform.position + _transform.forward * (_movementSpeed * _flightTimeIntoVoid);

            IsReleased = true;
            _virtualBlockGrid.OnBlockReleased();
            _submitRemovalRequester = StartCoroutine(SubmitRemovalRequest(_flightTimeIntoVoid));
        }
        else
        {
            if (hit.collider.gameObject.TryGetComponent(out ArrowBlock arrowBlock) == false)
                throw new Exception("Arrow block detected an unknown object.");

            _targetPosition = _virtualBlockGrid.GetCoordinatesOfNeighboringCell(
                arrowBlock.transform.position, _transform.forward);

            _obstructingBlock = arrowBlock;
        }

        if (Vector3.Distance(_transform.position, _targetPosition) >= _minTrailDistance)
            _trail.enabled = true;

        _isMoving = true;
    }

    public void PerformChainPushAnimation(Vector3 direction)
    {
        PlayPushAnimation(direction);

        RaycastHit[] hits = Physics.RaycastAll(_transform.position, direction);

        if (hits.Length == 0)
            return;

        foreach (var hit in hits)
        {
            if (hit.collider.gameObject.TryGetComponent(out ArrowBlock arrowBlock) == false)
                throw new Exception("Arrow block detected an unknown object.");

            arrowBlock.PlayPushAnimation(direction);
        }
    }

    public void ResetValues()
    {
        _transform.position = _targetPosition = _basePosition;
        _trail.Clear();

        IsReleased = false;

        if (_submitRemovalRequester != null)
            StopCoroutine(_submitRemovalRequester);
    }

    private IEnumerator SubmitRemovalRequest(float delay)
    {
        yield return new WaitForSeconds(delay);
        _virtualBlockGrid.RemoveBlock(this);
    }

    private void PlayPushAnimation(Vector3 direction)
    {
        int movementPartQuantity = 2;

        Vector3 basePosition = _transform.position;
        Vector3 shiftPosition = _transform.position + direction * _pushAnimationDistance;
        float time = _pushAnimationDistance * movementPartQuantity / _movementSpeed;

        _isAnimated = true;
        Invoke(nameof(DisableAnimationMode), time * movementPartQuantity);

        _transform.DOMove(shiftPosition, time);
        _transform.DOMove(basePosition, time).SetDelay(time);
    }

    private void DisableAnimationMode()
    {
        _isAnimated = false;
    }
}