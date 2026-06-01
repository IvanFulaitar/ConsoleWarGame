namespace ConsoleWarGame;

// Клас ворога з типом, який може давати спеціальний ефект.
internal class Opponent : Character
{
    // Тип ворога (Shadow/Berserker/Warden/Hunter).
    public EnemyType Type { get; }

    // Створення ворога з типом і характеристиками.
    public Opponent(string name, CombatStats stats, EnemyType type) : base(name, stats)
    {
        Type = type;
    }

    // Поліморфна поведінка атаки ворога.
    public override int RollDamage(Random random)
    {
        var damage = base.RollDamage(random);

        // Berserker має 20% шанс додатково посилити удар на +5.
        if (Type == EnemyType.Berserker && random.Next(100) < 20)
        {
            damage += 5;
        }

        return damage;
    }
}
