using System.Text;

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
            Surface.WrapText(text, rect.W - 1, rangesOfLinesBuffer, out _);

        Interaction interaction = Interactions.Get(rect, controlId);
        if (interaction.Clicked) FocusedControlId = controlId;

        if (FocusedControlId == controlId)
        {
            Span<Range> lines = rangesOfLinesBuffer.WithoutTrailingRanges(text.Length);
            Pos cursorPos = _textBoxCursorPos.GetValueOrDefault(controlId, new(0, 0));

            // do text modification before cursor pos shenanigans to reuse the pos constraint etc. logic
            if (text.Length < textBuffer.Length)
            {
                if (_charInputThisFrame.Length > 0)
                {
                    int cursorPosAsTextIndex = rangesOfLinesBuffer[cursorPos.Y].GetOffsetAndLength(text.Length).Offset + cursorPos.X;

                    textBuffer[cursorPosAsTextIndex..text.Length].CopyTo(textBuffer[(cursorPosAsTextIndex + 1)..]);
                    textBuffer[cursorPosAsTextIndex] = _charInputThisFrame[0];

                    text = textBuffer[0..(text.Length + 1)];
                    Surface.WrapText(text, rect.W - 1, rangesOfLinesBuffer, out _);

                    cursorPos.X++;
                    ReqRedraw = true;
                }
            }

            if (Input.KeyPressed(Key.LeftArrow))
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

            if (Input.KeyPressed(Key.RightArrow))
            {
                cursorPos.X++; ReqRedraw = true;

                if (cursorPos.X > lines.GetLength(cursorPos.Y, text.Length))
                {
                    if (cursorPos.Y < lines.Length - 1)
                    {
                        cursorPos.Y++;
                        cursorPos.X = 0;
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
}
