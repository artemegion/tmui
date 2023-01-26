// TODO:
// + ScrollView
// + Dropdown
// + SelectBox
// + Button checkbox
// + TextBox built-in scrolling
//   + no vertical align with scrolling
// + Checkbox with label overload
// + Docs
// + comments explaining code
// + refactor common code

namespace Tmui.Immediate;

public readonly struct InteractionOverride
{
    public InteractionOverride()
    {
        _stack = new();
    }

    private readonly Stack<InteractionState> _stack;

    // public bool Exists => _stack.Count > 0;
    public InteractionState? Current => _stack.Count > 0 ? _stack.Peek() : null;

    public InteractionOverride Push(InteractionState state)
    {
        _stack.Push(state);
        return this;
    }

    public void Pop()
    {
        _stack.TryPop(out _);
    }

    public void Clear()
    {
        _stack.Clear();
    }
}
