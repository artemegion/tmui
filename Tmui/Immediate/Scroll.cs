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

public class Scroll
{
	public Scroll(int scrollX, int scrollY)
	{
		ScrollX = scrollX;
		ScrollY = scrollY;
	}

	public int ScrollX;
	public int ScrollY;
}
