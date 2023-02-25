using Tmui.Core;

namespace Tmui.Immediate;

public partial class Ui
{
    /// <summary>
    /// Draws a scrollbar.
    /// </summary>
    /// <param name="rect"></param>
    /// <param name="contentAxis"></param>
    /// <param name="contentAxisLength"></param>
    /// <param name="scrollValue"></param>
    /// <param name="inputIgnoresAxis"></param>
    /// <param name="scrollbarStyle"></param>
    /// <param name="accentStyle"></param>
    /// <exception cref="ArgumentException"></exception>
    public void Scrollbar(Rect rect, Axis contentAxis, int contentAxisLength, ref int scrollValue, bool inputIgnoresAxis = true, ScrollbarStyle? scrollbarStyle = null, AccentStyle? accentStyle = null)
    {
        scrollbarStyle ??= contentAxis == Axis.Vertical ? Style.ScrollbarV : Style.ScrollbarH;
        accentStyle ??= Style.Accent;

        int controlId = CreateControlId();
        int rectAxisLength = contentAxis == Axis.Vertical ? rect.H : rect.W;

        Surface.FillCharRect(rect, scrollbarStyle.Value.TrackChar, scrollbarStyle.Value.TrackColor);

        float visibleFrac = float.Clamp((float)rectAxisLength / contentAxisLength, 0f, 1f);
        int maxScrollValue = int.Max(0, contentAxisLength - (contentAxis == Axis.Vertical ? rect.H : rect.W));

        Interaction thumbInteraction = Interactions.Get(rect, controlId);

        if (thumbInteraction.Hover)
        {
            int scrollValueChange = 0;
            if (inputIgnoresAxis)
            {
                if (Input.Scroll.X == 0) scrollValueChange = Input.Scroll.Y;
                else if (Input.Scroll.Y == 0) scrollValueChange = Input.Scroll.X;
            }
            else scrollValueChange = contentAxis == Axis.Vertical ? Input.Scroll.Y : Input.Scroll.X;

            int newScrollValue = int.Clamp(
                scrollValue + scrollValueChange,
                0,
                maxScrollValue
            );

            if (scrollValue != newScrollValue || scrollValueChange != 0)
                ReqRedraw = true;

            scrollValue = newScrollValue;
        }

        if (visibleFrac > 0f)
        {
            float scrollFrac = (float)scrollValue / contentAxisLength;

            float thumbOffset =
                // only put the thumb at the start if scroll is 0, so its clear to the user that there is no more scrolling possible
                // similarly, thumb should be placed at the end only if scroll is max for the same reason
                (scrollFrac < 0.5f && scrollValue > 0) || (scrollFrac >= 0.5f && scrollValue == maxScrollValue)
                ? float.Ceiling(scrollFrac * rectAxisLength)
                : float.Floor(scrollFrac * rectAxisLength);

            Rect thumbRect = contentAxis switch
            {
                Axis.Vertical => new(rect.X, rect.Y + (int)thumbOffset, rect.W, (int)(rect.H * visibleFrac)),
                Axis.Horizontal => new(rect.X + (int)thumbOffset, rect.Y, (int)(rect.W * visibleFrac), rect.H),
                _ => throw new ArgumentException($"Not recognized {nameof(contentAxis)} value, expected one of Vertical or Horizontal, got {(int)contentAxis} instead.")
            };

            Surface.FillCharRect(thumbRect, scrollbarStyle.Value.ThumbChar, accentStyle.Value.GetColor(Enabled));
        }
    }
}
