namespace Tmui;

/// <summary>
/// An RGB24 color.
/// </summary>
/// <param name="R">The red component.</param>
/// <param name="G">The green component.</param>
/// <param name="B">The blue component.</param>
public record struct OpaqueColor(byte R, byte G, byte B)
{
    /// <summary>
    /// Implicitly convert a <see cref="Color"/> into <see cref="OpaqueColor"/>.
    /// </summary>
    /// <param name="v">The <see cref="Color"/> to convert into an <see cref="OpaqueColor"/>.</param>
    public static implicit operator OpaqueColor(Color v)
    {
        return new OpaqueColor(v.R, v.G, v.B);
    }

    /// <summary>
    /// Implicitly convert a <see cref="OpaqueColor"/> into <see cref="Color"/> with alpha component set to 255.
    /// </summary>
    /// <param name="v">The <see cref="OpaqueColor"/> to convert into an <see cref="Color"/>.</param>
    public static implicit operator Color(OpaqueColor v)
    {
        return new Color(v.R, v.G, v.B, 255);
    }
}
