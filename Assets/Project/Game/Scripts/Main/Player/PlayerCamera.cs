using UnityEngine.InputSystem;
using UnityEngine;
using System;

public class PlayerCamera : MonoBehaviour
{
    [SerializeField][Range(0, 100)] private float _sensitivity;
    [SerializeField][Range(0, 90)] private float _limitVerticalAngle;

    [SerializeField][Range(0, 100)] private float _offsetSpeed;
    [SerializeField] private Vector3 _offset;
    [SerializeField] private Transform _target;

    private Transform _transform;
    private PlayerInput _input;
    private float _currentVerticalAngle;
    private float _offsetDistance;
    private Vector3 _targetPositionWithOffset;

    private void Start()
    {
        _transform = transform;
        _targetPositionWithOffset = _target.position + _offset;

        Vector3 horizontalPosition = _transform.position;
        horizontalPosition.y = _targetPositionWithOffset.y;

        Vector3 horizontalDirection = (horizontalPosition - _targetPositionWithOffset).normalized;
        Vector3 directionFromTarget = (_transform.position - _targetPositionWithOffset).normalized;

        _transform.LookAt(_targetPositionWithOffset);
        Vector3 rotationAxis = _transform.right;

        _currentVerticalAngle = Vector3.SignedAngle(horizontalDirection, directionFromTarget, rotationAxis);
        _offsetDistance = Vector3.Distance(_transform.position, _targetPositionWithOffset);
    }

    private void Update()
    {
        Vector3 direction = (_transform.position - _targetPositionWithOffset).normalized;
        Vector3 targetOffsetPosition = (direction * _offsetDistance) + _targetPositionWithOffset;

        _transform.position = Vector3.MoveTowards(_transform.position, targetOffsetPosition, _offsetSpeed);
    }

    public void Initialize(PlayerInput playerInput)
    {
        _input = playerInput;

        _input.Player.MoveCamera.performed += OnMoveCamera;
    }

    private void MoveAroundTarget(Vector2 translation)
    {
        translation *= _sensitivity;

        float newAbsoluteAngleY = Math.Abs(_currentVerticalAngle + translation.y);

        if (newAbsoluteAngleY < _limitVerticalAngle)
        {
            _transform.RotateAround(_targetPositionWithOffset, _transform.right, translation.y);
            _currentVerticalAngle += translation.y;
        }

        _transform.RotateAround(_targetPositionWithOffset, Vector3.up, translation.x);
        _transform.LookAt(_targetPositionWithOffset);
    }

    private void OnMoveCamera(InputAction.CallbackContext context)
    {
        if (_input.Player.Inspect.IsPressed() == false)
            return;

        MoveAroundTarget(context.action.ReadValue<Vector2>());
    }
}