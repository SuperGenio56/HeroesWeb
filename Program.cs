using Microsoft.EntityFrameworkCore;
using HeroesWeb.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();

// "Sqlite" o "SqlServer". Se elige en appsettings.json.
var proveedor = builder.Configuration["DatabaseProvider"] ?? "SqlServer";
var cadena = builder.Configuration.GetConnectionString("HeroesDb")
    ?? throw new InvalidOperationException("Falta la conexión HeroesDb.");

builder.Services.AddDbContext<HeroesContext>(options =>
{
    if (proveedor.Equals("Sqlite", StringComparison.OrdinalIgnoreCase))
        options.UseSqlite(cadena);
    else
        options.UseSqlServer(cadena);
});

var app = builder.Build();

// Con SQLite la base es un archivo local: si todavía no existe, se crea
// ejecutando el mismo DDL del script, igual que se haría en SSMS.
if (proveedor.Equals("Sqlite", StringComparison.OrdinalIgnoreCase))
{
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<HeroesContext>();
    if (!db.Database.CanConnect() || !db.Heroes.Any())
    {
        db.Database.EnsureCreated();
        if (!db.Heroes.Any())
        {
            var ddl = File.ReadAllText(Path.Combine(AppContext.BaseDirectory, "HeroesDb.sqlite.sql"));
            var soloInserts = string.Join(";\n",
                ddl.Split(';').Where(s => s.TrimStart().StartsWith("INSERT", StringComparison.OrdinalIgnoreCase)));
            if (!string.IsNullOrWhiteSpace(soloInserts))
                db.Database.ExecuteSqlRaw(soloInserts);
        }
    }
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthorization();
app.MapStaticAssets();
app.MapRazorPages().WithStaticAssets();
app.Run();
