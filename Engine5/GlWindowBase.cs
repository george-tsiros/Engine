namespace Engine;
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Gl;
using static Extra;
using static Gl.Utilities;
using static Gl.Calls;
using Shaders;
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

    private GlWindowBase () {
        if (File.Exists("hints.txt"))
            SetHintsFrom("hints.txt");
    }
    private static bool TryGetHintValue (string hintValue, out int value) {
        if (bool.TryParse(hintValue, out var b)) {
            value = b ? 1 : 0;
            return true;
        }
        if (int.TryParse(hintValue, out value))
            return true;
        var parts = hintValue.Split('.');
        if (parts.Length == 2 && typeof(Glfw).Assembly.GetType($"GLFW.{parts[0]}", false, true) is Type t && t.GetField(parts[1], BindingFlags.Static | BindingFlags.Public | BindingFlags.IgnoreCase) is FieldInfo fi) {
            value = (int)fi.GetValue(null);
            return true;
        }
        value = 0;
        return false;
    }
    private static void SetHintsFrom (string filepath) {
        var hintRegex = new Regex(@"^ *(\w+) *= *(true|false|\d+|(\w+)\.(\w+)) *$");
        foreach (var line in File.ReadAllLines(filepath))
            if (hintRegex.TryMatch(line, out var m)) {
                if (Enum.TryParse<Hint>(m.Groups[1].Value, true, out var hint)) {
                    if (TryGetHintValue(m.Groups[2].Value, out var i))
                        Glfw.WindowHint(hint, i);
                    else
                        throw new ApplicationException($"could not get an int out of '{m.Groups[2].Value}' for {hint}");
                }
            }
    }

    private GlWindowBase (int width, int height, Monitor monitor) : this() {
        Window = Glfw.CreateWindow(Width = width, Height = height, GetType().Name, monitor, Window.None);
        Glfw.MakeContextCurrent(Window);
        Assign();
        SwapInterval = 1;
        glDebugMessageCallback(debugProc = HandleDebug, IntPtr.Zero);
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
    public bool Iconified { get; private set; }

    protected virtual void Init () { }
    protected virtual void Render (float dt) { }
    protected Stopwatch Timer { get; } = Stopwatch.StartNew();
    protected Camera Camera { get; } = new Camera(new());

    private readonly Dictionary<Keys, Action<Keys, InputState>> keys = new();
    private const int _DESIRED_FRAMERATE = 140, _SAMPLES = 256;
    private static readonly double TIMER_PERIOD = 1.0 / Stopwatch.Frequency;
    private static readonly long EXPECTED_TICKS_PER_FRAME = Stopwatch.Frequency / _DESIRED_FRAMERATE;
    public ulong FrameCount { get; private set; }
    private long lastSwapTicks = 0l, lastTicks = 0l;
    private readonly float[] xcoords = new float[_SAMPLES];
    private readonly float[] ycoords = new float[_SAMPLES];
    private VertexArray plotVao;
    private VertexBuffer<float> plotBuffer;

    public void Run () {
        GLFW.Glfw.GetCursorPosition(Window, out var mx, out var my);
        lastMousePosition = new(Convert.ToInt32(mx), Convert.ToInt32(my));
        for (var i = 0; i < xcoords.Length; ++i)
            xcoords[i] = i * 2f / _SAMPLES - 1f;
        for (var i = 0; i < ycoords.Length; ++i)
            ycoords[i] = (float)Math.Sin(Math.PI * 2 * i / _SAMPLES) + 1f;
        State.Program = Plot.Id;
        Plot.Color(new(0f, 1f, 0f, 1f));
        plotVao = new();
        plotVao.Assign(new VertexBuffer<float>(xcoords), Plot.X);
        plotBuffer = new(ycoords);
        plotVao.Assign(plotBuffer, Plot.Y);
        State.LineSmooth = true;
        State.Dither = false;
        Glfw.ShowWindow(Window);
        OnWindowFocusInternal(IntPtr.Zero, Glfw.GetWindowAttribute(Window, WindowAttribute.Focused));

        while (!Glfw.WindowShouldClose(Window)) {
            if (!Focused) {
                lastTicks = 0l;
                Glfw.WaitEvents();
                continue;
            }
            Render();
        }
    }

    private void Render () {
        var now = Timer.ElapsedTicks;
        var dt = lastTicks > 0l ? (float)((now - lastTicks) * TIMER_PERIOD) : 0f;
        lastTicks = now;
        if (Focused && CursorGrabbed)
            Camera.Move(10f * dt);

        Render(dt);
        if (plot)
            RenderPlot();
        UpdatePlotData();
        DelayForRetrace();
        Glfw.SwapBuffers(Window);
        lastSwapTicks = Timer.ElapsedTicks;
        ++FrameCount;
    }

    private void RenderPlot () {
        var (min, max) = Extrema(ycoords);
        State.DepthTest = false;
        glViewport(0, 0, _SAMPLES * 2, 200);
        State.VertexArray = plotVao;
        plotBuffer.BufferData(ycoords, _SAMPLES, 0, 0);
        State.Program = Plot.Id;
        var alpha = 2f / (max - min);
        Plot.A(alpha);
        Plot.B(1f - alpha * max);
        glDrawArrays(Primitive.LineStrip, 0, _SAMPLES);
    }

    private void UpdatePlotData () {
        var render = Timer.ElapsedTicks - lastTicks;
        Array.Copy(ycoords, 1, ycoords, 0, ycoords.Length - 1);
        ycoords[ycoords.Length - 1] = (float)(render * TIMER_PERIOD);
    }

    private void DelayForRetrace () {
        var delayed = lastSwapTicks + 3 * EXPECTED_TICKS_PER_FRAME / 4;

        if (Timer.ElapsedTicks < delayed)
            while (Timer.ElapsedTicks < delayed)
                Glfw.PollEvents();
        else {
            SetTitle($"delayed frame {FrameCount}");
            Glfw.PollEvents();
        }
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

    private bool plot;
    [KeyBinding(Keys.P)]
    protected void TogglePlot (Keys _, InputState state) {
        if (state == InputState.Release)
            plot = !plot;
    }

#pragma warning disable IDE0052 // Remove unread private members
    private KeyCallback onKey;
    private MouseCallback onCursorPosition;
    private MouseEnterCallback onCursorEnter;
    private MouseButtonCallback onMouseButton;
    private CharCallback onChar;
    private SizeCallback onFramebufferSize, onWindowSize;
    private WindowCallback onWindowRefresh, onClose;
    private MonitorCallback onMonitor;
    private IconifyCallback onWindowIconify;
    private PositionCallback onWindowPosition;
    private FocusCallback onWindowFocus;
    private WindowMaximizedCallback onWindowMaximize;
    private readonly DebugProc debugProc;
    [KeyBinding(GLFW.Keys.Tab)]
    protected void ToggleCursorGrabbed (Keys _, InputState state) {
        if (state == InputState.Release)
            CursorGrabbed = !CursorGrabbed;
    }
#pragma warning restore IDE0052 // Remove unread private members
    private void Assign () {
        onKey = OnKeyInternal;
        onCursorPosition = OnCursorPositionInternal;
        onCursorEnter = OnCursorEnterInternal;
        onMouseButton = OnMouseButtonInternal;
        onChar = OnChar;
        onFramebufferSize = OnFramebufferSizeInternal;
        onWindowRefresh = OnWindowRefresh;
        onMonitor = OnMonitor;
        onWindowIconify = OnWindowIconifyInternal;
        onClose = OnCloseInternal;
        onWindowPosition = OnWindowPosition;
        onWindowSize = OnWindowSize;
        onWindowFocus = OnWindowFocusInternal;
        onWindowMaximize = OnWindowMaximize;
        _ = Glfw.SetKeyCallback(Window, onKey);
        _ = Glfw.SetCursorPositionCallback(Window, onCursorPosition);
        _ = Glfw.SetCursorEnterCallback(Window, onCursorEnter);
        _ = Glfw.SetMouseButtonCallback(Window, onMouseButton);
        _ = Glfw.SetCharCallback(Window, onChar);
        _ = Glfw.SetFramebufferSizeCallback(Window, onFramebufferSize);
        _ = Glfw.SetWindowRefreshCallback(Window, onWindowRefresh);
        _ = Glfw.SetMonitorCallback(onMonitor);
        _ = Glfw.SetWindowIconifyCallback(Window, onWindowIconify);
        _ = Glfw.SetCloseCallback(Window, onClose);
        _ = Glfw.SetWindowPositionCallback(Window, onWindowPosition);
        _ = Glfw.SetWindowSizeCallback(Window, onWindowSize);
        _ = Glfw.SetWindowFocusCallback(Window, onWindowFocus);
        _ = Glfw.SetWindowMaximizeCallback(Window, onWindowMaximize);
    }

    private Vector2i lastMousePosition;
    protected virtual void OnCursorPosition (Vector2i delta) { }
    private void OnCursorPositionInternal (IntPtr _, double x, double y) {
        var delta = new Vector2i(Convert.ToInt32(x), Convert.ToInt32(y));
        var mouseDelta = delta - lastMousePosition;
        if (Focused && CursorGrabbed)
            Camera.Mouse(mouseDelta);
        lastMousePosition = delta;
        OnCursorPosition(delta);
    }

    protected virtual void OnCursorEnter (bool entering) { }
    private void OnCursorEnterInternal (IntPtr _, bool entering) => OnCursorEnter(entering);

    protected virtual void OnWindowIconify (bool iconified) { }
    private void OnWindowIconifyInternal (IntPtr _, bool iconified) {
        Iconified = iconified;
        OnWindowIconify(iconified);
    }

    protected virtual void OnWindowSize (IntPtr _, int width, int height) { }
    protected virtual void OnWindowRefresh (IntPtr _) { }
    protected virtual void OnMonitor (Monitor monitor, ConnectionStatus status) { }
    protected virtual void OnWindowPosition (IntPtr _, double x, double y) { }
    protected virtual void OnWindowFocus (bool focused) { }
    private void OnWindowFocusInternal (IntPtr _, bool focused) {
        Focused = focused;
        if (!focused)
            CursorGrabbed = false;
        OnWindowFocus(focused);
    }

    protected virtual void OnWindowMaximize (IntPtr _, bool maximized) { }
    protected virtual void OnFramebufferSize (int width, int height) { }
    private void OnFramebufferSizeInternal (IntPtr _, int width, int height) => OnFramebufferSize(Width = width, Height = height);

    protected virtual void OnMouseButton (MouseButton button, InputState state, ModifierKeys modifiers) { }
    private void OnMouseButtonInternal (IntPtr _, MouseButton button, InputState state, ModifierKeys modifiers) => OnMouseButton(button, state, modifiers);

    protected virtual void OnKey (Keys key, int code, InputState state, ModifierKeys modifier) { }
    private void OnKeyInternal (IntPtr _, Keys key, int code, InputState state, ModifierKeys modifier) {
        if (keys.TryGetValue(key, out var keyAction))
            keyAction(key, state);
        else
            OnKey(key, code, state, modifier);
    }

    [KeyBinding(Keys.Z, Keys.X, Keys.C, Keys.D, Keys.LeftShift, Keys.LeftControl)]
    protected void CameraKey (Keys key, InputState state) => _ = Camera.Key(key, state);

    protected virtual void OnChar (IntPtr _, uint code) { }
    protected virtual void OnClose () { }
    private void OnCloseInternal (IntPtr _) {
        Glfw.SetWindowShouldClose(Window, true);
        OnClose();
    }
    private const string _DEBSTR = "source: {0}\ntype: {1}\nseverity: {2}\nmessage: {3}";
    private void HandleDebug (int source, int type, int id, int severity, int length, string message, IntPtr _) => throw new ApplicationException(string.Format(_DEBSTR, source, type, severity, message));

    public void Dispose () => Dispose(true);

    private bool disposed;
    protected virtual void Dispose (bool disposing) {
        if (!disposed && Window != Window.None) {
            if (disposing) {
                Glfw.DestroyWindow(Window);
                Window = Window.None;
            }
            disposed = true;
        }
    }
}
