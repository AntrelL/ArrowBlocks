using IJunior.CompositeRoot;
using UnityEngine;
using System;

namespace IJunior.ArrowBlocks.Main
{
    public class ArrowBlockMover : Script, IRootFixedUpdateble
    {
        [SerializeField] private float _movementSpeed;

        private Transform _transform;
        private Vector3 _basePosition;

        public event Action TargetPositionReached;

        public Vector3 TargetPosition { get; private set; }
        public bool IsMoving { get; private set; }

        public void Initialize(Transform transform)
        {
            _transform = transform;
            _basePosition = TargetPosition = _transform.position;
        }

        public void RootFixedUpdate()
        {
            if (IsMoving == false)
                return;

            _transform.position = Vector3.MoveTowards(_transform.position,
                TargetPosition, _movementSpeed * Time.fixedDeltaTime);

            if (_transform.position == TargetPosition)
            {
                IsMoving = false;
                TargetPositionReached?.Invoke();
            }     
        }

        public void MoveTo(Vector3 direction, float time)
        {
            MoveTo(_transform.position + direction * (_movementSpeed * time));
        }

        public void MoveTo(Vector3 position)
        {
            TargetPosition = position;
            IsMoving = true;
        }

        public void ResetValues()
        {
            _transform.position = TargetPosition = _basePosition;
        }
    }
}