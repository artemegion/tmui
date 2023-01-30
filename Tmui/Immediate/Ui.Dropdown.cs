using System.Buffers;

using Tmui.Core;
using Tmui.Graphics;

namespace Tmui.Immediate;

// TODO: + options list scrolling
//       + overload with parameter for maximum options list height
//       + auto maximum options list with/height/position as to fit in the buffer

public partial class Ui
{
    public void Dropdown(Rect rect, Span<string> options, ref int selectedOption, DropdownStyle? dropdownStyle = null, AccentStyle? accentStyle = null)
    {
        int controlId = CreateControlId();
        dropdownStyle ??= Style.Dropdown;
        accentStyle ??= Style.Accent;

        selectedOption = int.Clamp(selectedOption, 0, options.Length - 1);

        // var headerInteraction = GetInteraction(rect, controlId);
        var headerInteraction = Interactions.Get(rect, controlId);

        Color bgColor = dropdownStyle.Value.Header.BgColor.GetColor(headerInteraction);

        Surface.FillRect(rect, bgColor);
        Surface.DrawText(rect with { X = rect.X + 3, W = rect.W - 3 }, options[selectedOption], (TextAlign.Center, TextAlign.Start), true, dropdownStyle.Value.Header.TextColor);

        Pos triangleCharPos = new(rect.X + 1, rect.Y + rect.H / 2);
        Surface.DrawChar(triangleCharPos, _openedDropdownId == controlId ? Glyph.TRIANGLE_DOWN_CHAR : Glyph.TRIANGLE_RIGHT_CHAR, accentStyle.Value.GetColor(Enabled));

        Span<Range> lines = stackalloc Range[10];

        if (_openedDropdownId == controlId)
        {
            // calculate bounds of all the options to use as mask
            Rect optionsRect = rect;

            for (int optionIndex = 0; optionIndex < options.Length; optionIndex++)
            {
                Surface.WrapText(options[optionIndex], rect.W, lines, out int wrappedLinesCount);

                Rect optionRect = new(rect.X, optionsRect.Y + optionsRect.H, rect.W, wrappedLinesCount);
                optionsRect.H += wrappedLinesCount;

                // var optionInteraction = GetInteraction(optionRect, controlId + 1000000 + optionIndex);
                var optionInteraction = Interactions.Get(optionRect, controlId * 100 + optionIndex);

                bgColor = dropdownStyle.Value.Option.BgColor.GetColor(optionInteraction);

                Surface.FillRect(optionRect, bgColor);
                Surface.DrawChar(new(optionRect.X + 1, optionRect.Y), '•', accentStyle.Value.GetColor(Enabled));
                Surface.DrawText(new(optionRect.X + 3, optionRect.Y, optionRect.W - 3, wrappedLinesCount), options[optionIndex], lines[..wrappedLinesCount], TextAlign.Start, dropdownStyle.Value.Option.TextColor);

                if (optionInteraction.Clicked)
                {
                    _ = Interactions.Get(optionRect, controlId);

                    int newSelectedOption = optionIndex;
                    if (newSelectedOption != selectedOption) Changed = true;

                    selectedOption = newSelectedOption;

                    _openedDropdownId = -1;

                    break;
                }
            }

            // `opened` may change inside the for loop above
            if (_openedDropdownId == controlId)
            {
                //_openedDropdownRect = optionsRect;    
                Interactions.PushOverride(new(Interaction.None, optionsRect, Enumerable.Range(controlId * 100 + 0, controlId * 100 + options.Length).Prepend(controlId).ToArray()));

                // masking the bounds of the options so controls drawn later can't draw over the dropdown
                Surface.Mask.Push(optionsRect, CompoundMask.MaskType.Exclusive);

                // clicking at the screen but not at the control should close the dropdown
                var bufferInteraction = Interactions.Get(new(0, 0, Terminal.BufferSize.X, Terminal.BufferSize.Y), -1);
                if (bufferInteraction.Clicked && !headerInteraction.Clicked)
                {
                    _openedDropdownId = -1;
                    ReqRedraw = true;
                }
            }
            else ReqRedraw = true;
        }

        if (headerInteraction.Clicked)
        {
            if (_openedDropdownId == controlId) _openedDropdownId = -1;
            else _openedDropdownId = controlId;

            ReqRedraw = true;
        }
    }
}
