using CallOpetatorWebApp.Services.Cache;
using CallOpetatorWebApp.Services.Crm;
using Data8.PowerPlatform.Dataverse.Client.Wsdl;
using System.Configuration;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews(); // ��������� ������� MVC

builder.Services.AddSingleton<IConfiguration, Configuration>();
builder.Services.AddScoped<ICrmService, CrmService>(); // ������ � CRM
builder.Services.AddSingleton<ICacheService, CacheService>(); // ����������� (singleton)

var app = builder.Build();

// ������������� ������������� ��������� � �������������
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Login}/{action=Index}/{id?}");

app.Run();