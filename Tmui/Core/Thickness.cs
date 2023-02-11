namespace Tmui.Core;

/// <summary>
/// Thickness of every edge of a rectangle.
/// </summary>
/// <param name="Top">The thickness of the top edge.</param>
/// <param name="Right">The thickness of the right edge.</param>
/// <param name="Bottom">The thickness of the bottom edge.</param>
/// <param name="Left">The thickness of the left edge.</param>
public record struct Thickness(int Top, int Right, int Bottom, int Left);
