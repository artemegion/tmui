using Tmui.Core;

namespace Tmui.Immediate;

/// <summary>
/// Mouse pointer interactions.
/// </summary>
public readonly partial struct Interactions
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

    /// <summary>
    /// Get interaction state for specified <paramref name="rect"/> and <paramref name="controlId"/>.
    /// </summary>
    /// <param name="rect">The rect of the control's interactable area.</param>
    /// <param name="controlId">The id of the control.</param>
    /// <returns>Interaction state over <paramref name="rect"/> for control with id <paramref name="controlId"/>.</returns>
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
            && !interactionOverride.CanIgnoreOverride(controlId))
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

    /// <summary>
    /// Updates the internal state required to observe changes in interaction state between frames.
    /// </summary>
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

    /// <summary>
    /// Push a new interaction <see cref="Override"/>.
    /// </summary>
    /// <param name="interactionOverride">The <see cref="Override"/> to push.</param>
    public void PushOverride(Override interactionOverride)
    {
        _overrides.Push(interactionOverride);
    }

    /// <summary>
    /// Pop the most recently pushed interaction <see cref="Override"/>.
    /// Does nothing if there are no overrides.
    /// </summary>
    public void PopOverride()
    {
        _overrides.TryPop(out _);
    }
}
