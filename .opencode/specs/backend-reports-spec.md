Actúa como un arquitecto y desarrollador senior en .NET / C#. Necesito que diseñes y construyas una API REST utilizando .NET 10 Web API basada en Controladores (Controller-based, no Minimal APIs) para la gestión básica de inventario y generación de reportes en PDF con QuestPDF.

### 1. Requerimientos Técnicos y Arquitectura
- **Framework:** .NET 10 Web API.
- **Patrón:** Controladores (`[ApiController]`).
- **Base de Datos:** SQL Server.
- **Arquitectura:** Limpia/Modular (Domain, Infrastructure/Data, Application/Services, Controllers).
- **ORM / Persistencia:** Entity Framework Core (SQL Server, configurable vía ConnectionString) con In-Memory habilitado como alternativa rápida.
- **Librería de Reportes:** QuestPDF (usando la interfaz `IDocument` y componentes modulares). Configurar `QuestPDF.Settings.License = LicenseType.Community;` en el inicio.

---

### 2. Modelo de Dominio y Entidades
Diseña las siguientes entidades con sus relaciones y Data Annotations/Fluent API:
1. **User (Usuario):** `Id`, `Username`, `Email`, `FullName`, `Role`, `CreatedAt`, `IsActive`.
2. **Warehouse (Depósito):** `Id`, `Code`, `Name`, `Location`, `IsActive`.
3. **Product (Producto):** `Id`, `Sku`, `Name`, `Description`, `Price`, `Stock`, `WarehouseId`, `Warehouse` (navegación), `CreatedAt`.

---

### 3. Endpoints a Implementar
Proporciona endpoints CRUD estándar con DTOs (Data Transfer Objects) para evitar exponer entidades directamente:
- **Usuarios:** `/api/users` (GET, GET by Id, POST, PUT, DELETE).
- **Depósitos:** `/api/warehouses` (GET, GET by Id, POST, PUT, DELETE).
- **Productos:** `/api/products` (GET con soporte opcional de filtro por `warehouseId`, GET by Id, POST, PUT, DELETE).
- **Reportes:** `/api/reports`:
  - `GET /api/reports/products-pdf` (Genera y devuelve el PDF del listado de productos como `application/pdf` descargable `FileStreamResult`). Debe permitir un query param opcional `warehouseId`.
  - `GET /api/reports/users-pdf` (Genera y devuelve el PDF del listado de usuarios como `application/pdf`).

---

### 4. Especificaciones de Diseño para QuestPDF
Crea documentos independientes implementando `IDocument`:
1. **ProductsReportDocument:**
   - **Header:** Título del reporte ("Catálogo / Listado de Productos"), fecha y hora de emisión, depósito seleccionado (si aplica).
   - **Body:** Tabla estilizada con encabezados grises/azules, filas alternadas (zebra striping), columnas: `SKU`, `Nombre`, `Depósito`, `Precio` (formato moneda), `Stock` y `Total Valorizado`.
   - **Footer:** Paginación ("Página X de Y") y texto de confidencialidad o autor.
2. **UsersReportDocument:**
   - **Header:** Título ("Listado de Usuarios del Sistema"), fecha de generación.
   - **Body:** Tabla con columnas: `Id`, `Nombre Completo`, `Usuario`, `Email`, `Rol`, `Estado (Activo/Inactivo)`.
   - **Footer:** Paginación ("Página X de Y").

---

### 5. Entregables Esperados en la Respuesta
1. **Estructura del Proyecto:** Árbol de carpetas recomendado.
2. **Dependencias:** Paquetes NuGet necesarios (`QuestPDF`, `Microsoft.EntityFrameworkCore.*`, etc.).
3. **Modelos y DbContext:** Definición de entidades y configuración del contexto.
4. **Clases de Reportes (QuestPDF):** Implementaciones completas y limpias de `ProductsReportDocument` y `UsersReportDocument`.
5. **Servicio de Reportes:** `IReportService` y su implementación que recupere los datos del DbContext y genere los bytes del PDF.
6. **Controllers:** Implementación de `ReportsController` y un ejemplo de CRUD (`ProductsController`).
7. **`Program.cs`:** Configuración de inyección de dependencias, DbContext, Swagger y licencia de QuestPDF.
