# ApiSpaceShooter
# Space Shooter FE-001 — Backend (.NET 8, Hexagonal/Clean)

Backend minimalista para el reto **Space Shooter** orientado a registrar y consultar puntajes. Implementa **.NET 8**, **arquitectura hexagonal (Clean)**, **EF Core + SQL Server**, **Minimal APIs**, **CORS**, **ProblemDetails** y **Swagger**.

---

## 🚀 Características

- **Endpoints públicos:**
  - `POST /api/v1/scores` → crear puntaje (alias + points [+ opcional: maxCombo, durationSec, metadata]).
  - `GET  /api/v1/scores/top?limit=10` → Top N ordenado por `Points DESC`, luego `DurationSec ASC`, luego `CreatedAt ASC`.
  - `GET  /api/v1/scores/alias/{alias}` → historial de puntajes por alias (orden `CreatedAt DESC`).

- **Dominio claro**: entidad `Score` como núcleo del modelo.
- **Validación** en casos de uso (Application).
- **Persistencia** vía EF Core (SQL Server) con **migraciones automáticas** al arrancar.
- **CORS** abierto a `http://localhost:4200` (frontend Angular por defecto).
- **Swagger** en Development.
- **ProblemDetails** para errores consistentes.

---

## 🧱 Arquitectura (Hexagonal / Clean)

```
SpaceShooterFE001.sln
├─ SpaceShooter.Domain/          # Núcleo del dominio
│  ├─ Entities/Score.cs
│  └─ Ports/IScoreRepository.cs
├─ SpaceShooter.Application/     # Casos de uso (lógica de aplicación)
│  └─ UseCases/
│     ├─ CreateScore.cs
│     ├─ GetTopScores.cs
│     └─ GetScoresByAlias.cs
├─ SpaceShooter.Infrastructure/  # Adaptadores (EF Core SQL Server)
│  └─ Persistence/
│     ├─ AppDbContext.cs
│     └─ ScoreRepository.cs
└─ SpaceShooter.Web/             # Capa de entrega (Minimal APIs)
   ├─ Program.cs
   ├─ appsettings.json
   └─ SpaceShooter.Web.csproj
```

- **Domain**: Entidades y puertos (interfaces). Sin dependencias externas.
- **Application**: Orquesta casos de uso y valida reglas de entrada.
- **Infrastructure**: Implementa puertos (repositorios) con EF Core + SQL Server.
- **Web**: Mapea HTTP ↔ casos de uso (Minimal APIs), configura DI, CORS, Swagger y migraciones.

---

## 🗄️ Modelo de datos

Tabla `dbo.Score` (SQL Server):

| Campo       | Tipo          | Notas                                      |
|-------------|---------------|--------------------------------------------|
| `Id`        | INT IDENTITY  | PK                                         |
| `Alias`     | NVARCHAR(30)  | NOT NULL                                   |
| `Points`    | INT           | NOT NULL, CHECK (Points >= 0)              |
| `MaxCombo`  | INT           | NULL, CHECK (MaxCombo >= 0)                |
| `DurationSec`| INT          | NULL, CHECK (DurationSec >= 0)             |
| `Metadata`  | NVARCHAR(400) | NULL                                       |
| `CreatedAt` | DATETIME2     | NOT NULL, DEFAULT SYSUTCDATETIME()         |

Índices:
- `IX_Score_Points_DESC` en `Points DESC`
- `IX_Score_Alias_CreatedAt` en `(Alias, CreatedAt DESC)`

> Las migraciones EF crean este esquema automáticamente al levantar el servicio.

---

## 🔧 Requisitos

- **.NET SDK 8**
- **SQL Server 2019+** (local o contenedor)
- (Opcional) **EF Core Tools**  
  ```bash
  dotnet tool install --global dotnet-ef
  ```

---

## ⚙️ Configuración

Cadena de conexión (prioridad descendente):

1. `appsettings.json` → `ConnectionStrings.sqlserver`
2. Variable de entorno `SQLSERVER_CONN`
3. Valor por defecto de desarrollo:
   ```
   Server=localhost,1433;Database=SpaceShooter;
   User Id=sa;Password=Your_password123;TrustServerCertificate=True
   ```

**CORS**: permitido `http://localhost:4200` por defecto (ajústalo en `Program.cs`).

---

## ▶️ Ejecución local

1) **Clonar** el repo y abrir la solución `SpaceShooterFE001.sln` en Visual Studio 2022 (o VS Code con C# Dev Kit).  
2) **Configurar** conexión en `SpaceShooter.Web/appsettings.json` o exportar `SQLSERVER_CONN`.  
3) **Restaurar & Ejecutar** el proyecto `SpaceShooter.Web`:
   ```bash
   dotnet restore
   dotnet run --project ./SpaceShooter.Web/SpaceShooter.Web.csproj
   ```
   - Swagger en: `http://localhost:5187/swagger` (puerto según tu perfil).
   - La base de datos se **migra automáticamente** al arrancar.

> Si prefieres crear la BD manualmente, usa tu SQL Server Management Studio o un contenedor Docker y ejecuta el script `db/schema.sql` (si lo incluyes).

---

## 📚 API de Referencia

### Crear puntaje
`POST /api/v1/scores`  
Body (`application/json`):
```json
{
  "alias": "kevin",
  "points": 320,
  "maxCombo": 7,
  "durationSec": 61,
  "metadata": "spaceshooter"
}
```
Respuesta `201 Created`:
```json
{ "id": 42 }
```

### Top N puntajes
`GET /api/v1/scores/top?limit=10`  
Respuesta `200 OK`:
```json
[
  { "id": 42, "alias": "neo", "points": 350, "maxCombo": 9, "durationSec": 55, "metadata": "spaceshooter", "createdAt": "2025-09-16T12:34:56Z" },
  { "id": 41, "alias": "trinity", "points": 350, "maxCombo": 8, "durationSec": 58, "metadata": "spaceshooter", "createdAt": "2025-09-16T12:32:10Z" }
]
```

### Historial por alias
`GET /api/v1/scores/alias/{alias}`  
Respuesta `200 OK` (orden `CreatedAt DESC`):
```json
[
  { "id": 42, "alias": "kevin", "points": 320, "maxCombo": 7, "durationSec": 61, "metadata": "spaceshooter", "createdAt": "2025-09-16T12:34:56Z" },
  { "id": 35, "alias": "kevin", "points": 250, "maxCombo": 5, "durationSec": 59, "metadata": "spaceshooter", "createdAt": "2025-09-15T09:11:02Z" }
]
```

---

## 🧪 Pruebas rápidas con `curl`

```bash
# Crear puntaje
curl -X POST http://localhost:5187/api/v1/scores \
  -H "Content-Type: application/json" \
  -d '{ "alias":"tester", "points":123, "maxCombo":5, "durationSec":60, "metadata":"spaceshooter" }'

# Top 5
curl "http://localhost:5187/api/v1/scores/top?limit=5"

# Historial por alias
curl "http://localhost:5187/api/v1/scores/alias/tester"
```

---

## 🧭 Decisiones y lineamientos

- **Minimal APIs** en la capa Web para reducir boilerplate y enfocar la lógica en **UseCases**.
- **Hexagonal/Clean**: dependencia unidireccional hacia el dominio; `Web` y `Infrastructure` dependen de `Application` y `Domain`, nunca al revés.
- **Validaciones** en UseCases; persistencia aislada en repositorios.
- **Orden del Top**: `Points DESC` → `DurationSec ASC` → `CreatedAt ASC` (desempate estable).
- **Fechas** en UTC (`CreatedAt = DateTime.UtcNow`).

---

## 🛠️ Migraciones EF (opcional)

Si necesitas generar migraciones explícitas:
```bash
# Desde el directorio SpaceShooter.Web o la raíz de la solución
dotnet ef migrations add Init --project ../SpaceShooter.Infrastructure --startup-project ./SpaceShooter.Web
dotnet ef database update --project ../SpaceShooter.Infrastructure --startup-project ./SpaceShooter.Web
```

> La app llama `Database.Migrate()` al iniciar; estas instrucciones son opcionales.

---

## 🔐 CORS y Seguridad

- CORS está habilitado por defecto para `http://localhost:4200`. Ajusta orígenes en `Program.cs` para tu dominio productivo.
- Este backend no implementa autenticación (requisito del reto). Si deseas agregar JWT, colócalo **en la capa Web** y mantén puertos limpios en Application/Domain.

---

## 🐳 (Opcional) Docker

Ejemplo de `docker run` para SQL Server local:

```bash
docker run -e "ACCEPT_EULA=Y" -e "MSSQL_SA_PASSWORD=Your_password123" \
  -p 1433:1433 --name sql2019 -d mcr.microsoft.com/mssql/server:2019-latest
```

Luego exporta la variable `SQLSERVER_CONN` y ejecuta el backend.

---

## ❓Troubleshooting

- **No conecta a SQL Server**: revisa firewall/puerto 1433 y `TrustServerCertificate=True` en desarrollo.
- **Error de migración**: borra la BD y vuelve a ejecutar; verifica permisos del usuario en SQL Server.
- **CORS bloqueado**: añade tu origen front en `Program.cs` (AllowAnyHeader/AllowAnyMethod).

---

## 📄 Licencia

Este proyecto se distribuye bajo licencia **MIT**. Si tu institución requiere otra licencia, ajusta este archivo.
