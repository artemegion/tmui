namespace Tmui;

/// <summary>
/// 
/// </summary>
/// <param name="Char">The character.</param>
/// <param name="ForegroundColor">The foreground color. It is not alpha blended with the background.</param>
/// <param name="BackgroundColor">The background color.</param>
public record struct Glyph(char Char, Color ForegroundColor, Color BackgroundColor)
{
    public const char BLOCK_CHAR = '█';
    public const char QUADRANT_LOWER_LEFT_CHAR = '▖';
    public const char QUADRANT_LOWER_RIGHT_CHAR = '▗';
    public const char QUADRANT_UPPER_LEFT_CHAR = '▘';
    public const char QUADRANT_UPPER_RIGHT_CHAR = '▝';
    public const char LOWER_HALF_BLOCK_CHAR = '▄';
    public const char LOWER_THREE_EIGHTS_BLOCK_CHAR = '▃';
    public const char UPPER_HALF_BLOCK_CHAR = '▀';
    public const char RIGHT_HALF_BLOCK_CHAR = '▐';
    public const char LEFT_HALF_BLOCK_CHAR = '▌';
    public const char TRIANGLE_DOWN_CHAR = '▼';
    public const char TRIANGLE_RIGHT_CHAR = '▶';

    public Color AlphaBlendedForegroundColor => Color.AlphaBlend(ForegroundColor, BackgroundColor);
}
