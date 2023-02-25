using Tmui.Core;
using Tmui.Graphics;

namespace Tmui.Immediate;

public partial class Ui
{
    /// <summary>
    /// Draws a radial. Has to be drawn between calls to <see cref="Ui.BeginRadialGroup(int)"/> and <see cref="Ui.EndRadialGroup(out int)"/> to work properly.
    /// 
    /// <para>
    /// A radial is a type of checkbox used in groups where only one radial can be checked at a time.
    /// The selected radial is controlled through the parameter passed to <see cref="Ui.BeginRadialGroup(int)"/> and <see cref="Ui.EndRadialGroup(out int)"/>.
    /// </para>
    /// </summary>
    /// <param name="pos">Position to draw the radial at.</param>
    /// <param name="checkboxStyle">Checkbox style, if null the checkbox style from <see cref="Ui.Style"/> will be used.</param>
    /// <param name="accentStyle">Accent style, if null the accent style from <see cref="Ui.Style"/> will be used.</param>
    /// <seealso cref="Ui.BeginRadialGroup(int)"/>
    /// <seealso cref="Ui.EndRadialGroup(out int)"/>
    public void Radial(Pos pos, CheckboxStyle? checkboxStyle = null, AccentStyle? accentStyle = null)
    {
        int controlId = CreateControlId();
        checkboxStyle ??= Style.Checkbox;
        accentStyle ??= Style.Accent;

        Rect rect = new(pos.X, pos.Y, 2, 1);

        var interaction = Interactions.Get(rect, controlId);
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