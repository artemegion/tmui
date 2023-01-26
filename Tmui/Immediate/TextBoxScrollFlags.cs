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

namespace Tmui.Immediate;

[Flags]
public enum TextBoxScrollFlags
{
    Vertical = 2,
    Horizontal = 4,
    AlwaysShow = 8,
    None = 0
}
