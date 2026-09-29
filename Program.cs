using HeroesWeb.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;

var builder = WebApplication.CreateBuilder(args);

// "Sqlite" o "SqlServer". Se elige en appsettings.json.
var proveedor = builder.Configuration["DatabaseProvider"] ?? "SqlServer";
var connectionString = builder.Configuration.GetConnectionString("HeroesDb")
    ?? throw new InvalidOperationException("No se encontró la conexión HeroesDb.");

void Configurar(DbContextOptionsBuilder options)
{
    if (proveedor.Equals("Sqlite", StringComparison.OrdinalIgnoreCase))
        options.UseSqlite(connectionString);
    else
        options.UseSqlServer(connectionString);
}

// Las carpetas del CRUD exigen haber iniciado sesión.
// El laboratorio de Tag Helpers queda abierto a propósito.
builder.Services.AddRazorPages(options =>
{
    options.Conventions.AuthorizeFolder("/Heroes");
    options.Conventions.AuthorizeFolder("/SuperPoderes");
});

builder.Services.AddDbContext<HeroesContext>(Configurar);
builder.Services.AddDbContext<ApplicationDbContext>(Configurar);

builder.Services
    .AddDefaultIdentity<IdentityUser>(options =>
    {
        options.SignIn.RequireConfirmedAccount = false;

        options.User.RequireUniqueEmail = true;

        options.Password.RequiredLength = 8;
        options.Password.RequireDigit = true;
        options.Password.RequireLowercase = true;
        options.Password.RequireUppercase = true;
        options.Password.RequireNonAlphanumeric = true;
    })
    .AddEntityFrameworkStores<ApplicationDbContext>();

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
    // Ambos contextos comparten el mismo archivo SQLite, así que EnsureCreated()
    // no basta para el segundo: crea la base solo si aún no existe. Se piden
    // explícitamente las tablas de Identity. En SQL Server esto lo hace la migración.
    var identityDb = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    try
    {
        var creador = (RelationalDatabaseCreator)identityDb.Database
            .GetService<IDatabaseCreator>();
        creador.CreateTables();
    }
    catch (Exception)
    {
        // Las tablas de Identity ya existían.
    }
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();
app.MapRazorPages().WithStaticAssets();
app.Run();
