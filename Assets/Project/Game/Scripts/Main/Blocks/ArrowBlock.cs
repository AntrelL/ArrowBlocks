using System;
using System.Collections;
using UnityEngine;

public class ArrowBlock : MonoBehaviour
{
    [SerializeField] private float _movementSpeed;
    [SerializeField] private float _flightTimeIntoVoid;
    [SerializeField] private TrailRenderer _trail;
    [SerializeField] private float _minTrailDistance;

    private bool _isMoving = false;

    private Vector3 _basePosition;
    private Vector3 _targetPosition;
    private Transform _transform;
    private VirtualBlockGrid _virtualBlockGrid;
    private Coroutine _submitRemovalRequester;

    private void Start()
    {
        _transform = transform;
        _basePosition = _targetPosition = _transform.position;
        _trail.enabled = false;
    }

    private void FixedUpdate()
    {
        if (_isMoving == false)
            return;

        _transform.position = Vector3.MoveTowards(_transform.position,
            _targetPosition, _movementSpeed * Time.fixedDeltaTime);

        if (_transform.position == _targetPosition)
        {
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
        if (_isMoving)
            return;

        if (Physics.Raycast(_transform.position, _transform.forward, out RaycastHit hit) == false)
        {
            _targetPosition = _transform.position + _transform.forward * (_movementSpeed * _flightTimeIntoVoid);

            _virtualBlockGrid.OnBlockReleased();
            _submitRemovalRequester = StartCoroutine(SubmitRemovalRequest(_flightTimeIntoVoid));
        }
        else
        {
            if (hit.collider.gameObject.TryGetComponent(out ArrowBlock arrowBlock) == false)
                throw new Exception("Arrow block detected an unknown object.");

            _targetPosition = _virtualBlockGrid.GetCoordinatesOfNeighboringCell(
                arrowBlock.transform.position, _transform.forward);
        }

        if (Vector3.Distance(_transform.position, _targetPosition) >= _minTrailDistance)
            _trail.enabled = true;

        _isMoving = true;
    }

    public void ResetValues()
    {
        _transform.position = _targetPosition = _basePosition;
        _trail.Clear();

        if (_submitRemovalRequester != null)
            StopCoroutine(_submitRemovalRequester);
    }

    private IEnumerator SubmitRemovalRequest(float delay)
    {
        yield return new WaitForSeconds(delay);
        _virtualBlockGrid.RemoveBlock(this);
    }
}