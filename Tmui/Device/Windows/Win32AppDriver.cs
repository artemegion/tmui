using System.Text;
using Windows.Win32;
using Windows.Win32.System.Console;
using Microsoft.Win32.SafeHandles;
using Tmui.Core;
using Tmui.Messages;

namespace Tmui.Device.Windows;

/// <summary>
/// <see cref="IAppDriver"/> and <see cref="ITerminal"/> for Win32 platform.
/// </summary>
public class Win32AppDriver : IAppDriver, ITerminal
{
    public Win32AppDriver()
    {
        if (!OperatingSystem.IsWindows())
            throw new PlatformNotSupportedException("");

        _stdInHandle = PInvoke.GetStdHandle_SafeHandle(STD_HANDLE.STD_INPUT_HANDLE);
        _stdOutHandle = PInvoke.GetStdHandle_SafeHandle(STD_HANDLE.STD_OUTPUT_HANDLE);
        _inputRecords = new INPUT_RECORD[30];
    }

    public Pos BufferSize
    {
        get => new(Console.BufferWidth, Console.BufferHeight);
        set
        {
            if (!OperatingSystem.IsWindows())
                throw new PlatformNotSupportedException("");

            (Console.BufferWidth, Console.BufferHeight) = value;
            (Console.WindowWidth, Console.WindowHeight) = value;
        }
    }

    public bool CursorVisible
    {
        get
        {
            if (!OperatingSystem.IsWindows())
                throw new PlatformNotSupportedException("");
            return Console.CursorVisible;
        }

        set => Console.CursorVisible = value;
    }

    private readonly SafeFileHandle _stdInHandle;
    private readonly SafeFileHandle _stdOutHandle;
    private readonly INPUT_RECORD[] _inputRecords;

    public void Init()
    {
        if (!PInvoke.GetConsoleMode(_stdInHandle, out CONSOLE_MODE consoleMode)) throw new Exception("1");
        if (!PInvoke.SetConsoleMode(_stdInHandle, (consoleMode & ~CONSOLE_MODE.ENABLE_QUICK_EDIT_MODE) | CONSOLE_MODE.ENABLE_EXTENDED_FLAGS | CONSOLE_MODE.ENABLE_MOUSE_INPUT | CONSOLE_MODE.ENABLE_WINDOW_INPUT))
            throw new Exception("2");

        if (!PInvoke.GetConsoleMode(_stdOutHandle, out consoleMode)) throw new Exception("3");
        if (!PInvoke.SetConsoleMode(_stdOutHandle, CONSOLE_MODE.ENABLE_PROCESSED_OUTPUT | CONSOLE_MODE.ENABLE_VIRTUAL_TERMINAL_PROCESSING)) throw new Exception("4");

        Console.InputEncoding = Encoding.UTF8;
        Console.OutputEncoding = Encoding.UTF8;

        CursorVisible = false;
    }

    public void PumpMessages(IMsgDispatcher dispatcher)
    {
        if (PInvoke.GetNumberOfConsoleInputEvents(_stdInHandle, out uint lpNumberOfEvents) && lpNumberOfEvents > 0)
        {
            PInvoke.ReadConsoleInput(_stdInHandle, _inputRecords.AsSpan(), out lpNumberOfEvents);

            for (int i = 0; i < lpNumberOfEvents; i++)
            {
                switch ((uint)_inputRecords[i].EventType)
                {
                    case PInvoke.KEY_EVENT:
                        KEY_EVENT_RECORD keyEventRecord = _inputRecords[i].Event.KeyEvent;
                        Key key = TranslateVirtualKeyCode(keyEventRecord.wVirtualKeyCode);

                        dispatcher.DispatchMsg<KeyChangedMsg>(new(key, keyEventRecord.bKeyDown));

                        char ch = keyEventRecord.uChar.UnicodeChar;

                        if (ch != '\0' && !char.IsControl(ch))
                            dispatcher.DispatchMsg<CharMsg>(new(ch));
                        break;
                    case PInvoke.WINDOW_BUFFER_SIZE_EVENT:
                        WINDOW_BUFFER_SIZE_RECORD windowBufferSizeRecord = _inputRecords[i].Event.WindowBufferSizeEvent;

                        dispatcher.DispatchMsg<BufferSizeChangedMsg>(new(new(windowBufferSizeRecord.dwSize.X, windowBufferSizeRecord.dwSize.Y)));
                        break;
                    case PInvoke.MOUSE_EVENT:
                        MOUSE_EVENT_RECORD mouseEventRecord = _inputRecords[i].Event.MouseEvent;

                        int wheelDir;
                        switch (mouseEventRecord.dwEventFlags)
                        {
                            case PInvoke.MOUSE_MOVED:
                                dispatcher.DispatchMsg(new MouseMovedMsg(new(mouseEventRecord.dwMousePosition.X, mouseEventRecord.dwMousePosition.Y)));
                                break;
                            case PInvoke.DOUBLE_CLICK: break;
                            case PInvoke.MOUSE_HWHEELED:
                                // If the high word of the dwButtonState member contains a positive value,
                                // the wheel was rotated to the right. Otherwise, the wheel was rotated to the left.

                                wheelDir = (int)(mouseEventRecord.dwButtonState & 0xFFFF0000u);
                                if (wheelDir > 0) wheelDir = 1;
                                else wheelDir = -1;

                                dispatcher.DispatchMsg(new MouseScrollMsg(wheelDir, 0));
                                break;
                            case PInvoke.MOUSE_WHEELED:
                                // If the high word of the dwButtonState member contains a positive value,
                                // the wheel was rotated forward, away from the user.
                                // Otherwise, the wheel was rotated backward, toward the user.

                                wheelDir = (int)(mouseEventRecord.dwButtonState & 0xFFFF0000u);
                                if (wheelDir > 0) wheelDir = -1;
                                else wheelDir = 1;

                                dispatcher.DispatchMsg(new MouseScrollMsg(0, wheelDir));
                                break;
                            default:
                                if ((mouseEventRecord.dwButtonState & PInvoke.FROM_LEFT_1ST_BUTTON_PRESSED) == PInvoke.FROM_LEFT_1ST_BUTTON_PRESSED)
                                    dispatcher.DispatchMsg(new KeyChangedMsg(Key.MouseLeft, true));
                                else
                                    dispatcher.DispatchMsg(new KeyChangedMsg(Key.MouseLeft, false));

                                if ((mouseEventRecord.dwButtonState & PInvoke.FROM_LEFT_2ND_BUTTON_PRESSED) == PInvoke.FROM_LEFT_2ND_BUTTON_PRESSED)
                                    dispatcher.DispatchMsg(new KeyChangedMsg(Key.MouseMiddle, true));
                                else
                                    dispatcher.DispatchMsg(new KeyChangedMsg(Key.MouseMiddle, false));

                                if ((mouseEventRecord.dwButtonState & PInvoke.FROM_LEFT_3RD_BUTTON_PRESSED) == PInvoke.FROM_LEFT_3RD_BUTTON_PRESSED)
                                    dispatcher.DispatchMsg(new KeyChangedMsg(Key.MouseRight, true));
                                else
                                    dispatcher.DispatchMsg(new KeyChangedMsg(Key.MouseRight, false));
                                break;
                        }
                        break;
                }
            }
        }
    }

    private static Key TranslateVirtualKeyCode(ushort keyCode)
    {
        return keyCode switch
        {
            0x01 => Key.MouseLeft,
            0x02 => Key.MouseRight,
            0x04 => Key.MouseMiddle,
            0x05 => Key.Mouse1,
            0x06 => Key.Mouse2,
            0x08 => Key.Backspace,
            0x09 => Key.Tab,
            0x0D => Key.Enter,
            0x10 => Key.LeftShift,
            0x11 => Key.LeftCtrl,
            0x12 => Key.LeftAlt,
            0x13 => Key.Pause,
            0x14 => Key.CapsLock,
            0x20 => Key.Space,
            0x21 => Key.PageUp,
            0x22 => Key.PageDown,
            0x23 => Key.End,
            0x24 => Key.Home,
            0x25 => Key.LeftArrow,
            0x26 => Key.UpArrow,
            0x27 => Key.RightArrow,
            0x28 => Key.DownArrow,
            0x2C => Key.PrtSc,
            0x2D => Key.Insert,
            0x2E => Key.Delete,
            0x30 => Key.Alpha0,
            0x31 => Key.Alpha1,
            0x32 => Key.Alpha2,
            0x33 => Key.Alpha3,
            0x34 => Key.Alpha4,
            0x35 => Key.Alpha5,
            0x36 => Key.Alpha6,
            0x37 => Key.Alpha7,
            0x38 => Key.Alpha8,
            0x39 => Key.Alpha9,
            0x41 => Key.A,
            0x42 => Key.B,
            0x43 => Key.C,
            0x44 => Key.D,
            0x45 => Key.E,
            0x46 => Key.F,
            0x47 => Key.G,
            0x48 => Key.H,
            0x49 => Key.I,
            0x4A => Key.J,
            0x4B => Key.K,
            0x4C => Key.L,
            0x4D => Key.M,
            0x4E => Key.N,
            0x4F => Key.O,
            0x50 => Key.P,
            0x51 => Key.Q,
            0x52 => Key.R,
            0x53 => Key.S,
            0x54 => Key.T,
            0x55 => Key.U,
            0x56 => Key.V,
            0x57 => Key.W,
            0x58 => Key.X,
            0x59 => Key.T,
            0x5A => Key.Z,
            0x5B => Key.Super,
            0x5C => Key.RightSuper,
            0x60 => Key.Numpad0,
            0x61 => Key.Numpad1,
            0x62 => Key.Numpad2,
            0x63 => Key.Numpad3,
            0x64 => Key.Numpad4,
            0x65 => Key.Numpad5,
            0x66 => Key.Numpad6,
            0x67 => Key.Numpad7,
            0x68 => Key.Numpad8,
            0x69 => Key.Numpad9,
            0x6A => Key.NumpadMultiply,
            0x6B => Key.NumpadPlus,
            // 0x6C => Key.NumpadDot,  VK_SEPARATOR?
            0x6D => Key.NumpadMinus,
            0x6E => Key.NumpadDot,
            0x6F => Key.NumpadDivide,
            0x70 => Key.F1,
            0x71 => Key.F2,
            0x72 => Key.F3,
            0x73 => Key.F4,
            0x74 => Key.F5,
            0x75 => Key.F6,
            0x76 => Key.F7,
            0x77 => Key.F8,
            0x78 => Key.F9,
            0x79 => Key.F10,
            0x7A => Key.F11,
            0x7B => Key.F12,
            0x7C => Key.F13,
            0x7D => Key.F14,
            0x7E => Key.F15,
            0x7F => Key.F16,
            0x80 => Key.F17,
            0x81 => Key.F18,
            0x82 => Key.F19,
            0x83 => Key.F20,
            0x84 => Key.F21,
            0x85 => Key.F22,
            0x86 => Key.F23,
            0x87 => Key.F24,
            0x90 => Key.NumLock,
            0x91 => Key.ScrLk,
            0xA0 => Key.LeftShift,
            0xA1 => Key.RightShift,
            0xA2 => Key.LeftCtrl,
            0xA3 => Key.RightCtrl,
            0xA4 => Key.LeftAlt,
            0xA5 => Key.RightAlt,
            0xBB => Key.Plus,
            0xBC => Key.Dot,
            0xBD => Key.Minus,
            0xBE => Key.Dot,
            0xBF => Key.FwdSlash,
            0xC0 => Key.Tilde,
            0xDB => Key.LeftBracket,
            0xDC => Key.BackSlash,
            0xDD => Key.RightBracket,
            0xDE => Key.Quote,
            _ => Key.Unknown
        };
    }
}
