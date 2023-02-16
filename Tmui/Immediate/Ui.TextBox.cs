using System.Buffers;

using Tmui.Core;
using Tmui.Extensions;
using Tmui.Graphics;

namespace Tmui.Immediate;

public partial class Ui
{
    public void TextBox(Rect rect, ReadOnlySpan<char> text, ReadOnlySpan<Range> rangesOfLines, TextAlignVH textAlign, TextBoxScrollFlags scrollFlags, TextBoxStyle? textBoxStyle = null)
    {
        textBoxStyle ??= Style.TextBox;
        int controlId = CreateControlId();
        Scroll scroll = GetScroll(controlId);

        Rect textRect = rect;

        int verticalContentLength = rangesOfLines.Length;
        bool canScrollVertical = scrollFlags.HasFlag(TextBoxScrollFlags.Vertical) && verticalContentLength > rect.H;

        int horizontalContentLength = rangesOfLines.GetLongest(text.Length);
        bool canScrollHorizontal = scrollFlags.HasFlag(TextBoxScrollFlags.Horizontal) && horizontalContentLength > rect.W;

        if (canScrollVertical)
        {
            // text align doesn't change anything in this case so no sense computing it
            textAlign.V = TextAlign.Start;
        }

        Surface.FillRect(rect, textBoxStyle.Value.BgColor);

        Interaction textInteraction = Interactions.Get(new(rect.X, rect.Y, canScrollVertical ? rect.W - 1 : rect.W, canScrollHorizontal ? rect.H - 1 : rect.H), controlId);

        if (canScrollHorizontal || scrollFlags.HasFlag(TextBoxScrollFlags.AlwaysShow))
        {
            bool overrideScrollbarInteraction = textInteraction.Hover && Input.KeyHeld(Key.LeftShift);
            if (overrideScrollbarInteraction) Interactions.PushOverride(new(textInteraction, rect));

            bool e = Enabled;
            if (scrollFlags.HasFlag(TextBoxScrollFlags.DisableHorizontal))
                Enabled = false;

            if (!scrollFlags.HasFlag(TextBoxScrollFlags.HideHorizontal))
            {
                // TODO: textbox with height 1
                // textRect.H -= 1;
                Scrollbar(new(rect.X, rect.Y + rect.H - 1, canScrollVertical ? rect.W - 1 : rect.W, 1), Axis.Horizontal, horizontalContentLength, ref scroll.ScrollX);
            }

            Enabled = e;

            // if (overrideScrollbarInteraction) InteractionOverride.Pop();
            if (overrideScrollbarInteraction) Interactions.PopOverride();
        }
        else CreateControlId(); // use id even if no scrollbar is present, so when content changes it doesn't fuck up other control's state

        if (canScrollVertical || scrollFlags.HasFlag(TextBoxScrollFlags.AlwaysShow))
        {
            // if (textInteraction.Hover) InteractionOverride.Push(textInteraction);
            if (textInteraction.Hover) Interactions.PushOverride(new(textInteraction, rect));

            bool e = Enabled;
            if (scrollFlags.HasFlag(TextBoxScrollFlags.DisableVertical))
                Enabled = false;

            if (!scrollFlags.HasFlag(TextBoxScrollFlags.HideVertical))
            {
                // textRect.W -= 1;
                Scrollbar(new(rect.X + rect.W - 1, rect.Y, 1, canScrollHorizontal ? rect.H - 1 : rect.H), Axis.Vertical, verticalContentLength, ref scroll.ScrollY);
            }

            Enabled = e;

            // if (textInteraction.Hover) InteractionOverride.Pop();
            if (textInteraction.Hover) Interactions.PopOverride();
        }
        else CreateControlId();

        // scroll horizontal by moving the text left by scroll value
        textRect.X -= scroll.ScrollX;
        textRect.W += scroll.ScrollX;

        // because we only draw rect.H lines, in order to scroll vertically we can just skip scroll.Y lines
        if (scroll.ScrollY >= rangesOfLines.Length) rangesOfLines = Span<Range>.Empty;
        else rangesOfLines = rangesOfLines[scroll.ScrollY..];

        bool h = (canScrollHorizontal || scrollFlags.HasFlag(TextBoxScrollFlags.AlwaysShow)) && !scrollFlags.HasFlag(TextBoxScrollFlags.HideHorizontal);
        bool v = (canScrollVertical || scrollFlags.HasFlag(TextBoxScrollFlags.AlwaysShow)) && !scrollFlags.HasFlag(TextBoxScrollFlags.HideVertical);
        Rect maskRect = (h, v) switch
        {
            (true, true) => new(rect.X, rect.Y, rect.W - 1, rect.H - 1),
            (true, false) => new(rect.X, rect.Y, rect.W, rect.H - 1),
            (false, true) => new(rect.X, rect.Y, rect.W - 1, rect.H),
            (false, false) => rect
        };

        Surface.Mask.PushExclusiveArea(maskRect);

        Surface.DrawText(textRect, text, rangesOfLines, textAlign, textBoxStyle.Value.TextColor, longestLineLength: horizontalContentLength);

        Surface.Mask.PopExclusiveArea();
    }

    public void TextBox(Rect rect, ReadOnlySpan<char> text, ReadOnlySpan<Range> rangesOfLines, TextAlignVH textAlign, TextBoxStyle? textBoxStyle = null)
    {
        TextBox(rect, text, rangesOfLines, textAlign, TextBoxScrollFlags.None, textBoxStyle);
    }

    public void TextBox(Rect rect, ReadOnlySpan<char> text, TextAlignVH textAlign, TextBoxScrollFlags scrollFlags, TextBoxStyle? textBoxStyle = null)
    {
        int textWidth = scrollFlags.HasFlag(TextBoxScrollFlags.Horizontal)
            ? int.MaxValue
            : (scrollFlags.HasFlag(TextBoxScrollFlags.Vertical) && !scrollFlags.HasFlag(TextBoxScrollFlags.HideVertical)
                ? rect.W - 1
                : rect.W);

        Range[] rentedArr = ArrayPool<Range>.Shared.Rent(rect.H);
        Span<Range> rangesOfLines = new(rentedArr, 0, rect.H);
        rangesOfLines.Clear();

        Surface.WrapText(text, textWidth, rangesOfLines, out int wrappedLines);

        // If there are more lines than what we predicted (rect.H), get a sufficiently sized buffer and wrap the text again.
        // May not be the most optimal solution, but it works.
        if (wrappedLines > rangesOfLines.Length)
        {
            ArrayPool<Range>.Shared.Return(rentedArr);

            rentedArr = ArrayPool<Range>.Shared.Rent(wrappedLines);
            rangesOfLines = new(rentedArr, 0, wrappedLines);
            rangesOfLines.Clear();

            Surface.WrapText(text, textWidth, rangesOfLines, out _);
        }

        TextBox(rect, text, rangesOfLines, textAlign, scrollFlags, textBoxStyle);

        ArrayPool<Range>.Shared.Return(rentedArr);
    }

    public void TextBox(Rect rect, ReadOnlySpan<char> text, TextAlignVH textAlign, TextBoxStyle? textBoxStyle = null)
    {
        TextBox(rect, text, textAlign, TextBoxScrollFlags.None, textBoxStyle);
    }

    public void TextBox(Rect rect, ReadOnlySpan<char> text, TextBoxStyle? textBoxStyle = null)
    {
        TextBox(rect, text, TextAlign.Start, TextBoxScrollFlags.None, textBoxStyle);
    }
}
