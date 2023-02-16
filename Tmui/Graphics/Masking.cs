using Tmui.Core;

namespace Tmui.Graphics;

/// <summary>
/// Mask areas so access to them is restricted, or that they are the only accessible areas.
/// </summary>
/// <seealso cref="Masking.Test(Pos)"/>
public class Masking
{
    public Masking()
    {
        _exclusiveAreas = new();
        _restrictedAreas = new();
    }

    private readonly Stack<Rect> _exclusiveAreas;
    private readonly Stack<Rect> _restrictedAreas;

    /// <summary>
    /// A point at <paramref name="pos"/> tests <c>true</c> if its not inside any restricted area
    /// and is inside at least one exclusive area, if there are any.
    /// </summary>
    /// <param name="pos">The position of the point to test.</param>
    /// <returns>True if a point at <paramref name="pos"/> is accessible, false otherwise.</returns>
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

    /// <summary>
    /// Push a new exclusive area.
    /// </summary>
    /// <param name="area">The area to push.</param>
    public void PushExclusiveArea(Rect area)
    {
        _exclusiveAreas.Push(area);
    }

    /// <summary>
    /// Remove the most recently pushed exclusive area.
    /// </summary>
    public void PopExclusiveArea()
    {
        _ = _exclusiveAreas.TryPop(out _);
    }

    /// <summary>
    /// Push a new restricted area.
    /// </summary>
    /// <param name="area">The area to push.</param>
    public void PushRestrictedArea(Rect area)
    {
        _restrictedAreas.Push(area);
    }

    /// <summary>
    /// Remove the most recently pushed restricted area.
    /// </summary>
    public void PopRestrictedArea()
    {
        _ = _restrictedAreas.TryPop(out _);
    }

    /// <summary>
    /// Remove all areas.
    /// </summary>
    public void Clear()
    {
        _restrictedAreas.Clear();
        _exclusiveAreas.Clear();
    }
}
