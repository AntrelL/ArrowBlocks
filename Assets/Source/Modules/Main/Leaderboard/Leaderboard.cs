#pragma warning disable CS0162

using System;
using System.Collections.Generic;
using Agava.YandexGames;
using IJunior.CompositeRoot;
using IJunior.UI;
using Lean.Localization;
using TMPro;
using UnityEngine;

using AgavaLeaderboard = Agava.YandexGames.Leaderboard;

namespace IJunior.ArrowBlocks.Main
{
    public class Leaderboard : Script, IActivatable
    {
        public const float TimeConversionFactor = 1000f;

        private const string NameTemplate = "Level";

        [SerializeField] private string _localizationAnonymousNameField;
        [SerializeField] private int _maxNumberOfLines;

        private TMP_Dropdown _levelNumberDropdown;
        private TimeText _playerTime;
        private DigitalText _playerPosition;

        private List<LeaderboardLine> _leaderboardLines;
        private Transform _transform;

        private int _maxLevelNumber;

        public int MinLevelNumber { get; } = 1;

        public int LevelNumber
        {
            set
            {
                _levelNumberDropdown.value = Math.Clamp(value, MinLevelNumber, _maxLevelNumber) - 1;
                OnLevelNumberChanged(_levelNumberDropdown.value);
            }
        }

        public static string GetName(int levelNumber) => NameTemplate + levelNumber;

        public void Initialize(LeaderboardLine leaderboardLineTemplate, int numberOflevels)
        {
            _leaderboardLines = new List<LeaderboardLine>();
            _transform = transform;

            _maxLevelNumber = numberOflevels;

            for (int i = 0; i < _maxNumberOfLines; i++)
            {
                var leaderboardLine = Instantiate(leaderboardLineTemplate, _transform);

                _leaderboardLines.Add(leaderboardLine);
            }
        }

        public void InitializeIOElements(
            TMP_Dropdown levelNumberDropdown, TimeText playerTime, DigitalText playerPosition)
        {
            _levelNumberDropdown = levelNumberDropdown;
            _playerTime = playerTime;
            _playerPosition = playerPosition;
        }

        public void FinalInitialize()
        {
            foreach (var leaderboardLine in _leaderboardLines)
            {
                leaderboardLine.Initialize();
            }

            for (int i = MinLevelNumber; i <= _maxLevelNumber; i++)
            {
                _levelNumberDropdown.options.Add(new TMP_Dropdown.OptionData(i.ToString()));
            }

            _levelNumberDropdown.captionText.text = _levelNumberDropdown.options[0].text;

            UpdateData();
        }

        public void OnActivate()
        {
            _levelNumberDropdown.onValueChanged.AddListener(OnLevelNumberChanged);
        }

        public void OnDeactivate()
        {
            _levelNumberDropdown.onValueChanged.RemoveListener(OnLevelNumberChanged);
        }

        private void UpdateData()
        {
#if !UNITY_WEBGL || UNITY_EDITOR
            return;
#endif

            int levelNumber = _levelNumberDropdown.value + 1;

            string leaderboardName = GetName(levelNumber);
            AgavaLeaderboard.GetPlayerEntry(leaderboardName, OnGetPlayerEntry);

            AgavaLeaderboard.GetEntries(
                leaderboardName,
                OnGetEntries,
                topPlayersCount: _maxNumberOfLines,
                competingPlayersCount: _maxNumberOfLines);
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
                    name = LeanLocalization.GetTranslationText(_localizationAnonymousNameField);

                _leaderboardLines[i].Activate();
                _leaderboardLines[i].SetData(entry.rank, name, ConvertToFloatTime(entry.score));

                Debug.Log(name + " " + entry.score);
            }

            int entriesLength = result.entries.Length;

            if (entriesLength < _maxNumberOfLines)
                DisableUnnecessaryLines(_maxNumberOfLines - entriesLength);
        }

        private float ConvertToFloatTime(int time) => time / TimeConversionFactor;

        private void OnLevelNumberChanged(int index) => UpdateData();
    }
}