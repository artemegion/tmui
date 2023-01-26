using Tmui.Core;
using Tmui.Device;

namespace Tmui.Graphics;

public class TerminalGraphicsContext : IGraphicsContext
{
    public TerminalGraphicsContext(ITerminal terminal)
    {
        Terminal = terminal;

        Pos bufferSize = Terminal.BufferSize;
        _charBuffer = new char[bufferSize.X * bufferSize.Y * 39];

        Clear();
        // Flush();
    }

    public readonly ITerminal Terminal;
    private readonly char[] _charBuffer;

    public void DrawSurface(Pos pos, Surface surface, Rect? surfaceClipArea = null)
    {
        Rect bufferArea = new(0, 0, Terminal.BufferSize);
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

        Glyph glyph;
        for (int yOffset = 0; yOffset < drawArea.H; yOffset++)
        {
            for (int xOffset = 0; xOffset < drawArea.W; xOffset++)
            {
                glyph = surface.GetGlyph(new(clipArea.X + xOffset, clipArea.Y + yOffset));

                Span<char> bufferSpan = new(_charBuffer, ((drawArea.Y + yOffset) * bufferArea.W + drawArea.X + xOffset) * 39, 39);
                bufferSpan.Clear();

                Ansi.WriteBgColor(glyph.BackgroundColor, bufferSpan, out int bgWritten);
                Ansi.WriteFgColor(glyph.AlphaBlendedForegroundColor, bufferSpan[bgWritten..], out int fgWritten);
                bufferSpan[fgWritten + bgWritten] = glyph.Char;
            }
        }
    }

    public void Clear()
    {
        Pos bufferSize = Terminal.BufferSize;
        Color c = Color.Black;

        for (int y = 0; y < bufferSize.Y; y++)
        {
            for (int x = 0; x < bufferSize.X; x++)
            {
                Span<char> bufferSpan = new(_charBuffer, (y * bufferSize.X + x) * 39, 39);
                bufferSpan.Clear();

                Ansi.WriteBgColor(c, bufferSpan, out int bgWritten);
                Ansi.WriteFgColor(c, bufferSpan[bgWritten..], out int fgWritten);
                bufferSpan[fgWritten + bgWritten] = ' ';
            }
        }
    }

    public void Flush()
    {
        Pos bufferSize = Terminal.BufferSize;

        for (int y = 0, yMax = bufferSize.Y; y < yMax; y++)
        {
            Terminal.CursorPos = (0, y);
            Terminal.Write(new Span<char>(_charBuffer, y * bufferSize.X * 39, bufferSize.X * 39));
        }

        Terminal.Flush();
    }
}
