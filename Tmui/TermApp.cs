using Tmui.Device;
using Tmui.Device.Win32;
using Tmui.Messages;

namespace Tmui;

public class TermApp : IMsgDispatcher, IMsgHandlerRegistry
{
    public TermApp()
    {
        _msgQueue = new(100);
        _handlers = new(20);

        _appDriver = new Win32AppDriver();

        Input = new(this);
        Terminal = _appDriver;
    }

    private readonly Queue<Msg> _msgQueue;
    private readonly Dictionary<Type, Delegate> _handlers;
    private readonly IAppDriver _appDriver;

    public Input Input { get; }
    public ITerminal Terminal { get; }

    public bool QuitRequested { get; private set; }

    public void Run()
    {
        _appDriver.Init();

        if (_handlers.TryGetValue(typeof(StartMsg), out Delegate? startHandler) && startHandler is not null)
            startHandler.DynamicInvoke(new StartMsg());

        while (!QuitRequested)
        {
            long dt0 = DateTime.Now.Ticks;

            _appDriver.PumpMessages(this);

            if (_msgQueue.Count > 0)
            {
                while (!QuitRequested && _msgQueue.TryDequeue(out Msg? msg))
                {
                    if (msg is not null && _handlers.TryGetValue(msg.GetType(), out Delegate? handler) && handler is not null)
                        handler.DynamicInvoke(msg);
                }
            }

            Input.Update();

            if (_handlers.TryGetValue(typeof(UpdateMsg), out Delegate? updateHandler) && updateHandler is not null)
                updateHandler.DynamicInvoke(new UpdateMsg());

            if (_handlers.TryGetValue(typeof(PostUpdateMsg), out updateHandler) && updateHandler is not null)
                updateHandler.DynamicInvoke(new PostUpdateMsg());

            Input.PostUpdate();

            // quit may be requested in a msg handler
            if (QuitRequested)
                break;

            // keep 20 fps
            dt0 = DateTime.Now.Ticks - dt0;
            if (dt0 < 1000 / 20)
            {
                Thread.Sleep(1000 / 20 - (int)dt0);
            }
        }
    }

    public void AddMsgHandler<T>(Action<T> handler) where T : Msg
    {
        Delegate? rootHandler = _handlers.GetValueOrDefault(typeof(T));
        rootHandler = Delegate.Combine(rootHandler, handler);

        _handlers[typeof(T)] = rootHandler;
    }

    public void RemoveMsgHandler<T>(Action<T> handler) where T : Msg
    {
        Delegate? rootHandler = _handlers.GetValueOrDefault(typeof(T));

        if (rootHandler is not null)
        {
            rootHandler = Delegate.Remove(rootHandler, handler);
        }

        if (rootHandler is not null)
        {
            _handlers[typeof(T)] = rootHandler;
        }
    }

    public void DispatchMsg<T>(T msg) where T : Msg
    {
#if DEBUG
        if (typeof(T) == typeof(UpdateMsg))
            throw new InvalidOperationException("Can't dispatch an UpdateMsg.");
#endif

        _msgQueue.Enqueue(msg);
    }

    public void Quit()
    {
        QuitRequested = true;
    }
}
