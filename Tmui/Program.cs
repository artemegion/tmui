//using Tmui;
//using Tmui.Rendering;
//using Windows.Win32;
//using Windows.Win32.Foundation;
//using Windows.Win32.System.Console;
//// using Tmui.Windows;

//TerminalRenderer renderer = new(Console.WindowWidth, Console.WindowHeight);
//renderer.Init();

//GlyphSurface surface = new(Console.WindowWidth, Console.WindowHeight);

//// surface.FillRect(new Rect(5, 5, 20, 10), Color.White);

//Color foregroundColor = new(222, 222, 222);
//Color dimmedForegroundColor = new(191, 191, 191);
//Color backgroundColor = new(51, 51, 51);
//Color panelColor = new(77, 77, 77);
//Color elementColor = new(102, 102, 102);
//Color activeElementColor = new(128, 128, 128);
//Color interactableColor = new(102, 178, 255);

//char rightHalfBlockChar = '▐';
//char downPointingTriangleChar = '▼';
//char rightPointingTriangleChar = '▶';


////while (true)
////{
//// surface.DrawTextSimple(new Rect(5, 5, 20, 1), "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum.".AsSpan(), Color.White);

//// background
//surface.FillRect(new Rect(0, 0, surface.Width, surface.Height), backgroundColor);

//// left panel
//int leftPanelX = 2;
//int leftPanelY = 1;
//int leftPanelW = 30;// (int)(surface.Width * 0.3f);

//surface.DrawTextSimple((2, 0), "Modlists".AsSpan(), foregroundColor);
//surface.FillRect(new Rect(leftPanelX, leftPanelY, leftPanelW, surface.Height - 2), panelColor);

//surface.FillRect(new Rect(leftPanelX + 2, leftPanelY + 1, leftPanelW - 4, 3), elementColor);
//for (int y = 0; y < 3; y++) surface.DrawChar((leftPanelX + 1, leftPanelY + 1 + y), rightHalfBlockChar, interactableColor);
//surface.DrawTextSimple((leftPanelX + 3, leftPanelY + 1), "Kasztelan".AsSpan(), dimmedForegroundColor);
//surface.DrawTextSimple((leftPanelX + 3, leftPanelY + 2), "Fabric 1.19.2".AsSpan(), foregroundColor);
//surface.DrawTextSimple((leftPanelX + 3, leftPanelY + 3), "182 mods".AsSpan(), foregroundColor);

//leftPanelY += 4;

//surface.FillRect(new Rect(leftPanelX + 2, leftPanelY + 1, leftPanelW - 4, 3), activeElementColor);
//for (int y = 0; y < 3; y++) surface.DrawChar((leftPanelX + 1, leftPanelY + 1 + y), rightHalfBlockChar, interactableColor);
//surface.DrawTextSimple((leftPanelX + 3, leftPanelY + 1), "Test".AsSpan(), dimmedForegroundColor);
//surface.DrawTextSimple((leftPanelX + 3, leftPanelY + 2), "Fabric 1.16.0".AsSpan(), foregroundColor);
//surface.DrawTextSimple((leftPanelX + 3, leftPanelY + 3), "2 mods".AsSpan(), foregroundColor);

//leftPanelY += 4;

//for (int y = 0; y < 3; y++) surface.DrawChar((leftPanelX + 1, leftPanelY + 1 + y), rightHalfBlockChar, interactableColor);
//surface.FillRect(new Rect(leftPanelX + 2, leftPanelY + 1, 7, 3), elementColor);
//surface.DrawTextSimple((leftPanelX + 4, leftPanelY + 2), "add".AsSpan(), interactableColor);

//// fabric panel
//int rightPanelX = leftPanelW + 4;
//int rightPanelY = 1;
//int rightPanelW = 60;
//int rightPanelH = 7;

//surface.DrawChar((rightPanelX, 0), downPointingTriangleChar, interactableColor);
//surface.DrawTextSimple((rightPanelX + 2, 0), "Fabric".AsSpan(), foregroundColor);
//surface.FillRect(new Rect(rightPanelX, rightPanelY, rightPanelW, rightPanelH), panelColor);

//surface.DrawTextSimple((rightPanelX + 2, rightPanelY + 1), "Minecraft".AsSpan(), foregroundColor);
//surface.FillRect((rightPanelX + rightPanelW - 30 - 2, rightPanelY + 1, 30, 1), elementColor);
//surface.DrawChar((rightPanelX + rightPanelW - 30 - 3, rightPanelY + 1), rightHalfBlockChar, interactableColor);
//surface.DrawChar((rightPanelX + rightPanelW - 4, rightPanelY + 1), downPointingTriangleChar, interactableColor);
//surface.DrawTextSimple((rightPanelX + rightPanelW - 30 - 2 + 1, rightPanelY + 1), "1.16.2", foregroundColor);

//surface.DrawTextSimple((rightPanelX + 2, rightPanelY + 3), "Loader".AsSpan(), foregroundColor);
//surface.FillRect((rightPanelX + rightPanelW - 30 - 2, rightPanelY + 3, 30, 1), elementColor);
//surface.DrawChar((rightPanelX + rightPanelW - 30 - 3, rightPanelY + 3), rightHalfBlockChar, interactableColor);
//surface.DrawChar((rightPanelX + rightPanelW - 4, rightPanelY + 3), downPointingTriangleChar, interactableColor);
//surface.DrawTextSimple((rightPanelX + rightPanelW - 30 - 2 + 1, rightPanelY + 3), "1.14.10", foregroundColor);

//surface.DrawTextSimple((rightPanelX + 2, rightPanelY + 5), "Installer".AsSpan(), foregroundColor);
//surface.FillRect((rightPanelX + rightPanelW - 30 - 2, rightPanelY + 5, 30, 1), elementColor);
//surface.DrawChar((rightPanelX + rightPanelW - 30 - 3, rightPanelY + 5), rightHalfBlockChar, interactableColor);
//surface.DrawChar((rightPanelX + rightPanelW - 4, rightPanelY + 5), downPointingTriangleChar, interactableColor);
//surface.DrawTextSimple((rightPanelX + rightPanelW - 30 - 2 + 1, rightPanelY + 5), "1.11.1", foregroundColor);

//// java panel
//rightPanelY = 10;
//rightPanelH = 6;

//surface.DrawChar((rightPanelX, rightPanelY - 1), downPointingTriangleChar, interactableColor);
//surface.DrawTextSimple((rightPanelX + 2, rightPanelY - 1), "Java".AsSpan(), foregroundColor);
//surface.FillRect(new Rect(rightPanelX, rightPanelY, rightPanelW, rightPanelH), panelColor);

//surface.DrawTextSimple((rightPanelX + 2, rightPanelY + 1), "Version".AsSpan(), foregroundColor);
//surface.FillRect((rightPanelX + rightPanelW - 30 - 2, rightPanelY + 1, 30, 1), elementColor);
//surface.DrawChar((rightPanelX + rightPanelW - 30 - 3, rightPanelY + 1), rightHalfBlockChar, interactableColor);
//surface.DrawChar((rightPanelX + rightPanelW - 4, rightPanelY + 1), downPointingTriangleChar, interactableColor);
//surface.DrawTextSimple((rightPanelX + rightPanelW - 30 - 2 + 1, rightPanelY + 1), "18", foregroundColor);

//surface.DrawTextSimple((rightPanelX + 2, rightPanelY + 3), "Arguments".AsSpan(), foregroundColor);
//surface.FillRect((rightPanelX + 2, rightPanelY + 4, rightPanelW - 4, 1), elementColor);
//surface.DrawChar((rightPanelX + 1, rightPanelY + 4), rightHalfBlockChar, interactableColor);
//surface.DrawTextSimple((rightPanelX + 3, rightPanelY + 4), "-Xms2G -Xmx4G".AsSpan(), foregroundColor);

//// server properties panel
//rightPanelY = 18;
//rightPanelH = 0;

//surface.DrawChar((rightPanelX, rightPanelY - 1), rightPointingTriangleChar, interactableColor);
//surface.DrawTextSimple((rightPanelX + 2, rightPanelY - 1), "Server properties".AsSpan(), foregroundColor);

//// mods panel
//rightPanelY = 20;
//rightPanelH = 11;

//surface.DrawChar((rightPanelX, rightPanelY - 1), downPointingTriangleChar, interactableColor);
//surface.DrawTextSimple((rightPanelX + 2, rightPanelY - 1), "Mods (2)".AsSpan(), foregroundColor);
//surface.FillRect(new Rect(rightPanelX, rightPanelY, rightPanelW, rightPanelH), panelColor);

//surface.FillRect((rightPanelX + 2, rightPanelY + 1, rightPanelW - 4, 2), elementColor);
//surface.DrawTextSimple((rightPanelX + 3, rightPanelY + 1), "Fabric API", foregroundColor);
//surface.DrawTextSimple((rightPanelX + 3, rightPanelY + 2), "0.67.1", dimmedForegroundColor);
//for (int y = 0; y < 2; y++) surface.DrawChar((rightPanelX + 1, rightPanelY + 1 + y), rightHalfBlockChar, interactableColor);

//surface.FillRect((rightPanelX + 2, rightPanelY + 1 + 3, rightPanelW - 4, 2), elementColor);
//surface.DrawTextSimple((rightPanelX + 3, rightPanelY + 1 + 3), "Lithium", foregroundColor);
//surface.DrawTextSimple((rightPanelX + 3, rightPanelY + 2 + 3), "0.10.2", dimmedForegroundColor);
//for (int y = 0; y < 2; y++) surface.DrawChar((rightPanelX + 1, rightPanelY + 1 + 3 + y), rightHalfBlockChar, interactableColor);


//// renderer.RenderToBuffer((0, 0), surface);
//// renderer.RenderDirtyAreaToScreen();

//// surface.FillRect((20, 5, 40, 20), new Color(255, 255, 255, 50));
//// surface.FillRect((25, 7, 40, 20), new Color(255, 0, 0, 50));


//GlyphSurface surf = new(20, 3);
//surf.FillRect((0, 0, 20, 3), new Color(255, 255, 255));
//// surf.DrawTextSimple((0, 0), "Hello, world!", Color.Black);

//// renderer.RenderToBuffer((0, 0), surf);
//// renderer.RenderDirtyAreaToScreen();

//Rect prevDirtyArea = (0, 0, surface.Width, surface.Height);

//// nint consoleWnd = Kernel32.GetConsoleWindow();
//HANDLE stdInputHandle = PInvoke.GetStdHandle(STD_HANDLE.STD_INPUT_HANDLE);

//CONSOLE_MODE consoleMode = 0;

//// idk man just keep it here
//GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced, true, true);
//GC.Collect(GC.MaxGeneration, GCCollectionMode.Aggressive, true, true);

//unsafe
//{
//    PInvoke.GetConsoleMode(stdInputHandle, &consoleMode);
//    PInvoke.SetConsoleMode(stdInputHandle, (consoleMode & ~CONSOLE_MODE.ENABLE_QUICK_EDIT_MODE) | CONSOLE_MODE.ENABLE_MOUSE_INPUT | CONSOLE_MODE.ENABLE_WINDOW_INPUT);

//    Span<INPUT_RECORD> inputRecords = stackalloc INPUT_RECORD[128];
//    uint lpNumberOfEvents = 0;

//    while (true)
//    {
//        if (PInvoke.GetNumberOfConsoleInputEvents(stdInputHandle, &lpNumberOfEvents) && lpNumberOfEvents > 0)
//        {
//            fixed (INPUT_RECORD* ir = inputRecords)
//            {
//                PInvoke.ReadConsoleInput(stdInputHandle, ir, (uint)inputRecords.Length, &lpNumberOfEvents);
//            }
//            // surf.DrawTextSimple((0, 0), "events read: " + eventsRead, Color.Black);
//            // surf.DrawTextSimple((0, 2), inputRecords[0].EventType.ToString(), Color.Black);

//            for (int i = 0; i < lpNumberOfEvents; i++)
//            {
//                switch (inputRecords[i].EventType)
//                {
//                    case 0x0002: // mouse event
//                        // case Kernel32.InputEventTypeFlag.MOUSE_EVENT:
//                        MOUSE_EVENT_RECORD mer = inputRecords[i].Event.MouseEvent;
//                        COORD mP = mer.dwMousePosition;

//                        if (mer.dwEventFlags == 0x0001) // mouse moved
//                        {
//                            if (mP.X >= 0 && mP.X < 20 && mP.Y >= 0 && mP.Y < 3)
//                            {
//                                surf.FillRect((0, 0, 20, 3), new Color(255, 0, 0));
//                            }
//                            else
//                            {
//                                surf.FillRect((0, 0, 20, 3), new Color(255, 255, 255));
//                            }

//                            surf.DrawTextSimple((0, 0), "X: " + mP.X, Color.Black);
//                            surf.DrawTextSimple((0, 1), "Y: " + mP.Y, Color.Black);
//                        }

//                        break;
//                }
//            }
//        }
//        else
//        {
//            Thread.Sleep(1000 / 30);
//        }

//        renderer.RenderToBuffer((0, 0), surface);
//        renderer.RenderToBuffer((0, 0), surf);

//        renderer.RenderDirtyAreaToScreen();
//        renderer.ClearDirtyArea();
//    }
//}


using System.Security.Cryptography.X509Certificates;
using System.Text;

using Tmui;
using Tmui.Core;
using Tmui.Graphics;
using Tmui.Immediate;
using Tmui.Messages;

TermApp app = new();
Ui ui = new(app.Terminal, app.Input);

Pos pos = (0, 0);
bool checked0 = false;

int checkedId = -1;
int sel = 0;
bool opnd = false;
string[] options = new[] { "Enable radials!", "Disable radials!", "Don't care about radials!", "Oh my gosh this option is soooo loooong!" };

string longText = "Lorem ipsum dolor sit amet,\nconsectetur adipiscing elit,\nsed do eiusmod tempor incididunt ut labore et dolore magna aliqua.\nUt enim ad minim veniam, \nquis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat.\nDuis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur.\nExcepteur sint occaecat cupidatat non proident,\nsunt in culpa qui officia deserunt mollit anim id est laborum.\n1\n2\n3\n4\n5\n6\n7\n8\n9";
Range[] longTextLines = new Range[40];
Surface.WrapText(longText, int.MaxValue, longTextLines.AsSpan(), out int p0);
int ltvs = 0, lths = 4;

int x = 0;

char[] editTextBoxChars = new char[512];
Range[] editTextBoxRangesOfLines = new[] { new Range(0, 12) };
"Hello, World!".CopyTo(editTextBoxChars);
EditTextBoxState editTextBoxState = new() { CursorX = 2 };

int redraws = 0;

app.AddMsgHandler<UpdateMsg>(_ =>
{
    if (app.Input.KeyPressed(Key.LeftArrow))
    {
        x--;
        ui.ReqRedraw = true;
    }

    if (app.Input.KeyPressed(Key.RightArrow))
    {
        x++;
        ui.ReqRedraw = true;
    }
});

app.AddMsgHandler<UpdateMsg>(_ =>
{
    ui.Clear();

    ui.TextBox(
        (1, 1, 38, 11),
        "Lorem ipsum dolor sit amet, consectetura align this\n\n\n\nnicely please!",
        wrapText: true,
        (TextAlign.Center, TextAlign.End)
    );

    ui.Checkbox(ref checked0, (1, 13));

    if (sel == 1) ui.Enabled = false;
    ui.BeginRadialGroup(checkedId);
    {
        ui.Radial((4, 13));
        ui.Radial((7, 13));
        ui.Radial((10, 13));
    }
    ui.EndRadialGroup(out checkedId);
    ui.Enabled = true;

    ui.Checkbox(ref checked0, (1, 20, 20, 1), "This is checkbox!", TextAlign.Start);

    ui.Enabled = checked0;
    if (ui.Button((4, 15, 18, 1), "Reset radials", TextAlign.Center))
    {
        checkedId = -1;
    }
    ui.Enabled = true;

    ui.Dropdown(new(50, 7, 20, 3), options.AsSpan(), ref sel);

    // ui.TextBox(new(50, 12, 30, 15), longText, longTextLines.AsSpan()[..p0], TextAlign.Start, TextBoxScrollFlags.Vertical | TextBoxScrollFlags.Horizontal, ref ltvs, ref lths);

    ui.EditTextBox(new(50, 12, 30, 15), editTextBoxChars, editTextBoxRangesOfLines, TextAlign.Start, TextBoxScrollFlags.None, ref editTextBoxState);

    ui.Label(new(x, 0), $"Redraws: {redraws}, Changed: {ui.Changed}");

    if (ui.Flush()) redraws++;
});

app.Run();
