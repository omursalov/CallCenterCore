using CallOpetatorWebApp.Services.Cache;
using CallOpetatorWebApp.Services.Crm;
using CallOpetatorWebApp.Services.Kafka;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews(); // ��������� ������� MVC

builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.Cookie.Name = "Operator.Of.CallCenter.Session";
    options.IdleTimeout = TimeSpan.FromSeconds(28800); // 8 �����
    options.Cookie.IsEssential = true;
});

builder.Services.AddScoped<ICrmService, CrmService>(); // ������ � CRM
builder.Services.AddSingleton<ICacheService, CacheService>(); // ����������� (singleton)
builder.Services.AddSingleton<IKafkaCallsReader, KafkaCallsReader>(); // ������ ������� �� Kafka (singleton)

var app = builder.Build();

app.UseStaticFiles();

app.UseSession();   // ��������� middleware ��� ������ � ��������

// ������������� ������������� ��������� � �������������
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Login}/{action=Index}/{id?}");

app.Run();