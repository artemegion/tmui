using Tmui.Core;

namespace Tmui.Graphics;

public interface IGraphicsContext
{
    void DrawSurface(Pos pos, Surface surface, Rect? surfaceClipArea = null);
}
