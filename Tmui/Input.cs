using Tmui.Core;
using Tmui.Messages;

namespace Tmui;

public class Input
{
    public Input(IMsgHandlerRegistry msgHandlerRegistry)
    {
        _keys = new();

        foreach (Key key in Enum.GetValues<Key>())
        {
            _keys[key] = 0;
        }

        MousePos = (0, 0);

        msgHandlerRegistry.AddMsgHandler<KeyChangedMsg>(HandleKeyChangedMsg);
        msgHandlerRegistry.AddMsgHandler<MouseMovedMsg>(HandleMouseMovedMsg);
        msgHandlerRegistry.AddMsgHandler<MouseScrollMsg>(HandleMouseScrollMsg);
    }

    private readonly Dictionary<Key, KeyState> _keys;

    public Pos MousePos { get; private set; }
    public Pos? PressedMousePos { get; private set; }

    public Pos Scroll { get; private set; }

    public bool KeyHeld(Key key)
    {
        return _keys[key] == KeyState.Down || _keys[key] ==  KeyState.Pressed;
    }

    public bool KeyPressed(Key key)
    {
        return _keys[key] == KeyState.Pressed;
    }

    public bool KeyReleased(Key key)
    {
        return _keys[key] == KeyState.Released;
    }

    private void HandleMouseScrollMsg(MouseScrollMsg msg)
    {
        Pos s = Scroll;

        s.X += msg.X;
        s.Y += msg.Y;

        //if (s.Y != 0 && KeyHeld(Key.LeftShift))
        //{
        //    s.X = s.Y;
        //    s.Y = 0;
        //}

        Scroll = s;
    }

    private void HandleKeyChangedMsg(KeyChangedMsg msg)
    {
        if (msg.Held)
        {
            if (_keys[msg.Key] != KeyState.Down)
                _keys[msg.Key] = KeyState.Pressed; // _keys[msg.Key] == KeyState.Pressed ? KeyState.Down : KeyState.Pressed;
        }
        else
        {
            if (_keys[msg.Key] != KeyState.Up)
                _keys[msg.Key] = KeyState.Released; // _keys[msg.Key] == KeyState.Released ? KeyState.Up : KeyState.Released;
        }
    }

    private void HandleMouseMovedMsg(MouseMovedMsg msg)
    {
        MousePos = msg.Pos;
    }

    public void Update()
    {
        if (_keys[Key.MouseLeft] == KeyState.Pressed)
            PressedMousePos = MousePos;
    }

    public void PostUpdate()
    {
        foreach (Key key in _keys.Keys)
        {
            if (_keys[key] == KeyState.Released) _keys[key] = KeyState.Up;
            else if (_keys[key] == KeyState.Pressed) _keys[key] = KeyState.Down;
        }

        if (_keys[Key.MouseLeft] == KeyState.Up)
            PressedMousePos = null;

        Scroll = new(0, 0);
    }

    private enum KeyState : byte
    {
        Up   = 0b0100,
        Down = 0b1000,

        ChangedThisFrame = 0b0001,

        Pressed = Down | ChangedThisFrame,
        Released = Up | ChangedThisFrame
    }
}
