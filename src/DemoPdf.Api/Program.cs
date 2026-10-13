using DemoPdf.Api.Application.Services;
using DemoPdf.Api.Data;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Infrastructure;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Licencia Community de QuestPDF (obligatoria antes de generar documentos)
QuestPDF.Settings.License = LicenseType.Community;

// Controllers + OpenAPI
builder.Services.AddControllers();
builder.Services.AddOpenApi();

// DbContext: SQL Server (Trusted Connection) o In-Memory según configuración
var useInMemory = builder.Configuration.GetValue<bool>("UseInMemory");
if (useInMemory)
{
    builder.Services.AddDbContext<AppDbContext>(options =>
        options.UseInMemoryDatabase("DemoPdfDb"));
}
else
{
    var connectionString = builder.Configuration.GetConnectionString("Default")
        ?? throw new InvalidOperationException(
            "No se encontró la cadena de conexión 'Default' en la configuración.");

    builder.Services.AddDbContext<AppDbContext>(options =>
        options.UseSqlServer(connectionString));
}

// Servicios de aplicación
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IWarehouseService, WarehouseService>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IReportService, ReportService>();

var app = builder.Build();

// Aplicar migraciones (SQL Server) o crear la base (In-Memory) + datos de demo
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

    if (db.Database.IsRelational())
        await db.Database.MigrateAsync();
    else
        db.Database.EnsureCreated();

    await DbSeeder.SeedAsync(db);
}

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference(); // UI en /scalar/v1
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
