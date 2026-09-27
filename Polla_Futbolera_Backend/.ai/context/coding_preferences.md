# Preferencias y Correcciones de Código

Convenciones acordadas durante el desarrollo. Aplicar en todo el proyecto.

---

## C# General

### Constructores primarios
Usar **primary constructors** siempre que sea posible y la clase no requiera lógica adicional en el cuerpo del constructor.

```csharp
// Correcto
public class DomainException(string message) : Exception(message);

// Evitar cuando el primary constructor es suficiente
public class DomainException : Exception
{
    public DomainException(string message) : base(message) { }
}
```

### Enums
Usar **PascalCase** para los valores de los enums, siguiendo las convenciones de .NET y las sugerencias del IDE.

```csharp
// Correcto
public enum MatchStatus
{
    Pending,
    InProgress,
    Finished,
    Cancelled
}

// Incorrecto
public enum MatchStatus
{
    PENDING,
    IN_PROGRESS,
    FINISHED,
    CANCELLED
}
```

---

## Repositorios

Las **interfaces de repositorio** se definen siempre en **Domain**, nunca en Infrastructure ni Application.

```
Domain/Repositories/IUserRepository.cs      ← interfaz
Infrastructure/Repositories/UserRepository.cs ← implementación concreta con EF Core
```

Las **implementaciones concretas** (que usan `AppDbContext` u otros mecanismos de persistencia) viven exclusivamente en **Infrastructure**.

---

*Agregar nuevas correcciones o preferencias aquí a medida que surjan.*
