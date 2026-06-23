# Tutorial: Despliegue de una Aplicación ASP.NET Core con EF Core y Docker

## Introducción

Este documento tiene el paso a paso en la creación y despliegue de una aplicación **ASP.NET Core** con **Entity Framework Core** y **PostgreSQL** usando Docker y Docker Compose.

---

## Requisitos Previos

- **Docker** y **Docker Compose** instalados. Consultá la [documentación oficial de Docker](https://docs.docker.com/get-docker/).
- **SDK de .NET 10** instalado. Descargalo de [dotnet.microsoft.com](https://dotnet.microsoft.com/download).
- Conocimientos básicos de C# (no excluyente, el tutorial es autoexplicativo).

### Instalar la herramienta `dotnet-ef`

`dotnet ef` es una herramienta de línea de comandos separada del SDK que se usa para generar migrations. Instalala globalmente con:

> **Podés copiar todo este bloque y pegarlo directamente en tu terminal.**
```sh
dotnet tool install --global dotnet-ef
```

Después de instalarla, recargá el PATH para que la terminal la reconozca:

> **Podés copiar todo este bloque y pegarlo directamente en tu terminal.**
```sh
source ~/.bashrc
```

> Si usás Zsh en lugar de Bash, reemplazá `.bashrc` por `.zshrc`.

Si el comando `dotnet ef` sigue sin encontrarse después de recargar, agregá el directorio de herramientas globales de .NET al PATH manualmente. Editá tu `~/.bashrc` y agregá esta línea al final:

> **Podés copiar todo este bloque y pegarlo directamente en tu archivo `~/.bashrc`.**
```sh
export PATH="$PATH:$HOME/.dotnet/tools"
```

Luego recargá:

> **Podés copiar todo este bloque y pegarlo directamente en tu terminal.**
```sh
source ~/.bashrc
```

Verificá que quedó instalada correctamente:

> **Podés copiar todo este bloque y pegarlo directamente en tu terminal.**
```sh
dotnet ef --version
```

### Herramientas útiles

- [Documentación de EF Core](https://learn.microsoft.com/en-us/ef/core/)
- [Documentación de ASP.NET Core](https://learn.microsoft.com/en-us/aspnet/core/)
- [CoreAdmin (panel de administración)](https://github.com/edandersen/core-admin)

---

## 1. Creación del Proyecto

Creá el proyecto con la plantilla de ASP.NET Core Web API. Esto genera automáticamente la estructura base, el `.csproj` y los archivos de configuración.

> **Podés copiar todo este bloque y pegarlo directamente en tu terminal.**
```sh
dotnet new webapi -n PracticoOrm
cd PracticoOrm
```

La estructura final del proyecto es la siguiente:

```
PracticoOrm/
├── Models/
│   ├── DetalleReceta.cs
│   ├── DetalleVenta.cs
│   ├── Ingrediente.cs
│   ├── Mostrador.cs
│   ├── Producto.cs
│   ├── PuntoDeVenta.cs
│   ├── Receta.cs
│   ├── TipoProducto.cs
│   └── Venta.cs
├── Seeds/
│   ├── DataSeeder.cs
│   ├── detallesReceta.json
│   ├── detallesVenta.json
│   ├── ingredientes.json
│   ├── mostradores.json
│   ├── productos.json
│   ├── puntosDeVenta.json
│   ├── recetas.json
│   ├── tipoProductos.json
│   └── ventas.json
├── Migrations/          ← generado automáticamente por EF Core
├── Properties/
│   └── launchSettings.json
├── AppDbContext.cs
├── Program.cs
├── PracticoOrm.csproj
├── appsettings.json
├── appsettings.Development.json
├── Dockerfile
├── docker-compose.yml
└── .env.db
```

---

## 2. Instalación de Paquetes

Instalá las dependencias necesarias con los siguientes comandos. Cada uno agrega una referencia al `.csproj` automáticamente.

> **Podés copiar todo este bloque y pegarlo directamente en tu terminal.**
```sh
dotnet add package Microsoft.EntityFrameworkCore
dotnet add package Npgsql.EntityFrameworkCore.PostgreSQL
dotnet add package Microsoft.EntityFrameworkCore.Design
dotnet add package EFCore.NamingConventions
dotnet add package CoreAdmin
```

**Paquetes instalados:**
- `Microsoft.EntityFrameworkCore` — ORM principal.
- `Npgsql.EntityFrameworkCore.PostgreSQL` — driver para conectar EF Core con PostgreSQL.
- `Microsoft.EntityFrameworkCore.Design` — herramientas necesarias para generar migrations con `dotnet ef`.
- `EFCore.NamingConventions` — convenciones de nombres automáticas que convierten las tablas y columnas a snake_case en PostgreSQL (ej: `TipoProducto` → `tipo_producto`).
- `CoreAdmin` — panel de administración web automático, similar al `/admin` de Django.

### Verificar que el proyecto compila

> **Podés copiar todo este bloque y pegarlo directamente en tu terminal.**
```sh
dotnet build
```

Si no hay errores, el entorno está listo para continuar.

---

## 3. Variables de Entorno

Creá el archivo `.env.db` a partir del `.env.example.db` y completá los valores con tus credenciales. Nunca commitees el `.env.db` a git.

> **Podés copiar todo este bloque y pegarlo directamente en tu archivo `.env.db`.**
```conf
POSTGRES_DB=fabrica_pastas
POSTGRES_USER=postgres
POSTGRES_PASSWORD=admin
POSTGRES_HOST=db
POSTGRES_PORT=5432
PGUSER=postgres
```

> **Nota:** `POSTGRES_HOST=db` hace referencia al nombre del servicio `db` definido en `docker-compose.yml`. La app y la base de datos se comunican por la red interna de Docker, no por localhost.

---

## 4. Dockerfile

El `Dockerfile` construye la imagen de la aplicación usando un proceso en dos etapas (_multi-stage build_): primero compila la app con el SDK completo, luego genera una imagen más liviana con solo el runtime.

> **Podés copiar todo este bloque y pegarlo directamente en tu archivo `Dockerfile`.**
```dockerfile
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY PracticoOrm.csproj .
RUN dotnet restore
COPY . .
RUN dotnet publish -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .
ENV POSTGRES_DISABLE_GSS=true
ENTRYPOINT ["dotnet", "PracticoOrm.dll"]
```

**Etapas:**
- `build` — Copia primero solo el `.csproj` y restaura los paquetes NuGet (esto permite que Docker cachee esa capa y no vuelva a bajar paquetes si el `.csproj` no cambió). Luego copia el resto del código y publica en modo Release.
- `runtime` — Imagen mínima (~220MB vs ~900MB del SDK) que solo contiene lo necesario para ejecutar la app. Todo lo de la etapa anterior queda descartado.

> `ENV POSTGRES_DISABLE_GSS=true` evita un error de autenticación Kerberos que aparece en entornos Linux con el driver de Npgsql. En los logs aparece como `Cannot load library libgssapi_krb5.so.2` pero la app funciona igual gracias a esta variable.

---

## 5. Docker Compose

El archivo `docker-compose.yml` orquesta los dos servicios: la base de datos PostgreSQL y el backend ASP.NET Core.

> **Podés copiar todo este bloque y pegarlo directamente en tu archivo `docker-compose.yml`.**
```yml
services:
  db:
    image: postgres:alpine
    env_file:
      - .env.db
    ports:
      - "5434:5432"
    healthcheck:
      test: ["CMD-SHELL", "pg_isready"]
      interval: 10s
      timeout: 2s
      retries: 5
    volumes:
      - postgres-db:/var/lib/postgresql
    networks:
      - net

  backend:
    build: ./
    env_file:
      - .env.db
    environment:
      - ASPNETCORE_ENVIRONMENT=Development
    ports:
      - "8000:8080"
    depends_on:
      db:
        condition: service_healthy
    networks:
      - net

networks:
  net:

volumes:
  postgres-db:
```

**Puntos importantes:**
- El puerto `5434` en el host evita colisionar con una PostgreSQL local que corra en el `5432` estándar.
- El backend expone el puerto `8000` en el host (mapeado al `8080` interno de Kestrel).
- `depends_on` con `condition: service_healthy` garantiza que el backend no arranca hasta que PostgreSQL esté listo para recibir conexiones, usando el healthcheck definido.
- **No hay servicio `manage` separado**: las migrations y el seeding se ejecutan automáticamente al iniciar el backend.

---

## 6. Configuración de la Aplicación

### `appsettings.json`

Configuración base que aplica en todos los entornos.

> **Podés copiar todo este bloque y pegarlo directamente en tu archivo `appsettings.json`.**
```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*",
  "ConnectionStrings": {
    "DefaultConnection": ""
  }
}
```

### `appsettings.Development.json`

Se carga encima del base cuando `ASPNETCORE_ENVIRONMENT=Development`, sobreescribiendo solo las claves que aparezcan acá. Permite tener configuración diferente entre desarrollo y producción sin tocar el archivo base.

> **Podés copiar todo este bloque y pegarlo directamente en tu archivo `appsettings.Development.json`.**
```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  }
}
```

---

## 7. Modelos

Los modelos son clases C# que representan las tablas de la base de datos. EF Core lee estas clases y genera el esquema SQL automáticamente mediante migrations. Creá la carpeta `Models/` y agregá un archivo por cada entidad.

### `Models/TipoProducto.cs`

> **Podés copiar todo este bloque y pegarlo directamente en tu archivo `Models/TipoProducto.cs`.**
```csharp
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PracticoOrm.Models;

public class TipoProducto
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int TipoProductoId { get; set; }

    [MaxLength(100)]
    public string Nombre { get; set; } = null!;

    public ICollection<Producto> Productos { get; set; } = [];

    public override string ToString() => Nombre;
}
```

### `Models/Ingrediente.cs`

> **Podés copiar todo este bloque y pegarlo directamente en tu archivo `Models/Ingrediente.cs`.**
```csharp
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PracticoOrm.Models;

public class Ingrediente
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int IngredienteId { get; set; }

    [MaxLength(100)]
    public string Nombre { get; set; } = null!;

    [Column(TypeName = "decimal(10,2)")]
    public decimal Costo { get; set; }

    public ICollection<DetalleReceta> DetalleRecetas { get; set; } = [];

    public override string ToString() => Nombre;
}
```

### `Models/Receta.cs`

> **Podés copiar todo este bloque y pegarlo directamente en tu archivo `Models/Receta.cs`.**
```csharp
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PracticoOrm.Models;

public class Receta
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int RecetaId { get; set; }

    [MaxLength(100)]
    public string Nombre { get; set; } = null!;

    public ICollection<DetalleReceta> DetalleRecetas { get; set; } = [];

    public ICollection<Producto> Productos { get; set; } = [];

    public override string ToString() => Nombre;
}
```

### `Models/DetalleReceta.cs`

Tabla intermedia entre `Receta` e `Ingrediente`, con la cantidad de cada ingrediente por receta.

> **Podés copiar todo este bloque y pegarlo directamente en tu archivo `Models/DetalleReceta.cs`.**
```csharp
using System.ComponentModel.DataAnnotations.Schema;

namespace PracticoOrm.Models;

public class DetalleReceta
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int DetalleRecetaId { get; set; }

    [Column(TypeName = "decimal(10,2)")]
    public decimal Cantidad { get; set; }

    public int RecetaId { get; set; }
    public Receta Receta { get; set; } = null!;

    public int IngredienteId { get; set; }
    public Ingrediente Ingrediente { get; set; } = null!;
}
```

### `Models/Producto.cs`

> **Podés copiar todo este bloque y pegarlo directamente en tu archivo `Models/Producto.cs`.**
```csharp
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PracticoOrm.Models;

public class Producto
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int ProductoId { get; set; }

    [MaxLength(100)]
    public string Nombre { get; set; } = null!;

    [MaxLength(100)]
    public string? Descripcion { get; set; }

    [Column(TypeName = "decimal(10,2)")]
    public decimal PorcentajeDeGanancia { get; set; }

    public int RecetaId { get; set; }
    public Receta Receta { get; set; } = null!;

    public int TipoProductoId { get; set; }
    public TipoProducto TipoProducto { get; set; } = null!;

    public ICollection<DetalleVenta> DetalleVentas { get; set; } = [];

    public override string ToString() => Nombre;
}
```

### `Models/PuntoDeVenta.cs`

> **Podés copiar todo este bloque y pegarlo directamente en tu archivo `Models/PuntoDeVenta.cs`.**
```csharp
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PracticoOrm.Models;

public class PuntoDeVenta
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int PuntoDeVentaId { get; set; }

    [MaxLength(100)]
    public string Nombre { get; set; } = null!;

    public ICollection<Mostrador> Mostradores { get; set; } = [];

    public override string ToString() => Nombre;
}
```

### `Models/Mostrador.cs`

> **Podés copiar todo este bloque y pegarlo directamente en tu archivo `Models/Mostrador.cs`.**
```csharp
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PracticoOrm.Models;

public class Mostrador
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int MostradorId { get; set; }

    [MaxLength(100)]
    public string Nombre { get; set; } = null!;

    public int PuntoDeVentaId { get; set; }
    public PuntoDeVenta PuntoDeVenta { get; set; } = null!;

    public ICollection<Venta> Ventas { get; set; } = [];

    public override string ToString() => Nombre;
}
```

### `Models/Venta.cs`

> **Podés copiar todo este bloque y pegarlo directamente en tu archivo `Models/Venta.cs`.**
```csharp
using System.ComponentModel.DataAnnotations.Schema;

namespace PracticoOrm.Models;

public class Venta
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int VentaId { get; set; }

    public DateTime FechaDeVenta { get; set; } = DateTime.UtcNow;

    public int MostradorId { get; set; }
    public Mostrador Mostrador { get; set; } = null!;

    public ICollection<DetalleVenta> DetalleVentas { get; set; } = [];
}
```

### `Models/DetalleVenta.cs`

> **Podés copiar todo este bloque y pegarlo directamente en tu archivo `Models/DetalleVenta.cs`.**
```csharp
using System.ComponentModel.DataAnnotations.Schema;

namespace PracticoOrm.Models;

public class DetalleVenta
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int DetalleVentaId { get; set; }

    public int Cantidad { get; set; }

    public int VentaId { get; set; }
    public Venta Venta { get; set; } = null!;

    public int ProductoId { get; set; }
    public Producto Producto { get; set; } = null!;
}
```

---

## 8. DbContext

El `AppDbContext` es la clase central de EF Core: representa una sesión con la base de datos y expone todas las tablas como propiedades `DbSet<T>`. Equivale al `models.py` + configuración de base de datos de Django.

> **Podés copiar todo este bloque y pegarlo directamente en tu archivo `AppDbContext.cs`.**
```csharp
using Microsoft.EntityFrameworkCore;
using PracticoOrm.Models;

namespace PracticoOrm;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) {}

    public DbSet<DetalleReceta> DetalleRecetas => Set<DetalleReceta>();
    public DbSet<DetalleVenta> DetalleVentas => Set<DetalleVenta>();
    public DbSet<Ingrediente> Ingredientes => Set<Ingrediente>();
    public DbSet<Mostrador> Mostradores => Set<Mostrador>();
    public DbSet<Producto> Productos => Set<Producto>();
    public DbSet<PuntoDeVenta> PuntoDeVentas => Set<PuntoDeVenta>();
    public DbSet<Receta> Recetas => Set<Receta>();
    public DbSet<TipoProducto> TipoProductos => Set<TipoProducto>();
    public DbSet<Venta> Ventas => Set<Venta>();
}
```

---

## 9. Datos Iniciales (Seeds)

Los seeds cargan datos de ejemplo en la base de datos la primera vez que la app arranca. Se leen desde archivos JSON en la carpeta `Seeds/`. Creá la carpeta y agregá los siguientes archivos:

### `Seeds/puntosDeVenta.json`

> **Podés copiar todo este bloque y pegarlo directamente en `Seeds/puntosDeVenta.json`.**
```json
[
  { "Nombre": "Sucursal Centro" },
  { "Nombre": "Sucursal Norte" },
  { "Nombre": "Sucursal Sur" }
]
```

### `Seeds/mostradores.json`

> **Podés copiar todo este bloque y pegarlo directamente en `Seeds/mostradores.json`.**
```json
[
  { "Nombre": "Mostrador A", "PuntoDeVentaNombre": "Sucursal Centro" },
  { "Nombre": "Mostrador B", "PuntoDeVentaNombre": "Sucursal Centro" },
  { "Nombre": "Mostrador C", "PuntoDeVentaNombre": "Sucursal Norte" },
  { "Nombre": "Mostrador D", "PuntoDeVentaNombre": "Sucursal Sur" }
]
```

### `Seeds/ingredientes.json`

> **Podés copiar todo este bloque y pegarlo directamente en `Seeds/ingredientes.json`.**
```json
[
  { "Nombre": "Harina 000", "Costo": 120.00 },
  { "Nombre": "Harina Integral", "Costo": 150.00 },
  { "Nombre": "Huevo", "Costo": 80.00 },
  { "Nombre": "Sal fina", "Costo": 20.00 },
  { "Nombre": "Papa", "Costo": 60.00 },
  { "Nombre": "Ricota", "Costo": 200.00 },
  { "Nombre": "Nuez Moscada", "Costo": 90.00 },
  { "Nombre": "Aceite de oliva", "Costo": 350.00 }
]
```

### `Seeds/tipoProductos.json`

> **Podés copiar todo este bloque y pegarlo directamente en `Seeds/tipoProductos.json`.**
```json
[
  { "Nombre": "Pasta Larga" },
  { "Nombre": "Pasta Corta" },
  { "Nombre": "Pasta Rellena" },
  { "Nombre": "Al peso" }
]
```

### `Seeds/recetas.json`

> **Podés copiar todo este bloque y pegarlo directamente en `Seeds/recetas.json`.**
```json
[
  { "Nombre": "Receta Tallarines Clásicos" },
  { "Nombre": "Receta Ñoquis de Papa" },
  { "Nombre": "Receta Ravioles de Ricota" },
  { "Nombre": "Receta Fetuccini Integral" }
]
```

### `Seeds/detallesReceta.json`

> **Podés copiar todo este bloque y pegarlo directamente en `Seeds/detallesReceta.json`.**
```json
[
  { "RecetaIndex": 0, "IngredienteNombre": "Harina 000", "Cantidad": 0.500 },
  { "RecetaIndex": 0, "IngredienteNombre": "Huevo", "Cantidad": 3.000 },
  { "RecetaIndex": 0, "IngredienteNombre": "Sal fina", "Cantidad": 0.010 },
  { "RecetaIndex": 1, "IngredienteNombre": "Papa", "Cantidad": 1.000 },
  { "RecetaIndex": 1, "IngredienteNombre": "Harina 000", "Cantidad": 0.250 },
  { "RecetaIndex": 1, "IngredienteNombre": "Huevo", "Cantidad": 1.000 },
  { "RecetaIndex": 1, "IngredienteNombre": "Sal fina", "Cantidad": 0.010 },
  { "RecetaIndex": 2, "IngredienteNombre": "Harina 000", "Cantidad": 0.400 },
  { "RecetaIndex": 2, "IngredienteNombre": "Huevo", "Cantidad": 2.000 },
  { "RecetaIndex": 2, "IngredienteNombre": "Ricota", "Cantidad": 0.300 },
  { "RecetaIndex": 2, "IngredienteNombre": "Nuez Moscada", "Cantidad": 0.005 },
  { "RecetaIndex": 3, "IngredienteNombre": "Harina Integral", "Cantidad": 0.500 },
  { "RecetaIndex": 3, "IngredienteNombre": "Huevo", "Cantidad": 2.000 },
  { "RecetaIndex": 3, "IngredienteNombre": "Aceite de oliva", "Cantidad": 0.020 },
  { "RecetaIndex": 3, "IngredienteNombre": "Sal fina", "Cantidad": 0.010 }
]
```

> `RecetaIndex` es la posición [0] de la receta en el array de `recetas.json`. El seeder resuelve el ID real en tiempo de ejecución.

### `Seeds/productos.json`

> **Podés copiar todo este bloque y pegarlo directamente en `Seeds/productos.json`.**
```json
[
  { "Nombre": "Tallarines 500g",    "RecetaNombre": "Receta Tallarines Clásicos", "TipoProductoNombre": "Pasta Larga",   "PorcentajeDeGanancia": 65.00, "Descripcion": "Pasta fresca al huevo, corte fino"    },
  { "Nombre": "Fetuccini Integral", "RecetaNombre": "Receta Fetuccini Integral",  "TipoProductoNombre": "Pasta Larga",   "PorcentajeDeGanancia": 60.00, "Descripcion": "Pasta integral con aceite de oliva"   },
  { "Nombre": "Ñoquis de Papa 500g","RecetaNombre": "Receta Ñoquis de Papa",      "TipoProductoNombre": "Pasta Corta",   "PorcentajeDeGanancia": 70.00, "Descripcion": "Ñoquis artesanales de papa"           },
  { "Nombre": "Ravioles de Ricota", "RecetaNombre": "Receta Ravioles de Ricota",  "TipoProductoNombre": "Pasta Rellena", "PorcentajeDeGanancia": 75.00, "Descripcion": "Rellenos con ricota y nuez moscada"   },
  { "Nombre": "Tallarines al peso", "RecetaNombre": "Receta Tallarines Clásicos", "TipoProductoNombre": "Al peso",       "PorcentajeDeGanancia": 55.00, "Descripcion": null                                   }
]
```

### `Seeds/ventas.json`

> **Podés copiar todo este bloque y pegarlo directamente en `Seeds/ventas.json`.**
```json
[
  { "FechaDeVenta": "2025-05-01T09:30:00Z", "MostradorNombre": "Mostrador A" },
  { "FechaDeVenta": "2025-05-01T11:15:00Z", "MostradorNombre": "Mostrador B" },
  { "FechaDeVenta": "2025-05-02T10:00:00Z", "MostradorNombre": "Mostrador C" },
  { "FechaDeVenta": "2025-05-03T14:45:00Z", "MostradorNombre": "Mostrador D" },
  { "FechaDeVenta": "2025-05-03T16:20:00Z", "MostradorNombre": "Mostrador A" }
]
```

### `Seeds/detallesVenta.json`

> **Podés copiar todo este bloque y pegarlo directamente en `Seeds/detallesVenta.json`.**
```json
[
  { "VentaIndex": 0, "ProductoNombre": "Tallarines 500g", "Cantidad": 3 },
  { "VentaIndex": 0, "ProductoNombre": "Ñoquis de Papa 500g", "Cantidad": 2 },
  { "VentaIndex": 1, "ProductoNombre": "Ravioles de Ricota", "Cantidad": 1 },
  { "VentaIndex": 1, "ProductoNombre": "Fetuccini Integral", "Cantidad": 2 },
  { "VentaIndex": 2, "ProductoNombre": "Tallarines 500g", "Cantidad": 5 },
  { "VentaIndex": 3, "ProductoNombre": "Tallarines al peso", "Cantidad": 1 },
  { "VentaIndex": 4, "ProductoNombre": "Ñoquis de Papa 500g", "Cantidad": 4 },
  { "VentaIndex": 4, "ProductoNombre": "Ravioles de Ricota", "Cantidad": 2 }
]
```

### `Seeds/DataSeeder.cs`

Clase que orquesta la carga de todos los JSON en el orden correcto respetando las foreign keys. Se ejecuta automáticamente al iniciar la app.

> **Podés copiar todo este bloque y pegarlo directamente en tu archivo `Seeds/DataSeeder.cs`.**
```csharp
namespace PracticoOrm.Seeds;

using System.Text.Json;
using PracticoOrm;
using PracticoOrm.Models;

public class DataSeeder
{
    public static async Task SeedAsync (AppDbContext context)
    {
        if (!context.PuntoDeVentas.Any()) await SeedEntidad<PuntoDeVenta>("Seeds/puntosDeVenta.json", context);
        if (!context.Mostradores.Any()) await SeedMostradores(context);
        if (!context.Ingredientes.Any()) await SeedEntidad<Ingrediente>("Seeds/ingredientes.json", context);
        if (!context.TipoProductos.Any()) await SeedEntidad<TipoProducto>("Seeds/tipoProductos.json", context);
        if (!context.Recetas.Any()) await SeedEntidad<Receta>("Seeds/recetas.json", context);
        if (!context.DetalleRecetas.Any()) await SeedDetallesRecetas(context);
        if (!context.Productos.Any()) await SeedProductos(context);
        if (!context.Ventas.Any()) await SeedVentas(context);
        if (!context.DetalleVentas.Any()) await SeedDetallesVentas(context);
    }
    
    public static async Task SeedEntidad<T>(string rutaArchivo, AppDbContext context) where T: class
    {
        var json = await File.ReadAllTextAsync(rutaArchivo);
        var datos = JsonSerializer.Deserialize<List<T>>(json);
        await context.Set<T>().AddRangeAsync(datos!);
        await context.SaveChangesAsync();
    }

    public static async Task SeedMostradores(AppDbContext context)
    {
        var json = await File.ReadAllTextAsync("Seeds/mostradores.json");
        var datos = JsonSerializer.Deserialize<List<JsonElement>>(json);

        foreach (var dato in datos!)
        {
            var puntoDeVentaNombre = dato.GetProperty("PuntoDeVentaNombre").GetString();
            var puntoDeVentaId = context.PuntoDeVentas.First(p => p.Nombre == puntoDeVentaNombre).PuntoDeVentaId;

            var mostrador = new Mostrador
            {
              Nombre = dato.GetProperty("Nombre").GetString()!,
              PuntoDeVentaId = puntoDeVentaId  
            };         

            await context.Mostradores.AddAsync(mostrador);
        }
        await context.SaveChangesAsync();
    }

    public static async Task SeedVentas(AppDbContext context)
    {
        var json = await File.ReadAllTextAsync("Seeds/ventas.json");
        var datos = JsonSerializer.Deserialize<List<JsonElement>>(json);

        foreach (var dato in datos!)
        {
            var mostradorNombre = dato.GetProperty("MostradorNombre").GetString();
            var mostradorId = context.Mostradores.First(m => m.Nombre == mostradorNombre).MostradorId;

            var venta = new Venta
            {
                FechaDeVenta = dato.GetProperty("FechaDeVenta").GetDateTime(),
                MostradorId = mostradorId
            };
            await context.Ventas.AddAsync(venta);
        }
        await context.SaveChangesAsync();
    }


    public static async Task SeedDetallesVentas(AppDbContext context)
    {
        var json = await File.ReadAllTextAsync("Seeds/detallesVenta.json");
        var datos = JsonSerializer.Deserialize<List<JsonElement>>(json);
        var ventas = context.Ventas.ToList();

        foreach (var dato in datos!)
        {
            var ventaIndex = dato.GetProperty("VentaIndex").GetInt16();
            var venta = ventas[ventaIndex];
            var ventaId = venta.VentaId;

            var productoNombre = dato.GetProperty("ProductoNombre").GetString();
            var productoId = context.Productos.First(p => p.Nombre == productoNombre).ProductoId;

            var detalleVenta = new DetalleVenta
            {
             VentaId = ventaId,
             ProductoId = productoId,
             Cantidad = dato.GetProperty("Cantidad").GetInt16()   
            };
            await context.AddAsync(detalleVenta);
        }
        await context.SaveChangesAsync();
    }


    public static async Task SeedProductos(AppDbContext context)
    {
        var json = await File.ReadAllTextAsync("Seeds/productos.json");
        var datos = JsonSerializer.Deserialize<List<JsonElement>>(json);

        foreach (var dato in datos!)
        {
            var recetaNombre = dato.GetProperty("RecetaNombre").GetString();
            var recetaId = context.Recetas.First(r => r.Nombre == recetaNombre).RecetaId;

            var tipoProductoNombre = dato.GetProperty("TipoProductoNombre").GetString();
            var tipoProductoId = context.TipoProductos.First(tp => tp.Nombre == tipoProductoNombre).TipoProductoId;

            var producto = new Producto
            {
                Nombre = dato.GetProperty("Nombre").GetString()!,
                RecetaId = recetaId,
                TipoProductoId = tipoProductoId,
                PorcentajeDeGanancia = dato.GetProperty("PorcentajeDeGanancia").GetDecimal(),
                Descripcion = dato.GetProperty("Descripcion").GetString()
            };
            await context.AddAsync(producto); 
        }
        await context.SaveChangesAsync();
    }


    public static async Task SeedDetallesRecetas(AppDbContext context)
    {
        var json = await File.ReadAllTextAsync("Seeds/detallesReceta.json");
        var datos = JsonSerializer.Deserialize<List<JsonElement>>(json);
        var recetas = context.Recetas.ToList();

        foreach (var dato in datos!)
        {
            var recetaIndex = dato.GetProperty("RecetaIndex").GetInt16();
            var receta = recetas[recetaIndex];
            var recetaID = receta.RecetaId;

            var ingredienteNombre = dato.GetProperty("IngredienteNombre").GetString();
            var ingredienteId = context.Ingredientes.First(i => i.Nombre == ingredienteNombre).IngredienteId;

            var detallesReceta = new DetalleReceta
            {
                RecetaId = recetaID,
                IngredienteId = ingredienteId,
                Cantidad = dato.GetProperty("Cantidad").GetDecimal()
            };
            await context.AddAsync(detallesReceta);
        }   
        await context.SaveChangesAsync();
    }
}
```

---

## 10. Punto de Entrada de la Aplicación

`Program.cs` configura y arranca la app. Aquí se registra EF Core, se conecta a la base de datos leyendo las variables de entorno, y se ejecutan las migrations y el seeding automáticamente en cada inicio.

> **Podés copiar todo este bloque y pegarlo directamente en tu archivo `Program.cs`.**
```csharp
using Microsoft.EntityFrameworkCore;
using PracticoOrm;
using PracticoOrm.Seeds;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
var host = Environment.GetEnvironmentVariable("POSTGRES_HOST") ?? "localhost";
var port = Environment.GetEnvironmentVariable("POSTGRES_PORT") ?? "5434";
var db = Environment.GetEnvironmentVariable("POSTGRES_DB") ?? "fabrica_pastas";
var user = Environment.GetEnvironmentVariable("POSTGRES_USER") ?? "postgres";
var password = Environment.GetEnvironmentVariable("POSTGRES_PASSWORD") ?? "admin";
var connectionString = $"Host={host};Port={port};Database={db};Username={user};Password={password}";
builder.Services.AddDbContext<AppDbContext>(options => 
        options.UseNpgsql(connectionString)
        .UseSnakeCaseNamingConvention());
builder.Services.AddCoreAdmin();
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}
app.UseHttpsRedirection();
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    dbContext.Database.Migrate();
    await DataSeeder.SeedAsync(dbContext);
}
app.UseStaticFiles();
app.MapDefaultControllerRoute();
await app.RunAsync();
```

**Puntos clave:**
- `.UseSnakeCaseNamingConvention()` convierte automáticamente todos los nombres de tablas y columnas a snake_case en PostgreSQL (`TipoProducto` → `tipo_producto`, `RecetaId` → `receta_id`).
- `dbContext.Database.Migrate()` aplica todas las migrations pendientes al arrancar. EF Core lleva registro en la tabla `__EFMigrationsHistory` y nunca aplica dos veces la misma.
- `DataSeeder.SeedAsync` carga los datos iniciales solo si las tablas están vacías, por lo que es seguro correr siempre.

---

## 11. Migrations

Las migrations son archivos de código que describen los cambios en el esquema de la base de datos. Se generan **localmente** con el SDK y se aplican **en Docker** al hacer el build.

### Generar la migration inicial

Una vez que tenés todos los modelos y el `AppDbContext` definidos:

> **Podés copiar todo este bloque y pegarlo directamente en tu terminal.**
```sh
dotnet ef migrations add Initial
```

Esto crea la carpeta `Migrations/` con los archivos que describen el esquema completo.

### Levantar la app

> **Podés copiar todo este bloque y pegarlo directamente en tu terminal.**
```sh
docker compose up --build
```

Este comando compila la imagen con el código actualizado y levanta los servicios. Al arrancar, el backend aplica automáticamente todas las migrations pendientes y ejecuta el seeding.

### Flujo para cuando modificás un modelo

Cada vez que cambiás un modelo (agregás una propiedad, una tabla, una relación), el flujo es siempre el mismo:

> **Podés copiar todo este bloque y pegarlo directamente en tu terminal.**
```sh
dotnet ef migrations add NombreDescriptivoDeLaCambio
docker compose up --build
```

> **Nota:** No es necesario correr `dotnet ef database update` de forma manual. El propio `Program.cs` aplica todas las migrations pendientes al iniciar la app dentro del contenedor.

### Agregar una nueva entidad

Cuando agregás una clase nueva al proyecto, los pasos son:

1. Crear la clase en `Models/`
2. Agregar el `DbSet<T>` en `AppDbContext.cs`
3. Crear el JSON de seed en `Seeds/` (si querés datos iniciales)
4. Agregar el método de seed en `DataSeeder.cs` y llamarlo desde `SeedAsync`
5. `dotnet ef migrations add NombreMigration`
6. `docker compose up --build`

### Correr el proyecto localmente (sin Docker)

> **Podés copiar todo este bloque y pegarlo directamente en tu terminal.**
```sh
dotnet run
```

---

## 12. Levantar la Aplicación

### Primera vez (o tras agregar una migration)

> **Podés copiar todo este bloque y pegarlo directamente en tu terminal.**
```sh
docker compose up --build
```

Al ejecutar este comando:
1. Docker compila la imagen con el código actualizado.
2. Levanta PostgreSQL y espera a que esté saludable (healthcheck).
3. Levanta el backend, que aplica las migrations pendientes y carga los datos iniciales.

### Arranques posteriores (sin cambios en el código)

> **Podés copiar todo este bloque y pegarlo directamente en tu terminal.**
```sh
docker compose up -d
```

Accedé al panel de administración en [http://localhost:8000/coreadmin](http://localhost:8000/coreadmin)

Ver los logs en tiempo real:

> **Podés copiar todo este bloque y pegarlo directamente en tu terminal.**
```sh
docker compose logs -f
```

---

## 13. Comandos Útiles

- **Levantar con rebuild (tras cambios de código o migrations):**
  ```sh
  docker compose up --build
  ```

- **Levantar en segundo plano (sin cambios):**
  ```sh
  docker compose up -d
  ```

- **Correr localmente sin Docker:**
  ```sh
  dotnet run
  ```

- **Verificar que el proyecto compila:**
  ```sh
  dotnet build
  ```

- **Agregar una nueva migration:**
  ```sh
  dotnet ef migrations add NombreDeLaMigration
  ```

- **Ver logs en tiempo real:**
  ```sh
  docker compose logs -f
  ```

- **Detener los contenedores:**
  ```sh
  docker compose down
  ```

- **Detener y eliminar contenedores, volúmenes e imágenes:**
  ```sh
  docker compose down -v --remove-orphans --rmi all
  ```

- **Limpiar todos los recursos de Docker no utilizados:**
  ```sh
  docker system prune -a
  ```

---
