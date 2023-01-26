namespace Tmui;

public record struct OpaqueColor(byte R, byte G, byte B)
{
    public static implicit operator OpaqueColor(Color v)
    {
        return new OpaqueColor(v.R, v.G, v.B);
    }

    public static implicit operator Color(OpaqueColor v)
    {
        return new Color(v.R, v.G, v.B, 255);
    }
}
