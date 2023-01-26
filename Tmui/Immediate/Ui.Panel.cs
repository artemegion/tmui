using Tmui.Core;

namespace Tmui.Immediate;

public partial class Ui
{
    public void Panel(Rect rect, PanelStyle? panelStyle = null)
    {
        int controlId = CreateControlId();;
        panelStyle ??= Style.Panel;
        Surface.FillRect(rect, panelStyle.Value.BgColor);
    }
}
