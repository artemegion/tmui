using Tmui.Core;

namespace Tmui.Graphics;

public readonly struct CompoundMask
{
    public CompoundMask()
    {
        _masks = new();
    }

    private readonly Stack<Mask> _masks;

    public bool IsPointInside(Pos pos)
    {
        // the point has to be in at least one of the inclusive parts
        // and it cannot be in any exclusive parts

        if (_masks.Count < 1) return true;

        bool hasInclusive = false, inAnyInclusive = false;

        foreach (Mask mask in _masks)
        {
            if (mask.Type == MaskType.Inclusive)
            {
                hasInclusive = true;
                if (Rect.IsPointInside(mask.Rect, pos)) inAnyInclusive = true;
            }
            else if (mask.Type == MaskType.Exclusive)
            {
                if (Rect.IsPointInside(mask.Rect, pos)) return false;
            }
        }

        return !hasInclusive || inAnyInclusive;
    }

    public void Push(Rect rect, MaskType maskType = MaskType.Inclusive)
    {
        _masks.Push(new Mask(rect, maskType));
    }

    public void Pop()
    {
        _masks.TryPop(out _);
    }

    public void Clear()
    {
        _masks.Clear();
    }
    
    private struct Mask
    {
        public Mask(Rect rect, MaskType type)
        {
            Rect = rect;
            Type = type;
        }

        public Rect Rect;
        public MaskType Type;
    }

    public enum MaskType
    {
        Inclusive,
        Exclusive
    }
}
