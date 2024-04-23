#pragma warning disable

using System.Collections.Generic;
using IJunior.CompositeRoot;
using Agava.YandexGames;
using UnityEngine;
using IJunior.UI;
using System;
using TMPro;

using AgavaLeaderboard = Agava.YandexGames.Leaderboard;

namespace IJunior.ArrowBlocks.Main
{
    public class Leaderboard : Script, IActivatable
    {
        public const float TimeConversionFactor = 1000f;

        private const string NameTemplate = "Level";

        [SerializeField] private string _anonymousUsername;
        [SerializeField] private int _maxNumberOfLines;

        private TMP_InputField _levelNumberInputField;
        private TimeText _playerTime;
        private DigitalText _playerPosition;

        private List<LeaderboardLine> _leaderboardLines;
        private Transform _transform;

        private int _minLevelNumber = 1;
        private int _maxLevelNumber;

        public static string GetName(int levelNumber) => NameTemplate + levelNumber;

        public void Initialize(LeaderboardLine leaderboardLineTemplate, int numberOflevels)
        {
            _leaderboardLines = new List<LeaderboardLine>();
            _transform = transform;

            _maxLevelNumber = numberOflevels;

            for (int i = 0; i < _maxNumberOfLines; i++)
            {
                var leaderboardLine = Instantiate(leaderboardLineTemplate, _transform);
                leaderboardLine.Initialize();

                _leaderboardLines.Add(leaderboardLine);
            }
        }

        public void InitializeIOElements(TMP_InputField levelNumberInputField,
            TimeText playerTime, DigitalText playerPosition)
        {
            _levelNumberInputField = levelNumberInputField;
            _playerTime = playerTime;
            _playerPosition = playerPosition;
        }

        public void FinalInitialize()
        {
            UpdateData();
        }

        public void OnActivate()
        {
            _levelNumberInputField.onEndEdit.AddListener(OnLevelNumberChanged);
        }

        public void OnDeactivate()
        {
            _levelNumberInputField.onEndEdit.RemoveListener(OnLevelNumberChanged);
        }

        private void UpdateData()
        {
            if (string.IsNullOrEmpty(_levelNumberInputField.text) || PlayerAccount.IsAuthorized == false)
            {
                SetUndefinedMode();
                return;
            }

            int levelNumber = int.Parse(_levelNumberInputField.text);
            int clampedLevelNumber = Math.Clamp(levelNumber, _minLevelNumber, _maxLevelNumber);
            _levelNumberInputField.text = clampedLevelNumber.ToString();

#if !UNITY_WEBGL || UNITY_EDITOR
            SetUndefinedMode();
            return;
#endif
            string leaderboardName = GetName(clampedLevelNumber);
            AgavaLeaderboard.GetPlayerEntry(leaderboardName, OnGetPlayerEntry);

            AgavaLeaderboard.GetEntries(leaderboardName, OnGetEntries,
                topPlayersCount: _maxNumberOfLines, competingPlayersCount: _maxNumberOfLines);
        }

        private void DisableUnnecessaryLines(int quantity)
        {
            if (quantity == 0)
                return;

            for (int i = _maxNumberOfLines - quantity; i < _maxNumberOfLines; i++)
            {
                _leaderboardLines[i].Deactivate();
            }
        }

        private void SetUndefinedMode()
        {
            SetUndefinedValues();
            DisableUnnecessaryLines(_maxNumberOfLines);
        }

        private void SetUndefinedValues()
        {
            _playerTime.SetUndefinedValue();
            _playerPosition.SetUndefinedValue();
        }

        private void OnGetPlayerEntry(LeaderboardEntryResponse entry)
        {
            if (entry == null)
            {
                SetUndefinedValues();
                return;
            }

            _playerTime.Value = ConvertToFloatTime(entry.score);
            _playerPosition.Value = entry.rank;
        }

        private void OnGetEntries(LeaderboardGetEntriesResponse result)
        {
            for (int i = 0; i < result.entries.Length; i++)
            {
                LeaderboardEntryResponse entry = result.entries[i];
                string name = entry.player.publicName;

                if (string.IsNullOrEmpty(name))
                    name = _anonymousUsername;

                _leaderboardLines[i].Activate();
                _leaderboardLines[i].SetData(entry.rank, name, ConvertToFloatTime(entry.score));

                Debug.Log(name + " " + entry.score);
            }

            int entriesLength = result.entries.Length;

            if (entriesLength < _maxNumberOfLines)
                DisableUnnecessaryLines(_maxNumberOfLines - entriesLength);
        }

        private float ConvertToFloatTime(int time) => time / TimeConversionFactor;

        private void OnLevelNumberChanged(string number) => UpdateData();
    }
}