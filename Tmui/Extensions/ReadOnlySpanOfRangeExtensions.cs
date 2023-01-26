// TODO:
// + ScrollView
// + Dropdown
// + SelectBox
// + Button checkbox
// + TextBox built-in scrolling
//   + no vertical align with scrolling
// + Checkbox with label overload
// + Docs
// + comments explaining code
// + refactor common code

namespace Tmui.Extensions;

public static class ReadOnlySpanOfRangeExtensions
{
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
}

public static class SpanOfRangeExtensions
{
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
}
