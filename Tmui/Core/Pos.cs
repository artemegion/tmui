namespace Tmui.Core;

public record struct Pos(int X, int Y)
{
    public static implicit operator Pos((int X, int Y) v)
    {
        return new Pos(v.X, v.Y);
    }
}
