using UnityEngine.InputSystem;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private PlayerCamera _camera;

    private PlayerInput _input;
    private Vector2 _pointerPosition;

    public PlayerInput Input => _input;

    private void Awake()
    {
        _input = new PlayerInput();
        _camera.Initialize(_input);

        _input.Player.ActivateArrowBlock.performed += OnActivateArrowBlock;
        _input.Player.MovePointer.performed += OnPointerMove;
    }

    private void OnPointerMove(InputAction.CallbackContext context)
    {
        _pointerPosition = context.ReadValue<Vector2>();
    }

    private void OnActivateArrowBlock(InputAction.CallbackContext context)
    {
        if (Physics.Raycast(Camera.main.ScreenPointToRay(_pointerPosition), out RaycastHit hit) == false)
            return;

        if (hit.collider.gameObject.TryGetComponent(out ArrowBlock arrowBlock) == false)
            return;

        arrowBlock.TryActivate();
    }

    private void OnEnable()
    {
        _input.Enable();
    }

    private void OnDisable()
    {
        _input.Disable();
    }
}