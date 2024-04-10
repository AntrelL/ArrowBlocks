using IJunior.CompositeRoot;
using IJunior.UI;
using System;

namespace IJunior.ArrowBlocks.Main
{
    public class PlayerMoneyView : ILinkedDigitalTextSource, IActivatable
    {
        private PlayerData _playerData;

        public PlayerMoneyView(PlayerData playerData)
        {
            _playerData = playerData;
        }

        public event Action<float> ValueChanged;

        public float Value => _playerData.Money;

        public void OnActivate()
        {
            _playerData.MoneyQuantityChanged += OnPlayerMoneyQuantityChanged;
        }

        public void OnDeactivate()
        {
            _playerData.MoneyQuantityChanged -= OnPlayerMoneyQuantityChanged;
        }

        public void OnPlayerMoneyQuantityChanged(int value) => ValueChanged?.Invoke(value);
    }
}