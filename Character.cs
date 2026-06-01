namespace ConsoleWarGame;

// Базовий клас будь-якого бійця в грі (гравець або ворог).
internal abstract class Character : IDamageDealer
{
    // Ім'я персонажа, яке показується в консолі.
    public string Name { get; protected set; }

    // Максимальний запас здоров'я.
    public int MaxHealth { get; protected set; }

    // Поточне здоров'я під час бою.
    public int Health { get; protected set; }

    // Мінімальна можлива сила удару.
    public int MinAttack { get; protected set; }

    // Максимальна можлива сила удару.
    public int MaxAttack { get; protected set; }

    // Ініціалізуємо базові характеристики персонажа.
    protected Character(string name, CombatStats stats)
    {
        Name = name;
        MaxHealth = stats.Health;
        Health = stats.Health;
        MinAttack = stats.MinAttack;
        MaxAttack = stats.MaxAttack;
    }

    // Базовий кидок шкоди: випадкове значення в межах атаки.
    public virtual int RollDamage(Random random)
    {
        return random.Next(MinAttack, MaxAttack + 1);
    }

    // Перевантаження №1: отримати повну вхідну шкоду.
    public void TakeDamage(int damage)
    {
        Health = Math.Max(0, Health - damage);
    }

    // Перевантаження №2: можна врахувати/ігнорувати захист.
    public void TakeDamage(int damage, bool ignoreDefense)
    {
        var reducedDamage = ignoreDefense ? damage : Math.Max(1, damage - 2);
        TakeDamage(reducedDamage);
    }

    // Повністю відновлює здоров'я персонажа.
    public void RestoreHealth()
    {
        Health = MaxHealth;
    }
}
