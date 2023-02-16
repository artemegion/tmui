namespace Tmui.Immediate;

[Flags]
public enum TextBoxScrollFlags
{
    Vertical = 2,
    Horizontal = 4,
    AlwaysShow = 8,
    DisableVertical = 16,
    DisableHorizontal = 32,
    HideVertical = 64, // hide also always disables
    HideHorizontal = 128,
    None = 0
}
