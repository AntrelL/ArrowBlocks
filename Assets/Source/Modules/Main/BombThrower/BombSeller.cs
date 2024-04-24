using IJunior.CompositeRoot;
using System;
using UnityEngine;

namespace IJunior.ArrowBlocks.Main
{
    public class BombSeller : Script
    {
        [SerializeField] private int _bombPrice;

        private PlayerData _playerData;

        public event Action<int> AvailableBombsCountChanged;

        public bool CanBuy => _playerData.Money >= _bombPrice;
        public int AvailableBombsCount => _playerData.Money / _bombPrice;

        public void Initialize(PlayerData playerData)
        {
            _playerData = playerData;
        }

        public void PayForBomb()
        {
            _playerData.Money -= _bombPrice;
            UpdateAvailableBombsCount();
        }

        public void UpdateAvailableBombsCount() => AvailableBombsCountChanged?.Invoke(AvailableBombsCount);
    }
}