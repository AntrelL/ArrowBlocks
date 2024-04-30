using System.Collections.Generic;
using IJunior.ArrowBlocks.Main;
using IJunior.CompositeRoot;
using IJunior.TypedScenes;
using Lean.Localization;
using UnityEngine.UI;
using UnityEngine;
using IJunior.UI;

using Screen = IJunior.UI.Screen;

namespace IJunior.ArrowBlocks
{
    internal class LevelCompositeRoot : MonoBehaviour, ISceneLoadHandler<PlayerData>
    {
        [Header("Screens")]
        [SerializeField] private Screen _mainScreen;
        [SerializeField] private Screen _victoryScreen;
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
        [SerializeField] private LeanLocalization _leanLocalization;
        [SerializeField] private DigitalText _levelNumberText;
        [SerializeField] private TimeText _passingTimeText;
        [SerializeField] private LinkedDigitalText _playerMoneyText;
        [SerializeField] private LinkedDigitalText _availableBombsCount;
        [SerializeField] private ColorIndicatingIcon _availableBombsCountBackground;
        [SerializeField] private Button _nextLevelButton;
        [SerializeField] private Button _throwBombButton;
        [SerializeField] private ProgressBar _levelProgressBar;
        [SerializeField] private ProgressBar _victoryLevelProgressBar;
        [SerializeField] private Button _playRewardVideoButton;
        [Space]
        [Header("Bomb Thrower")]
        [SerializeField] private BombThrower _bombThrower;
        [SerializeField] private BombSeller _bombSeller;
        [SerializeField] private Bomb _bombTemplate;
        [Space]
        [Header("Level Audio")]
        [SerializeField] private AudioSource _levelAudioSource;
        [SerializeField] private AudioClip _levelVictorySound;
        [SerializeField] private SoundSwitch _soundSwitch;
        [Space]
        [Header("Advertising")]
        [SerializeField] private AdvertisingVisualizer _advertisingVisualizer;
        [Space]
        [Header("Level Control")]
        [SerializeField] private Level _level;
        [SerializeField] private FlowControl _levelFlowControl;
        [SerializeField] private LevelSceneSwitcher _levelSceneSwitcher;
        [SerializeField] private BrowserTabFocus _browserTabFocus;

        private PlayerInput _playerInput;
        private List<ArrowBlock> _arrowBlocks;
        private LevelLoader _levelLoader;
        private PlayerData _playerData;
        private PlayerMoneyPresenter _playerMoneyPresenter;
        private AvailableBombsCountPresenter _availableBombsCountPresenter;
        private BlockConstructionProgressPresenter _blockConstructionProgressPresenter;

        private void Awake() => InitializeEarly();

        private void OnDisable() => UnsubscribeEvents();

        private void Start()
        {
            Initialize();
            SubscribeEvents();

            _blockConstruction.OnStart();
            _levelFlowControl.Run();
        }

        private void InitializeEarly()
        {
            InitializeScreens();
        }

        private void Initialize()
        {
            Localization.SetLanguage(_leanLocalization);

            var rootFixedUpdatebleElements = new List<IRootFixedUpdateble>();
            var rootUpdatebleElements = new List<IRootUpdateble>();

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

            _playerMoneyPresenter = new PlayerMoneyPresenter(_playerData);
            _playerMoneyText.Initialize(_playerMoneyPresenter);

            InitializeLevelControl(timer);

            _levelNumberText.Initialize();
            _levelNumberText.Value = _level.Number;

            _nextLevelButton.gameObject.SetActive(_level.Number < _playerData.LevelsData.Count);

            _availableBombsCountBackground.Initialize();

            InitializeBombThrower();

            _availableBombsCountPresenter = new AvailableBombsCountPresenter(_bombSeller);
            _availableBombsCount.Initialize(_availableBombsCountPresenter);

            _advertisingVisualizer.Initialize(_playRewardVideoButton,
                _playerData, BackgroundMusicPlayer.CurrentInstance, _bombSeller);

            _browserTabFocus.Initialize(BackgroundMusicPlayer.CurrentInstance, _advertisingVisualizer);
            _soundSwitch.Initialize();

            _levelFlowControl.Initialize(rootUpdatebleElements, rootFixedUpdatebleElements);
        }

        private void SubscribeEvents()
        {
            _playerCamera.OnActivate();
            _blockConstruction.OnActivate();

            foreach (var block in _arrowBlocks)
                block.OnActivate();

            _playerMoneyPresenter.OnActivate();
            _playerMoneyText.OnActivate();

            _blockConstructionProgressPresenter.OnActivate();
            _levelProgressBar.Connect(_blockConstructionProgressPresenter);

            _availableBombsCountPresenter.OnActivate();
            _availableBombsCount.OnActivate();
            _bombThrower.OnActivate();

            _advertisingVisualizer.OnActivate();
            _browserTabFocus.OnActivate();
            _soundSwitch.OnActivate();

            _level.OnActivate();
        }

        private void UnsubscribeEvents()
        {
            _playerCamera.OnDeactivate();
            _blockConstruction.OnDeactivate();

            foreach (var block in _arrowBlocks)
                block.OnDeactivate();

            _playerMoneyPresenter.OnDeactivate();
            _playerMoneyText.OnDeactivate();

            _blockConstructionProgressPresenter.OnDeactivate();
            _levelProgressBar.Disconnect();

            _availableBombsCountPresenter.OnDeactivate();
            _availableBombsCount.OnDeactivate();
            _bombThrower.OnDeactivate();

            _advertisingVisualizer.OnDeactivate();
            _browserTabFocus.OnDeactivate();
            _soundSwitch.OnDeactivate();

            _level.OnDeactivate();
        }

        public void OnSceneLoaded(PlayerData playerData)
        {
            _playerData = playerData;
        }

        private void InitializeScreens()
        {
            _mainScreen.Initialize();
            _victoryScreen.Initialize();
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
            _blockConstructionProgressPresenter = new BlockConstructionProgressPresenter(_blockConstruction);

            return blockMovers;
        }

        private void InitializeBombThrower()
        {
            _bombSeller.Initialize(_playerData);

            _bombThrower.InitializeBase(_bombTemplate, _bombSeller, new BombThrowerCalculations());
            _bombThrower.InitializeUI(_throwBombButton, _availableBombsCountBackground);
            _bombThrower.FinalInitialize(_playerCamera, _blockConstruction);
        }

        private void InitializeLevelControl(Timer timer)
        {
            _level.InitializeBaseInfo(_blockConstruction, _passingTimeText, _bombThrower, _bombSeller, timer);
            _level.InitializePlayerInfo(_playerInput, _playerData);
            _level.InitializeScreensInfo(_mainScreen, _victoryScreen);
            _level.InitializeAudio(_levelAudioSource, _levelVictorySound);

            _levelLoader = new LevelLoader(_playerData);
            _levelSceneSwitcher.Initialize(_levelLoader, _playerData, _level.Number, _advertisingVisualizer);
        }
    }
}