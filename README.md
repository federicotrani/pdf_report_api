# DemoPdf.Api

API REST de gestión básica de inventario con generación de reportes en PDF, construida con **.NET 10 Web API** (controller-based), **Entity Framework Core** (SQL Server) y **QuestPDF**.

## Características

- CRUD completo de **Usuarios**, **Depósitos** y **Productos** con DTOs y validaciones.
- Reportes PDF descargables (`application/pdf`) con QuestPDF (licencia Community):
  - **Listado de productos** (A4 horizontal): encabezado con título, fecha/hora de emisión y depósito seleccionado; tabla con zebra striping, columnas SKU / Nombre / Depósito / Precio / Stock / Total Valorizado y fila de totales; footer con "Página X de Y" y aviso de confidencialidad.
  - **Listado de usuarios** (A4 vertical): tabla con Id / Nombre Completo / Usuario / Email / Rol / Estado (Activo/Inactivo con color); footer con paginación.
- Persistencia en **SQL Server** con Trusted Connection y **migraciones EF Core** aplicadas automáticamente al iniciar.
- Proveedor **In-Memory** alternativo para pruebas rápidas (flag de configuración).
- Documentación de API con **OpenAPI nativo + Scalar**.

## Requisitos

| Herramienta | Versión |
|---|---|
| .NET SDK | 10.0+ |
| SQL Server | Express / LocalDB / instancia completa (Trusted Connection) |
| dotnet-ef (solo para crear migraciones) | `dotnet tool install --global dotnet-ef` |

## Estructura del proyecto

```
DemoPdf.slnx
src\DemoPdf.Api\
├── Program.cs                        → DI, DbContext, OpenAPI + Scalar, licencia QuestPDF, migración + seed
├── appsettings.json                  → ConnectionString y flag UseInMemory
├── Domain\Entities\                  → User, Warehouse, Product
├── Data\
│   ├── AppDbContext.cs               → Fluent API (índices únicos, decimal(18,2), FK restrict)
│   ├── DbSeeder.cs                   → Datos de demostración (idempotente)
│   └── Migrations\                   → Migraciones EF Core
├── Application\
│   ├── DTOs\                         → DTOs de lectura/creación/actualización con Data Annotations
│   ├── Services\                     → IUserService, IWarehouseService, IProductService, IReportService
│   └── Reports\                      → ProductsReportDocument, UsersReportDocument (IDocument)
└── Controllers\                      → Users, Warehouses, Products, Reports
```

## Configuración

`appsettings.json`:

```json
{
  "ConnectionStrings": {
    "Default": "Server=.\\SQLEXPRESS;Database=DemoPdfDb;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=true"
  },
  "UseInMemory": false
}
```

- **SQL Server (por defecto):** ajusta `Server=` según tu instancia (`.\SQLEXPRESS`, `(localdb)\MSSQLLocalDB`, `localhost`, etc.). La base `DemoPdfDb` se crea y migra automáticamente al iniciar.
- **In-Memory (pruebas rápidas):** cambia `"UseInMemory": true`. No requiere SQL Server; la base se recrea en cada arranque y se pobla con el seeder.

## Ejecución

```powershell
# Desde la raíz del repositorio
dotnet run --project src\DemoPdf.Api --launch-profile http
```

- API: `http://localhost:5116`
- UI de documentación (Scalar): `http://localhost:5116/scalar/v1`
- Documento OpenAPI: `http://localhost:5116/openapi/v1.json`

Al iniciar por primera vez con SQL Server se aplican las migraciones y el `DbSeeder` carga datos de demo (3 depósitos, 10 productos, 5 usuarios) solo si las tablas están vacías.

## Endpoints

### Usuarios — `/api/users`

| Método | Ruta | Descripción |
|---|---|---|
| GET | `/api/users` | Lista todos los usuarios |
| GET | `/api/users/{id}` | Obtiene un usuario |
| POST | `/api/users` | Crea un usuario (409 si Username/Email duplicado) |
| PUT | `/api/users/{id}` | Actualiza un usuario |
| DELETE | `/api/users/{id}` | Elimina un usuario |

### Depósitos — `/api/warehouses`

| Método | Ruta | Descripción |
|---|---|---|
| GET | `/api/warehouses` | Lista todos los depósitos |
| GET | `/api/warehouses/{id}` | Obtiene un depósito |
| POST | `/api/warehouses` | Crea un depósito (409 si Code duplicado) |
| PUT | `/api/warehouses/{id}` | Actualiza un depósito |
| DELETE | `/api/warehouses/{id}` | Elimina (409 si tiene productos asociados) |

### Productos — `/api/products`

| Método | Ruta | Descripción |
|---|---|---|
| GET | `/api/products?warehouseId={opcional}` | Lista productos, filtrable por depósito |
| GET | `/api/products/{id}` | Obtiene un producto |
| POST | `/api/products` | Crea un producto (409 si SKU duplicado o depósito inexistente) |
| PUT | `/api/products/{id}` | Actualiza un producto |
| DELETE | `/api/products/{id}` | Elimina un producto |

### Reportes — `/api/reports`

| Método | Ruta | Descripción |
|---|---|---|
| GET | `/api/reports/products-pdf?warehouseId={opcional}` | PDF del listado de productos (FileStreamResult descargable) |
| GET | `/api/reports/users-pdf` | PDF del listado de usuarios |

### Ejemplos con PowerShell

```powershell
# Listar productos del depósito 1
Invoke-RestMethod "http://localhost:5116/api/products?warehouseId=1"

# Crear producto
Invoke-RestMethod "http://localhost:5116/api/products" -Method Post -ContentType "application/json" `
  -Body '{"sku":"SKU-9001","name":"Producto nuevo","price":50000,"stock":3,"warehouseId":2}'

# Descargar PDF de productos
Invoke-WebRequest "http://localhost:5116/api/reports/products-pdf" -OutFile "productos.pdf"

# Descargar PDF de usuarios
Invoke-WebRequest "http://localhost:5116/api/reports/users-pdf" -OutFile "usuarios.pdf"
```

## Migraciones EF Core

Las migraciones se aplican automáticamente al iniciar (`Database.MigrateAsync()`). Para trabajar con ellas manualmente:

```powershell
# Nueva migración tras cambiar entidades
dotnet ef migrations add NombreDeLaMigracion --project src\DemoPdf.Api --output-dir Data\Migrations

# Aplicar/deshacer manualmente
dotnet ef database update --project src\DemoPdf.Api
dotnet ef database update NombreDeLaMigracionAnterior --project src\DemoPdf.Api

# Eliminar la última migración (si aún no se aplicó)
dotnet ef migrations remove --project src\DemoPdf.Api
```

Migración actual: `InitialCreate` (tablas `Users`, `Warehouses`, `Products` con índices únicos en `Username`, `Email`, `Code` y `Sku`).

## Paquetes NuGet

| Paquete | Uso |
|---|---|
| `QuestPDF` | Generación de reportes PDF (`IDocument`, licencia Community) |
| `Microsoft.EntityFrameworkCore.SqlServer` | Proveedor SQL Server |
| `Microsoft.EntityFrameworkCore.InMemory` | Proveedor In-Memory alternativo |
| `Microsoft.EntityFrameworkCore.Design` | Herramientas de migraciones |
| `Scalar.AspNetCore` | UI de documentación de la API |

## Notas de implementación

- **QuestPDF 2026.9+:** las fuentes del sistema ya no se usan por defecto; los reportes utilizan la fuente incorporada **Lato**. Para usar otra fuente, copiar el archivo `.ttf` al directorio de la aplicación o registrarla con `FontManager.RegisterFontFrom*`.
- **Formato de moneda:** cultura `es-PY` (separador de miles `.`, decimales `,`), por ejemplo `450.000,00`.
- **Errores de negocio:** los servicios lanzan `InvalidOperationException` ante duplicados o violaciones de integridad; los controladores los traducen a `409 Conflict` con mensaje descriptivo.
- **Integridad referencial:** un depósito con productos no puede eliminarse (`DeleteBehavior.Restrict`); se recomienda desactivarlo (`IsActive = false`).
