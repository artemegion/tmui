using Tmui.Core;

namespace Tmui.Immediate;

public readonly struct Interactions
{
    public Interactions(Ui ui)
    {
        _ui = ui;

        _overrides = new(10);
        _thisFrameInteractions = new(100);
        _prevFrameInteractions = new(100);
    }

    private readonly Ui _ui;

    private readonly Stack<Override> _overrides;
    private readonly Dictionary<int, Interaction> _thisFrameInteractions;
    private readonly Dictionary<int, Interaction> _prevFrameInteractions;

    public Interaction Get(Rect rect, int controlId)
    {
        Interaction interaction;
        Pos mousePos = _ui.Input.MousePos;

        if (_thisFrameInteractions.TryGetValue(controlId, out interaction))
        {

        }
        else if (!_ui.Enabled)
        {
            interaction = new(false, false, false);
        }
        else if (_overrides.TryPeek(out Override interactionOverride)
            && (!interactionOverride.Area.HasValue || Rect.IsPointInside(interactionOverride.Area.Value, mousePos))
            && !interactionOverride.IsControlIdExempt(controlId))
        {
            interaction = interactionOverride.Interaction;
        }
        else
        {
            // if a mouse button is pressed, while the mouse is over a control, and the mouse is moved,
            // the control should remain in 'hover' state, and other controls should not enter that state
            bool isHover = Rect.IsPointInside(rect, mousePos) && (!_ui.Input.PressedMousePos.HasValue || Rect.IsPointInside(rect, _ui.Input.PressedMousePos.Value));

            interaction = new(
                hover: isHover,
                active: isHover && _ui.Input.KeyHeld(Key.MouseLeft),
                clicked: isHover && _ui.Input.KeyReleased(Key.MouseLeft)
            );
        }

        _thisFrameInteractions[controlId] = interaction;
        return interaction;
    }

    public void UpdateInteractionCache()
    {
        // _prevFrameInteractions.Clear();

        foreach (KeyValuePair<int, Interaction> pair in _thisFrameInteractions)
        {
            if (!_ui.Changed && (!_prevFrameInteractions.TryGetValue(pair.Key, out var state) || state != pair.Value)) _ui.Changed = true;
            _prevFrameInteractions[pair.Key] = pair.Value;
        }

        _thisFrameInteractions.Clear();
    }

    public void PushOverride(Override interactionOverride)
    {
        _overrides.Push(interactionOverride);
    }

    public void PopOverride()
    {
        _overrides.TryPop(out _);
    }


    public readonly struct Override
    {
        public Override(Interaction interaction, Rect? area = null, int[]? exceptionControlId = null)
        {
            Interaction = interaction;
            Area = area;
            ExceptionControlId = exceptionControlId;
        }

        public readonly Interaction Interaction;
        public readonly Rect? Area;
        public readonly int[]? ExceptionControlId;

        public bool IsControlIdExempt(int controlId)
        {
            if (ExceptionControlId == null) return false;

            for (int i = 0, len = ExceptionControlId.Length; i < len; i++)
                if (ExceptionControlId[i] == controlId) return true;

            return false;
        }
    }
}

public readonly struct InteractionOverride
{
    public InteractionOverride()
    {
        _stack = new();
    }

    private readonly Stack<Interaction> _stack;

    // public bool Exists => _stack.Count > 0;
    public Interaction? Current => _stack.Count > 0 ? _stack.Peek() : null;

    public InteractionOverride Push(Interaction state)
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
