using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Tmui.Core;
using Tmui.Extensions;
using Tmui.Graphics;

namespace Tmui.Immediate;

public struct EditTextBoxState
{
    public int ScrollX;
    public int ScrollY;
    public int CursorX;
    public int CursorY;
    public bool Focus;
}

public partial class Ui
{
    public void EditTextBox(Rect rect, Span<char> text, Span<Range> rangesOfLines, TextAlignVH textAlign, TextBoxScrollFlags scrollFlags, ref EditTextBoxState state, TextBoxStyle? textBoxStyle = null)
    {
        int controlId = CreateControlId();
        textBoxStyle ??= Style.TextBox;

        int textX = rect.X, textY = rect.Y;
        int textW = rect.W, textH = rect.H;

        int verticalContentLength = rangesOfLines.Length;
        bool canScrollVertical = scrollFlags.HasFlag(TextBoxScrollFlags.Vertical) && verticalContentLength > rect.H;

        int horizontalContentLength = rangesOfLines.GetLongest(text.Length);
        bool canScrollHorizontal = scrollFlags.HasFlag(TextBoxScrollFlags.Horizontal) && horizontalContentLength > rect.W;

        if (canScrollHorizontal)
        {
            textX -= state.ScrollX;
            textW += state.ScrollX;
        }

        if (canScrollVertical)
        {
            // text align doesn't change anything in this case so no sense computing it
            textAlign.V = TextAlign.Start;

            // because we only draw rect.H lines, in order to scroll we can just skip vScroll lines
            if (state.ScrollY >= rangesOfLines.Length) rangesOfLines = Span<Range>.Empty;
            else rangesOfLines = rangesOfLines[state.ScrollY..];
        }

        Surface.FillRect(rect, textBoxStyle.Value.BgColor);

        Rect tbContentRect = rect with { W = canScrollVertical ? rect.W - 1 : rect.W, H = canScrollHorizontal ? rect.H - 1 : rect.H };
        InteractionState tbIxnState = GetInteraction(tbContentRect, controlId);

        if (canScrollHorizontal || scrollFlags.HasFlag(TextBoxScrollFlags.AlwaysShow))
        {
            bool overrideIxn = tbIxnState.Hover && Input.KeyHeld(Key.LeftShift);
            if (overrideIxn) InteractionOverride.Push(tbIxnState);

            textH -= 1;
            Scrollbar(new(rect.X, rect.Y + rect.H - 1, canScrollVertical ? rect.W - 1 : rect.W, 1), Axis.Horizontal, horizontalContentLength, ref state.ScrollX);

            if (overrideIxn) InteractionOverride.Pop();
        }
        else CreateControlId(); // textbox should always use 3 control ids, so changes in textbox's content will not change ids of controls drawn after it

        if (canScrollVertical || scrollFlags.HasFlag(TextBoxScrollFlags.AlwaysShow))
        {
            if (tbIxnState.Hover) InteractionOverride.Push(tbIxnState);

            textW -= 1;
            Scrollbar(new(rect.X + rect.W - 1, rect.Y, 1, canScrollHorizontal ? rect.H - 1 : rect.H), Axis.Vertical, verticalContentLength, ref state.ScrollY);

            if (tbIxnState.Hover) InteractionOverride.Pop();
        }
        else CreateControlId();

        Rect maskRect = (canScrollHorizontal || scrollFlags.HasFlag(TextBoxScrollFlags.AlwaysShow), canScrollVertical || scrollFlags.HasFlag(TextBoxScrollFlags.AlwaysShow)) switch
        {
            (true, true) => new(rect.X, rect.Y, rect.W - 1, rect.H - 1),
            (true, false) => new(rect.X, rect.Y, rect.W, rect.H - 1),
            (false, true) => new(rect.X, rect.Y, rect.W - 1, rect.H),
            (false, false) => rect
        };

        Surface.Mask.Push(maskRect);

        Surface.DrawText(new(textX, textY, textW, textH), text, rangesOfLines, textAlign, textBoxStyle.Value.TextColor);

        Surface.Mask.Pop();

        if (state.Focus)
        {
            // Surface.DrawChar(new(rect.X + state.CursorX, rect.Y + state.CursorY + 1), Glyph.UPPER_HALF_BLOCK_CHAR, Color.White);
            _cursorPosPlease = new(rect.X + state.CursorX, rect.Y + state.CursorY);
        }
        else
        {
            _cursorPosPlease = null;
        }

        InteractionState bufferIxnState = GetInteraction(new(0, 0, Terminal.BufferSize.X, Terminal.BufferSize.Y), -1);
        if (tbIxnState.Clicked) state.Focus = true;
        else if (bufferIxnState.Clicked) state.Focus = false;
    }
}
