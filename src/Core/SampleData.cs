using Core.Domain;

namespace Core;

public static class SampleData
{
    public static IEnumerable<Product> Products()
    {
        return
        [
            Product.Create("P001", "SKU-001", "Кабель UTP cat5", "м", 100),
            Product.Create("P002", "SKU-002", "Кабель UTP cat6", "м", 150),
            Product.Create("P003", "SKU-003", "Миша Logitech", "шт", 20),
            Product.Create("P004", "SKU-004", "Клавіатура", "шт", 15),
            Product.Create("P005", "SKU-005", "Монітор Samsung", "шт", 10),
            Product.Create("P006", "SKU-006", "Флешка 32GB", "шт", 30),
            Product.Create("P007", "SKU-007", "Флешка 64GB", "шт", 25),
            Product.Create("P008", "SKU-008", "SSD 512GB", "шт", 12),
            Product.Create("P009", "SKU-009", "HDD 1TB", "шт", 8),
            Product.Create("P010", "SKU-010", "Оперативна пам'ять", "шт", 18),
            Product.Create("P011", "SKU-011", "Блок живлення", "шт", 14),
            Product.Create("P012", "SKU-012", "Материнська плата", "шт", 7),
            Product.Create("P013", "SKU-013", "Процесор", "шт", 9),
            Product.Create("P014", "SKU-014", "Відеокарта", "шт", 5),
            Product.Create("P015", "SKU-015", "Навушники", "шт", 22)
        ];
    }
}
