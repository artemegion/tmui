namespace Tmui.Messages;

public interface IMsgDispatcher
{
    public void DispatchMsg<T>(T msg) where T : Msg;
}
