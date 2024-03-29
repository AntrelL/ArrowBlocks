using System;
using System.Collections.Generic;

public class PlayerData
{
    private int _money;
    private Level[] _levels;

    public PlayerData(int money, int levelsQuantity)
    {
        Money = money;
        _levels = new Level[levelsQuantity];

        for (int i = 0; i < levelsQuantity; i++)
        {
            _levels[i] = new Level(LevelState.Blocked, float.MaxValue);
        }

        _levels[0].Open();
    }

    public event Action<int> MoneyQuantityChanged;

    public IReadOnlyList<IReadOnlyLevel> Levels => _levels;
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
        _levels[number - 1].Pass(time);

        if (number >= _levels.Length)
            return;

        if (_levels[number].State != LevelState.Blocked)
            return;

        _levels[number].Open();
    }
}