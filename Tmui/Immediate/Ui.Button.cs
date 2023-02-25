using Tmui.Core;
using Tmui.Graphics;

namespace Tmui.Immediate;

public partial class Ui
{
    /// <summary>
    /// Draws a button.
    /// </summary>
    /// <param name="rect">Rectangle to draw the button on.</param>
    /// <param name="text">Text to draw inside the button.</param>
    /// <param name="textAlign">Alignment of the <paramref name="text"/>.</param>
    /// <param name="buttonStyle">Button style, if null the button style from <see cref="Ui.Style"/> will be used.</param>
    /// <param name="accentStyle">Accent style, if null the accent style from <see cref="Ui.Style"/> will be used..</param>
    /// <returns>True if the button was clicked, false otherwise.</returns>
    public bool Button(Rect rect, ReadOnlySpan<char> text, TextAlignVH textAlign, ButtonStyle? buttonStyle = null, AccentStyle? accentStyle = null)
    {
        int controlId = CreateControlId();
        buttonStyle ??= Style.Button;
        accentStyle ??= Style.Accent;

        var interaction = Interactions.Get(rect, controlId);

        Color bgColor = buttonStyle.Value.BgColor.GetColor(interaction);

        Surface.FillRect(rect, bgColor);
        Surface.DrawChar(new(rect.X, rect.Y), Glyph.LEFT_HALF_BLOCK_CHAR, accentStyle.Value.GetColor(Enabled));
        Surface.DrawText(rect with { X = rect.X + 1, W = rect.W - 1 }, text, textAlign, true, buttonStyle.Value.TextColor);

        if (interaction.Clicked) Changed = true;
        return interaction.Clicked;
    }
}
