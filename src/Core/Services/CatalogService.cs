using Core.Abstractions;
using Core.Domain;
using Core.Events;
namespace Core.Services;
using Core.Notifications;
public sealed class CatalogService
{
    private readonly ICatalogStore _store;
    private readonly IEnumerable<INotificationSink> _sinks;

    public CatalogService(
        ICatalogStore store,
        IEnumerable<INotificationSink> sinks)
    {
        _store = store ?? throw new ArgumentNullException(nameof(store));
        _sinks = sinks ?? throw new ArgumentNullException(nameof(sinks));
    }
    // Подія для повідомлення про зміни каталогу
    public event EventHandler<CatalogChangedEventArgs>? Changed;
//делегат, який приймає один параметр типу string
public int Import(IEnumerable<string> rows, Action<string> reportSkipped)
{
    ArgumentNullException.ThrowIfNull(rows);
    ArgumentNullException.ThrowIfNull(reportSkipped);

    int imported = 0;
    int lineNumber = 0;

    foreach (var row in rows)
    {
        lineNumber++;

        if (string.IsNullOrWhiteSpace(row))
        {
            reportSkipped($"Рядок {lineNumber}: порожній рядок.");
            continue;
        }

        var parts = row.Split(';');

        if (parts.Length != 4)
        {
            reportSkipped($"Рядок {lineNumber}: неправильний формат.");
            continue;
        }

        string sku = parts[0].Trim();
        string name = parts[1].Trim();
        string unit = parts[2].Trim();

        if (string.IsNullOrWhiteSpace(sku) ||
            string.IsNullOrWhiteSpace(name) ||
            string.IsNullOrWhiteSpace(unit))
        {
            reportSkipped($"Рядок {lineNumber}: відсутні дані.");
            continue;
        }

        if (!int.TryParse(parts[3].Trim(), out int quantity) ||
            quantity <= 0)
        {
            reportSkipped($"Рядок {lineNumber}: неправильна кількість.");
            continue;
        }

        try
        {
            Add(sku, name, unit, quantity);
            imported++;
        }
        catch (ArgumentException ex)
        {
            reportSkipped($"Рядок {lineNumber}: {ex.Message}");
        }
    }

    return imported;
}

    // Приватний метод виклику події
    private void OnChanged(
        Product product,
        CatalogChangeKind kind,
        int delta)
    {
        Changed?.Invoke(
            this,
            new CatalogChangedEventArgs(
                product.Id,
                kind,
                delta,
                product.Quantity));
    }
    private void NotifySinks(string message)
{
    foreach (var sink in _sinks)
    {
        sink.Notify(message);
    }
}

    // Додавання товару
    public Product Add(
        string sku,
        string name,
        string unit,
        int quantity)
    {
        var product = Product.Create(
            Guid.NewGuid().ToString("N")[..8],
            sku,
            name,
            unit,
            quantity);

  _store.Add(product);

NotifySinks(
    $"Додано товар: {product.Name}, кількість: {quantity}");

OnChanged(product, CatalogChangeKind.Added, quantity);

        return product;
    }

    // Отримання товару
    public void Receive(string id, int quantity)
    {
        var product = _store.GetById(id)
            ?? throw new InvalidOperationException(
                $"Немає запису з id={id}.");

        product.RegisterArrival(quantity);

        _store.Update(product);
NotifySinks(
    $"Надходження товару: {product.Name}, кількість: {quantity}");
        // Повідомлення про успішне отримання
        OnChanged(product, CatalogChangeKind.Received, quantity);
    }

    // Отримання всіх товарів
    public IReadOnlyList<Product> All()
    {
        return _store.List();
    }

    // Пошук товару за ID
    public Product? Find(string id)
    {
        return _store.GetById(id);
    }

    // Пошук товарів за заданою умовою через Func
    public IReadOnlyList<Product> FindAll(
        Func<Product, bool> predicate)
    {
        return _store.List()
            .Where(predicate)
            .ToList();
    }
}
