namespace Tmui;

/// <summary>
/// Utilities for writing ANSI escape sequences.
/// </summary>
public static class Ansi
{
    public const string CLEAR_SCREEN_SEQ = "\x1b[2J";

    /// <summary>
    /// Write a `background color` escape sequence to the <paramref name="target"/>.
    /// </summary>
    /// <param name="color">The color to use as the background color.</param>
    /// <param name="target">The target <see cref="Span{char}"/> to write to.</param>
    /// <param name="charsWritten">Will be set to the count of characters written to the <paramref name="target"/>.</param>
    public static void WriteBgColor(Color color, Span<char> target, out int charsWritten)
    {
        // \x1b[48;2;RRR;GGG;BBBm

        target[0] = '\x1b';
        target[1] = '[';
        target[2] = '4';
        target[3] = '8';
        target[4] = ';';
        target[5] = '2';
        target[6] = ';';

        charsWritten = 7;

        WriteColor(color, target[charsWritten..], out int written);
        charsWritten += written;

        target[charsWritten] = 'm';
        charsWritten++;
    }

    /// <summary>
    /// Write a `foreground color` escape sequence to the <paramref name="target"/>.
    /// </summary>
    /// <param name="color">The color to use as the foreground color.</param>
    /// <param name="target">The target <see cref="Span{char}"/> to write to.</param>
    /// <param name="charsWritten">Will be set to the count of characters written to the <paramref name="target"/>.</param>
    public static void WriteFgColor(Color color, Span<char> target, out int charsWritten)
    {
        // \x1b[38;2;RRR;GGG;BBBm

        target[0] = '\x1b';
        target[1] = '[';
        target[2] = '3';
        target[3] = '8';
        target[4] = ';';
        target[5] = '2';
        target[6] = ';';

        charsWritten = 7;

        WriteColor(color, target[charsWritten..], out int written);
        charsWritten += written;

        target[charsWritten] = 'm';
        charsWritten++;
    }

    /// <summary>
    /// Writes a 'move cursor' escape sequence to the <paramref name="target"/> <see cref="Span{char}"/>.
    /// </summary>
    /// <param name="x">The column to which the cursor will be moved.</param>
    /// <param name="y">The row to which the cursor will be moved.</param>
    /// <param name="target">The target <see cref="Span{char}"/> to write to.</param>
    /// <param name="charsWritten">Will be set to the count of characters written to the <paramref name="target"/>.</param>
    public static void WriteMoveCursor(int x, int y, Span<char> target, out int charsWritten)
    {
        // \x1b[x;yH

        target[0] = '\x1b';
        target[1] = '[';
        charsWritten = 2;

        x.TryFormat(target, out int written);
        charsWritten += written;

        target[charsWritten] = ';';
        charsWritten++;

        y.TryFormat(target, out written);
        charsWritten += written;

        target[charsWritten] = 'H';
        charsWritten++;
    }

    /// <summary>
    /// Writes a 'erase screen' escape sequence to the <paramref name="target"/> <see cref="Span{char}"/>.
    /// </summary>
    /// <param name="target">The target <see cref="Span{char}"/> to write to.</param>
    /// <param name="charsWritten">Will be set to the count of characters written to the <paramref name="target"/>.</param>
    public static void WriteEraseScreen(Span<char> target, out int charsWritten)
    {
        // \1xb[2J

        target[0] = '\x1b';
        target[1] = '[';
        target[2] = '2';
        target[3] = 'J';

        charsWritten = 4;
    }

    private static void WriteColor(Color color, Span<char> target, out int charsWritten)
    {
        charsWritten = 0;
        int written = 0;

        if (!color.R.TryFormat(target = target[written..], out written)) throw new InvalidOperationException($"Could not write color '{color}' into the target Span.");
        target[written] = ';';
        charsWritten += written + 1;

        if (!color.G.TryFormat(target = target[(written + 1)..], out written)) throw new InvalidOperationException($"Could not write color '{color}' into the target Span.");
        target[written] = ';';
        charsWritten += written + 1;

        if (!color.B.TryFormat(target[(written + 1)..], out written)) throw new InvalidOperationException($"Could not write color '{color}' into the target Span.");
        charsWritten += written;
    }
}
