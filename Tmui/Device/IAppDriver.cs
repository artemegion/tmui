using Tmui.Messages;

namespace Tmui.Device;

/// <summary>
/// App driver receives and translates system messages.
/// </summary>
public interface IAppDriver : ITerminal
{
    /// <summary>
    /// Initialize the app driver.
    /// </summary>
    void Init();

    /// <summary>
    /// Receive messages from the system, translate and dispatch them.
    /// </summary>
    /// <param name="dispatcher">Dispatcher used to dispatch translated messages.</param>
    void PumpMessages(IMsgDispatcher dispatcher);
}
