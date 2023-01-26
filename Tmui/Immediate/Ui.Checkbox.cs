using Tmui.Core;
using Tmui.Graphics;

namespace Tmui.Immediate;

public partial class Ui
{
    public void Checkbox(ref bool isChecked, Pos pos, CheckboxStyle? checkboxStyle = null, AccentStyle? accentStyle = null)
    {
        int controlId = CreateControlId();;
        checkboxStyle ??= Style.Checkbox;
        accentStyle ??= Style.Accent;

        Rect rect = new(pos.X, pos.Y, 2, 1);

        var interaction = GetInteraction(rect, controlId);

        Color bgColor = checkboxStyle.Value.BgColor.GetColor(interaction);

        Surface.DrawPixel(pos, bgColor);
        Surface.DrawPixel(pos with { X = pos.X + 1 }, bgColor);
        Surface.DrawChar(pos with { X = pos.X + 2 }, Glyph.LEFT_HALF_BLOCK_CHAR, bgColor);
        Surface.DrawChar(pos, Glyph.LEFT_HALF_BLOCK_CHAR, accentStyle.Value.GetColor(Enabled));

        if (interaction.Clicked)
        {
            isChecked = !isChecked;
            Changed = true;
        }

        if (isChecked)
            Surface.DrawChar(pos with { X = pos.X + 1 }, checkboxStyle.Value.CheckmarkChar, checkboxStyle.Value.CheckmarkColor);
    }

    public void Checkbox(ref bool isChecked, Rect rect, ReadOnlySpan<char> label, TextAlignVH labelAlign, CheckboxStyle? checkboxStyle = null, AccentStyle? accentStyle = null, TextBoxStyle? textBoxStyle = null)
    {
        int controlId = CreateControlId();;
        checkboxStyle ??= Style.Checkbox;
        accentStyle ??= Style.Accent;
        textBoxStyle ??= Style.TextBox with { BgColor = Color.Transparent };

        InteractionState state = GetInteraction(rect, controlId);
        if (state.Any) InteractionOverride.Push(state);

        Checkbox(ref isChecked, new(rect.X, rect.Y), checkboxStyle, accentStyle);
        TextBox(rect with { X = rect.X + 3, W = rect.W - 3 }, label, labelAlign, textBoxStyle);

        if (state.Any) InteractionOverride.Pop();
    }
}
