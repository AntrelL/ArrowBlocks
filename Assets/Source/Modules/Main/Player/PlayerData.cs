using System;
using System.Collections.Generic;

namespace IJunior.ArrowBlocks.Main
{
    public class PlayerData
    {
        private int _money;
        private LevelData[] _levelsData;

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
            }
        }

        public void PassLevel(int number, float time)
        {
            _levelsData[number - 1].Pass(time);

            if (number == _levelsData.Length)
                return;

            if (_levelsData[number].State != LevelState.Blocked)
                return;

            _levelsData[number].Open();
        }
    }
}