namespace Tmui.Core;

/// <summary>
/// A position defined by x and y components.
/// </summary>
/// <param name="X">x component</param>
/// <param name="Y">y component</param>
public record struct Pos(int X, int Y)
{
    /// <summary>
    /// Implicitly converts a <see cref="(int X, int Y)"/> to a <see cref="Pos"/>.
    /// </summary>
    /// <param name="v"></param>
    public static implicit operator Pos((int X, int Y) v)
    {
        return new Pos(v.X, v.Y);
    }
}
