using Tmui.Core;
using Tmui.Device;
using Tmui.Graphics;

namespace Tmui.Immediate;

public struct InteractionMask
{
    public Rect Rect;
    public int ControlId;
}

public partial class Ui
{
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

    public Style Style;

    public ITerminal Terminal { get; }
    public Surface Surface { get; }
    public Input Input { get; }

    public Interactions Interactions { get; }

    public int ControlId { get; private set; }

    public bool Enabled { get; set; }
    public bool Changed { get => _changed; set { _changed = value; if (value) ReqRedraw = true; } }
    public bool ReqRedraw { get; set; }

    public void Clear()
    {
        Surface.Clear();
        Surface.Mask.Clear();
        Enabled = true;
        Changed = false;

        ControlId = -1;

        Interactions.UpdateInteractionCache();
        Interactions.Get(new(0, 0, Terminal.BufferSize), -1);
    }

    public bool Flush(bool force = false)
    {
        if (ReqRedraw || force)
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

        return false;
    }

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
