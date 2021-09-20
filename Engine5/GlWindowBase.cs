namespace Engine;
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Gl;
using System.Collections.Generic;
using System.Reflection;
using GLFW;
#if !DEBUG
using System.Runtime.CompilerServices;
#endif

class GlWindowBase:IDisposable {

    protected enum Events {
        Render = 1,
        Delay,
        Swap,
        Camera,
        CursorPosition,
        Noise,
        Texture,
    }

    unsafe private GlWindowBase (int width, int height, Monitor monitor) {
        Window = Glfw.CreateWindow(Width = width, Height = height, GetType().Name, monitor, Window.None);
        isFullscreen = monitor != Monitor.None;
        Glfw.MakeContextCurrent(Window);
        Calls.DebugMessageCallback(debugProc = HandleDebug, IntPtr.Zero);
        Assign();
        Glfw.SwapInterval(swapInterval = 1);
        State.DebugOutput = true;
        Init();
        BindKeys();
    }

    private void BindKeys () {
        foreach (var mi in GetType().GetMethods(BindingFlags.NonPublic | BindingFlags.Instance))
            if (mi.GetCustomAttribute<KeyBindingAttribute>(false) is KeyBindingAttribute a) {
                var action = mi.CreateDelegate<Action<Keys, InputState>>(this);
                foreach (var k in a.Key)
                    keys.Add(k, action);
            }
    }

    public GlWindowBase (Monitor monitor) : this(monitor.WorkArea.Width, monitor.WorkArea.Height, monitor) { }
    public GlWindowBase (int width, int height) : this(width, height, Monitor.None) { }

    protected int Width { get; private set; }
    protected int Height { get; private set; }
    protected Window Window { get; private set; }
    protected ulong FramesRendered { get; private set; }

    protected virtual void Init () { }
    protected virtual void Render (float dt) { }

#if !DEBUG
    [MethodImpl(MethodImplOptions.AggressiveOptimization | MethodImplOptions.AggressiveInlining)]
#endif
    protected long GetTicks () => Stopwatch.GetTimestamp() - startTicks;
    protected Camera Camera { get; } = new Camera(new(0f, 0f, 4f));

    private int desiredFramerate = 80;
    private float FrameDuration => 1.0f / desiredFramerate;
    private long ExpectedTicksPerFrame => (long)((double)Stopwatch.Frequency / desiredFramerate);
    private long startTicks;
    private long lastSwapTicks = 0l;
    private readonly bool isFullscreen;
    private readonly Dictionary<Keys, Action<Keys, InputState>> keys = new();
#if __PERF__
    private readonly Perf<Events> perf = new("log.bin");
#if !DEBUG
    [MethodImpl(MethodImplOptions.AggressiveOptimization | MethodImplOptions.AggressiveInlining)]
#endif
    protected void Enter (Events e) => perf.Enter((int)e);
#if !DEBUG
    [MethodImpl(MethodImplOptions.AggressiveOptimization | MethodImplOptions.AggressiveInlining)]
#endif
    protected void Leave () => perf.Leave();
#endif

    public void Run () {
        Glfw.GetCursorPosition(Window, out var mx, out var my);
        lastMousePosition = new(Convert.ToInt32(mx), Convert.ToInt32(my));
        State.LineSmooth = true;
        State.Dither = false;
        Glfw.ShowWindow(Window);
        OnWindowFocus(Window, Glfw.GetWindowAttribute(Window, WindowAttribute.Focused));
        CursorGrabbed = isFullscreen;
        startTicks = Stopwatch.GetTimestamp();
        while (!Glfw.WindowShouldClose(Window)) {
            if (Focused) {
                Render();
            } else {
                lastSwapTicks = 0l;
                Glfw.WaitEvents();
            }
        }
    }
    private void Render () {
#if __PERF__
        Enter(Events.Render);
#endif
        if (Focused && CursorGrabbed)
            Camera.Move(10f * FrameDuration);
        Render(FrameDuration);
        Calls.Finish();
        DelayForRetrace();
#if __PERF__
        Enter(Events.Swap);
#endif
        //Calls.Flush();
        Glfw.SwapBuffers(Window);
#if __PERF__
        Leave();
        Leave();
#endif
        lastSwapTicks = GetTicks();
        ++FramesRendered;
    }

    private void DelayForRetrace () {
#if __PERF__
        Enter(Events.Delay);
#endif
        var delayed = lastSwapTicks + ExpectedTicksPerFrame;
        do {
            Glfw.PollEvents();
        } while (GetTicks() < delayed);
#if __PERF__
        Leave();
#endif
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
    [KeyBinding(Keys.F1)]
    protected void CycleSwapInterval (Keys _, InputState state) {
        if (state != InputState.Release)
            return;
        SwapInterval = SwapInterval == 1 ? -1 : SwapInterval + 1;
        Utilities.Trace($"{nameof(SwapInterval)}: {SwapInterval}");
    }

    [KeyBinding(Keys.Escape)]
    protected void Close (Keys _, InputState state) {
        if (state == InputState.Release)
            OnCloseInternal(Window);
    }
    [KeyBinding(Keys.PageDown, Keys.PageUp)]
    protected void AlterFramerate (Keys k, InputState state) {
        if (state != InputState.Release)
            return;
        var fr = desiredFramerate;
        fr += (k == Keys.PageUp) ? 10 : -10;
        desiredFramerate = Math.Max(50, Math.Min(fr, 140));
        Utilities.Trace($"{nameof(desiredFramerate)}: {desiredFramerate}");
    }
    [KeyBinding(Keys.Tab)]
    protected void ToggleCursorGrabbed (Keys _, InputState state) {
        if (state == InputState.Release && !isFullscreen) {
            CursorGrabbed = !CursorGrabbed;
            Utilities.Trace($"{nameof(CursorGrabbed)}: {CursorGrabbed}");
        }
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

    private void OnWindowContentScale (Window _, float xScale, float yScale) { }
    private void OnScroll (Window _, double x, double y) { }
    private void OnJoystick (Joystick joystick, ConnectionStatus status) { }
    private void OnDrop (Window _, int count, IntPtr arrayPtr) { }
    private void OnCharMods (Window _, uint codePoint, ModifierKeys mods) { }
    private void OnCursorEnter (Window _, bool entering) { }
    private void OnWindowIconify (Window _, bool iconified) { }
    private void OnWindowSize (Window _, int width, int height) { }
    private void OnWindowRefresh (Window _) { }
    private void OnMonitor (Monitor monitor, ConnectionStatus status) { }
    private void OnWindowPosition (Window _, double x, double y) { }
    private void OnWindowMaximize (Window _, bool maximized) { }
    private void OnMouseButton (Window _, MouseButton button, InputState state, ModifierKeys modifiers) { }
    private void OnChar (Window _, uint code) { }

    private Vector2i lastMousePosition;
    private void OnCursorPosition (Window _, double x, double y) {
#if __PERF__
        Enter(Events.CursorPosition);
#endif
        var delta = new Vector2i(Convert.ToInt32(x), Convert.ToInt32(y));
        var mouseDelta = delta - lastMousePosition;
        if (Focused && CursorGrabbed)
            Camera.Mouse(mouseDelta);
        lastMousePosition = delta;
#if __PERF__
        Leave();
#endif
    }

    private void OnWindowFocus (Window _, bool focused) {
        Focused = focused;
        if (!focused)
            CursorGrabbed = false;
        else if (isFullscreen)
            CursorGrabbed = true;
    }

    private void OnFramebufferSize (Window _, int width, int height) {
        Width = width;
        Height = height;
    }

    private void OnKey (Window _, Keys key, int code, InputState state, ModifierKeys modifier) {
        if (keys.TryGetValue(key, out var keyAction))
            keyAction(key, state);
    }

    [KeyBinding(Keys.Z, Keys.X, Keys.C, Keys.D, Keys.LeftShift, Keys.LeftControl)]
    protected void CameraKey (Keys key, InputState state) => _ = Camera.Key(key, state);

    protected virtual void OnClose () { }
    private void OnCloseInternal (Window _) {
        Glfw.SetWindowShouldClose(Window, true);
        OnClose();
    }
    private const string _DEBUG_FORMAT_STRING = "source: {0}\ntype: {1}\nseverity: {2}\nmessage: {3}";
    unsafe private void HandleDebug (int source, int type, int id, int severity, int length, byte* message, void* _) {
        throw new ApplicationException(string.Format(_DEBUG_FORMAT_STRING, source, type, severity, Marshal.PtrToStringAnsi(new(message)) ?? "?"));
    }

    public void Dispose () {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    private bool disposed;
    protected virtual void Dispose (bool disposing) {
        if (!disposed && Window != Window.None) {
            if (disposing) {
                Glfw.DestroyWindow(Window);
                Window = Window.None;
#if __PERF__
                perf.Dispose();
#endif
            }
            disposed = true;
        }
    }
}
