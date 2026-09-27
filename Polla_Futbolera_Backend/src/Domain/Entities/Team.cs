using Domain.Exceptions;

namespace Domain.Entities;

public class Team
{
    public int Id { get; private set; }
    public string Name { get; private set; }

    private Team() { Name = string.Empty; }

    public static Team Create(string name)
    {
        return string.IsNullOrWhiteSpace(name) ? throw new DomainException("El nombre del equipo no puede ser vacío.") : new Team { Name = name };
    }
}
