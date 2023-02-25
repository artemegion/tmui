using Tmui.Core;
using Tmui.Graphics;

namespace Tmui.Immediate;

public partial class Ui
{
    /// <summary>
    /// Draws a checkbox without a label.
    /// The <paramref name="isChecked"/> value will change if the checkbox was clicked.
    /// </summary>
    /// <param name="isChecked">True if the checkbox is checked, false otherwise.</param>
    /// <param name="pos">Position to draw the checkbox at.</param>
    /// <param name="checkboxStyle">Checkbox style, if null the checkbox style from <see cref="Ui.Style"/> will be used.</param>
    /// <param name="accentStyle">Accent style, if null the accent style from <see cref="Ui.Style"/> will be used.</param>
    public void Checkbox(ref bool isChecked, Pos pos, CheckboxStyle? checkboxStyle = null, AccentStyle? accentStyle = null)
    {
        int controlId = CreateControlId(); ;
        checkboxStyle ??= Style.Checkbox;
        accentStyle ??= Style.Accent;

        Rect rect = new(pos.X, pos.Y, 2, 1);

        var interaction = Interactions.Get(rect, controlId);

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

    /// <summary>
    /// Draws a checkbox with a label.
    /// The <paramref name="isChecked"/> value will change if the checkbox or label were clicked.
    /// </summary>
    /// <param name="isChecked">True if the checkbox is checked, false otherwise.</param>
    /// <param name="rect">Rectangle to draw the checkbox on.</param>
    /// <param name="label">The label to draw beside the checkbox.</param>
    /// <param name="labelAlign">The label's align inside the <paramref name="rect"/>.</param>
    /// <param name="checkboxStyle">Checkbox style, if null the checkbox style from <see cref="Ui.Style"/> will be used.</param>
    /// <param name="accentStyle">Accent style, if null the accent style from <see cref="Ui.Style"/> will be used.</param>
    /// <param name="textBoxStyle">TextBox style, if null the text box style from <see cref="Ui.Style"/> will be used.</param>
    public void Checkbox(ref bool isChecked, Rect rect, ReadOnlySpan<char> label, TextAlignVH labelAlign, CheckboxStyle? checkboxStyle = null, AccentStyle? accentStyle = null, TextBoxStyle? textBoxStyle = null)
    {
        int controlId = CreateControlId();
        checkboxStyle ??= Style.Checkbox;
        accentStyle ??= Style.Accent;
        textBoxStyle ??= Style.TextBox with { BgColor = Color.Transparent };

        // InteractionState state = GetInteraction(rect, controlId);
        Interaction state = Interactions.Get(rect, controlId);
        // if (state.Any) InteractionOverride.Push(state);
        if (state.Any) Interactions.PushOverride(new(state));

        Checkbox(ref isChecked, new(rect.X, rect.Y), checkboxStyle, accentStyle);
        TextBox(rect with { X = rect.X + 3, W = rect.W - 3 }, label, labelAlign, textBoxStyle);

        if (state.Any) Interactions.PopOverride();
    }
}
