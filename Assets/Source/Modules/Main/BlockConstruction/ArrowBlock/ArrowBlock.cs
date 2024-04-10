using IJunior.CompositeRoot;
using System.Collections;
using UnityEngine;
using System;

namespace IJunior.ArrowBlocks.Main
{
    [RequireComponent(typeof(ArrowBlockEffector))]
    [RequireComponent(typeof(ArrowBlockAnimator))]
    [RequireComponent(typeof(ArrowBlockMover))]
    public class ArrowBlock : Script, IActivatable
    {
        [SerializeField] private float _flightTimeIntoVoid;

        private ArrowBlockEffector _effector;
        private ArrowBlockAnimator _animator;
        private ArrowBlockMover _mover;

        private BlockConstruction _blockConstruction;
        private Coroutine _submitRemovalRequester;
        private ArrowBlock _obstructingBlock;

        public event Action<ArrowBlock> Released;
        public event Action Activated;
        public event Action TouchedOther;

        public bool IsReleased { get; private set; }
        public Transform Transform { get; private set; }

        public ArrowBlockMover Initialize(BlockConstruction blockConstruction)
        {
            _blockConstruction = blockConstruction;
            Transform = transform;

            _effector = GetComponent<ArrowBlockEffector>();
            _animator = GetComponent<ArrowBlockAnimator>();
            _mover = GetComponent<ArrowBlockMover>();

            _effector.Initialize(Transform);
            _animator.Initialize(Transform);
            _mover.Initialize(Transform);

            _effector.DeactivateTrail();

            return _mover;
        }

        public void OnActivate()
        {
            _mover.TargetPositionReached += OnTargetPositionReached;
        }

        public void OnDeactivate()
        {
            _mover.TargetPositionReached -= OnTargetPositionReached;
        }

        public void TryActivate()
        {
            if (_mover.IsMoving || _animator.IsAnimated || IsReleased)
                return;

            Activated?.Invoke();

            if (Physics.Raycast(Transform.position, Transform.forward, out RaycastHit hit) == false)
            {
                StartMovingToVoid();
            }
            else
            {
                if (hit.collider.gameObject.TryGetComponent(out ArrowBlock arrowBlock) == false)
                    throw new Exception("Arrow block detected an unknown object.");

                if (arrowBlock.IsReleased)
                    StartMovingToVoid();
                else
                    StartMovingToTargetPosition(arrowBlock);
            }

            _effector.TryActivateTrail(_mover.TargetPosition);
        }

        public void ResetValues()
        {
            _mover.ResetValues();
            _effector.ClearTrail();

            IsReleased = false;

            if (_submitRemovalRequester != null)
                StopCoroutine(_submitRemovalRequester);
        }

        public void FastRelease()
        {
            Activated?.Invoke();

            Release();
            Destroy();
        }

        private void Release()
        {
            IsReleased = true;
            Released?.Invoke(this);
        }

        private void Destroy()
        {       
            _blockConstruction.RemoveBlock(this);
        }

        private void StartMovingToVoid()
        {
            _mover.MoveTo(Transform.forward, _flightTimeIntoVoid);
            Release();

            _submitRemovalRequester = StartCoroutine(SubmitRemovalRequest(_flightTimeIntoVoid));
        }

        private void StartMovingToTargetPosition(ArrowBlock obstructingArrowBlock)
        {
            Vector3 targetPosition = _blockConstruction.Calculations.GetNeighboringCellPosition(
                obstructingArrowBlock.transform.position, Transform.forward);

            _mover.MoveTo(targetPosition);
            _obstructingBlock = obstructingArrowBlock;
        }

        private IEnumerator SubmitRemovalRequest(float delay)
        {
            yield return new WaitForSeconds(delay);
            Destroy();
        }

        private void OnTargetPositionReached()
        {
            if (_obstructingBlock != null)
            {
                TouchedOther?.Invoke();
                _animator.PerformChainPushAnimation(Transform.forward);
                _obstructingBlock = null;
            }

            _effector.DeactivateTrail();
        }
    }
}