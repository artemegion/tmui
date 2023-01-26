using Tmui.Core;

namespace Tmui.Messages;

public record class BufferSizeChangedMsg(Pos BufferSize) : Msg;
