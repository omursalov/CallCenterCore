using CallOpetatorWebApp.Services.Cache;
using CallOpetatorWebApp.Services.Crm;
using CallOpetatorWebApp.Services.Kafka;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews(); // добавл€ем сервисы MVC

builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.Cookie.Name = "Operator.Of.CallCenter.Session";
    options.IdleTimeout = TimeSpan.FromSeconds(28800); // 8 часов
    options.Cookie.IsEssential = true;
});

builder.Services.AddScoped<ICrmService, CrmService>(); // –абота с CRM
builder.Services.AddSingleton<ICacheService, CacheService>(); //  еширование (singleton)
builder.Services.AddSingleton<IKafkaCallsReader, KafkaCallsReader>(); // „тение звонков из Kafka (singleton)

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

app.UseSession();   // добавл€ем middleware дл€ работы с сесси€ми

// устанавливаем сопоставление маршрутов с контроллерами
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Login}/{action=Index}/{id?}");

app.Run();