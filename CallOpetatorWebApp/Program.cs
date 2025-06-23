using CallOpetatorWebApp.Services.Cache;
using CallOpetatorWebApp.Services.Crm;
using CallOpetatorWebApp.Services.Kafka;
using Data8.PowerPlatform.Dataverse.Client.Wsdl;
using System.Configuration;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews(); // ��������� ������� MVC

builder.Services.AddScoped<ICrmService, CrmService>(); // ������ � CRM
builder.Services.AddSingleton<ICacheService, CacheService>(); // ����������� (singleton)
builder.Services.AddSingleton<IKafkaCallsReader, KafkaCallsReader>(); // ����������� (singleton)

var app = builder.Build();

// ������������� ������������� ��������� � �������������
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Login}/{action=Index}/{id?}");

app.Run();