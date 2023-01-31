using System.Text;

using Tmui.Core;
using Tmui.Extensions;
using Tmui.Graphics;

namespace Tmui.Immediate;

public partial class Ui
{
    private readonly Dictionary<int, Pos> _textBoxCursorPos = new(5);

    public void TextBox(Rect rect, Span<char> text, Span<Range> rangesOfLines, TextAlignVH textAlign, TextBoxScrollFlags scrollFlags, TextBoxStyle? textBoxStyle = null)
    {
        textBoxStyle ??= Style.TextBox;
        int controlId = CreateControlId();

        if (text.Length > 0 && rangesOfLines.GetLongest(text.Length) < 1)
            Surface.WrapText(text, rect.W - 1, rangesOfLines, out _);

        Interaction interaction = Interactions.Get(rect, controlId);
        if (interaction.Clicked) FocusedControlId = controlId;

        if (FocusedControlId == controlId)
        {
            Span<Range> lines = rangesOfLines.WithoutTrailingRanges(text.Length);
            Pos cursorPos = _textBoxCursorPos.GetValueOrDefault(controlId, new(0, 0));

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
        TextBox(rect, (ReadOnlySpan<char>)text, rangesOfLines, textAlign, scrollFlags, textBoxStyle);
    }
}
