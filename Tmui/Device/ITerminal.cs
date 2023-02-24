using Tmui.Core;

namespace Tmui.Device;

/// <summary>
/// Provides access to methods and properties used to control the terminal.
/// </summary>
public interface ITerminal
{
    private static readonly char[] NewlineChars = new char[1] { '\n' };

    /// <summary>
    /// The size of the buffer (in characters).
    /// </summary>
    Pos BufferSize
    {
        get => new(Console.BufferWidth, Console.BufferHeight);
        set { throw new NotSupportedException("The default ITerminal implementation does not support changing the buffer size."); }
    }

    /// <summary>
    /// The position of the cursor (in characters).
    /// </summary>
    Pos CursorPos
    {
        get => new(Console.CursorLeft, Console.CursorTop);
        set => (Console.CursorLeft, Console.CursorTop) = value;
    }

    /// <summary>
    /// The visibility of the cursor.
    /// </summary>
    bool CursorVisible
    {
        get => true;
        set => Console.CursorVisible = value;
    }

    /// <summary>
    /// Writes <paramref name="chars"/> to the output buffer.
    /// </summary>
    /// <param name="chars">Characters to write to the output buffer.</param>
    void Write(Span<char> chars)
    {
        Console.Out.Write(chars);
    }

    /// <summary>
    /// Writes <paramref name="chars"/> to the output buffer.
    /// </summary>
    /// <param name="chars">Characters to write to the output buffer.</param>
    void Write(ReadOnlySpan<char> chars)
    {
        Console.Out.Write(chars);
    }

    /// <summary>
    /// Writes <paramref name="chars"/> to the output buffer and follows it with a newline character.
    /// </summary>
    /// <param name="chars">Characters to write to the output buffer.</param>
    void WriteLine(Span<char> chars)
    {
        Write(chars);
        WriteNewline();
    }

    /// <summary>
    /// Writes <paramref name="chars"/> to the output buffer and follows it with a newline character.
    /// </summary>
    /// <param name="chars">Characters to write to the output buffer.</param>
    void WriteLine(ReadOnlySpan<char> chars)
    {
        Write(chars);
        WriteNewline();
    }

    /// <summary>
    /// Writes a newline character to the output buffer.
    /// </summary>
    void WriteNewline()
    {
        Write(NewlineChars.AsSpan());
    }

    /// <summary>
    /// Clears the terminal.
    /// </summary>
    void Clear()
    {
        Console.Clear();
    }

    /// <summary>
    /// Flushes the output buffer to the terminal.
    /// </summary>
    void Flush()
    {
        Console.Out.Flush();
    }
}
