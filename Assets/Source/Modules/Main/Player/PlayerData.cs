#pragma warning disable

using System.Collections.Generic;
using Agava.YandexGames;
using Newtonsoft.Json;
using System.Linq;
using System;

namespace IJunior.ArrowBlocks.Main
{
    public class PlayerData
    {
        private const int DefaultMoney = 250;

        private int _money;
        private LevelData[] _levelsData;

        public PlayerData(CleanPlayerData cleanPlayerData)
        {
            _money = cleanPlayerData.Money;
            _levelsData = cleanPlayerData.LevelsData.Select(levelData => new LevelData(levelData)).ToArray();
        }

        public PlayerData(CleanPlayerData cleanPlayerData, int numberOfLevels) : this(cleanPlayerData)
        {
            Array.Resize(ref _levelsData, numberOfLevels);

            for (int i = _levelsData.Length; i < numberOfLevels; i++)
            {
                _levelsData[i] = new LevelData();
            }
        }

        public PlayerData(int money, int levelsQuantity)
        {
            _money = money;
            _levelsData = new LevelData[levelsQuantity];

            for (int i = 0; i < levelsQuantity; i++)
            {
                _levelsData[i] = new LevelData();
            }

            if (levelsQuantity > 0)
                _levelsData[0].Open();
        }

        public event Action<int> MoneyQuantityChanged;

        public IReadOnlyList<IReadOnlyLevelData> LevelsData => _levelsData;
        public int Money
        {
            get => _money;
            set
            {
                if (value < 0)
                    throw new Exception("Money cannot be less than zero.");

                _money = value;

                MoneyQuantityChanged?.Invoke(_money);
                SaveToCloud();
            }
        }

        public static PlayerData GetFromCloud(int numberOfLevels)
        {
#if !UNITY_WEBGL || UNITY_EDITOR
            return new PlayerData(DefaultMoney, numberOfLevels);
#endif

            string jsonCleanPlayerData = null;
            PlayerAccount.GetCloudSaveData((string jsonString) => jsonCleanPlayerData = jsonString);

            if (jsonCleanPlayerData == null)
                return new PlayerData(DefaultMoney, numberOfLevels);

            CleanPlayerData cleanPlayerData = JsonConvert.DeserializeObject<CleanPlayerData>(jsonCleanPlayerData);

            if (cleanPlayerData.LevelsData.Length < numberOfLevels)
                return new PlayerData(cleanPlayerData, numberOfLevels);

            return new PlayerData(cleanPlayerData);
        }

        public void SaveToCloud()
        {
#if !UNITY_WEBGL || UNITY_EDITOR
            return;
#endif

            CleanPlayerData cleanPlayerData = ConvertToCleanData();
            string jsonString = JsonConvert.SerializeObject(cleanPlayerData);

            PlayerAccount.SetCloudSaveData(jsonString);
        }

        public void PassLevel(int number, float time)
        {
            _levelsData[number - 1].Pass(time);

            if (number == _levelsData.Length)
                return;

            if (_levelsData[number].State != LevelState.Blocked)
                return;

            _levelsData[number].Open();
            SaveToCloud();
        }

        private CleanPlayerData ConvertToCleanData()
        {
            CleanPlayerData cleanPlayerData = new CleanPlayerData();

            cleanPlayerData.Money = Money;
            cleanPlayerData.LevelsData = _levelsData.Select(levelData => levelData.ConvertToCleanData()).ToArray();

            return cleanPlayerData;
        }
    }
}