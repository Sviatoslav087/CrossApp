# CrossApp 
Наскрізний проєкт з крос-платформного програмування. 
Предметна область:  Склад будівельних матеріалів. Сутності: BookProduct, BookCopyStockBatch, ReadeWarehouse, Movement. 
 Призначення: облік залишків товарів по партіях.  
## Запуск 
dotnet build 
dotnet run --project src/Clі  
dotnet run --project src/Cli -- data/valsd.csv//завантаження записів
dotnet run --project src/Cli -- --file //Запуск із використанням файлового сховища
## Середовище 
.NET SDK 10.0, Windows 11 x64 
## Таблиця
RID Режим                  Розмір publish Потрібен runtime
 win-x64 self-contained ~  76,6МБ         ні
 win-x64 framework-dependent ~0.19 МБ     так (.NET 10)
## Для обробки помилок використовуються:
ArgumentException,
ArgumentOutOfRangeException,
InvalidOperationException.
## Реалізовані інваріанти:
Стан об'єкта захищений за допомогою приватного поля
_quantity та властивостей лише для читання.
## Інтерфейс ICatalogStore
Створено інтерфейс ICatalogStore, який визначає спільні операції для роботи з каталогом товарів:
List() — отримання списку товарів.
GetById() — пошук товару за ідентифікатором.
Add() — додавання товару.
Update() — оновлення товару.
Remove() — видалення товару.
## Реалізації сховищ
Реалізовано два способи зберігання даних:
InMemoryCatalogStore — зберігає дані в оперативній пам'яті. Після завершення програми дані не зберігаються.
FileCatalogStore — зберігає дані у JSON-файлі data/catalog.json. Дані залишаються доступними після перезапуску програми.
Обидва сховища реалізують інтерфейс ICatalogStore.