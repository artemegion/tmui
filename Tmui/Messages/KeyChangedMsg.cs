using Tmui.Core;

namespace Tmui.Messages;

public record class KeyChangedMsg(Key Key, bool Held) : Msg;
