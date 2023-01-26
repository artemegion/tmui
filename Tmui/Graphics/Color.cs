namespace Tmui;

public record struct Color(byte R, byte G, byte B, byte A)
{
    public static Color White { get; } = new Color(255, 255, 255, 255);
    public static Color White10 { get; } = new Color(255, 255, 255, 25);
    public static Color White20 { get; } = new Color(255, 255, 255, 51);
    public static Color White30 { get; } = new Color(255, 255, 255, 76);

    public static Color Transparent { get; } = new Color(0, 0, 0, 0);

    public static Color Black { get; } = new Color(0, 0, 0, 255);
    public static Color Red { get; } = new Color(255, 0, 0, 255);
    public static Color Green { get; } = new Color(0, 255, 0, 255);
    public static Color Blue { get; } = new Color(0, 0, 255, 255);
    public static Color BlueViolet { get; } = new Color(138, 43, 226);
    public static Color CadetBlue { get; } = new Color(95, 158, 160);
    public static Color CornglowerBlue { get; } = new Color(100, 149, 237);
    public static Color DodgerBlue { get; } = new Color(30, 144, 255);
    public static Color HotPink { get; } = new Color(255, 105, 180);
    public static Color OrangeRed { get; } = new Color(255, 69, 0);
    public static Color YellowGreen { get; } = new Color(154, 205, 50);
    public static Color Yellow { get; } = new Color(255, 255, 0);
    public static Color Magenta { get; } = new Color(255, 0, 255);

    public static Color AlphaBlend(Color fore, Color back)
    {
        float aAlpha = fore.A / 255.0f;
        float aR = fore.R / 255.0f;
        float aG = fore.G / 255.0f;
        float aB = fore.B / 255.0f;

        float bAlpha = back.A / 255.0f;
        float bR = back.R / 255.0f;
        float bG = back.G / 255.0f;
        float bB = back.B / 255.0f;

        float outAlpha = aAlpha + (bAlpha - bAlpha * aAlpha);
        float outR = (aR * aAlpha + (bR * bAlpha - bR * bAlpha * aAlpha)) / outAlpha;
        float outG = (aG * aAlpha + (bG * bAlpha - bG * bAlpha * aAlpha)) / outAlpha;
        float outB = (aB * aAlpha + (bB * bAlpha - bB * bAlpha * aAlpha)) / outAlpha;

        return new Color(
            (byte)(outR * 255),
            (byte)(outG * 255),
            (byte)(outB * 255),
            (byte)(outAlpha * 255)
        );
    }

    public Color(byte R, byte G, byte B) : this(R, G, B, 255)
    {

    }
}
