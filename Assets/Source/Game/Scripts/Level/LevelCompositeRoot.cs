using System.Collections.Generic;
using IJunior.ArrowBlocks.Main;
using IJunior.CompositeRoot;
using IJunior.TypedScenes;
using UnityEngine.UI;
using UnityEngine;
using IJunior.UI;

using Screen = IJunior.UI.Screen;

namespace IJunior.ArrowBlocks
{
    internal class LevelCompositeRoot : MonoBehaviour, ISceneLoadHandler<PlayerData>
    {
        [Header("Screens")]
        [SerializeField] private Screen _main;
        [SerializeField] private Screen _victory;
        [Space]
        [Header("Player Elements")]
        [SerializeField] private Player _player;
        [SerializeField] private PlayerCamera _playerCamera;
        [SerializeField] private PlayerCameraBooster _playerCameraBooster;
        [Space]
        [Header("Block Construction")]
        [SerializeField] private BlockConstruction _blockConstruction;
        [SerializeField] private VirtualBlockGrid _virtualBlockGrid;
        [Space]
        [Header("User Interface")]
        [SerializeField] private DigitalText _levelNumberText;
        [SerializeField] private TimeText _passingTimeText;
        [SerializeField] private LinkedDigitalText _playerMoneyText;
        [SerializeField] private Button _nextLevelButton;
        [SerializeField] private ProgressBar _levelProgressBar;
        [SerializeField] private ProgressBar _victoryLevelProgressBar;
        [Space]
        [Header("Level Control")]
        [SerializeField] private Level _level;
        [SerializeField] private LevelFlowControl _levelFlowControl;
        [SerializeField] private LevelSceneSwitcher _levelSceneSwitcher;

        private PlayerInput _playerInput;
        private List<ArrowBlock> _arrowBlocks;
        private LevelLoader _levelLoader;
        private PlayerData _playerData;
        private PlayerMoneyView _playerMoneyView;
        private BlockConstructionProgressView _blockConstructionProgressView;

        private void Awake()
        {
            var rootFixedUpdatebleElements = new List<IRootFixedUpdateble>();
            var rootUpdatebleElements = new List<IRootUpdateble>();

            InitializeScreens();
            rootFixedUpdatebleElements.AddRange(InitializeBlockConstruction());

            _playerInput = new PlayerInput();
            InitializePlayerElements(_playerInput, _blockConstruction);

            rootUpdatebleElements.Add(_playerCamera);

            Timer timer = new Timer();
            rootUpdatebleElements.Add(timer);

            _levelProgressBar.Initialize();
            _victoryLevelProgressBar.Initialize();

            rootUpdatebleElements.Add(_levelProgressBar);
            rootUpdatebleElements.Add(_victoryLevelProgressBar);

            _passingTimeText.Initialize();

            _playerMoneyView = new PlayerMoneyView(_playerData);
            _playerMoneyText.Initialize(_playerMoneyView);

            _level.InitializeBaseInfo(_blockConstruction, _passingTimeText, timer);
            _level.InitializePlayerInfo(_playerInput, _playerData);
            _level.InitializeScreensInfo(_main, _victory);

            _levelLoader = new LevelLoader(_playerData);
            _levelSceneSwitcher.Initialize(_levelLoader, _playerData, _level.Number);

            _levelNumberText.Initialize();
            _levelNumberText.Value = _level.Number;

            _nextLevelButton.gameObject.SetActive(_level.Number < _playerData.LevelsData.Count);

            _levelFlowControl.Initialize(rootUpdatebleElements, rootFixedUpdatebleElements);
        }

        private void OnEnable()
        {
            _playerCamera.OnActivate();
            _blockConstruction.OnActivate();

            foreach (var block in _arrowBlocks)
                block.OnActivate();

            _playerMoneyView.OnActivate();
            _playerMoneyText.OnActivate();

            _blockConstructionProgressView.OnActivate();
            _levelProgressBar.Connect(_blockConstructionProgressView);

            _playerMoneyText.OnActivate();
            _level.OnActivate();
        }

        private void OnDisable()
        {
            _playerCamera.OnDeactivate();
            _blockConstruction.OnDeactivate();

            foreach (var block in _arrowBlocks)
                block.OnDeactivate();

            _playerMoneyView.OnDeactivate();
            _playerMoneyText.OnDeactivate();

            _blockConstructionProgressView.OnDeactivate();
            _levelProgressBar.Disconnect();

            _playerMoneyText.OnDeactivate();
            _level.OnDeactivate();
        }

        private void Start()
        {
            _blockConstruction.OnStart();

            _levelFlowControl.Run();
        }

        public void OnSceneLoaded(PlayerData playerData)
        {
            _playerData = playerData;
        }

        private void InitializeScreens()
        {
            _main.Initialize();
            _victory.Initialize();
        }

        private void InitializePlayerElements(PlayerInput playerInput,
            IPlayerCameraTarget playerCameraTarget)
        {
            _playerCameraBooster.Initialize();
            _playerCamera.Initialize(playerInput, playerCameraTarget, _playerCameraBooster);
            _player.Initialize(playerInput);
        }

        private List<IRootFixedUpdateble> InitializeBlockConstruction()
        {
            _virtualBlockGrid.Initialize();
            _arrowBlocks = _virtualBlockGrid.TakeBlocks();

            var blockMovers = new List<IRootFixedUpdateble>();

            foreach (var block in _arrowBlocks)
            {
                blockMovers.Add(block.Initialize(_blockConstruction));
            }

            var _calculations = new BlockConstructionCalculations(_virtualBlockGrid.CellSize);
            _blockConstruction.Initialize(_calculations, _arrowBlocks);
            _blockConstructionProgressView = new BlockConstructionProgressView(_blockConstruction);

            return blockMovers;
        }
    }
}