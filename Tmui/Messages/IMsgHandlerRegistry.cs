namespace Tmui.Messages;

public interface IMsgHandlerRegistry
{
    void AddMsgHandler<T>(Action<T> handler) where T : Msg;
    void RemoveMsgHandler<T>(Action<T> handler) where T : Msg;
}
