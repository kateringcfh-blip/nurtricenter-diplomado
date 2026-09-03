# NurTricenter — Sistema de Gestión Nutricional

Sistema de microservicios para la gestión integral de nutrición clínica, desarrollado con .NET 8, Clean Architecture y Domain-Driven Design.

---

## Microservicios

### `ms-produccion-alimentos`

Gestiona la **producción de alimentos y el armado de paquetes** para entrega a pacientes.

A partir de los planes alimentarios, genera Órdenes de Producción que los encargados de cocina utilizan para preparar recetas. Una vez completada la preparación, los alimentos se envasan y se arman paquetes etiquetados por paciente, listos para ser entregados al microservicio de logística.

| Método | Ruta | Descripción |
|--------|------|-------------|
| `POST` | `/api/ordenes` | Genera una nueva orden de producción con sus ítems de recetas |
| `GET` | `/api/ordenes` | Lista todas las órdenes de producción |
| `GET` | `/api/ordenes/{id}` | Obtiene el detalle de una orden por su Id |
| `POST` | `/api/ordenes/{id}/cancelar` | Cancela una orden indicando un motivo |
| `POST` | `/api/ordenes/desde-pedidos` | Genera orden y paquetes a partir de pedidos por paciente |
| `POST` | `/api/paquetes` | Arma un paquete etiquetado para un paciente vinculado a una orden |
| `GET` | `/api/paquetes/{id}` | Obtiene el detalle de un paquete por su Id |
| `POST` | `/api/paquetes/{id}/entregar` | Marca el paquete como listo y lo entrega a logística |

**Base de datos:** `NurTricenter` (LocalDB)
**Puerto:** `http://localhost:5234`

---

### `ms-pacientes-evaluacion`

Gestiona el **registro, evaluación nutricional y seguimiento clínico de pacientes**.

Registra pacientes y los asigna a nutricionistas. Almacena la Consulta Inicial (datos antropométricos y anamnesis) y las Evaluaciones de Seguimiento periódicas que miden evolución, adherencia al plan y medidas corporales.

| Método | Ruta | Descripción |
|--------|------|-------------|
| `POST` | `/api/pacientes` | Registra un nuevo paciente con datos de contacto y nutricionista asignado |
| `GET` | `/api/pacientes` | Lista todos los pacientes con estado y total de evaluaciones |
| `GET` | `/api/pacientes/{id}` | Obtiene el detalle completo de un paciente por su Id |
| `POST` | `/api/pacientes/{id}/consulta-inicial` | Registra la consulta inicial con peso, altura y anamnesis |
| `POST` | `/api/pacientes/{id}/evaluaciones` | Agrega una evaluación de seguimiento con medidas y adherencia |
| `POST` | `/api/pacientes/{id}/asignar-nutricionista` | Reasigna el nutricionista responsable del paciente |
| `POST` | `/api/pacientes/{id}/desactivar` | Cambia el estado del paciente a `Inactivo` |

**Base de datos:** `NurTricenterPacientes` (LocalDB)
**Puerto:** `http://localhost:5144`

---

## Arquitectura y Patrones

Ambos microservicios siguen la misma arquitectura y convenciones:

- **Domain-Driven Design (DDD):** Aggregate Roots, Entities, Value Objects, Domain Events
- **Clean Architecture:** dependencias solo hacia adentro — Domain ← Application ← Infrastructure ← Api
- **CQRS con MediatR 14:** Commands (escritura) y Queries (lectura) separados, sin acoplamiento
- **Unit of Work:** una transacción por Command — `IUnitOfWork.GuardarCambios()` al final del handler
- **Proyecciones directas a DTO:** los Query Handlers usan `IQueryable` con `.Select()` directo, sin materializar agregados
- **Value Objects tipados para IDs:** `readonly record struct` con constructores privados — errores de tipo en compilación, no en runtime
- **Entity Framework Core 8** con SQL Server / LocalDB, `OwnsOne` y `OwnsMany` para entidades propias

---

## Estructura de la Solución

```
NurTricenter.sln
├── MsProduccionAlimentos.Domain/
├── MsProduccionAlimentos.Application/
├── MsProduccionAlimentos.Infrastructure/
├── MsProduccionAlimentos.Api/            ← README detallado aquí
├── MsPacientesEvaluacion.Domain/
├── MsPacientesEvaluacion.Application/
├── MsPacientesEvaluacion.Infrastructure/
└── MsPacientesEvaluacion.Api/            ← README detallado aquí
```

Cada microservicio tiene su propio README detallado con diagrama de clases Mermaid, decisiones de diseño y guía de ejecución.

---

## Cómo ejecutar

### Requisitos

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8)
- SQL Server LocalDB (incluido con Visual Studio) o SQL Server Express

### Aplicar migraciones

```bash
# ms-produccion-alimentos → base NurTricenter
dotnet ef database update --project MsProduccionAlimentos.Infrastructure --startup-project MsProduccionAlimentos.Api

# ms-pacientes-evaluacion → base NurTricenterPacientes
dotnet ef database update --project MsPacientesEvaluacion.Infrastructure --startup-project MsPacientesEvaluacion.Api
```

### Correr los microservicios

```bash
# Terminal 1
dotnet run --project MsProduccionAlimentos.Api

# Terminal 2
dotnet run --project MsPacientesEvaluacion.Api
```

| Microservicio | URL | Swagger |
|---|---|---|
| ms-produccion-alimentos | http://localhost:5234 | http://localhost:5234/swagger |
| ms-pacientes-evaluacion | http://localhost:5144 | http://localhost:5144/swagger |

[![Review Assignment Due Date](https://classroom.github.com/assets/deadline-readme-button-22041afd0340ce965d47ae6ef1cefeee28c7c493a6346c4f15d667ab976d596c.svg)](https://classroom.github.com/a/4gfZ4JAR)
