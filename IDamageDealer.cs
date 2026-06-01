namespace ConsoleWarGame;

// Інтерфейс для будь-якого об'єкта, який може наносити шкоду.
internal interface IDamageDealer
{
    // Повертає значення шкоди для поточного удару.
    int RollDamage(Random random);
}
