using System.Buffers;

using Tmui.Core;
using Tmui.Extensions;
using Tmui.Graphics;

namespace Tmui.Immediate;

public partial class Ui
{
    private readonly Dictionary<int, int> _textBoxCursorPos = new(5);

    public void TextBox(Rect rect, Span<char> textBuffer, Span<Range> rangesOfLinesBuffer, TextAlignVH textAlign, TextBoxScrollFlags scrollFlags, TextBoxStyle? textBoxStyle = null)
    {
        textBoxStyle ??= Style.TextBox;
        int controlId = CreateControlId();

        ReadOnlySpan<char> text = textBuffer.TrimEnd('\0');

        if (text.Length > 0 && rangesOfLinesBuffer.GetLongest(text.Length) < 1)
            Surface.WrapText(text, CalcTextBoxTextWidth(rect, scrollFlags), rangesOfLinesBuffer, out _);

        if (Enabled)
        {
            Interaction interaction = Interactions.Get(rect, controlId);
            if (interaction.Clicked) FocusedControlId = controlId;

            if (FocusedControlId == controlId)
            {
                int cursorPos = _textBoxCursorPos.GetValueOrDefault(controlId, 0);
                var lineAtCursorPos = rangesOfLinesBuffer.GetFromIndex(text.Length, cursorPos);

                bool anyCharsWritten = false, anyCharsRemoved = false;

                // do text modification before cursor pos shenanigans to reuse the pos constraint etc. logic
                if (text.Length < textBuffer.Length)
                {
                    ReadOnlySpan<char> chars = Input.Chars;

                    if (chars.Length > 0)
                    {
                        textBuffer[cursorPos..text.Length].CopyTo(textBuffer[(cursorPos + 1)..]);
                        textBuffer[cursorPos] = chars[0];

                        text = textBuffer[0..(text.Length + 1)];
                        Surface.WrapText(text, rect.W, rangesOfLinesBuffer, out _);

                        anyCharsWritten = true;
                        ReqRedraw = true;
                    }
                }

                if (Input.KeyPressed(Key.Backspace))
                {
                    if (cursorPos > 0)
                    {
                        textBuffer[(cursorPos)..].CopyTo(textBuffer[(cursorPos - 1)..]);
                        text = textBuffer.TrimEnd('\0');

                        Surface.WrapText(text, rect.W, rangesOfLinesBuffer, out _);

                        anyCharsRemoved = true;
                        ReqRedraw = true;
                    }
                }

                if (Input.KeyPressed(Key.LeftArrow) || anyCharsRemoved)
                {
                    cursorPos = int.Clamp(cursorPos - 1, 0, text.Length);
                    ReqRedraw = true;
                }

                if (Input.KeyPressed(Key.RightArrow) || anyCharsWritten)
                {
                    cursorPos = int.Clamp(cursorPos + 1, 0, text.Length);
                    ReqRedraw = true;
                }

                if (Input.KeyPressed(Key.UpArrow))
                {
                    if (lineAtCursorPos.Index == 0) return;

                    int o = lineAtCursorPos.Offset;
                    cursorPos = int.Clamp(cursorPos - lineAtCursorPos.Offset - 1, 0, text.Length);

                    if (lineAtCursorPos.Offset <= rangesOfLinesBuffer.GetLength(lineAtCursorPos.Index - 1, text.Length))
                    {
                        lineAtCursorPos = rangesOfLinesBuffer.GetFromIndex(text.Length, cursorPos);
                        cursorPos = int.Clamp(cursorPos - rangesOfLinesBuffer.GetLength(lineAtCursorPos.Index, text.Length) + o, 0, text.Length);
                    }

                    ReqRedraw = true;
                }

                if (Input.KeyPressed(Key.DownArrow))
                {
                    if (lineAtCursorPos.Index >= rangesOfLinesBuffer.WithoutTrailingRanges(text.Length).Length) return;

                    cursorPos = int.Clamp(cursorPos + rangesOfLinesBuffer.GetLength(lineAtCursorPos.Index, text.Length) - lineAtCursorPos.Offset + 1 + lineAtCursorPos.Offset, 0, text.Length);
                    ReqRedraw = true;
                }

                lineAtCursorPos = rangesOfLinesBuffer.GetFromIndex(text.Length, cursorPos);

                _cursorPosPlease = new(rect.X + lineAtCursorPos.Offset, rect.Y + lineAtCursorPos.Index);
                _textBoxCursorPos[controlId] = cursorPos;
            }
        }

        --ControlId; // textbox gets a new id, this way it is the same
        TextBox(rect, text, rangesOfLinesBuffer, textAlign, scrollFlags, textBoxStyle);
    }

    public void TextBox(Rect rect, Span<char> textBuffer, Span<Range> rangesOfLinesBuffer, TextAlignVH textAlign, TextBoxStyle? textBoxStyle = null)
    {
        TextBox(rect, textBuffer, rangesOfLinesBuffer, textAlign, TextBoxScrollFlags.None, textBoxStyle);
    }

    public void TextBox(Rect rect, Span<char> textBuffer, TextAlignVH textAlign, TextBoxScrollFlags scrollFlags, TextBoxStyle? textBoxStyle = null)
    {
        int textWidth = CalcTextBoxTextWidth(rect, scrollFlags);

        ReadOnlySpan<char> text = textBuffer.TrimEnd('\0');

        // we don't how many lines the text will occupy, so we rent an array of size rect.H to calculate that
        // and if its not sufficient we rent a new one of appropriate size
        Range[] rangesOfLinesRented = ArrayPool<Range>.Shared.Rent(rect.H);

        Span<Range> rangesOfLinesSpan = new(rangesOfLinesRented, 0, rect.H);
        rangesOfLinesSpan.Clear();

        Surface.WrapText(text, textWidth, rangesOfLinesRented.AsSpan(0, rect.H), out int wrappedLines);

        // if there are more lines than what we predicted (rect.H), rent a sufficiently sized buffer and wrap the text again
        if (wrappedLines > rect.H)
        {
            ArrayPool<Range>.Shared.Return(rangesOfLinesRented);

            // request one line more than required so we can add characters/a new line to the text
            rangesOfLinesRented = ArrayPool<Range>.Shared.Rent(wrappedLines + 1);

            rangesOfLinesSpan = new(rangesOfLinesRented, 0, wrappedLines + 1);
            rangesOfLinesSpan.Clear();

            Surface.WrapText(text, textWidth, rangesOfLinesSpan, out _);
        }

        TextBox(rect, textBuffer, rangesOfLinesSpan, textAlign, scrollFlags, textBoxStyle);

        ArrayPool<Range>.Shared.Return(rangesOfLinesRented);
    }

    public void TextBox(Rect rect, Span<char> textBuffer, TextAlignVH textAlign, TextBoxStyle? textBoxStyle = null)
    {
        TextBox(rect, textBuffer, textAlign, TextBoxScrollFlags.None, textBoxStyle);
    }

    public void TextBox(Rect rect, Span<char> textBuffer, TextBoxStyle? textBoxStyle = null)
    {
        TextBox(rect, textBuffer, TextAlign.Start, TextBoxScrollFlags.None, textBoxStyle);
    }
}
