using Tmui.Graphics;

namespace Tmui.Core;

public record struct Rect(int X, int Y, int W, int H)
{
    public Rect(int X, int Y, Pos Pos) : this(X, Y, Pos.X, Pos.Y)
    {

    }

    public bool IsOrigin => X == 0 && Y == 0 && W == 0 && H == 0;

    public void Reset()
    {
        X = 0;
        Y = 0;
        W = 0;
        H = 0;
    }

    public static bool IsPointInside(Rect rect, Pos pos)
    {
        return pos.X >= rect.X && pos.X < rect.X + rect.W && pos.Y >= rect.Y && pos.Y < rect.Y + rect.H;
    }

    public static Rect Combine(Rect a, Rect b)
    {
        return new Rect(
            Math.Min(a.X, b.X),
            Math.Min(a.Y, b.Y),
            Math.Max(a.W, b.W),
            Math.Max(a.H, b.H)
        );
    }

    public static Rect Intersect(Rect a, Rect b)
    {
        return new(
            int.Max(a.X, b.X),
            int.Max(a.Y, b.Y),
            int.Min(a.W, b.W),
            int.Min(a.H, b.H)
        );
    }

    public static Rect Intersect(params Rect[] args)
    {
        Rect area = new(int.MinValue, int.MinValue, int.MaxValue, int.MaxValue);
        for (int i = 0, len = args.Length; i < len; i++)
        {
            area = Intersect(area, args[i]);
        }

        return area;
    }

    public static Rect Padding(Rect rect, Thickness padding)
    {
        return new(rect.X + padding.Left, rect.Y + padding.Top, rect.W - padding.Left - padding.Right, rect.H - padding.Top - padding.Bottom);
    }

    public static implicit operator Rect((int X, int Y, int W, int H) rect)
    {
        return new Rect(rect.X, rect.Y, rect.W, rect.H);
    }
}
