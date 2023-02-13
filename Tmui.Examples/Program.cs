using Tmui;
using Tmui.Graphics;
using Tmui.Immediate;
using Tmui.Messages;

int clickCount = 0;

Color[] accentColors = new[] { Color.DodgerBlue, Color.BlueViolet, Color.CadetBlue, Color.Blue, Color.Green, Color.HotPink, Color.Magenta, Color.OrangeRed, Color.Red, Color.Yellow, Color.YellowGreen };
string[] accentColorNames = new[] { "DodgerBlue", "BlueViolet", "CadetBlue", "Blue", "Green", "HotPink", "Magenta", "OrangeRed", "Red", "Yellow", "YellowGreen" };
int selectedAccentColorIndex = 0;

char[] loremIpsum = "Lorem ipsum dolor sit amet,\nconsectetur adipiscing elit,\nsed do eiusmod tempor incididunt ut labore et dolore magna aliqua.\nUt enim ad minim veniam, \nquis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat.\nDuis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur.\nExcepteur sint occaecat cupidatat non proident,\nsunt in culpa qui officia deserunt mollit anim id est laborum.\n1\n2\n3\n4\n5\n6\n7\n8\n9".ToCharArray();
Range[] loremIpsumLines = new Range[30];
Surface.WrapText(loremIpsum, 30, loremIpsumLines, out int w);
loremIpsumLines = loremIpsumLines[..w];

TextAlign textBoxHorizontalAlign = TextAlign.Center;
int selectedTextAlignIndex = 1;

TextBoxScrollFlags scrollFlags = TextBoxScrollFlags.None;
bool scrollFlagsVertical = true;
bool scrollFlagsHorizontal = false;
bool disableVertical = false;
bool disableHorizontal = false;
bool hideVertical = false;
bool hideHorizontal = false;

TermApp app = new();
Ui ui = new(app.Terminal, app.Input);

app.AddMsgHandler<UpdateMsg>(_ =>
{
    ui.Clear();

    ui.Label((2, 1), "Hello, Tmui!");

    if (ui.Button((2, 3, 30, 1), clickCount == 0 ? "Click me!" : $"Clicked {clickCount} times!", TextAlign.Center))
        clickCount++;


    ui.Label((33, 1), "Choose an accent color!");

    ui.Changed = false;
    ui.Dropdown((33, 3, 20, 1), accentColorNames, ref selectedAccentColorIndex);

    if (ui.Changed)
    {
        ui.Style.Accent = ui.Style.Accent with { Default = accentColors[selectedAccentColorIndex] };
    }

    ui.TextBox((2, 5, 30, 15), new ReadOnlySpan<char>(loremIpsum), (TextAlign.Start, textBoxHorizontalAlign), scrollFlags);

    ui.Label((34, 5), "Text alignment");

    ui.Changed = false;
    ui.BeginRadialGroup(selectedTextAlignIndex);
    {
        ui.Radial((34, 7)); ui.Label((37, 7), "Start");
        ui.Radial((34, 9)); ui.Label((37, 9), "Center");
        ui.Radial((34, 11)); ui.Label((37, 11), "End");
    }
    ui.EndRadialGroup(out selectedTextAlignIndex);

    if (ui.Changed)
    {
        textBoxHorizontalAlign = selectedTextAlignIndex switch
        {
            0 => TextAlign.Start,
            1 => TextAlign.Center,
            2 => TextAlign.End,
            _ => TextAlign.Start
        };
    }

    ui.Label((34, 13), "Scrolling");

    ui.Checkbox(ref scrollFlagsVertical, (34, 15, 15, 1), "Vertical", TextAlign.Start);
    ui.Checkbox(ref scrollFlagsHorizontal, (34, 17, 20, 1), "Horizontal", TextAlign.Start);

    ui.Checkbox(ref disableVertical, (34, 19, 20, 1), "Disable Vertical", TextAlign.Start);
    ui.Checkbox(ref disableHorizontal, (34, 21, 24, 1), "Disable Horizontal", TextAlign.Start);

    ui.Checkbox(ref hideVertical, (34, 23, 20, 1), "Hide Vertical", TextAlign.Start);
    ui.Checkbox(ref hideHorizontal, (34, 25, 20, 1), "Hide Horizontal", TextAlign.Start);

    scrollFlags = TextBoxScrollFlags.None;

    if (scrollFlagsVertical) scrollFlags |= TextBoxScrollFlags.Vertical;
    if (scrollFlagsHorizontal) scrollFlags |= TextBoxScrollFlags.Horizontal;
    if (disableVertical) scrollFlags |= TextBoxScrollFlags.DisableVertical;
    if (disableHorizontal) scrollFlags |= TextBoxScrollFlags.DisableHorizontal;
    if (hideVertical) scrollFlags |= TextBoxScrollFlags.HideVertical;
    if (hideHorizontal) scrollFlags |= TextBoxScrollFlags.HideHorizontal;

    ui.Flush();
});

app.Run();
