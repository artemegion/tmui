using Tmui.Core;
using Tmui.Device;
using Tmui.Graphics;
using Tmui.Messages;
using Tmui.Extensions;

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

public partial class Ui
{
    public Ui(ITerminal terminal, Input input)
    {
        Terminal = terminal;
        Input = input;
        _context = new(Terminal);

        InteractionOverride = new();

        Surface = new Surface(terminal.BufferSize.X, terminal.BufferSize.Y);
        Surface.Clear();

        Style = CreateDefaultStyle();

        Enabled = true;
        Changed = false;

        _radialGroups = new();

        _controlId = -1;
        _openedDropdownId = -1;

        _cursorPosPlease = null;

        _prevFrameIxn = new();
    }

    private readonly TerminalGraphicsContext _context;

    private Dictionary<int, InteractionState> _prevFrameIxn;
    private readonly Stack<ValueTuple<int, int>> _radialGroups;
    private int _controlId;

    private int _openedDropdownId; // -1 means no dropdown is opened
    private Rect _openedDropdownRect;
    private bool _changed;

    private Pos? _cursorPosPlease;

    public Style Style;

    public ITerminal Terminal { get; }
    public Input Input { get; }

    public InteractionOverride InteractionOverride { get; }
    public bool Enabled { get; set; }
    public bool Changed { get => _changed; set { _changed = value; if (value) ReqRedraw = true; } }

    public Surface Surface { get; private set; }
    public bool ReqRedraw { get; set; }

    public void Clear()
    {
        Surface.Clear();
        Surface.Mask.Clear();
        Enabled = true;
        Changed = false;

        _controlId = -1;

        GetInteraction(new(0, 0, Terminal.BufferSize));
    }

    public bool Flush(bool force = false)
    {
        if (ReqRedraw || force)
        {
            Console.CursorVisible = false;

            _context.DrawSurface((0, 0), Surface);
            _context.Flush();

            if (_cursorPosPlease != null)
            {
                Console.CursorVisible = true;
                Terminal.CursorPos = _cursorPosPlease.Value;
            }
            else
            {
                Console.CursorVisible = false;
            }

            ReqRedraw = false;
            return true;
        }

        ReqRedraw = false;
        return false;
    }

    public InteractionState GetInteraction(Rect rect, int controlId = -1)
    {
        if (InteractionOverride.Current.HasValue)
        {
            return InteractionOverride.Current.Value;
        }
        else
        {
            if (!Enabled) return new InteractionState(false, false, false);

            bool isHover = (!Input.PressedMousePos.HasValue || Rect.IsPointInside(rect, Input.PressedMousePos.Value)) && Rect.IsPointInside(rect, Input.MousePos);

            if (isHover && _openedDropdownId > -1 && Rect.IsPointInside(_openedDropdownRect, Input.MousePos))
            {
                isHover = controlId == _openedDropdownId || (controlId >= _openedDropdownId + 1000000 && controlId < _openedDropdownId + 1000000 + 1000);
            }

            var ixn = new InteractionState(
                isHover,
                isHover && Input.KeyHeld(Key.MouseLeft),
                isHover && Input.KeyReleased(Key.MouseLeft) && Input.PressedMousePos.HasValue && Rect.IsPointInside(rect, Input.PressedMousePos.Value)
            );

            // commenting the line below fixes an issue where after selecting an option from a dropdown,
            // the dropdown options would only partially disappear, and some of them would stay
            // until an interaction was made with any control (like hover etc.);
            // it does that because after the dropdown closes we interact with the buffer(window) itself,
            // so while the bug still exists, it is not visible anymore-

            // if (controlId > -1) - 
            {
                if (!_prevFrameIxn.ContainsKey(controlId) || _prevFrameIxn[controlId] != ixn)
                    ReqRedraw = true;

                _prevFrameIxn[controlId] = ixn;
            }

            return ixn;
        }
    }

    private int CreateControlId()
    {
        return ++_controlId;
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
            ScrollbarV: new(Glyph.RIGHT_HALF_BLOCK_CHAR, new(80, 80, 80), new(defaultAccent, hoverAccent, activeAccent), Glyph.RIGHT_HALF_BLOCK_CHAR),
            ScrollbarH: new(Glyph.LOWER_THREE_EIGHTS_BLOCK_CHAR, new(80, 80, 80), new(defaultAccent, hoverAccent, activeAccent), Glyph.LOWER_THREE_EIGHTS_BLOCK_CHAR)
        );
    }
}
