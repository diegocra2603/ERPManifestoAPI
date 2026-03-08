namespace Domain.Primitives.Mediator;

/// <summary>
/// Representa un tipo vacío (void) para usar como respuesta genérica.
/// </summary>
public readonly struct Unit : IEquatable<Unit>, IComparable<Unit>
{
    public static readonly Unit Value = new();

    public int CompareTo(Unit other) => 0;
    public bool Equals(Unit other) => true;
    public override bool Equals(object? obj) => obj is Unit;
    public override int GetHashCode() => 0;
    public override string ToString() => "()";

    public static bool operator ==(Unit left, Unit right) => true;
    public static bool operator !=(Unit left, Unit right) => false;
}
