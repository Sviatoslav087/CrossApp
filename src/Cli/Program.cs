
using Core.Dto;
using Core.Import;

string path = args.Length > 0
    ? args[0]
    : Path.Combine("data", "sample.csv");

if (!File.Exists(path))
{
    Console.WriteLine(
        $"Файл не знайдено: {Path.GetFullPath(path)}");

    return 1;
}

try
{
    ImportResult<ProductDto> result =
        ProductCsvImporter.Load(path);

    Console.WriteLine(
        $"Завантажено записів: {result.Items.Count}");

    foreach (ProductDto p in result.Items.Take(5))
    {
        Console.WriteLine(
            $" {p.Id,-6} {p.Sku,-10} {p.Name,-32} " +
            $"{p.Quantity,5} {p.Unit}");
    }

    if (result.Errors.Count > 0)
    {
        Console.WriteLine(
            $"Пропущено рядків: {result.Errors.Count}");

        foreach (string error in result.Errors)
        {
            Console.WriteLine($" ! {error}");
        }
    }

    return 0;
}
catch (IOException ex)
{
    Console.WriteLine($"Помилка читання файлу: {ex.Message}");
    return 1;
}
catch (UnauthorizedAccessException)
{
    Console.WriteLine("Немає дозволу на читання цього файлу.");
    return 1;
}
