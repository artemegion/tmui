using Tmui.Core;
using Tmui.Device;
using Tmui.Graphics;

namespace Tmui.Immediate;

/// <summary>
/// Immediate mode user interface.
/// </summary>
public partial class Ui
{
    /// <summary>
    /// Creates a new instance of the <see cref="Ui"/> class.
    /// </summary>
    /// <param name="terminal">The terminal to draw the ui to.</param>
    /// <param name="input">Input provider.</param>
    public Ui(ITerminal terminal, Input input)
    {
        _radialGroups = new(5);
        _scrollStates = new(10);

        _openedDropdownId = -1;

        _cursorPosPlease = null;
        _changed = false;

        Style = CreateDefaultStyle();

        Terminal = terminal;
        _context = new(Terminal);

        Surface.Clear();
        Surface = new Surface(terminal.BufferSize.X, terminal.BufferSize.Y);

        Input = input;
        Interactions = new(this);

        ControlId = -1;

        Enabled = true;
        Changed = false;
    }

    private readonly TerminalGraphicsContext _context;

    private readonly Stack<ValueTuple<int, int>> _radialGroups;
    private readonly Dictionary<int, Scroll> _scrollStates;

    private int _openedDropdownId; // -1 means no dropdown is opened
    private bool _changed;

    private Pos? _cursorPosPlease; // move cursor here after rendering is done

    /// <summary>
    /// The style for all controls. Can be overriden for individual controls.
    /// </summary>
    public Style Style;

    /// <summary>
    /// Terminal to which the ui is drawn to.
    /// </summary>
    public ITerminal Terminal { get; }

    /// <summary>
    /// Surface used to draw the controls. Can be used to draw to the terminal.
    /// </summary>
    public Surface Surface { get; }

    /// <summary>
    /// Input provider.
    /// </summary>
    public Input Input { get; }

    /// <summary>
    /// Mouse pointer interactions.
    /// </summary>
    public Interactions Interactions { get; }

    /// <summary>
    /// The id of most recently drawn control.
    /// </summary>
    public int ControlId { get; private set; }

    /// <summary>
    /// The id of control that has focus.
    /// </summary>
    public int FocusedControlId { get; set; }

    /// <summary>
    /// Controls drawn while the <see cref="Ui.Enabled"/> property is set to <c>false</c> will not be interactable.
    /// </summary>
    public bool Enabled { get; set; }

    /// <summary>
    /// Set to true if a state of the most recently drawn control changed. Set to false after calling <see cref="Ui.Begin"/>.
    /// </summary>
    /// <example>
    /// <code>
    /// bool isChecked = false;
    /// 
    /// ui.Checkbox(ref isChecked, (1, 1));
    /// if (ui.Changed)
    /// {
    ///     // the value of field `isChecked` has changed because the user interacted with the control
    /// }
    /// </code>
    /// </example>
    public bool Changed { get => _changed; set { _changed = value; if (value) ReqRedraw = true; } }

    /// <summary>
    /// If set to true causes the <see cref="Ui.Surface"/> to be written to the terminal, and is then set to false.
    /// </summary>
    public bool ReqRedraw { get; set; }

    /// <summary>
    /// Has to be called each frame before drawing any controls.
    /// </summary>
    public void Begin()
    {
        Surface.Clear();
        Surface.Mask.Clear();
        Enabled = true;
        Changed = false;

        ControlId = -1;

        Interactions.UpdateInteractionCache();

        var windowInteraction = Interactions.Get(new(0, 0, Terminal.BufferSize), -1);
        if (windowInteraction.Clicked) FocusedControlId = -1;
    }

    /// <summary>
    /// Has to be called each frame after drawing all the controls.
    /// </summary>
    /// <param name="forceFlush">Forces the <see cref="Ui.Surface"/> to be written to the <see cref="Ui.Terminal"/>.</param>
    /// <returns>True if the <see cref="Ui.Surface"/> was written to the <see cref="Ui.Terminal"/>, false otherwise.</returns>
    public bool End(bool forceFlush = false)
    {
        if (ReqRedraw || forceFlush)
        {
            Terminal.CursorVisible = false;

            _context.DrawSurface((0, 0), Surface);
            _context.Flush();

            if (_cursorPosPlease != null)
            {
                Terminal.CursorVisible = true;
                Terminal.CursorPos = _cursorPosPlease.Value;
            }
            else
            {
                Terminal.CursorVisible = false;
            }

            ReqRedraw = false;
            return true;
        }

        ReqRedraw = false;

        if (_openedDropdownId == -1)
            Interactions.PopOverride();

        if (FocusedControlId == -1)
        {
            Terminal.CursorVisible = false;
            Terminal.CursorPos = new(0, 0);
        }

        return false;
    }

    /// <summary>
    /// Get the scroll data for a control with specific id.
    /// The data is stored internally and persists between frames.
    /// </summary>
    /// <param name="controlId">The control id.</param>
    /// <returns>Scroll data for control with id <paramref name="controlId"/>.</returns>
    public Scroll GetScroll(int controlId)
    {
        if (_scrollStates.TryGetValue(controlId, out var state))
        {
            return state;
        }
        else
        {
            state = new Scroll(0, 0);
            _scrollStates.Add(controlId, state);

            return state;
        }
    }

    private int CreateControlId()
    {
        return ++ControlId;
    }

    private Style CreateDefaultStyle()
    {
        Color defaultBgColor = new(60, 60, 60);
        Color hoverBgColor = new(80, 80, 80);
        Color activeBgColor = new(100, 100, 100);
        ColorByInteraction interactionColors = new(defaultBgColor, hoverBgColor, activeBgColor);

        Color defaultAccent = Color.DodgerBlue;
        Color hoverAccent = Color.AlphaBlend(Color.White10, defaultAccent);
        Color activeAccent = Color.AlphaBlend(Color.White20, defaultAccent);
        Color disabledAccent = new(180, 180, 180);

        return new(
            Accent: new(defaultAccent, disabledAccent),
            Panel: new(new(60, 60, 60)),
            Label: new(Color.White, new(0, 0, 0, 0)),
            TextBox: new(Color.White, new(60, 60, 60), new(0, 0, 0, 0)),
            Button: new(Color.White, interactionColors),
            Checkbox: new('✓', Color.White, interactionColors),
            Dropdown: new(new(Color.White, interactionColors), new(Color.White, interactionColors)),
            ScrollbarV: new(Glyph.RIGHT_HALF_BLOCK_CHAR, new(80, 80, 80), Glyph.RIGHT_HALF_BLOCK_CHAR),
            ScrollbarH: new(Glyph.LOWER_THREE_EIGHTS_BLOCK_CHAR, new(80, 80, 80), Glyph.LOWER_THREE_EIGHTS_BLOCK_CHAR)
        );
    }
}
