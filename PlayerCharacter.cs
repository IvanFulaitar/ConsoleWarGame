namespace ConsoleWarGame;

// Клас гравця: містить бонуси, крит та скейл між рівнями.
internal class PlayerCharacter : Character
{
    // Відсотковий бонус до шкоди (наприклад +20%).
    public int AttackBonusPercent { get; private set; }

    // Фіксований бонус до шкоди (наприклад +25).
    public int FlatBonusDamage { get; private set; }

    // Чи увімкнений критичний удар.
    public bool CritEnabled { get; private set; }

    // Шанс критичного удару у відсотках.
    public int CritChancePercent { get; } = 17;

    // Додаткова шкода криту у відсотках.
    public int CritBonusPercent { get; } = 50;

    // Створення гравця зі стартовими параметрами.
    public PlayerCharacter(string name, CombatStats stats) : base(name, stats)
    {
        AttackBonusPercent = 0;
        FlatBonusDamage = 0;
        CritEnabled = false;
    }

    // Основний урон гравця з урахуванням % бонусу і flat бонусу.
    public override int RollDamage(Random random)
    {
        var baseDamage = base.RollDamage(random);
        var percentDamage = baseDamage + (baseDamage * AttackBonusPercent / 100);
        return percentDamage + FlatBonusDamage;
    }

    // Перевантаження: додатково враховує режим потужного удару.
    public int RollDamage(Random random, bool powerStrike)
    {
        var damage = RollDamage(random);
        return powerStrike ? (int)(damage * 1.5) : damage;
    }

    // Додає відсотковий бонус атаки.
    public void AddAttackBonus(int bonusPercent)
    {
        AttackBonusPercent += bonusPercent;
    }

    // Додає фіксований бонус атаки.
    public void AddFlatBonusDamage(int bonusDamage)
    {
        FlatBonusDamage += bonusDamage;
    }

    // Вмикає механіку критичного удару.
    public void EnableCriticalStrike()
    {
        CritEnabled = true;
    }

    // Пробує застосувати крит; повертає true, якщо крит спрацював.
    public bool TryApplyCriticalStrike(Random random, ref int damage)
    {
        if (!CritEnabled)
        {
            return false;
        }

        if (random.Next(1, 101) > CritChancePercent)
        {
            return false;
        }

        damage += damage * CritBonusPercent / 100;
        return true;
    }

    // Посилення гравця між рівнями.
    public void ScaleForNextLevel()
    {
        MaxHealth += 15;
        MinAttack += 2;
        MaxAttack += 3;
        RestoreHealth();
    }
}
