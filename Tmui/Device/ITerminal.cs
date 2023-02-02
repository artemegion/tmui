using Tmui.Core;

namespace Tmui.Device;

public interface ITerminal
{
    private static readonly char[] NewlineChars = new char[1] { '\n' };

    Pos BufferSize
    {
        get => new(Console.BufferWidth, Console.BufferHeight);
        set { throw new NotSupportedException("The default ITerminal implementation does not support changing the buffer size."); }
    }

    Pos CursorPos
    {
        get => new(Console.CursorLeft, Console.CursorTop);
        set => (Console.CursorLeft, Console.CursorTop) = value;
    }

    bool CursorVisible
    {
        get => true;
        set => Console.CursorVisible = value;
    }

    void Write(Span<char> chars)
    {
        Console.Out.Write(chars);
    }

    void Write(ReadOnlySpan<char> chars)
    {
        Console.Out.Write(chars);
    }

    void WriteLine(Span<char> chars)
    {
        Write(chars);
        WriteNewline();
    }

    void WriteLine(ReadOnlySpan<char> chars)
    {
        Write(chars);
        WriteNewline();
    }

    void WriteNewline()
    {
        Write(NewlineChars.AsSpan());
    }

    void Flush()
    {
        Console.Out.Flush();
    }
}
