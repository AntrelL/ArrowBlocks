using System;
using IJunior.CompositeRoot;
using IJunior.UI;

namespace IJunior.ArrowBlocks.Main
{
    public class AvailableBombsCountPresenter : ILinkedDigitalTextSource, IActivatable
    {
        private readonly BombSeller BombSeller;

        public AvailableBombsCountPresenter(BombSeller bombSeller)
        {
            BombSeller = bombSeller;
        }

        public event Action<float> ValueChanged;

        public float Value => BombSeller.AvailableBombsCount;

        public void OnActivate()
        {
            BombSeller.AvailableBombsCountChanged += OnAvailableBombsCountChanged;
        }

        public void OnDeactivate()
        {
            BombSeller.AvailableBombsCountChanged -= OnAvailableBombsCountChanged;
        }

        private void OnAvailableBombsCountChanged(int quantity) => ValueChanged?.Invoke(quantity);
    }
}