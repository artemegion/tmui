using Tmui.Core;
using Tmui.Extensions;
using Tmui.Graphics;

namespace Tmui.Immediate;

public partial class Ui
{
    public void TextBox(Rect rect, ReadOnlySpan<char> text, bool wrapText, TextAlignVH textAlign, TextBoxStyle? textBoxStyle = null)
    {
        int p0 = 0;

        Span<Range> rangesOfLines = stackalloc Range[rect.H];
        Surface.WrapText(text, wrapText ? rect.W : int.MaxValue, rangesOfLines, out int wrappedLinesCount);

        TextBox(rect, text, rangesOfLines, textAlign, TextBoxScrollFlags.None, ref p0, ref p0, textBoxStyle);
    }

    public void TextBox(Rect rect, ReadOnlySpan<char> text, TextAlignVH textAlign, TextBoxStyle? textBoxStyle = null)
    {
        int p0 = 0;

        Span<Range> rangesOfLines = stackalloc Range[rect.H];
        Surface.WrapText(text, int.MaxValue, rangesOfLines, out int wrappedLinesCount);

        TextBox(rect, text, rangesOfLines, textAlign, TextBoxScrollFlags.None, ref p0, ref p0, textBoxStyle);
    }

    public void TextBox(Rect rect, ReadOnlySpan<char> text, bool wrapText, TextAlignVH textAlign, TextBoxScrollFlags scroll, ref int vScroll, ref int hScroll, TextBoxStyle? textBoxStyle = null)
    {
        Span<Range> rangesOfLines = stackalloc Range[rect.H];
        Surface.WrapText(text, wrapText ? rect.W : int.MaxValue, rangesOfLines, out int wrappedLinesCount);

        TextBox(rect, text, rangesOfLines, textAlign, scroll, ref vScroll, ref hScroll, textBoxStyle);
    }

    public void TextBox(Rect rect, ReadOnlySpan<char> text, TextAlignVH textAlign, TextBoxScrollFlags scroll, ref int vScroll, ref int hScroll, TextBoxStyle? textBoxStyle = null)
    {
        Span<Range> rangesOfLines = stackalloc Range[rect.H];
        Surface.WrapText(text, int.MaxValue, rangesOfLines, out int wrappedLinesCount);

        TextBox(rect, text, rangesOfLines, textAlign, scroll, ref vScroll, ref hScroll, textBoxStyle);
    }

    public void TextBox(Rect rect, ReadOnlySpan<char> text, ReadOnlySpan<Range> rangesOfLines, TextAlignVH textAlign, TextBoxScrollFlags scroll, ref int vScroll, ref int hScroll, TextBoxStyle? textBoxStyle = null)
    {
        int controlId = CreateControlId();
        textBoxStyle ??= Style.TextBox;

        int textX = rect.X, textY = rect.Y;
        int textW = rect.W, textH = rect.H;

        int verticalContentLength = rangesOfLines.Length;
        bool canScrollVertical = scroll.HasFlag(TextBoxScrollFlags.Vertical) && verticalContentLength > rect.H;

        int horizontalContentLength = rangesOfLines.GetLongest(text.Length);
        bool canScrollHorizontal = scroll.HasFlag(TextBoxScrollFlags.Horizontal) && horizontalContentLength > rect.W;

        if (canScrollHorizontal)
        {
            textX -= hScroll;
            textW += hScroll;
        }

        if (canScrollVertical)
        {
            // text align doesn't change anything in this case so no sense computing it
            textAlign.V = TextAlign.Start;

            // because we only draw rect.H lines, in order to scroll we can just skip vScroll lines
            if (vScroll >= rangesOfLines.Length) rangesOfLines = ReadOnlySpan<Range>.Empty;
            else rangesOfLines = rangesOfLines[vScroll..];
        }

        Surface.FillRect(rect, textBoxStyle.Value.BgColor);

        InteractionState tbIxnState = GetInteraction(rect with { W = canScrollVertical ? rect.W - 1 : rect.W, H = canScrollHorizontal ? rect.H - 1 : rect.H }, controlId);

        if (canScrollHorizontal || scroll.HasFlag(TextBoxScrollFlags.AlwaysShow))
        {
            bool overrideIxn = tbIxnState.Hover && Input.KeyHeld(Key.LeftShift);
            if (overrideIxn) InteractionOverride.Push(tbIxnState);

            textH -= 1;
            Scrollbar(new(rect.X, rect.Y + rect.H - 1, canScrollVertical ? rect.W - 1 : rect.W, 1), Axis.Horizontal, horizontalContentLength, ref hScroll);

            if (overrideIxn) InteractionOverride.Pop();
        }
        else CreateControlId(); // textbox should always use 3 control ids, so changes in textbox's content will not change ids of controls drawn after it

        if (canScrollVertical || scroll.HasFlag(TextBoxScrollFlags.AlwaysShow))
        {
            if (tbIxnState.Hover) InteractionOverride.Push(tbIxnState);

            textW -= 1;
            Scrollbar(new(rect.X + rect.W - 1, rect.Y, 1, canScrollHorizontal ? rect.H - 1 : rect.H), Axis.Vertical, verticalContentLength, ref vScroll);

            if (tbIxnState.Hover) InteractionOverride.Pop();
        }
        else CreateControlId();

        Rect maskRect = (canScrollHorizontal || scroll.HasFlag(TextBoxScrollFlags.AlwaysShow), canScrollVertical || scroll.HasFlag(TextBoxScrollFlags.AlwaysShow)) switch
        {
            (true, true) => new(rect.X, rect.Y, rect.W - 1, rect.H - 1),
            (true, false) => new(rect.X, rect.Y, rect.W, rect.H - 1),
            (false, true) => new(rect.X, rect.Y, rect.W - 1, rect.H),
            (false, false) => rect
        };

        Surface.Mask.Push(maskRect);

        Surface.DrawText(new(textX, textY, textW, textH), text, rangesOfLines, textAlign, textBoxStyle.Value.TextColor);

        Surface.Mask.Pop();
    }
}
