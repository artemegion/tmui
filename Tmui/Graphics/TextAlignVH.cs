namespace Tmui.Graphics;

/// <summary>
/// Vertical and horizontal text alignment.
/// </summary>
public ref struct TextAlignVH
{
    /// <summary>
    /// Implicitly convert a <see cref="ValueTuple"/> to an instance of <see cref="TextAlignVH"/>.
    /// The first element of the <see cref="ValueTuple"/> will be the vertical alignment, the second 
    /// element will be the horizontal alignment.
    /// </summary>
    /// <param name="val">The <see cref="ValueTuple"/> to convert to <see cref="TextAlignVH"/>.</param>
    public static implicit operator TextAlignVH((TextAlign V, TextAlign H) val)
    {
        return new(val.V, val.H);
    }

    /// <summary>
    /// Implicitly convert a <see cref="TextAlign"/> to an instance of <see cref="TextAlignVH"/>,
    /// with horizontal alignment specified and vertical alignment set to <see cref="TextAlign.Start"/>.
    /// </summary>
    /// <param name="H">The horizontal alignment.</param>
    public static implicit operator TextAlignVH(TextAlign H)
    {
        return new(H);
    }


    /// <summary>
    /// Create a new instance of this struct with both vertical and horizontal alignment.
    /// </summary>
    /// <param name="v">Vertical alignment.</param>
    /// <param name="h">Horizontal alignment.</param>
    public TextAlignVH(TextAlign v, TextAlign h)
    {
        V = v;
        H = h;
    }

    /// <summary>
    /// Create a new instance of this struct with only horizontal alignment specified.
    /// The vertical alignment will be set to <see cref="TextAlign.Start"/>.
    /// </summary>
    /// <param name="h"></param>
    public TextAlignVH(TextAlign h)
    {
        V = TextAlign.Start;
        H = h;
    }

    /// <summary>
    /// Create a new instance of this struct with no alignment specified.
    /// Both vertical and horizontal alignment will be set to <see cref="TextAlign.Start"/>.
    /// </summary>
    public TextAlignVH()
    {
        V = TextAlign.Start;
        H = TextAlign.Start;
    }

    /// <summary>
    /// Vertical alignment.
    /// </summary>
    public TextAlign V;

    /// <summary>
    /// Horizontal alignment.
    /// </summary>
    public TextAlign H;
}
