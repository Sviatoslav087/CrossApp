using Core;
using Core.Abstractions;
using Core.Services;
using Core.Storage;
using Core.Events;
using Core.Notifications;

try
{
    // Вибір способу зберігання
    bool useFile = args.Contains("--file");

    // Шлях до JSON-файлу
    string dataPath = Path.Combine(
        AppContext.BaseDirectory,
        "data",
        "catalog.json");

    // Створення необхідного сховища
    ICatalogStore store = useFile
        ? new FileCatalogStore(dataPath)
        : new InMemoryCatalogStore(SampleData.Products());

// Створення обробників повідомлень
string logPath = Path.Combine(
    AppContext.BaseDirectory,
    "logs",
    "app.log");

INotificationSink[] sinks =
{
    new ConsoleSink(),
    new FileSink(logPath)
};

// Передавання обробників у сервіс
var service = new CatalogService(store, sinks);
    Directory.CreateDirectory(
        Path.GetDirectoryName(logPath)!);

    // Перший підписник: консоль і файл
    void OnCatalogChanged(
        object? sender,
        CatalogChangedEventArgs e)
    {
        var line =
            $"{e.At:HH:mm:ss} {e.Kind,-8} {e.Id} " +
            $"{e.Delta,4:+0;-0} → залишок {e.QuantityAfter}";

        Console.WriteLine(line);
    }

    // Підписка першого обробника
    service.Changed += OnCatalogChanged;

    // Другий підписник: лічильник подій
    int changes = 0;

    service.Changed += (_, _) => changes++;

    Console.WriteLine($"Сховище: {store.GetType().Name}");
    Console.WriteLine();

    // Додавання товару
    var created = service.Add(
        "SKU-101",
        "Кабель UTP cat6",
        "м",
        50);

    Console.WriteLine("Додано новий товар.");

    // Реєстрація надходження
    service.Receive(created.Id, 25);

    Console.WriteLine("Зареєстровано надходження: 25.");

    Console.WriteLine();
    Console.WriteLine($"Подій отримано: {changes}");

    // Виведення всіх товарів
    Console.WriteLine();
    Console.WriteLine("СПИСОК ТОВАРІВ:");

    foreach (var p in service.All())
    {
        Console.WriteLine(
            $"{p.Id} {p.Sku,-12} {p.Name,-25} {p.Quantity,5} {p.Unit}");
    }

    // Пошук товару
    var found = service.Find(created.Id);

    Console.WriteLine();
    Console.WriteLine(
        found != null
            ? $"Знайдено товар: {found.Name}"
            : "Товар не знайдено.");

    // Сценарій відмови
    Console.WriteLine();
    Console.WriteLine("ПЕРЕВІРКА ПОМИЛКИ:");

    try
    {
        service.Receive("UNKNOWN", 10);
    }
    catch (InvalidOperationException ex)
    {
        Console.WriteLine($"Помилка: {ex.Message}");
    }

    // Відписка першого обробника
    Console.WriteLine();
    Console.WriteLine("ВІДПИСКА ВІД ПОДІЇ:");

    service.Changed -= OnCatalogChanged;

    // Після відписки повідомлення в консоль і файл не надходять
    // Але лямбда продовжує рахувати події
    service.Receive(created.Id, 5);

    Console.WriteLine("Виконано надходження після відписки.");
    Console.WriteLine($"Подій отримано: {changes}");

    // Пошук товарів за умовою через Func
    Console.WriteLine();
    Console.WriteLine("ПОШУК ТОВАРІВ З КІЛЬКІСТЮ МЕНШЕ 10:");

    var lowStock = service.FindAll(
        p => p.Quantity < 10);

    foreach (var product in lowStock)
    {
        Console.WriteLine(
            $"{product.Name}: {product.Quantity} {product.Unit}");
    }

    // ДОДАТКОВЕ ЗАВДАННЯ №2: ВИКОРИСТАННЯ ACTION
    Console.WriteLine();
    Console.WriteLine("ІМПОРТ ТОВАРІВ:");

    // Дані для імпорту
    string[] importRows =
    {
        "SKU-201;Миша Logitech;шт;10",
        "",
        "SKU-202;Клавіатура;шт;abc",
        "SKU-203;Монітор Samsung;шт;5",
        "SKU-204;Флешка 32GB;шт;15"
    };

    // Імпорт товарів із повідомленнями про пропущені рядки
    int imported = service.Import(
        importRows,
        message => Console.WriteLine($"Пропущено: {message}")
    );

    Console.WriteLine(
        $"Успішно імпортовано товарів: {imported}");
}
catch (Exception ex)
{
    Console.WriteLine($"Критична помилка: {ex.Message}");
}
