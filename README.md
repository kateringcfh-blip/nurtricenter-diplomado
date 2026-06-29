# NurTricenter — ms-produccion-alimentos

Capa de Dominio del microservicio `ms-produccion-alimentos`, implementada en C# (.NET 8) siguiendo **Domain-Driven Design (DDD)** y **Clean Architecture**.

---

## Diagrama de Clases

```mermaid
classDiagram
    direction TB

    class OrdenProduccion {
        <<Aggregate Root>>
        +OrdenId Id
        +DateOnly Fecha
        +EstadoOrden Estado
        +Guid EncargadoCocinaId
        +string LoteProduccion
        +IReadOnlyList~ItemOrden~ Items
        +IReadOnlyList~IDomainEvent~ EventosOcurridos
        +Generar(encargadoCocinaId, loteProduccion, fecha)$ OrdenProduccion
        +AgregarItem(recetaId, cantidadRequerida) void
        +MarcarItemPreparado(itemOrdenId, cantidad) void
        +Completar() void
        +Cancelar(motivo) void
        +LimpiarEventos() void
    }

    class ItemOrden {
        <<Entity>>
        +ItemOrdenId Id
        +Guid RecetaId
        +int CantidadRequerida
        +int CantidadPreparada
        +Crear(recetaId, cantidadRequerida)$ ItemOrden
        +MarcarPreparado(cantidad) void
        +EstaCompleto() bool
    }

    class Paquete {
        <<Aggregate Root>>
        +PaqueteId Id
        +Guid PacienteId
        +OrdenId OrdenId
        +DateOnly Fecha
        +Etiqueta Etiqueta
        +EstadoPaquete Estado
        +IReadOnlyList~Porcion~ Porciones
        +Armar(pacienteId, ordenId, etiqueta, fecha)$ Paquete
        +AgregarPorcion(recetaId, cantidad) void
        +MarcarListo() void
        +EntregarALogistica() void
    }

    class Porcion {
        <<Entity>>
        +PorcionId Id
        +Guid RecetaId
        +decimal Cantidad
        +bool EstaEnvasada
        +Crear(recetaId, cantidad)$ Porcion
        +Envasar() void
    }

    class Etiqueta {
        <<Value Object>>
        +string NombrePaciente
        +string DireccionEntrega
        +string NumeroId
        +Crear(nombrePaciente, direccionEntrega, numeroId)$ Etiqueta
    }

    class OrdenId {
        <<Value Object>>
        +Guid Valor
        +Crear()$ OrdenId
        +De(valor)$ OrdenId
    }

    class PaqueteId {
        <<Value Object>>
        +Guid Valor
        +Crear()$ PaqueteId
        +De(valor)$ PaqueteId
    }

    class ItemOrdenId {
        <<Value Object>>
        +Guid Valor
        +Crear()$ ItemOrdenId
        +De(valor)$ ItemOrdenId
    }

    class PorcionId {
        <<Value Object>>
        +Guid Valor
        +Crear()$ PorcionId
        +De(valor)$ PorcionId
    }

    class EstadoOrden {
        <<Enumeration>>
        Pendiente
        EnPreparacion
        Completada
        Cancelada
    }

    class EstadoPaquete {
        <<Enumeration>>
        EnPreparacion
        Listo
        EntregadoLogistica
    }

    class IDomainEvent {
        <<interface>>
    }

    class OrdenCancelada {
        <<Domain Event>>
        +OrdenId OrdenId
        +string Motivo
        +DateTime FechaCancelacion
        +Crear(ordenId, motivo)$ OrdenCancelada
    }

    OrdenProduccion "1" *-- "1..*" ItemOrden : contiene
    OrdenProduccion --> OrdenId : identificado por
    OrdenProduccion --> EstadoOrden : estado
    OrdenProduccion --> IDomainEvent : publica
    OrdenCancelada ..|> IDomainEvent : implementa
    OrdenCancelada --> OrdenId : referencia

    ItemOrden --> ItemOrdenId : identificado por

    Paquete "1" *-- "1..*" Porcion : contiene
    Paquete --> PaqueteId : identificado por
    Paquete --> OrdenId : referencia
    Paquete --> Etiqueta : tiene
    Paquete --> EstadoPaquete : estado

    Porcion --> PorcionId : identificado por
```

---

## Decisiones de Diseño

[![Review Assignment Due Date](https://classroom.github.com/assets/deadline-readme-button-22041afd0340ce965d47ae6ef1cefeee28c7c493a6346c4f15d667ab976d596c.svg)](https://classroom.github.com/a/4gfZ4JAR)

### IDs como Value Objects tipados

Cada agregado y entidad tiene su propio tipo de ID (`OrdenId`, `PaqueteId`, `ItemOrdenId`, `PorcionId`) en vez de usar `Guid` directamente. Esto previene errores de asignación cruzada que el compilador no detectaría con `Guid` plano — pasar un `PaqueteId` donde se espera un `OrdenId` es un error en tiempo de compilación, no en tiempo de ejecución. Todos siguen el mismo patrón: constructor privado, `Crear()` para instancias nuevas, `De(guid)` para reconstruir desde persistencia, con validación de `Guid.Empty` en ambos casos.

### Colecciones expuestas como `IReadOnlyList<T>`

Los agregados mantienen sus colecciones internas como `List<T>` privadas, pero las exponen públicamente como `IReadOnlyList<T>` mediante `.AsReadOnly()`. Esto garantiza que toda modificación pase obligatoriamente por los métodos del agregado (`AgregarItem`, `AgregarPorcion`), donde viven las reglas de negocio y las validaciones de estado. Nadie puede hacer `orden.Items.Add(...)` desde fuera y saltarse las invariantes del dominio.

### Domain Events mínimos

Se implementó un patrón de Domain Events sin bus de despacho, usando una interfaz marcador `IDomainEvent` y una lista interna en el agregado. Cuando `OrdenProduccion.Cancelar()` es invocado, publica un evento `OrdenCancelada` (con `OrdenId`, `Motivo` y `FechaCancelacion`) en su colección `EventosOcurridos`. La capa de Infrastructure es responsable de leer esos eventos después de persistir el agregado, despacharlos al bus correspondiente, y luego llamar a `LimpiarEventos()` para evitar reprocesamiento. El dominio no conoce ni depende del mecanismo de despacho — solo declara que algo ocurrió.
