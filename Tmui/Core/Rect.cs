using Tmui.Graphics;

namespace Tmui.Core;

/// <summary>
/// A rectangle defined by x, y, width and height components.
/// </summary>
/// <param name="X">The x component.</param>
/// <param name="Y">The y component.</param>
/// <param name="W">The width component.</param>
/// <param name="H">The height component.</param>
public record struct Rect(int X, int Y, int W, int H)
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Rect"/> structure using <paramref name="X"/> and <paramref name="Y"/> components.
    /// The width and height components are taken from the <paramref name="Pos"/> parameter.
    /// </summary>
    /// <param name="X">The x component.</param>
    /// <param name="Y">The y component.</param>
    /// <param name="Pos">The <see cref="Pos"/> to be used as width and height.</param>
    public Rect(int X, int Y, Pos Pos) : this(X, Y, Pos.X, Pos.Y)
    {

    }

    /// <summary>
    /// Is every component of this <see cref="Rect"/> equal to <c>0</c>.
    /// </summary>
    public bool IsOrigin => X == 0 && Y == 0 && W == 0 && H == 0;

    /// <summary>
    /// Sets the values of all components to <c>0</c>.
    /// </summary>
    public void Reset()
    {
        X = 0;
        Y = 0;
        W = 0;
        H = 0;
    }

    /// <summary>
    /// Determine whether a <paramref name="pos"/> is inside the <paramref name="rect"/>.
    /// </summary>
    /// <param name="rect">The rect to check against.</param>
    /// <param name="pos">The pos.</param>
    /// <returns></returns>
    public static bool IsPointInside(Rect rect, Pos pos)
    {
        return pos.X >= rect.X && pos.X < rect.X + rect.W && pos.Y >= rect.Y && pos.Y < rect.Y + rect.H;
    }

    /// <summary>
    /// Create a bounding box of two rects.
    /// </summary>
    /// <param name="a">Thew first rect.</param>
    /// <param name="b">The second rect.</param>
    /// <returns>Bounding box of rects <paramref name="a"/> and <paramref name="b"/>.</returns>
    public static Rect BoundingBox(Rect a, Rect b)
    {
        return new Rect(
            Math.Min(a.X, b.X),
            Math.Min(a.Y, b.Y),
            Math.Max(a.W, b.W),
            Math.Max(a.H, b.H)
        );
    }

    /// <summary>
    /// Create an intersection of two rects.
    /// </summary>
    /// <param name="a">The first rect.</param>
    /// <param name="b">The second rect.</param>
    /// <returns>The intersection of two rects.</returns>
    public static Rect Intersect(Rect a, Rect b)
    {
        return new(
            int.Max(a.X, b.X),
            int.Max(a.Y, b.Y),
            int.Min(a.W, b.W),
            int.Min(a.H, b.H)
        );
    }

    /// <summary>
    /// Create an intersection of any amount of rects.
    /// </summary>
    /// <param name="args">The rects.</param>
    /// <returns>The intersection between the rects.</returns>
    public static Rect Intersect(params Rect[] args)
    {
        Rect area = new(int.MinValue, int.MinValue, int.MaxValue, int.MaxValue);
        for (int i = 0, len = args.Length; i < len; i++)
        {
            area = Intersect(area, args[i]);
        }

        return area;
    }

    /// <summary>
    /// Create a new <see cref="Rect"/> from a <paramref name="rect"/> with <paramref name="padding"/> applied.
    /// </summary>
    /// <param name="rect">The rect to apply padding to.</param>
    /// <param name="padding">The padding to apply to the rect.</param>
    /// <returns>The <paramref name="rect"/> with <paramref name="padding"/> applied.</returns>
    public static Rect Padding(Rect rect, Thickness padding)
    {
        return new(rect.X + padding.Left, rect.Y + padding.Top, rect.W - padding.Left - padding.Right, rect.H - padding.Top - padding.Bottom);
    }

    /// <summary>
    /// Implicitly converts a <see cref="(int X, int Y, int W, int H)"/> to a <see cref="Rect"/>.
    /// </summary>
    /// <param name="rect">The tuple to convert to a <see cref="Rect"/>.</param>
    public static implicit operator Rect((int X, int Y, int W, int H) rect)
    {
        return new Rect(rect.X, rect.Y, rect.W, rect.H);
    }
}
