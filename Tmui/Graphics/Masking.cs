using Tmui.Core;

namespace Tmui.Graphics;

public class Masking
{
    public Masking()
    {
        _exclusiveAreas = new();
        _restrictedAreas = new();
    }

    private readonly Stack<Rect> _exclusiveAreas;
    private readonly Stack<Rect> _restrictedAreas;

    public bool Test(Pos pos)
    {
        bool e = false, r = true;

        if (_exclusiveAreas.Count > 0)
        {
            foreach (Rect rect in _exclusiveAreas)
            {
                if (Rect.IsPointInside(rect, pos))
                {
                    e = true;
                    break;
                }
            }
        }
        else e = true;

        if (!e) return false;

        if (_restrictedAreas.Count > 0)
        {
            foreach (Rect rect in _restrictedAreas)
            {
                if (Rect.IsPointInside(rect, pos))
                {
                    r = false;
                    break;
                }
            }
        }

        if (!r) return false;

        return true;
    }

    public void PushExclusiveArea(Rect area)
    {
        _exclusiveAreas.Push(area);
    }

    public void PopExclusiveArea()
    {
        _ = _exclusiveAreas.TryPop(out _);
    }

    public void PushRestrictedArea(Rect area)
    {
        _restrictedAreas.Push(area);
    }

    public void PopRestrictedArea()
    {
        _ = _restrictedAreas.TryPop(out _);
    }

    public void Clear()
    {
        _restrictedAreas.Clear();
        _exclusiveAreas.Clear();
    }
}
