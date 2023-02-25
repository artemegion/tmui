using System.Buffers;

using Tmui.Core;
using Tmui.Extensions;
using Tmui.Graphics;

namespace Tmui.Immediate;

public partial class Ui
{
    /// <summary>
    /// Draws a textbox.
    /// </summary>
    /// <param name="rect"></param>
    /// <param name="text"></param>
    /// <param name="rangesOfLines"></param>
    /// <param name="textAlign"></param>
    /// <param name="scrollFlags"></param>
    /// <param name="textBoxStyle"></param>
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

        bool showHorizontal = (canScrollHorizontal || scrollFlags.HasFlag(TextBoxScrollFlags.AlwaysShow)) && !scrollFlags.HasFlag(TextBoxScrollFlags.HideHorizontal);
        bool showVertical = (canScrollVertical || scrollFlags.HasFlag(TextBoxScrollFlags.AlwaysShow)) && !scrollFlags.HasFlag(TextBoxScrollFlags.HideVertical);

        if (canScrollVertical)
        {
            // text align doesn't change anything in this case so no sense computing it
            textAlign.V = TextAlign.Start;
        }

        Surface.FillRect(rect, textBoxStyle.Value.BgColor);

        Interaction textInteraction = Interactions.Get(new(rect.X, rect.Y, canScrollVertical ? rect.W - 1 : rect.W, canScrollHorizontal ? rect.H - 1 : rect.H), controlId);

        if (showHorizontal)
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
                Scrollbar(new(rect.X, rect.Y + rect.H - 1, showVertical ? rect.W - 1 : rect.W, 1), Axis.Horizontal, horizontalContentLength, ref scroll.ScrollX);
            }

            Enabled = e;

            // if (overrideScrollbarInteraction) InteractionOverride.Pop();
            if (overrideScrollbarInteraction) Interactions.PopOverride();
        }
        else CreateControlId(); // use id even if no scrollbar is present, so when content changes it doesn't fuck up other control's state

        if (showVertical)
        {
            // if (textInteraction.Hover) InteractionOverride.Push(textInteraction);
            if (textInteraction.Hover) Interactions.PushOverride(new(textInteraction, rect));

            bool e = Enabled;
            if (scrollFlags.HasFlag(TextBoxScrollFlags.DisableVertical))
                Enabled = false;

            if (!scrollFlags.HasFlag(TextBoxScrollFlags.HideVertical))
            {
                // textRect.W -= 1;
                Scrollbar(new(rect.X + rect.W - 1, rect.Y, 1, showHorizontal ? rect.H - 1 : rect.H), Axis.Vertical, verticalContentLength, ref scroll.ScrollY);
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

        Rect maskRect = (showHorizontal, showVertical) switch
        {
            (true, true) => new(rect.X, rect.Y, rect.W - 1, rect.H - 1),
            (true, false) => new(rect.X, rect.Y, rect.W, rect.H - 1),
            (false, true) => new(rect.X, rect.Y, rect.W - 1, rect.H),
            (false, false) => rect
        };

        if (showVertical) textRect.W -= 1;

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
        int textWidth = CalcTextBoxTextWidth(rect, scrollFlags);

        ReadOnlySpan<char> textTrimmed = text.TrimEnd('\0');

        // we don't how many lines the text will occupy, so we rent an array of size rect.H to calculate that
        // and if its not sufficient we rent a new one of appropriate size
        Range[] rangesOfLinesRented = ArrayPool<Range>.Shared.Rent(rect.H);

        Span<Range> rangesOfLinesSpan = new(rangesOfLinesRented, 0, rect.H);
        rangesOfLinesSpan.Clear();

        Surface.WrapText(textTrimmed, textWidth, rangesOfLinesRented.AsSpan(0, rect.H), out int wrappedLines);

        // if there are more lines than what we predicted (rect.H), rent a sufficiently sized buffer and wrap the text again
        if (wrappedLines > rect.H)
        {
            ArrayPool<Range>.Shared.Return(rangesOfLinesRented);

            // request one line more than required so we can add characters/a new line to the text
            rangesOfLinesRented = ArrayPool<Range>.Shared.Rent(wrappedLines + 1);

            rangesOfLinesSpan = new(rangesOfLinesRented, 0, wrappedLines);
            rangesOfLinesSpan.Clear();

            Surface.WrapText(textTrimmed, textWidth, rangesOfLinesSpan, out _);
        }

        TextBox(rect, textTrimmed, rangesOfLinesSpan, textAlign, scrollFlags, textBoxStyle);

        ArrayPool<Range>.Shared.Return(rangesOfLinesRented);
    }

    public void TextBox(Rect rect, ReadOnlySpan<char> text, TextAlignVH textAlign, TextBoxStyle? textBoxStyle = null)
    {
        TextBox(rect, text, textAlign, TextBoxScrollFlags.None, textBoxStyle);
    }

    public void TextBox(Rect rect, ReadOnlySpan<char> text, TextBoxStyle? textBoxStyle = null)
    {
        TextBox(rect, text, TextAlign.Start, TextBoxScrollFlags.None, textBoxStyle);
    }

    private static int CalcTextBoxTextWidth(Rect rect, TextBoxScrollFlags scrollFlags)
    {
        return scrollFlags.HasFlag(TextBoxScrollFlags.Horizontal)
            ? int.MaxValue
            : (scrollFlags.HasFlag(TextBoxScrollFlags.Vertical) && !scrollFlags.HasFlag(TextBoxScrollFlags.HideVertical)
                ? rect.W - 1
                : rect.W);
    }
}
