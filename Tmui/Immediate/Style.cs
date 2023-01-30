#if !DEBUG
using System.Runtime.CompilerServices;
#endif

using Tmui.Core;

namespace Tmui.Immediate;

public record struct Style(
    AccentStyle Accent,
    PanelStyle Panel,
    LabelStyle Label,
    TextBoxStyle TextBox,
    ButtonStyle Button,
    CheckboxStyle Checkbox,
    DropdownStyle Dropdown,
    ScrollbarStyle ScrollbarH,
    ScrollbarStyle ScrollbarV
);

public record struct ColorByInteraction(Color Default, Color Hover, Color Active)
{
#if !DEBUG || DEBUG_RELEASE
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
    public Color GetColor(Interaction interaction)
    {
        if (interaction.Active) return Active;
        else if (interaction.Hover) return Hover;
        else return Default;
    }
}

public record struct ScrollbarStyle(
    char TrackChar,
    Color TrackColor,

    char ThumbChar
);

public record struct AccentStyle(
    Color Default,
    Color Disabled
)
{
    public Color GetColor(bool enabled)
    {
        return enabled ? Default : Disabled;
    }
}

public record struct PanelStyle(
    Color BgColor
);

public record struct LabelStyle(
    Color TextColor,
    Color BgColor
);

public record struct TextBoxStyle(
    Color TextColor,
    Color BgColor,
    Thickness Padding
);

public record struct ButtonStyle(
    Color TextColor,
    ColorByInteraction BgColor
);

public record struct CheckboxStyle
(
    char CheckmarkChar,
    Color CheckmarkColor,
    ColorByInteraction BgColor
);

public record struct DropdownStyle(
    ButtonStyle Header,
    ButtonStyle Option
);
