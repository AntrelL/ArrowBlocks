#pragma warning disable

using System.Collections.Generic;
using System.Collections;
using Agava.YandexGames;
using Newtonsoft.Json;
using UnityEngine;
using System.Linq;
using System;

namespace IJunior.ArrowBlocks.Main
{
    public class PlayerData
    {
        private const int DefaultMoney = 250;
        private const int CloudPollingLatency = 1;
        private const string EmptyJsonString = "{}";

        private int _money;
        private LevelData[] _levelsData;

        public PlayerData(int levelsQuantity) : this(DefaultMoney, levelsQuantity) { }

        private PlayerData(int money, int levelsQuantity)
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

        public IEnumerator TryGetFromCloud()
        {
#if !UNITY_WEBGL || UNITY_EDITOR
            yield break;
#endif

            bool isTringToGetData = true;
            string jsonCleanPlayerData = null;

            PlayerAccount.GetCloudSaveData(
                (string jsonString) =>
                {
                    jsonCleanPlayerData = jsonString;
                    isTringToGetData = false;
                },
                (string jsonString) =>
                {
                    isTringToGetData = false;
                });

            WaitForSeconds waitForSeconds = new WaitForSeconds(CloudPollingLatency);

            while (isTringToGetData)       
                yield return waitForSeconds;          

            Debug.Log("Load: " + jsonCleanPlayerData);

            if (string.IsNullOrEmpty(jsonCleanPlayerData) || jsonCleanPlayerData == EmptyJsonString)
                yield break;

            CleanPlayerData cleanPlayerData = JsonConvert.DeserializeObject<CleanPlayerData>(jsonCleanPlayerData);

            if (cleanPlayerData.LevelsData.Length < _levelsData.Length)
                SetData(cleanPlayerData, _levelsData.Length);
            else
                SetData(cleanPlayerData);

            yield return null;
        }

        public void SaveToCloud()
        {
#if !UNITY_WEBGL || UNITY_EDITOR
            return;
#endif

            CleanPlayerData cleanPlayerData = ConvertToCleanData();
            string jsonString = JsonConvert.SerializeObject(cleanPlayerData);

            Debug.Log("Save: " + jsonString);
            PlayerAccount.SetCloudSaveData(jsonString);
        }

        public void PassLevel(int number, float time)
        {
            _levelsData[number - 1].Pass(time);

            if (number == _levelsData.Length)
            {
                SaveToCloud();
                return;
            }

            if (_levelsData[number].State != LevelState.Blocked)
                return;

            _levelsData[number].Open();
            SaveToCloud();
        }

        private void SetData(CleanPlayerData cleanPlayerData)
        {
            _money = cleanPlayerData.Money;
            _levelsData = cleanPlayerData.LevelsData.Select(levelData => new LevelData(levelData)).ToArray();
        }

        private void SetData(CleanPlayerData cleanPlayerData, int numberOfLevels)
        {
            int previusNumberOfLevels = _levelsData.Length;
            Array.Resize(ref _levelsData, numberOfLevels);

            for (int i = previusNumberOfLevels; i < _levelsData.Length; i++)
            {
                _levelsData[i] = new LevelData();
            }

            if (_levelsData[previusNumberOfLevels - 1].State == LevelState.Passed)
                _levelsData[previusNumberOfLevels].Open();

            SetData(cleanPlayerData);
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