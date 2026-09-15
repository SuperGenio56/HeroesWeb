# HeroesWeb

Práctica guiada de Web III: héroes y superpoderes con Razor Pages, Database First,
SQL Server y .NET 10.

## Base de datos

La base `HeroesDb` se crea en SSMS con el script `HeroesDb.sql` (en la raíz del proyecto).
Contiene dos tablas:

- `Heroes` (Id, Nombre, Ciudad, IdentidadSecreta)
- `SuperPoderes` (Id, Nombre, Descripcion, HeroeId)

La FK `FK_SuperPoderes_Heroes` usa `ON DELETE CASCADE`: al borrar un héroe
desaparecen sus poderes.

## Proveedor de base de datos

El proyecto funciona con SQL Server o con SQLite. Se elige con la clave
`DatabaseProvider` de `appsettings.json`.

Con **SQL Server** (`"DatabaseProvider": "SqlServer"`), la base se crea en SSMS
ejecutando `HeroesDb.sql`, y la cadena apunta a esa instancia:

```json
"DatabaseProvider": "SqlServer",
"ConnectionStrings": {
  "HeroesDb": "Server=(localdb)\\MSSQLLocalDB;Database=HeroesDb;Trusted_Connection=True;TrustServerCertificate=True;"
}
```

Con **SQLite** (`"DatabaseProvider": "Sqlite"`), la base es un archivo local que
se crea sola en el primer arranque a partir de `HeroesDb.sqlite.sql`, con el mismo
esquema y los mismos datos:

```json
"DatabaseProvider": "Sqlite",
"ConnectionStrings": { "HeroesDb": "Data Source=HeroesDb.db" }
```

El archivo `HeroesDb.db` esta excluido del repositorio: se regenera al ejecutar.

## Puesta en marcha

1. Elegir el proveedor en `appsettings.json` segun lo anterior.
2. Restaurar y ejecutar:

```powershell
dotnet restore
dotnet build
dotnet dev-certs https --trust
dotnet run --launch-profile https
```

No se ejecutan migraciones: las tablas ya existen en SQL Server.

## Cómo se generó

Paquetes y herramientas:

```powershell
dotnet add package Microsoft.EntityFrameworkCore.SqlServer --version "10.0.*"
dotnet add package Microsoft.EntityFrameworkCore.Design --version "10.0.*"
dotnet add package Microsoft.EntityFrameworkCore.Tools --version "10.0.*"
dotnet add package Microsoft.VisualStudio.Web.CodeGeneration.Design --version "10.0.*"

dotnet tool install -g dotnet-ef --version "10.0.*"
dotnet tool install -g dotnet-aspnet-codegenerator --version "10.0.*"
```

Ingeniería inversa (modelos y contexto):

```powershell
dotnet ef dbcontext scaffold "Server=(localdb)\MSSQLLocalDB;Database=HeroesDb;Trusted_Connection=True;TrustServerCertificate=True;" Microsoft.EntityFrameworkCore.SqlServer --context HeroesContext --context-dir Data --output-dir Models --no-onconfiguring --no-pluralize --data-annotations
```

Generación de los dos CRUD:

```powershell
dotnet aspnet-codegenerator razorpage -m HeroesWeb.Models.Heroes -dc HeroesWeb.Data.HeroesContext -udl -outDir Pages/Heroes --referenceScriptLibraries

dotnet aspnet-codegenerator razorpage -m HeroesWeb.Models.SuperPoderes -dc HeroesWeb.Data.HeroesContext -udl -outDir Pages/SuperPoderes --referenceScriptLibraries
```

## Ajustes manuales sobre el código generado

- `Models/SuperPoderes.cs`: `[ValidateNever]` sobre la navegación `Heroe`, para que
  el formulario no exija el objeto completo cuando solo envía `HeroeId`.
- `Pages/SuperPoderes/Create.cshtml.cs` y `Edit.cshtml.cs`: el `SelectList` usa
  `"Id"` como valor y `"Nombre"` como texto, se valida que el héroe exista y se
  reconstruye el selector antes de `return Page()`.
- `Pages/SuperPoderes/Details.cshtml.cs` y `Delete.cshtml.cs`: se añadió
  `.Include(s => s.Heroe)` para poder mostrar el nombre del héroe.
- Vistas de poderes: se muestra `Heroe.Nombre` en lugar de `Heroe.Id`.
- `Pages/Shared/_Layout.cshtml`: enlaces a Héroes y Superpoderes.
- `Pages/Heroes/Delete.cshtml`: aviso de que la eliminación arrastra los poderes.
- Los PageModel califican el tipo como `HeroesWeb.Models.Heroes` /
  `HeroesWeb.Models.SuperPoderes` porque el namespace de la carpeta de páginas
  (`HeroesWeb.Pages.Heroes`) choca con el nombre de la clase.
