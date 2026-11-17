using Core;
using Core.Abstractions;
using Core.Services;
using Core.Storage;

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

    // Створення сервісу
    var service = new CatalogService(store);

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

    // Виведення всіх товарів
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
}
catch (Exception ex)
{
    Console.WriteLine($"Критична помилка: {ex.Message}");
}
