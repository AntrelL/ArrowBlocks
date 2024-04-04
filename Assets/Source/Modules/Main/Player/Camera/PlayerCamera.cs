using UnityEngine.InputSystem;
using IJunior.CompositeRoot;
using UnityEngine;
using System;

namespace IJunior.ArrowBlocks.Main
{
    [RequireComponent(typeof(Camera))]
    public class PlayerCamera : Script, IRootUpdateble, IActivatable
    {
        [SerializeField][Range(0, 100)] private float _sensitivity;
        [SerializeField][Range(0, 90)] private float _limitVerticalAngle;
        [Space]
        [Header("Speeds")]
        [SerializeField][Range(0, 100)] private float _zoomSpeed;
        [SerializeField][Range(0, 100)] private float _speedToTarget;
        [Space]
        [Header("Offset Distance")]
        [SerializeField][Range(0, 100)] private float _additionalOffsetDistance;
        [SerializeField][Range(0, 100)] private float _minOffsetDistance;

        private PlayerInput _input;
        private Transform _transform;
        private IPlayerCameraTarget _target;
        private PlayerCameraCalculations _calculations;
        private PlayerCameraBooster _booster;

        private float _currentVerticalAngle;
        private float _offsetDistance;
        private Vector3 _targetPosition;

        public void Initialize(PlayerInput input, IPlayerCameraTarget target, PlayerCameraBooster booster)
        {
            _transform = transform;

            _input = input;
            _target = target;
            _booster = booster;

            _calculations = new PlayerCameraCalculations();

            _input.Player.MoveCamera.performed += OnMoveCamera;

            _targetPosition = _target.CenterPointPosition;
            _transform.LookAt(_targetPosition);

            _offsetDistance = Vector3.Distance(_transform.position, _targetPosition);
        }

        public void OnActivate()
        {
            _target.ThroughLineLengthChanged += OnThroughLineLengthChanged;
        }

        public void OnDeactivate()
        {
            _target.ThroughLineLengthChanged -= OnThroughLineLengthChanged;
        }

        public void RootUpdate()
        {
            Vector3 newTargetPosition = _target.CenterPointPosition;
            _currentVerticalAngle = _calculations.CalculateCurrentVerticalAngle(newTargetPosition, _transform);

            float speedToTarget = _speedToTarget * Time.deltaTime;
            float zoomSpeed = _zoomSpeed * Time.deltaTime;

            if (_booster.IsActivated)
                _booster.ProcessSpeedValues(ref speedToTarget, ref zoomSpeed);

            _targetPosition = Vector3.MoveTowards(_targetPosition,
                newTargetPosition, speedToTarget);

            if (_targetPosition != newTargetPosition)
                _transform.LookAt(_targetPosition);

            Vector3 offsetPosition = _calculations.CalculateOffsetPosition(_targetPosition, _offsetDistance, _transform);
            _booster.UpdateState(newTargetPosition, _offsetDistance, _calculations, _transform);

            _transform.position = Vector3.MoveTowards(
                _transform.position, offsetPosition, zoomSpeed);
        }

        private void OnMoveCamera(InputAction.CallbackContext context)
        {
            if (_input.Player.Inspect.IsPressed() == false)
                return;

            MoveAroundTarget(context.action.ReadValue<Vector2>(), _targetPosition);
        }

        private void OnThroughLineLengthChanged(float length)
        {
            if (length < 0)
                throw new Exception("Length cannot be negative.");

            _offsetDistance = Mathf.Max(length + _additionalOffsetDistance, _minOffsetDistance);
        }

        private void MoveAroundTarget(Vector2 shift, Vector3 targetPosition)
        {
            shift *= _sensitivity;

            float newAbsoluteVerticalAngle = Math.Abs(_currentVerticalAngle + shift.y);

            if (newAbsoluteVerticalAngle < _limitVerticalAngle)
            {
                _transform.RotateAround(targetPosition, _transform.right, shift.y);
                _currentVerticalAngle += shift.y;
            }

            _transform.RotateAround(targetPosition, Vector3.up, shift.x);
            _transform.LookAt(targetPosition);
        }
    }
}