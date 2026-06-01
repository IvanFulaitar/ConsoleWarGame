namespace ConsoleWarGame;

// Легка структура для зберігання стартових характеристик бійця.
internal readonly struct CombatStats
{
    // Початкове здоров'я.
    public int Health { get; }

    // Мінімальна атака.
    public int MinAttack { get; }

    // Максимальна атака.
    public int MaxAttack { get; }

    // Конструктор структури характеристик.
    public CombatStats(int health, int minAttack, int maxAttack)
    {
        Health = health;
        MinAttack = minAttack;
        MaxAttack = maxAttack;
    }
}
