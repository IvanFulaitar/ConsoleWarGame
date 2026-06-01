using ConsoleWarGame;

// Генератор випадкових чисел для всіх випадкових подій у грі.
var random = new Random();

// Поточний рівень гри.
var level = 1;

// Список бонусів, які ще доступні для вибору (кожен можна взяти лише один раз).
var availableBonuses = new List<string> { "percent", "flat", "crit" };

// Зчитуємо ім'я гравця.
Console.Write("Enter your name: ");
var playerName = Console.ReadLine();
if (string.IsNullOrWhiteSpace(playerName))
{
    playerName = "Player";
}

// Створюємо гравця зі стартовими характеристиками.
var player = new PlayerCharacter(playerName, new CombatStats(100, 8, 15));
Console.WriteLine($"Welcome, {player.Name}! The battle begins.");

// Зовнішній цикл гри: триває, поки гравець живий.
while (player.Health > 0)
{
    // Створюємо ворога під поточний рівень.
    var enemy = CreateEnemy(level, player);

    // Скільки бонусів вже видано на цьому рівні.
    var bonusesGivenThisLevel = 0;

    // Ліміт бонусів: 1-й рівень -> 1, 2-й -> 2, 3-й і далі -> 3.
    var maxBonusesThisLevel = Math.Min(level, 3);

    // Лічильник раундів для видачі бонусу кожен 3-й раунд.
    var roundsThisLevel = 0;

    Console.WriteLine();
    Console.WriteLine($"=== LEVEL {level} ===");
    PrintStats(player, enemy);

    // Внутрішній цикл бою: триває, поки живі і гравець, і ворог.
    while (player.Health > 0 && enemy.Health > 0)
    {
        Console.WriteLine();
        Console.WriteLine("Press Enter to attack...");
        Console.ReadLine();

        // Рахуємо шкоду гравця з урахуванням активних бонусів.
        var playerDamage = player.RollDamage(random);

        // Додатково пробуємо застосувати критичний удар.
        var isCritical = player.TryApplyCriticalStrike(random, ref playerDamage);

        // Ворог отримує шкоду.
        enemy.TakeDamage(playerDamage);
        Console.WriteLine($"{player.Name} dealt {playerDamage} damage{(isCritical ? " [CRITICAL HIT]" : string.Empty)}.");

        // Якщо ворог помер після удару гравця — завершуємо бій на рівні.
        if (enemy.Health <= 0)
        {
            Console.WriteLine($"{enemy.Name} is defeated!");
            break;
        }

        // Хід ворога.
        var enemyDamage = enemy.RollDamage(random);
        player.TakeDamage(enemyDamage);
        Console.WriteLine($"{enemy.Name} dealt {enemyDamage} damage.");

        // Якщо гравець помер після удару ворога — виходимо з циклу бою.
        if (player.Health <= 0)
        {
            break;
        }

        // Після кожного повного обміну ударами рахуємо раунд.
        roundsThisLevel++;

        // Кожен 3-й раунд пробуємо видати один бонус (до ліміту рівня).
        if (bonusesGivenThisLevel < maxBonusesThisLevel && roundsThisLevel % 3 == 0)
        {
            var bonusGiven = TryGrantSingleBonus(player, random, availableBonuses, bonusesGivenThisLevel + 1, maxBonusesThisLevel);
            if (bonusGiven)
            {
                bonusesGivenThisLevel++;
            }
        }

        // Поточний стан HP після раунду.
        Console.WriteLine($"Current HP: {player.Name} {player.Health} | {enemy.Name} {enemy.Health}");
    }

    // Якщо гравець програв, виводимо повідомлення і зберігаємо результат.
    if (player.Health <= 0)
    {
        Console.WriteLine($"{player.Name} was defeated on level {level}.");
        PrintLoseBanner();
        SaveResult(player.Name, level - 1);
        break;
    }

    // Якщо пройдено 3 рівні — перемога і завершення гри.
    if (level == 3)
    {
        Console.WriteLine();
        PrintWinBanner();
        SaveResult(player.Name, level);
        break;
    }

    // Переходимо на наступний рівень і підсилюємо гравця.
    level++;
    player.ScaleForNextLevel();
    Console.WriteLine($"Level cleared! New stats: HP {player.MaxHealth}, attack {player.MinAttack}-{player.MaxAttack}.");
}

// Створює ворога під поточний рівень.
static Opponent CreateEnemy(int level, PlayerCharacter player)
{
    // Випадково вибираємо тип ворога.
    var enemyTypes = Enum.GetValues<EnemyType>();
    var enemyType = enemyTypes[Random.Shared.Next(enemyTypes.Length)];

    // Базовий ріст здоров'я ворога від рівня.
    var enemyHealth = player.MaxHealth + (level - 1) * 20;

    // На 3 рівні  бос має подвоєне HP.
    if (level >= 3)
    {
        enemyHealth *= 2;
    }

    // Формуємо характеристики ворога на основі рівня.
    var stats = new CombatStats(
        health: enemyHealth,
        minAttack: player.MinAttack + (level - 1) * 2,
        maxAttack: player.MaxAttack + (level - 1) * 3);

    return new Opponent($"{enemyType} L{level}", stats, enemyType);
}

// Видає один бонус гравцю і повертає true, якщо бонус застосовано.
static bool TryGrantSingleBonus(
    PlayerCharacter player,
    Random random,
    List<string> availableBonuses,
    int currentBonusNumber,
    int maxBonusesThisLevel)
{
    // Немає доступних бонусів або ліміт рівня = 0.
    if (availableBonuses.Count == 0 || maxBonusesThisLevel <= 0)
    {
        return false;
    }

    Console.WriteLine();
    Console.WriteLine($"Bonus {currentBonusNumber}/{maxBonusesThisLevel} for this level. Choose your bonus:");
    if (availableBonuses.Contains("percent")) Console.WriteLine("1 - Percent bonus to attack (+20%)");
    if (availableBonuses.Contains("flat")) Console.WriteLine("2 - Flat bonus damage (+25)");
    if (availableBonuses.Contains("crit")) Console.WriteLine("3 - Critical strike unlock (17% chance for +50% damage each hit)");

    var choice = Console.ReadLine();

    // Вибір фіксованого бонусу +25.
    if (choice == "2" && availableBonuses.Contains("flat"))
    {
        const int flatBonus = 25;
        player.AddFlatBonusDamage(flatBonus);
        availableBonuses.Remove("flat");
        Console.WriteLine($"Flat damage bonus applied: +{flatBonus}. Current flat bonus: +{player.FlatBonusDamage}.");
        return true;
    }

    // Вибір критичного удару.
    if (choice == "3" && availableBonuses.Contains("crit"))
    {
        player.EnableCriticalStrike();
        availableBonuses.Remove("crit");
        Console.WriteLine($"Critical strike enabled: {player.CritChancePercent}% chance for +{player.CritBonusPercent}% damage.");
        return true;
    }

    // Вибір відсоткового бонусу.
    if (choice == "1" && availableBonuses.Contains("percent"))
    {
        player.AddAttackBonus(20);
        availableBonuses.Remove("percent");
        Console.WriteLine($"Percent attack bonus applied: +20%. Current percent bonus: +{player.AttackBonusPercent}%.");
        return true;
    }

    // Якщо ввід некоректний — автоматично даємо перший доступний бонус.
    Console.WriteLine("Invalid choice. First available bonus was applied automatically.");
    var fallback = availableBonuses[0];
    if (fallback == "percent")
    {
        player.AddAttackBonus(20);
        availableBonuses.Remove("percent");
        Console.WriteLine($"Percent attack bonus applied: +20%. Current percent bonus: +{player.AttackBonusPercent}%.");
        return true;
    }

    if (fallback == "flat")
    {
        const int flatBonus = 25;
        player.AddFlatBonusDamage(flatBonus);
        availableBonuses.Remove("flat");
        Console.WriteLine($"Flat damage bonus applied: +{flatBonus}. Current flat bonus: +{player.FlatBonusDamage}.");
        return true;
    }

    player.EnableCriticalStrike();
    availableBonuses.Remove("crit");
    Console.WriteLine($"Critical strike enabled: {player.CritChancePercent}% chance for +{player.CritBonusPercent}% damage.");
    return true;
}

// Вивід поточних характеристик гравця і ворога.
static void PrintStats(PlayerCharacter player, Opponent enemy)
{
    Console.WriteLine($"{player.Name}: HP {player.Health}/{player.MaxHealth}, attack {player.MinAttack}-{player.MaxAttack}, % bonus +{player.AttackBonusPercent}%, flat bonus +{player.FlatBonusDamage}, crit {(player.CritEnabled ? "ON" : "OFF")}");
    Console.WriteLine($"{enemy.Name}: HP {enemy.Health}/{enemy.MaxHealth}, attack {enemy.MinAttack}-{enemy.MaxAttack}");
}

// Зберігає результат гри у текстовий файл.
static void SaveResult(string playerName, int clearedLevels)
{
    try
    {
        var line = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} | {playerName} | cleared levels: {clearedLevels}";
        File.AppendAllLines("game_results.txt", new[] { line });
        Console.WriteLine("Result saved to game_results.txt");
    }
    catch (Exception ex)
    {
        // Якщо файл недоступний або сталася інша помилка запису.
        Console.WriteLine($"Could not save result: {ex.Message}");
    }
}

// Простий банер перемоги.
static void PrintWinBanner()
{
    Console.WriteLine("=================================");
    Console.WriteLine("           YOU ARE WIN           ");
    Console.WriteLine("=================================");
}

// Простий банер поразки.
static void PrintLoseBanner()
{
    Console.WriteLine("=================================");
    Console.WriteLine("          YOU ARE LOSE           ");
    Console.WriteLine("=================================");
}
