# CrossApp 
Наскрізний проєкт з крос-платформного програмування. 
Предметна область:  Склад. Сутності: BookProduct, BookCopyStockBatch, ReadeWarehouse, Movement. 
 Призначення: облік залишків товарів по партіях.  
## Запуск 
dotnet build 
dotnet run --project src/Cli 
## Середовище 
.NET SDK 10.0, Windows 11 x64 
## Таблиця
RID Режим                  Розмір publish Потрібен runtime
 win-x64 self-contained ~  76,6МБ         ні
 win-x64 framework-dependent ~0.19 МБ     так (.NET 10)
