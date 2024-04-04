using UnityEngine.InputSystem;
using IJunior.CompositeRoot;
using UnityEngine;

namespace IJunior.ArrowBlocks.Main
{
    public class Player : Script
    {
        private Vector3 _pointerPosition;

        public void Initialize(PlayerInput input)
        {
            input.Player.TryActivateArrowBlock.performed += OnTryActivateArrowBlock;
            input.Player.MovePointer.performed += OnMovePointer;
        }

        private void OnTryActivateArrowBlock(InputAction.CallbackContext context)
        {
            if (Physics.Raycast(Camera.main.ScreenPointToRay(_pointerPosition), out RaycastHit hit) == false)
                return;

            if (hit.collider.gameObject.TryGetComponent(out ArrowBlock arrowBlock) == false)
                return;

            arrowBlock.TryActivate();
        }

        private void OnMovePointer(InputAction.CallbackContext context)
        {
            _pointerPosition = context.ReadValue<Vector2>();
        }
    }
}