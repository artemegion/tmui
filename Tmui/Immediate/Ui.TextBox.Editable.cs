using System.Buffers;

using Tmui.Core;
using Tmui.Extensions;
using Tmui.Graphics;

namespace Tmui.Immediate;

public partial class Ui
{
    private readonly Dictionary<int, Pos> _textBoxCursorPos = new(5);

    public void TextBox(Rect rect, Span<char> textBuffer, Span<Range> rangesOfLinesBuffer, TextAlignVH textAlign, TextBoxScrollFlags scrollFlags, TextBoxStyle? textBoxStyle = null)
    {
        textBoxStyle ??= Style.TextBox;
        int controlId = CreateControlId();

        ReadOnlySpan<char> text = textBuffer.TrimEnd('\0');

        if (text.Length > 0 && rangesOfLinesBuffer.GetLongest(text.Length) < 1)
            Surface.WrapText(text, CalcTextBoxTextWidth(rect, scrollFlags), rangesOfLinesBuffer, out _);

        Interaction interaction = Interactions.Get(rect, controlId);
        if (interaction.Clicked) FocusedControlId = controlId;

        if (FocusedControlId == controlId)
        {
            Span<Range> lines = rangesOfLinesBuffer.WithoutTrailingRanges(text.Length);
            Pos cursorPos = _textBoxCursorPos.GetValueOrDefault(controlId, new(0, 0));
            bool anyCharsWritten = false, anyCharsRemoved = false;

            // do text modification before cursor pos shenanigans to reuse the pos constraint etc. logic
            if (text.Length < textBuffer.Length)
            {
                ReadOnlySpan<char> chars = Input.Chars;

                if (chars.Length > 0)
                {
                    int cursorPosAsTextIndex = rangesOfLinesBuffer[cursorPos.Y].GetOffsetAndLength(text.Length).Offset + cursorPos.X;

                    textBuffer[cursorPosAsTextIndex..text.Length].CopyTo(textBuffer[(cursorPosAsTextIndex + 1)..]);
                    textBuffer[cursorPosAsTextIndex] = chars[0];

                    text = textBuffer[0..(text.Length + 1)];
                    Surface.WrapText(text, rect.W, rangesOfLinesBuffer, out _);

                    // cursorPos.X++;
                    anyCharsWritten = true;
                    ReqRedraw = true;
                }
            }

            if (Input.KeyPressed(Key.Backspace))
            {
                int cursorPosAsTextIndex = rangesOfLinesBuffer[cursorPos.Y].GetOffsetAndLength(text.Length).Offset + cursorPos.X;

                textBuffer[cursorPosAsTextIndex..text.Length].CopyTo(textBuffer[(cursorPosAsTextIndex - 1)..]);

                text = textBuffer[0..(text.Length + 1)];
                Surface.WrapText(text, rect.W, rangesOfLinesBuffer, out _);

                // cursorPos.X--;
                anyCharsRemoved = true;
                ReqRedraw = true;
            }

            if (Input.KeyPressed(Key.LeftArrow) || anyCharsRemoved)
            {
                cursorPos.X--; ReqRedraw = true;

                if (cursorPos.X < 0)
                {
                    if (cursorPos.Y > 0)
                    {
                        cursorPos.Y--;
                        cursorPos.X = lines.GetLength(cursorPos.Y, text.Length);
                    }
                    else
                    {
                        cursorPos.X = 0;
                    }
                }
            }

            if (Input.KeyPressed(Key.RightArrow) || anyCharsWritten)
            {
                cursorPos.X++; ReqRedraw = true;

                if (cursorPos.X > lines.GetLength(cursorPos.Y, text.Length))
                {
                    if (cursorPos.Y < lines.Length - 1)
                    {
                        cursorPos.Y++;
                        cursorPos.X = anyCharsWritten ? 1 : 0;
                    }
                    else
                    {
                        cursorPos.X = lines.GetLength(cursorPos.Y, text.Length);
                    }
                }
            }

            if (Input.KeyPressed(Key.UpArrow))
            {
                cursorPos.Y--; ReqRedraw = true;

                if (cursorPos.Y < 0)
                {
                    cursorPos.Y = 0;
                }
                else
                {
                    if (cursorPos.X > lines.GetLength(cursorPos.Y, text.Length))
                        cursorPos.X = lines.GetLength(cursorPos.Y, text.Length);
                }
            }

            if (Input.KeyPressed(Key.DownArrow))
            {
                cursorPos.Y++; ReqRedraw = true;

                if (cursorPos.Y > lines.Length - 1)
                {
                    cursorPos.Y = lines.Length - 1;
                }
                else
                {
                    if (cursorPos.X > lines.GetLength(cursorPos.Y, text.Length))
                        cursorPos.X = lines.GetLength(cursorPos.Y, text.Length);
                }
            }

            _cursorPosPlease = new(rect.X + cursorPos.X, rect.Y + cursorPos.Y);
            _textBoxCursorPos[controlId] = cursorPos;
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
