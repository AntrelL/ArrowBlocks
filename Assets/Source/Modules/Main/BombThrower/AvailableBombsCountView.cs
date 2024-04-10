using System;
using IJunior.UI;
using IJunior.CompositeRoot;

namespace IJunior.ArrowBlocks.Main
{
    public class AvailableBombsCountView : ILinkedDigitalTextSource, IActivatable
    {
        private BombSeller _bombSeller;

        public AvailableBombsCountView(BombSeller bombSeller)
        {
            _bombSeller = bombSeller;
        }

        public event Action<float> ValueChanged;

        public float Value => _bombSeller.AvailableBombsCount;

        public void OnActivate()
        {
            _bombSeller.AvailableBombsCountChanged += OnAvailableBombsCountChanged;
        }

        public void OnDeactivate()
        {
            _bombSeller.AvailableBombsCountChanged -= OnAvailableBombsCountChanged;
        }

        private void OnAvailableBombsCountChanged(int quantity) => ValueChanged?.Invoke(quantity);
    }
}