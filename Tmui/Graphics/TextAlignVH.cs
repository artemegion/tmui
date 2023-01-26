namespace Tmui.Graphics;

public ref struct TextAlignVH
{
    public static implicit operator TextAlignVH((TextAlign V, TextAlign H) val)
    {
        return new(val.V, val.H);
    }

    public static implicit operator TextAlignVH(TextAlign H)
    {
        return new(H);
    }


    public TextAlignVH(TextAlign v, TextAlign h)
    {
        V = v;
        H = h;
    }

    public TextAlignVH(TextAlign h)
    {
        V = TextAlign.Start;
        H = h;
    }

    public TextAlignVH()
    {
        V = TextAlign.Start;
        H = TextAlign.Start;
    }

    public TextAlign V;
    public TextAlign H;
}
