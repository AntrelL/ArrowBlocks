using System;
using IJunior.CompositeRoot;
using IJunior.UI;

namespace IJunior.ArrowBlocks.Main
{
    public class PlayerMoneyPresenter : ILinkedDigitalTextSource, IActivatable
    {
        private readonly PlayerData PlayerData;

        public PlayerMoneyPresenter(PlayerData playerData)
        {
            PlayerData = playerData;
        }

        public event Action<float> ValueChanged;

        public float Value => PlayerData.Money;

        public void OnActivate()
        {
            PlayerData.MoneyQuantityChanged += OnPlayerMoneyQuantityChanged;
        }

        public void OnDeactivate()
        {
            PlayerData.MoneyQuantityChanged -= OnPlayerMoneyQuantityChanged;
        }

        public void OnPlayerMoneyQuantityChanged(int value) => ValueChanged?.Invoke(value);
    }
}