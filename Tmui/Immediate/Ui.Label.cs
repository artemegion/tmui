using Tmui.Core;

namespace Tmui.Immediate;

public partial class Ui
{
    public void Label(Pos pos, ReadOnlySpan<char> text, LabelStyle? labelStyle = null)
    {
        int controlId = CreateControlId();
        labelStyle ??= Style.Label;

        GetInteraction(new(0,0,0,0), controlId); // to req redraw

        Surface.DrawLabel(pos, text, labelStyle.Value.TextColor, labelStyle.Value.BgColor);
    }
}
