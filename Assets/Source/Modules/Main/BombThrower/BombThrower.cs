using IJunior.CompositeRoot;
using UnityEngine.UI;
using UnityEngine;
using IJunior.UI;

namespace IJunior.ArrowBlocks.Main
{
    public class BombThrower : Script, IActivatable
    {
        [SerializeField] private float _bombAutoDestructionDelay;
        [SerializeField] private float _throwAngle;
        [SerializeField] private float _minThrowAngle;
        [SerializeField] private float _offsetDistance;
        [SerializeField] private Vector3 _startAngularVelocity;

        private Transform _transform;
        private Button _throwBombButton;
        private ColorIndicatingIcon _availableBombsCountBackground;
        private PlayerCamera _playerCamera;
        private BlockConstruction _blockConstruction;
        private BombSeller _bombSeller;
        private BombThrowerCalculations _calculations;
        private Bomb _bomb;

        private bool _isBombThrown = false;

        private bool IsBombThrown
        {
            get => _isBombThrown;
            set
            {
                _isBombThrown = value;
                UpdateUIElements(_playerCamera.CurrentVerticalAngle);
            }
        }

        public void InitializeBase(Bomb bombTemplate, BombSeller bombSeller, BombThrowerCalculations calculations)
        {
            _bombSeller = bombSeller;
            _calculations = calculations;

            _transform = transform;

            _bomb = Instantiate(bombTemplate, _transform);
            _bomb.Initialize();
        }

        public void InitializeUI(Button throwBombButton, ColorIndicatingIcon availableBombsCountBackground)
        {
            _throwBombButton = throwBombButton;
            _availableBombsCountBackground = availableBombsCountBackground;
        }

        public void FinalInitialize(PlayerCamera playerCamera, BlockConstruction blockConstruction)
        {
            _playerCamera = playerCamera;
            _blockConstruction = blockConstruction;

            ResetValues();
        }

        public void OnActivate()
        {
            _throwBombButton.onClick.AddListener(OnThrowBombButtonClicked);
            _playerCamera.VerticalAngleToTargetChanged += OnCameraVerticalAngleChanged;
            _bomb.Destroyed += OnBombDestroyed;
        }

        public void OnDeactivate()
        {
            _throwBombButton.onClick.RemoveListener(OnThrowBombButtonClicked);
            _playerCamera.VerticalAngleToTargetChanged -= OnCameraVerticalAngleChanged;
            _bomb.Destroyed -= OnBombDestroyed;
        }

        public void ResetValues()
        {
            _bomb.ResetValues();
            IsBombThrown = false;
        }

        private void OnThrowBombButtonClicked()
        {
            Vector3 directionToTarget = _blockConstruction.CenterPointPosition - _playerCamera.Position;
            Vector3 startPosition = _playerCamera.Position + directionToTarget.normalized * _offsetDistance;

            Vector3 startVelocity = _calculations.CalculateThrowStartVelocity(startPosition,
                _blockConstruction.CenterPointPosition, _throwAngle);

            _bombSeller.PayForBomb();

            _bomb.gameObject.SetActive(true);
            _bomb.Throw(startPosition, startVelocity, _startAngularVelocity, _bombAutoDestructionDelay);

            IsBombThrown = true;
        }

        private void OnCameraVerticalAngleChanged(float angle) => UpdateUIElements(angle);

        private void OnBombDestroyed() => IsBombThrown = false;

        private void UpdateUIElements(float angle)
        {
            bool isActiveElements = IsBombThrown == false && angle >= _minThrowAngle && _bombSeller.CanBuy;

            _throwBombButton.interactable = isActiveElements;

            if (isActiveElements)
                _availableBombsCountBackground.Activate();
            else
                _availableBombsCountBackground.Deactivate();
        }
    }
}