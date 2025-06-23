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

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1",
        new Microsoft.OpenApi.Models.OpenApiInfo
        {
            Title = "Call center - V1",
            Version = "v1"
        }
     );

    var filePath = Path.Combine(System.AppContext.BaseDirectory, "CallOpetatorWebApp.xml");
    c.IncludeXmlComments(filePath);
});

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseStaticFiles();

app.UseSession();   // ��������� middleware ��� ������ � ��������

// ������������� ������������� ��������� � �������������
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Login}/{action=Index}/{id?}");

app.Run();