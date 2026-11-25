# Gestión de Proyectos y Tareas API

API REST desarrollada en **.NET 10** con **Clean Architecture**, **Entity Framework Core** y **PostgreSQL** para gestionar proyectos y sus tareas (`/api/v1/proyectos` y `/api/v1/tareas`). Incluye validaciones de reglas de negocio, manejo global de errores (`ProblemDetails`), documentación interactiva en **Swagger**, suite de **19 pruebas automatizadas**, pipeline de **CI con GitHub Actions** y preparación para despliegue en **Microsoft Azure**.

---

## Ejecución local y comandos principales

### 1. Levantar la base de datos PostgreSQL con Docker
```bash
docker compose up -d
```

### 2. Configurar cadena de conexión local
Crea `src/GestionProyectos.API/appsettings.Development.json` (ignorado por `.gitignore`):
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=gestion_proyectos;Username=postgres;Password=tu_password"
  }
}
```

### 3. Restaurar, compilar y ejecutar la API
Las migraciones de Entity Framework Core se aplican automáticamente al arrancar:
```bash
dotnet restore
dotnet build
dotnet run --project src/GestionProyectos.API
```

Al iniciar, **Swagger UI** queda disponible en `http://localhost:5229/`.

### 4. Comandos de Docker y Git útiles
```bash
# Detener PostgreSQL local
docker compose down

# Subir cambios al repositorio remoto (dispara el CI en GitHub Actions)
git add .
git commit -m "feat: descripcion del cambio"
git push
```

---

## Documentación Interactiva (Swagger / OpenAPI)

Swagger genera automáticamente la documentación de la API y permite probar cada endpoint desde el navegador (enviando JSON, parámetros de ruta/consulta y revisando códigos HTTP y respuestas `ProblemDetails`).

### Proyectos (`/api/v1/proyectos`)

| Método | Ruta | Qué hace en Swagger |
| :--- | :--- | :--- |
| `GET` | `/api/v1/proyectos` | **Listar proyectos:** Devuelve todos los proyectos registrados junto con su `totalTareas` y `porcentajeAvance`. |
| `GET` | `/api/v1/proyectos/{id}` | **Obtener proyecto por ID:** Muestra el detalle de un proyecto específico (`200 OK` o `404 Not Found`). |
| `GET` | `/api/v1/proyectos/{id}/tareas` | **Listar tareas del proyecto:** Devuelve todas las tareas que pertenecen a ese proyecto (`200 OK` o `404 Not Found`). |
| `POST` | `/api/v1/proyectos` | **Crear proyecto:** Recibe nombre, descripción y fechas; valida que la fecha de fin no sea anterior a la de inicio (`201 Created` o `400 Bad Request`). |
| `PUT` | `/api/v1/proyectos/{id}` | **Actualizar proyecto:** Modifica los datos de un proyecto existente validando sus fechas (`200 OK`, `400` o `404`). |
| `DELETE` | `/api/v1/proyectos/{id}` | **Eliminar proyecto:** Borra el proyecto solo si no tiene tareas en estado `Pendiente` (`204 No Content`, `404` o `409 Conflict`). |

### Tareas (`/api/v1/tareas`)

| Método | Ruta | Qué hace en Swagger |
| :--- | :--- | :--- |
| `GET` | `/api/v1/tareas` | **Listar tareas:** Devuelve todas las tareas y permite filtrar opcionalmente por `estado` y `prioridad` desde los parámetros *query*. |
| `GET` | `/api/v1/tareas/vencidas` | **Listar tareas vencidas:** Devuelve únicamente las tareas cuya fecha límite ya pasó y siguen sin completarse. |
| `GET` | `/api/v1/tareas/{id}` | **Obtener tarea por ID:** Muestra el detalle de una tarea específica (`200 OK` o `404 Not Found`). |
| `POST` | `/api/v1/tareas` | **Crear tarea:** Registra una nueva tarea en estado `Pendiente` vinculada a un `proyectoId` existente (`201 Created`, `400` o `404`). |
| `PUT` | `/api/v1/tareas/{id}` | **Actualizar tarea:** Modifica todos los campos de una tarea existente (`200 OK`, `400` o `404`). |
| `PATCH` | `/api/v1/tareas/{id}/estado` | **Cambiar estado de tarea:** Actualiza únicamente el estado (`Pendiente`, `EnProgreso` o `Completada`) sin reenviar todo el objeto. |
| `DELETE` | `/api/v1/tareas/{id}` | **Eliminar tarea:** Borra una tarea específica por su ID (`204 No Content` o `404 Not Found`). |

---

## Suite de Pruebas Automatizadas (19/19)

Las pruebas están organizadas por capa en `tests/GestionProyectos.Tests/`:

### 1. Pruebas Unitarias (`Unit/Application/`) — 11 pruebas
Prueban la lógica de negocio de forma aislada usando **xUnit** y **Moq** (sin base de datos real).

* **`ProyectoServiceTests.cs` (6 pruebas):**
  * `CreateAsync_DatosValidos_CreaYRetornaProyecto`: Valida la creación correcta de un proyecto.
  * `CreateAsync_FechaFinAnteriorAInicio_LanzaBadRequest`: Impide crear proyectos con fecha de fin menor a la de inicio (`400`).
  * `GetByIdAsync_NoExiste_LanzaNotFoundException`: Verifica el error `404` al buscar un proyecto inexistente.
  * `GetByIdAsync_CalculaPorcentajeAvanceCorrectamente`: Comprueba el cálculo del porcentaje de avance según tareas completadas.
  * `DeleteAsync_ProyectoConTareaPendiente_LanzaBusinessRuleException`: Bloquea el borrado si existen tareas pendientes (`409`).
  * `DeleteAsync_SinTareasPendientes_EliminaCorrectamente`: Permite eliminar el proyecto cuando no hay tareas pendientes.

* **`TareaServiceTests.cs` (5 pruebas):**
  * `CreateAsync_ProyectoExiste_CreaTareaEnEstadoPendiente`: Verifica que toda tarea nueva inicie en estado `Pendiente`.
  * `CreateAsync_ProyectoInexistente_LanzaNotFoundException`: Impide crear tareas para un proyecto que no existe (`404`).
  * `GetByIdAsync_NoExiste_LanzaNotFoundException`: Verifica el error `404` al buscar una tarea inexistente.
  * `CambiarEstadoAsync_EstadoValido_ActualizaEstado`: Comprueba la transición de estado de una tarea.
  * `DeleteAsync_TareaExiste_EliminaCorrectamente`: Valida la eliminación exitosa de una tarea.

### 2. Pruebas de Integración de Infraestructura (`Integration/Infrastructure/`) — 5 pruebas
Prueban los repositorios, consultas LINQ y relaciones 1:N contra **EF Core InMemory**.

* **`ProyectoRepositoryTests.cs` (3 pruebas):**
  * `AddYGetByIdAsync_PersisteProyectoConSusTareas`: Valida la persistencia y carga de proyectos junto con sus tareas asociadas.
  * `ExistsAsync_RetornaTrueSiExisteYFalseSiNo`: Comprueba la verificación de existencia por ID.
  * `DeleteAsync_EliminaProyectoDeLaBaseDeDatos`: Valida la eliminación real del registro en el contexto.

* **`TareaRepositoryTests.cs` (2 pruebas):**
  * `GetAllAsync_FiltraPorEstadoYPrioridad`: Verifica el filtrado combinado por estado y prioridad.
  * `GetVencidasAsync_RetornaSoloVencidasNoCompletadas`: Comprueba que solo devuelva tareas vencidas que no estén completadas.

### 3. Pruebas de Integración de API (`Integration/Api/`) — 3 pruebas
Levantan la API completa en memoria con **`WebApplicationFactory`** para probar controladores, servicios, EF Core y el middleware global de errores mediante peticiones HTTP reales.

* **`ApiIntegrationTests.cs` (3 pruebas):**
  * `FlujoCompleto_ProyectoYTarea_RespetaReglasDeNegocio`: Ejecuta el ciclo end-to-end (`POST` proyecto → `POST` tarea → `DELETE` bloqueado con `409` → `PATCH` completar tarea → `GET` avance 100% → `DELETE` exitoso con `204`).
  * `GetProyectoInexistente_Retorna404NotFound`: Valida que una consulta inexistente devuelva `404 Not Found`.
  * `DeleteTareaInexistente_Retorna404NotFound`: Valida que intentar borrar una tarea inexistente devuelva `404 Not Found`.

---

## Comandos para ejecutar los tests

```bash
# Ejecutar toda la suite (19 pruebas)
dotnet test

# Solo pruebas unitarias de servicios (11 pruebas)
dotnet test --filter "FullyQualifiedName~ServiceTests"

# Solo pruebas unitarias por archivo específico
dotnet test --filter "FullyQualifiedName~ProyectoServiceTests"
dotnet test --filter "FullyQualifiedName~TareaServiceTests"

# Solo pruebas de integración de infraestructura / EF Core (5 pruebas)
dotnet test --filter "FullyQualifiedName~Integration.Infrastructure"

# Solo pruebas de integración end-to-end de la API (3 pruebas)
dotnet test --filter "FullyQualifiedName~ApiIntegrationTests"
```

---

## CI/CD y Despliegue en Azure

* **GitHub Actions (`.github/workflows/ci.yml`):** Ejecuta automáticamente `dotnet restore`, `dotnet build` y `dotnet test` en cada `push` y `pull_request` hacia `main`.
* **Azure App Service:** Configura la cadena de conexión `DefaultConnection` apuntando a **Azure Database for PostgreSQL – Flexible Server** con `Ssl Mode=Require`. Al arrancar la app, `db.Database.Migrate()` aplica las migraciones automáticamente.