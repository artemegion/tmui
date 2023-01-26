using Tmui.Core;
using Tmui.Graphics;

namespace Tmui.Immediate;

public partial class Ui
{
    public bool Button(Rect rect, ReadOnlySpan<char> text, TextAlignVH textAlign, ButtonStyle? buttonStyle = null, AccentStyle? accentStyle = null)
    {
        int controlId = CreateControlId();;
        buttonStyle ??= Style.Button;
        accentStyle ??= Style.Accent;

        var interaction = GetInteraction(rect, controlId);

        Color bgColor = buttonStyle.Value.BgColor.GetColor(interaction);

        Surface.FillRect(rect, bgColor);
        Surface.DrawChar(new(rect.X, rect.Y), Glyph.LEFT_HALF_BLOCK_CHAR, accentStyle.Value.GetColor(Enabled));
        Surface.DrawText(rect with { X = rect.X + 1, W = rect.W - 1 }, text, textAlign, true, buttonStyle.Value.TextColor);

        if(interaction.Clicked) Changed = true;
        return interaction.Clicked;
    }
}
