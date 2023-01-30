using System.Numerics;

namespace Tmui.Immediate;

public struct Interaction : IEqualityOperators<Interaction, Interaction, bool>
{
    public static Interaction None => new Interaction(false, false, false);


    public Interaction(bool hover, bool active, bool clicked)
    {
        Hover = hover;
        Active = active;
        Clicked = clicked;
    }

    public bool Hover;
    public bool Active;
    public bool Clicked;

    public bool Any => Hover || Active || Clicked;

    public void Deconstruct(out bool Hover, out bool Active, out bool Clicked)
    {
        Hover = this.Hover;
        Active = this.Active;
        Clicked = this.Clicked;
    }

    // override object.Equals
    public override bool Equals(object? obj)
    {
        if (obj is Interaction ixn)
            return Hover == ixn.Hover && Active == ixn.Active && Clicked == ixn.Clicked;
        else
            return false;
    }

    // override object.GetHashCode
    public override int GetHashCode()
    {
        return Hover.GetHashCode() ^ Active.GetHashCode() ^ Clicked.GetHashCode();
    }

    public static bool operator ==(Interaction left, Interaction right)
    {
        return left.Active == right.Active && left.Hover == right.Hover && left.Clicked == right.Clicked;
    }

    public static bool operator !=(Interaction left, Interaction right)
    {
        return left.Active != right.Active || left.Hover != right.Hover || left.Clicked != right.Clicked;
    }
}
