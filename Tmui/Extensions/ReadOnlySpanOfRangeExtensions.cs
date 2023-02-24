using System.Runtime.CompilerServices;

namespace Tmui.Extensions;

public static class ReadOnlySpanOfRangeExtensions
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int GetLongest(this ReadOnlySpan<Range> ranges, int length)
    {
        int longest = 0;

        foreach (Range r in ranges)
        {
            int len = r.GetOffsetAndLength(length).Length;

            if (len > longest)
            {
                longest = len;
            }
        }

        return longest;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int GetLength(this ReadOnlySpan<Range> ranges, int index, int length)
    {
        return ranges[index].GetOffsetAndLength(length).Length;
    }

    public static (int Index, int Offset) GetFromIndex(this ReadOnlySpan<Range> ranges, int length, int index)
    {
        for (int lineIndex = 0; lineIndex < ranges.Length; lineIndex++)
        {
            (int Offset, int Length) = ranges[lineIndex].GetOffsetAndLength(length);

            if (index >= Offset && index <= Offset + Length)
            {
                return new(index - Offset, lineIndex);
            }
        }

        return new(0, 0);
    }
}

public static class SpanOfRangeExtensions
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int GetLongest(this Span<Range> ranges, int length)
    {
        int longest = 0;

        foreach (Range r in ranges)
        {
            int len = r.GetOffsetAndLength(length).Length;

            if (len > longest)
            {
                longest = len;
            }
        }

        return longest;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Span<Range> WithoutTrailingRanges(this Span<Range> ranges, int length)
    {
        int lIndex = 0;

        for (int i = 0, rangesLength = ranges.Length; i < rangesLength; i++)
        {
            int rangeLen = ranges[i].GetOffsetAndLength(length).Length;
            if (rangeLen > 0) lIndex = i;
        }

        return ranges[0..(lIndex + 1)];
    }

    public static (int Index, int Offset) GetFromIndex(this Span<Range> ranges, int length, int index)
    {
        for (int lineIndex = 0; lineIndex < ranges.Length; lineIndex++)
        {
            (int Offset, int Length) = ranges[lineIndex].GetOffsetAndLength(length);

            if (index >= Offset && index <= Offset + Length)
            {
                return (lineIndex, index - Offset);
            }
        }

        return (0, 0);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int GetLength(this Span<Range> ranges, int index, int length)
    {
        return ranges[index].GetOffsetAndLength(length).Length;
    }
}
