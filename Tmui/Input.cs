using Tmui.Core;
using Tmui.Messages;

namespace Tmui;

public class Input
{
    public Input(IMsgHandlerRegistry msgHandlerRegistry)
    {
        _keys = new();
        _charInputBuf = new char[20];
        _charsInputThisFrame = 0;

        foreach (Key key in Enum.GetValues<Key>())
            _keys[key] = 0;

        MousePos = (0, 0);

        msgHandlerRegistry.AddMsgHandler<CharMsg>(HandleCharMsg);
        msgHandlerRegistry.AddMsgHandler<KeyChangedMsg>(HandleKeyChangedMsg);
        msgHandlerRegistry.AddMsgHandler<MouseMovedMsg>(HandleMouseMovedMsg);
        msgHandlerRegistry.AddMsgHandler<MouseScrollMsg>(HandleMouseScrollMsg);
    }

    private readonly Dictionary<Key, KeyState> _keys;

    private readonly char[] _charInputBuf;
    private int _charsInputThisFrame;

    public Pos MousePos { get; private set; }
    public Pos? PressedMousePos { get; private set; }

    public Pos Scroll { get; private set; }

    public ReadOnlySpan<char> Chars => new(_charInputBuf, 0, _charsInputThisFrame);

    public bool KeyHeld(Key key)
    {
        return _keys[key] == KeyState.Down || _keys[key] == KeyState.Pressed;
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

        Scroll = s;
    }

    private void HandleCharMsg(CharMsg msg)
    {
        if (_charsInputThisFrame < _charInputBuf.Length)
        {
            _charInputBuf[_charsInputThisFrame] = msg.Char;
            _charsInputThisFrame++;
        }
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
            // don't change key state to released in the same frame it was changed to pressed
            // because the key press will not have any effect in the program
            if (_keys[msg.Key] == KeyState.Pressed)
                return;

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

        _charsInputThisFrame = 0;
    }

    private enum KeyState : byte
    {
        Up = 0b0100,
        Down = 0b1000,

        ChangedThisFrame = 0b0001,

        Pressed = Down | ChangedThisFrame,
        Released = Up | ChangedThisFrame
    }
}
