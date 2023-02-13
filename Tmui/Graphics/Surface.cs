using Tmui.Core;
using Tmui.Extensions;

namespace Tmui.Graphics;

public struct Surface : IGraphicsContext
{
    public static void WrapText(ReadOnlySpan<char> text, int maxWidth, Span<Range> rangesOfLines, out int wrappedLinesCount, int skipLines = 0, bool countAllLines = true)
    {
        text = text.Trim();

        int totalLinesLength = 0; // sum of lengths of all lines

        static void PutInLines(in Span<Range> lines, int index, Range range)
        {
            if (index >= 0 && index < lines.Length)
                lines[index] = range;
        }

        for (wrappedLinesCount = -skipLines; wrappedLinesCount < (countAllLines ? int.MaxValue : rangesOfLines.Length); /*rect.H;*/ wrappedLinesCount++)
        {
            if (totalLinesLength > text.Length - 1) break;
            ReadOnlySpan<char> textSegment = text[totalLinesLength..(totalLinesLength + Math.Min(maxWidth, text.Length - totalLinesLength))];

            // newline character takes precedence, because we have to break
            // a line that would otherwise fit if it has a newline char
            int newlineIndex = textSegment.IndexOf('\n');

            if (newlineIndex > -1)
            {
                PutInLines(in rangesOfLines, wrappedLinesCount, new Range(totalLinesLength, totalLinesLength + newlineIndex));

                totalLinesLength += newlineIndex + 1;
                continue;
            }

            // check if breaking is needed, since its possible that textSegment
            // is made of whole words and no breaking is necessary

            //  end of text is right at the end of this line         or before the end of this line
            if (totalLinesLength + textSegment.Length == text.Length || textSegment.Length < maxWidth)
            {
                PutInLines(in rangesOfLines, wrappedLinesCount, new Range(totalLinesLength, totalLinesLength + textSegment.Length));

                totalLinesLength += textSegment.Length;
                continue;
            }
            // there are more characters in text after textSegment and the character right after textSegment
            // is a breaking char; whole textSegment can be put in this line
            else if (totalLinesLength + Math.Min(maxWidth, text.Length - totalLinesLength) + 1 < text.Length)
            {
                char charAfterTextSegment = text[totalLinesLength + textSegment.Length];
                if (charAfterTextSegment == ' ' || charAfterTextSegment == '\n')
                {
                    PutInLines(in rangesOfLines, wrappedLinesCount, new Range(totalLinesLength, totalLinesLength + textSegment.Length));

                    totalLinesLength += textSegment.Length + 1; // skip the next whitespace
                    continue;
                }
            }

            int spaceIndex = textSegment.LastIndexOf(' ');

            if (spaceIndex > -1)
            {
                PutInLines(in rangesOfLines, wrappedLinesCount, new Range(totalLinesLength, totalLinesLength + spaceIndex));

                totalLinesLength += spaceIndex + 1;
                continue;
            }
            else
            {
                PutInLines(in rangesOfLines, wrappedLinesCount, totalLinesLength..(totalLinesLength + textSegment.Length));

                totalLinesLength += textSegment.Length;
            }
        }

        if (countAllLines)
            wrappedLinesCount += skipLines;
    }


    public Surface(int width, int height)
    {
        ClearGlyph = new Glyph(' ', new(255, 255, 255), new(0, 0, 0));
        Width = width;
        Height = height;

        _buffer = new Glyph[width * height];
        _fakeBuffer = new Glyph[1];

        Mask = new();
    }

    private readonly Glyph[] _buffer;
    private readonly Glyph[] _fakeBuffer; // used for drawing outside the buffer coords

    public Glyph ClearGlyph { get; set; }
    public int Width { get; }
    public int Height { get; }

    public Masking Mask { get; }

    public ref Glyph GetGlyphRef(Pos pos)
    {
        if (pos.X < 0 || pos.X >= Width || pos.Y < 0 || pos.Y >= Height || !Mask.Test(pos))
            return ref _fakeBuffer[0];

        return ref _buffer[pos.Y * Width + pos.X];
    }

    public Glyph GetGlyph(Pos pos)
    {
        if (pos.X < 0 || pos.X >= Width || pos.Y < 0 || pos.Y >= Height)
            return _fakeBuffer[0];

        return _buffer[pos.Y * Width + pos.X];
    }

    public void Clear()
    {
        new Span<Glyph>(_buffer).Fill(ClearGlyph);
    }

    public void Clear(Rect rect)
    {
        int w = Math.Min(rect.X + rect.W, Width);
        int h = Math.Min(rect.Y + rect.H, Height);

        for (int y = rect.Y; y < h; y++)
        {
            new Span<Glyph>(_buffer, y * Width + rect.X, w).Fill(ClearGlyph);
        }
    }

    public void DrawPixel(Pos pos, Color color)
    {
        ref Glyph g = ref GetGlyphRef(pos);

        if (color.A == 255)
        {
            g.ForegroundColor = color;
            g.BackgroundColor = color;
            g.Char = ' ';
        }
        else
        {
            g.BackgroundColor = Color.AlphaBlend(color, g.BackgroundColor);
            g.ForegroundColor = Color.AlphaBlend(color, g.ForegroundColor);
        }
    }

    public void FillRect(Rect rect, Color color)
    {
        for (int x = rect.X; x < rect.X + rect.W; x++)
            for (int y = rect.Y; y < rect.Y + rect.H; y++)
                DrawPixel((x, y), color);
    }

    public void DrawRect(Rect rect, Color color)
    {
        for (int x = rect.X; x < rect.X + rect.W; x++)
        {
            DrawPixel((x, rect.Y), color);
            DrawPixel((x, rect.Y + rect.H - 1), color);
        }

        for (int y = rect.Y + 1; y < rect.Y + rect.H; y++)
        {
            DrawPixel((rect.X, y), color);
            DrawPixel((rect.X + rect.W - 1, y), color);
        }
    }


    public void DrawChar(Pos pos, char ch, Color color, Color? bgColor = null)
    {
        ref Glyph g = ref GetGlyphRef(pos);

        if (bgColor.HasValue)
        {
            if (bgColor.Value.A == 255) g.BackgroundColor = bgColor.Value;
            else g.BackgroundColor = Color.AlphaBlend(bgColor.Value, g.BackgroundColor);
        }

        g.Char = ch;
        g.ForegroundColor = color;
    }

    public void FillCharRect(Rect rect, char ch, Color color, Color? bgColor = default)
    {
        for (int x = rect.X; x < rect.X + rect.W; x++)
            for (int y = rect.Y; y < rect.Y + rect.H; y++)
                DrawChar(new(x, y), ch, color, bgColor);
    }

    public void DrawCharRect(Rect rect, char ch, Color color, Color? bgColor = default)
    {
        for (int x = rect.X; x < rect.X + rect.W; x++)
        {
            DrawChar((x, rect.Y), ch, color, bgColor);
            DrawChar((x, rect.Y + rect.H), ch, color, bgColor);
        }

        for (int y = rect.Y + 1; y < rect.Y + rect.H; y++)
        {
            DrawChar((rect.X, y), ch, color, bgColor);
            DrawChar((rect.X + rect.W, y), ch, color, bgColor);
        }
    }

    public void DrawLabel(Pos pos, ReadOnlySpan<char> text, Color color, Color? bgColor = default)
    {
        for (int i = 0, len = text.Length; i < len; i++)
            DrawChar((pos.X + i, pos.Y), text[i], color, bgColor);
    }

    public void DrawText(Rect rect, ReadOnlySpan<char> text, ReadOnlySpan<Range> rangesOfLines, TextAlignVH textAlign, Color color, Color? bgColor = default)
    {
        int longestLine = rangesOfLines.GetLongest(text.Length);

        for (int lineIndex = 0; lineIndex < rangesOfLines.Length && lineIndex < rect.H; lineIndex++)
        {
            int lineLength = rangesOfLines[lineIndex].GetOffsetAndLength(text.Length).Length;

            int y = textAlign.V switch
            {
                TextAlign.Start => rect.Y + lineIndex,
                TextAlign.End => rect.Y + rect.H - rangesOfLines.Length + lineIndex,
                TextAlign.Center => rect.Y + (rect.H / 2) - (rangesOfLines.Length / 2) + lineIndex,
                _ => throw new Exception()
            };

            int x = textAlign.H switch
            {
                TextAlign.Start => rect.X,
                TextAlign.End => rect.X + rect.W - lineLength,
                // TextAlign.Center => rect.X + (rect.W / 2) - lineLength / 2,
                TextAlign.Center => rect.X - (lineLength / 2 - longestLine / 2),
                _ => throw new Exception()
            };

            ReadOnlySpan<char> lineText = text[rangesOfLines[lineIndex]];

            if (x < 0)
            {
                x = Math.Abs(x);

                if (x >= lineText.Length) lineText = Span<char>.Empty;
                else lineText = lineText[x..];

                x = 0;
            }

            if (x < rect.X)
            {
                if (x - rect.X >= lineText.Length) lineText = Span<char>.Empty;
                else lineText = lineText[(rect.X - x)..];

                x = rect.X;
            }

            if (lineText.Length > rect.W)
                lineText = lineText[..rect.W];

            for (int i = 0, len = lineText.Length; i < len; i++)
                DrawChar((x + i, y), lineText[i], color, bgColor);
        }
    }

    public void DrawText(Rect rect, ReadOnlySpan<char> text, TextAlignVH textAlign, bool wrapText, Color color, Color? bgColor = default)
    {
        Span<Range> rangesOfLines = stackalloc Range[rect.H];
        WrapText(text, wrapText ? rect.W : int.MaxValue, rangesOfLines, out int wrappedLinesCount);

        DrawText(rect, text, rangesOfLines[..wrappedLinesCount], textAlign, color, bgColor);
    }

    public void DrawSurface(Pos pos, Surface surface, Rect? surfaceClipArea = null)
    {
        Rect bufferArea = new(0, 0, Width, Height);
        Rect clipArea = surfaceClipArea ?? new(0, 0, surface.Width, surface.Height);

        // make sure the clip area is inside the surface
        clipArea = Rect.Intersect(clipArea, new(0, 0, surface.Width, surface.Height));

        Rect drawArea = new(pos.X, pos.Y, clipArea.W, clipArea.H);

        if (drawArea.X < 0)
        {
            clipArea.X -= drawArea.X;
            clipArea.W += drawArea.X;
            drawArea.X = 0;
        }

        if (drawArea.Y < 0)
        {
            clipArea.Y -= drawArea.Y;
            clipArea.H += drawArea.Y;
            drawArea.Y = 0;
        }

        drawArea = Rect.Intersect(drawArea, bufferArea);

        if (clipArea.W < 1 || clipArea.H < 1 || drawArea.W < 1 || drawArea.H < 1) return;

        for (int xOffset = 0; xOffset < drawArea.W; xOffset++)
        {
            for (int yOffset = 0; yOffset < drawArea.H; yOffset++)
            {
                ref Glyph surfaceGlyph = ref surface.GetGlyphRef(new(clipArea.X + xOffset, clipArea.Y + yOffset));
                ref Glyph glyph = ref GetGlyphRef(new(pos.X + xOffset, pos.Y + yOffset));

                glyph.Char = surfaceGlyph.Char;
                glyph.BackgroundColor = Color.AlphaBlend(surfaceGlyph.BackgroundColor, glyph.BackgroundColor);
                glyph.ForegroundColor = surfaceGlyph.ForegroundColor;
            }
        }
    }
}
