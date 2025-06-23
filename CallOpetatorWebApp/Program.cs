using CallOpetatorWebApp.Services.Cache;
using CallOpetatorWebApp.Services.Crm;
using Data8.PowerPlatform.Dataverse.Client.Wsdl;
using System.Configuration;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews(); // добавляем сервисы MVC

builder.Services.AddSingleton<IConfiguration, Configuration>();
builder.Services.AddScoped<ICrmService, CrmService>(); // Работа с CRM
builder.Services.AddSingleton<ICacheService, CacheService>(); // Кеширование (singleton)

var app = builder.Build();

// устанавливаем сопоставление маршрутов с контроллерами
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Login}/{action=Index}/{id?}");

app.Run();