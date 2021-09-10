namespace Engine;
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Gl;
using static Gl.Utilities;
//using static Gl.Calls;
using System.Collections.Generic;
using System.Reflection;
using GLFW;
using System.IO;
using System.Text.RegularExpressions;

class GlWindowBase:IDisposable {

    static GlWindowBase () {
        if (HANDLE_ERROR is null)
            _ = Glfw.SetErrorCallback(HANDLE_ERROR = HandleError);
    }

    private static readonly ErrorCallback HANDLE_ERROR;
    private static void HandleError (ErrorCode code, IntPtr message) => throw new ApplicationException(Marshal.PtrToStringAnsi(message) ?? "?");

    unsafe private GlWindowBase (int width, int height, Monitor monitor) {
        Window = Glfw.CreateWindow(Width = width, Height = height, GetType().Name, monitor, Window.None);
        Glfw.MakeContextCurrent(Window);
        Assign();
        SwapInterval = 1;
        Calls.DebugMessageCallback(debugProc = HandleDebug, IntPtr.Zero);
        State.DebugOutput = true;
        Init();
        BindKeys();
    }

    private void BindKeys () {
        foreach (var mi in GetType().GetMethods(BindingFlags.NonPublic | BindingFlags.Instance))
            if (TryGetAttribute<KeyBindingAttribute>(mi, out var a, false)) {
                var action = mi.CreateDelegate<Action<Keys, InputState>>(this);
                foreach (var k in a.Key)
                    keys.Add(k, action);
            }
    }

    public GlWindowBase (Monitor monitor) : this(monitor.WorkArea.Width, monitor.WorkArea.Height, monitor) { }
    public GlWindowBase (int width, int height) : this(width, height, Monitor.None) { }

    public int Width { get; private set; }
    public int Height { get; private set; }
    public Window Window { get; private set; }

    protected virtual void Init () { }
    protected virtual void Render (float dt) { }
    private static readonly long startTicks = Stopwatch.GetTimestamp();
    protected static long Ticks => Stopwatch.GetTimestamp() - startTicks;
    protected Camera Camera { get; } = new Camera(new());

    private readonly Dictionary<Keys, Action<Keys, InputState>> keys = new();
    private const double _DESIRED_FRAMERATE = 144.0;
    private static readonly float FRAME_DURATION = (float)(1.0 / _DESIRED_FRAMERATE);
    private static readonly long EXPECTED_TICKS_PER_FRAME = (long)(Stopwatch.Frequency / _DESIRED_FRAMERATE);
    public ulong FrameCount { get; private set; }
    private long lastSwapTicks = 0l;
    protected StreamWriter Log { get; } = new("log.txt", false, System.Text.Encoding.ASCII); // MOVE TO SEP. THREAD
    public void Run () {
        Glfw.GetCursorPosition(Window, out var mx, out var my);
        lastMousePosition = new(Convert.ToInt32(mx), Convert.ToInt32(my));
        State.LineSmooth = true;
        State.Dither = false;

        Glfw.ShowWindow(Window);
        OnWindowFocus(IntPtr.Zero, Glfw.GetWindowAttribute(Window, WindowAttribute.Focused));

        while (!Glfw.WindowShouldClose(Window)) {
            if (!Focused) {
                lastSwapTicks = 0l;
                Glfw.WaitEvents();
                continue;
            }
            Render();
        }
    }

    private void Render () {
        Stamp("start");
        if (Focused && CursorGrabbed)
            Camera.Move(10f * FRAME_DURATION);

        Render(FRAME_DURATION);
        DelayForRetrace();
        Stamp(nameof(Glfw.SwapBuffers));
        Glfw.SwapBuffers(Window);
        Stamp("end");
        lastSwapTicks = Ticks;
        ++FrameCount;
    }
    [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
    private void Stamp () {
        Log.Write(Ticks);
        Log.Write(' ');
        Log.WriteLine(new StackFrame(1).GetMethod().Name);
    }
    [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
    private void Stamp (string m) {
        Log.Write(Ticks);
        Log.Write(' ');
        Log.WriteLine(m);
    }
    private void DelayForRetrace () {
        Stamp();
        var delayed = lastSwapTicks + 3 * EXPECTED_TICKS_PER_FRAME / 4;
        do
            Glfw.PollEvents();
        while (Ticks < delayed);
    }

    private int swapInterval;
    protected int SwapInterval {
        get => swapInterval;
        set => Glfw.SwapInterval(swapInterval = value);
    }

    public bool Focused { get; private set; }

    private bool cursorGrabbed;
    public bool CursorGrabbed {
        get => cursorGrabbed;
        set => Glfw.SetInputMode(Window, InputMode.Cursor, (cursorGrabbed = value) ? (int)CursorMode.Disabled : (int)CursorMode.Normal);
    }

    protected void SetTitle (string value) => Glfw.SetWindowTitle(Window, value);

    [KeyBinding(Keys.Escape)]
    protected void Close (Keys _, InputState state) {
        if (state == InputState.Release)
            OnCloseInternal(IntPtr.Zero);
    }

#pragma warning disable IDE0052 // Remove unread private members
    private CharCallback onChar;
    private CharModsCallback onCharMods;
    private FileDropCallback onDrop;
    private FocusCallback onWindowFocus;
    private IconifyCallback onWindowIconify;
    private JoystickCallback onJoystick;
    private KeyCallback onKey;
    private MonitorCallback onMonitor;
    private MouseButtonCallback onMouseButton;
    private MouseCallback onCursorPosition, onScroll;
    private MouseEnterCallback onCursorEnter;
    private PositionCallback onWindowPosition;
    private SizeCallback onFramebufferSize, onWindowSize;
    private WindowCallback onWindowRefresh, onClose;
    private WindowContentsScaleCallback onWindowContentScale;
    private WindowMaximizedCallback onWindowMaximize;
    private readonly DebugProc debugProc;
    [KeyBinding(Keys.Tab)]
    protected void ToggleCursorGrabbed (Keys _, InputState state) {
        if (state == InputState.Release)
            CursorGrabbed = !CursorGrabbed;
    }
#pragma warning restore IDE0052 // Remove unread private members
    private void Assign () {
        onChar = OnChar;
        onCharMods = OnCharMods;
        onClose = OnCloseInternal;
        onCursorEnter = OnCursorEnter;
        onCursorPosition = OnCursorPosition;
        onDrop = OnDrop;
        onFramebufferSize = OnFramebufferSize;
        onKey = OnKey;
        onJoystick = OnJoystick;
        onMonitor = OnMonitor;
        onMouseButton = OnMouseButton;
        onScroll = OnScroll;
        onWindowContentScale = OnWindowContentScale;
        onWindowFocus = OnWindowFocus;
        onWindowIconify = OnWindowIconify;
        onWindowMaximize = OnWindowMaximize;
        onWindowPosition = OnWindowPosition;
        onWindowRefresh = OnWindowRefresh;
        onWindowSize = OnWindowSize;
        _ = Glfw.SetCharCallback(Window, onChar);
        _ = Glfw.SetCharModsCallback(Window, onCharMods);
        _ = Glfw.SetCloseCallback(Window, onClose);
        _ = Glfw.SetCursorEnterCallback(Window, onCursorEnter);
        _ = Glfw.SetCursorPositionCallback(Window, onCursorPosition);
        _ = Glfw.SetDropCallback(Window, onDrop);
        _ = Glfw.SetFramebufferSizeCallback(Window, onFramebufferSize);
        _ = Glfw.SetKeyCallback(Window, onKey);
        _ = Glfw.SetJoystickCallback(onJoystick);
        _ = Glfw.SetMonitorCallback(onMonitor);
        _ = Glfw.SetMouseButtonCallback(Window, onMouseButton);
        _ = Glfw.SetScrollCallback(Window, onScroll);
        _ = Glfw.SetWindowContentScaleCallback(Window, onWindowContentScale);
        _ = Glfw.SetWindowFocusCallback(Window, onWindowFocus);
        _ = Glfw.SetWindowIconifyCallback(Window, onWindowIconify);
        _ = Glfw.SetWindowMaximizeCallback(Window, onWindowMaximize);
        _ = Glfw.SetWindowPositionCallback(Window, onWindowPosition);
        _ = Glfw.SetWindowRefreshCallback(Window, onWindowRefresh);
        _ = Glfw.SetWindowSizeCallback(Window, onWindowSize);
    }

    private void OnWindowContentScale (IntPtr _, float xScale, float yScale) => Stamp();
    private void OnScroll (IntPtr _, double x, double y) => Stamp();
    private void OnJoystick (Joystick joystick, ConnectionStatus status) => Stamp();
    private void OnDrop (IntPtr _, int count, IntPtr arrayPtr) => Stamp();
    private void OnCharMods (IntPtr _, uint codePoint, ModifierKeys mods) => Stamp();
    private void OnCursorEnter (IntPtr _, bool entering) => Stamp();
    private void OnWindowIconify (IntPtr _, bool iconified) => Stamp();
    private void OnWindowSize (IntPtr _, int width, int height) => Stamp();
    private void OnWindowRefresh (IntPtr _) => Stamp();
    private void OnMonitor (Monitor monitor, ConnectionStatus status) => Stamp();
    private void OnWindowPosition (IntPtr _, double x, double y) => Stamp();
    private void OnWindowMaximize (IntPtr _, bool maximized) => Stamp();
    private void OnMouseButton (IntPtr _, MouseButton button, InputState state, ModifierKeys modifiers) => Stamp();
    private void OnChar (IntPtr _, uint code) => Stamp();

    private Vector2i lastMousePosition;
    private void OnCursorPosition (IntPtr _, double x, double y) {
        Stamp();
        var delta = new Vector2i(Convert.ToInt32(x), Convert.ToInt32(y));
        var mouseDelta = delta - lastMousePosition;
        if (Focused && CursorGrabbed)
            Camera.Mouse(mouseDelta);
        lastMousePosition = delta;
    }

    private void OnWindowFocus (IntPtr _, bool focused) {
        Stamp();
        Focused = focused;
        if (!focused)
            CursorGrabbed = false;
    }

    private void OnFramebufferSize (IntPtr _, int width, int height) {
        Stamp();
        Width = width;
        Height = height;
    }

    private void OnKey (IntPtr _, Keys key, int code, InputState state, ModifierKeys modifier) {
        Stamp();
        if (keys.TryGetValue(key, out var keyAction))
            keyAction(key, state);
    }

    [KeyBinding(Keys.Z, Keys.X, Keys.C, Keys.D, Keys.LeftShift, Keys.LeftControl)]
    protected void CameraKey (Keys key, InputState state) => _ = Camera.Key(key, state);

    protected virtual void OnClose () { }
    private void OnCloseInternal (IntPtr _) {
        Glfw.SetWindowShouldClose(Window, true);
        OnClose();
    }
    private const string _DEBSTR = "source: {0}\ntype: {1}\nseverity: {2}\nmessage: {3}";
    unsafe private void HandleDebug (int source, int type, int id, int severity, int length, byte* message, void* _) {
        throw new ApplicationException(string.Format(_DEBSTR, source, type, severity, Marshal.PtrToStringAnsi(new(message)) ?? "?"));
    }

    public void Dispose () => Dispose(true);

    private bool disposed;
    protected virtual void Dispose (bool disposing) {
        if (!disposed && Window != Window.None) {
            if (disposing) {
                Glfw.DestroyWindow(Window);
                Window = Window.None;
                Log.Dispose();
            }
            disposed = true;
        }
    }
}
