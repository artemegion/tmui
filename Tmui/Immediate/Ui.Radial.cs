using Tmui.Core;
using Tmui.Graphics;

namespace Tmui.Immediate;

public partial class Ui
{
    public void Radial(Pos pos, CheckboxStyle? checkboxStyle = null, AccentStyle? accentStyle = null)
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

        if (_radialGroups.Count > 0)
        {
            (int RadialId, int CheckedId) = _radialGroups.Pop();
            RadialId++;

            if (interaction.Clicked)
            {
                CheckedId = RadialId;
                Changed = true;
            }

            if (CheckedId == RadialId)
                Surface.DrawChar(pos with { X = pos.X + 1 }, checkboxStyle.Value.CheckmarkChar, checkboxStyle.Value.CheckmarkColor);

            _radialGroups.Push((RadialId, CheckedId));
        }
    }

    public void BeginRadialGroup(int checkedId)
    {
        _radialGroups.Push((-1, checkedId));
    }

    public void EndRadialGroup(out int checkedId)
    {
        int newCheckedId = _radialGroups.Pop().Item2;
        checkedId = newCheckedId;
    }
}