using Tmui.Messages;

namespace Tmui.Device;

public interface IAppDriver : ITerminal
{
    void Init();
    void PumpMessages(IMsgDispatcher dispatcher);
}
