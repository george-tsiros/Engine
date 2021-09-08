namespace Engine6;

using System;
using System.Numerics;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using GLFW;
using System.Reflection;

unsafe public delegate void glGetProgramInfoLog (int id, int bufferSize, ref int length, byte* log);
unsafe public delegate void glGetShaderInfoLog (int id, int bufferSize, ref int length, byte* log);
delegate void glEnable (Capability c);
delegate void glDisable (Capability c);
delegate bool glIsEnabled (Capability c);
delegate void glClearColor (float r, float g, float b, float a);
delegate void glClear (BufferBit b);
delegate int glCreateShader (Shader s);
delegate int glCreateProgram ();
delegate void glCompileShader (int i);
delegate void glLinkProgram (int p);
delegate void glNamedBufferStorage (int buffer, long size, IntPtr data, int flags);
delegate void glDebugMessageCallback (DebugProc proc, IntPtr userParam);
delegate void glShaderSource (int id, int count, [MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.LPStr)] string[] sources, int[] length);
delegate void DebugProc (int source, int type, int id, int severity, int length, string message, IntPtr userParam);
delegate void glGetShaderiv (int id, ShaderParameter parameter, out int result);
delegate void glGetProgramiv (int id, ProgramParameter parameter, out int result);
delegate void glAttachShader (int program, int shader);
delegate void glCreateVertexArrays (int c, int[] vaos);
delegate void glCreateBuffers (int c, int[] vaos);
delegate void glBindVertexArray (int v);
delegate void glUseProgram (int p);
delegate void glBindBuffer (BufferTarget t, int b);
delegate int glGetUniformLocation (int id, [MarshalAs(UnmanagedType.LPStr)] string str);
delegate int glGetFragDataLocation (int id, [MarshalAs(UnmanagedType.LPStr)] string str);
delegate int glGetAttribLocation (int id, [MarshalAs(UnmanagedType.LPStr)] string str);
delegate void glVertexAttribPointer (int id, int idx, AttribType type, bool norm, int stride, IntPtr offset);
unsafe delegate void glNamedBufferSubData (int buffer, long offset, long size, void* data);
delegate void glUniformMatrix4fv (int location, long count, bool transpose, Matrix4x4 matrix);
delegate void glUniform4f (int location, float x, float y, float z, float w);
delegate void glEnableVertexArrayAttrib (int i, int j);
delegate void glViewport (int x, int y, int w, int h);
delegate void glDrawArrays (Primitive p, int start, int count);
delegate void glDepthMask (bool b);
class Engine6 {
#pragma warning disable IDE0044 // Add readonly modifier
    static glEnable Enable;
    static glDisable Disable;
    static glIsEnabled IsEnabled;
    static glClear Clear;
    static glClearColor ClearColor;
    static glCreateShader CreateShader;
    static glCompileShader CompileShader;
    static glCreateProgram CreateProgram;
    static glLinkProgram LinkProgram;
    static glNamedBufferStorage NamedBufferStorage;
    static glDebugMessageCallback DebugMessageCallback;
    static glShaderSource ShaderSource;
    static glGetShaderiv GetShaderiv;
    static glGetProgramiv GetProgramiv;
    static glAttachShader AttachShader;
    static glUseProgram UseProgram;
    static glCreateVertexArrays CreateVertexArrays;
    static glCreateBuffers CreateBuffers;
    static glBindVertexArray BindVertexArray;
    static glBindBuffer BindBuffer;
    static glGetAttribLocation GetAttribLocation;
    static glGetUniformLocation GetUniformLocation;
    static glGetFragDataLocation GetFragDataLocation;
    static glNamedBufferSubData NamedBufferSubData;
    static glEnableVertexArrayAttrib EnableVertexArrayAttrib;
    static glVertexAttribPointer VertexAttribPointer;
    static glUniformMatrix4fv UniformMatrix4fv;
    static glUniform4f Uniform4f;
    static glViewport Viewport;
    static glDrawArrays DrawArrays;
    static glDepthMask DepthMask;
#pragma warning restore IDE0044 // Add readonly modifier

    private const string _VS = @"#version 460 core
#pragma debug(on)
in vec4 vertexPosition;
uniform mat4 model, view, projection;
void main () {
    gl_Position = projection * view * model * vertexPosition; 
}";

    private const string _FS = @"#version 460 core
#pragma debug(on)
uniform vec4 color;
out vec4 out0;
void main () { 
    out0 = color;
}";

    public Window Window { get; private set; }
    private const int _WIDTH = 1280, _HEIGHT = 720;
    private const int _DESIRED_FRAMERATE = 140;
    private const BindingFlags _PRIVATE_STATIC = BindingFlags.Static | BindingFlags.NonPublic;
    private static readonly double TIMER_PERIOD = 1.0 / Stopwatch.Frequency;
    private static readonly long EXPECTED_TICKS_PER_FRAME = Stopwatch.Frequency / _DESIRED_FRAMERATE;
    private static IntPtr GetProcAddress (string name) {
        var p = Glfw.GetProcAddress(name);
        return p != IntPtr.Zero ? p : throw new ApplicationException();
    }
    private static Delegate GetDelegateFromType (Type t) {
        var x = Marshal.GetDelegateForFunctionPointer(GetProcAddress(t.Name), t);
        return x ?? throw new ApplicationException();
    }
    private bool isFocused;
    private readonly Stopwatch timer = Stopwatch.StartNew();
    public Engine6 () {
        if (File.Exists("hints.txt"))
            ReadHints("hints.txt");
        Glfw.WindowHint(Hint.ContextVersionMajor, 4);
        Glfw.WindowHint(Hint.ContextVersionMinor, 6);
        Glfw.WindowHint(Hint.OpenglProfile, Profile.Core);
        Glfw.WindowHint(Hint.DepthBits, 24);
        Glfw.WindowHint(Hint.OpenglForwardCompatible, true);
        Glfw.WindowHint(Hint.OpenglDebugContext, true);
        Glfw.WindowHint(Hint.Resizable, false);
        Glfw.WindowHint(Hint.FocusOnShow, true);

        _ = Glfw.SetErrorCallback(HANDLE_ERROR = HandleError);
        Window = Glfw.CreateWindow(_WIDTH, _HEIGHT, string.Empty, Monitor.None, Window.None);
        Glfw.MakeContextCurrent(Window);
        foreach (var fi in Array.FindAll(GetType().GetFields(_PRIVATE_STATIC), f => f.FieldType.Name == "gl" + f.Name))
            fi.SetValue(null, GetDelegateFromType(fi.FieldType));
        DebugMessageCallback(debugProc = HandleDebug, IntPtr.Zero);

        Disable(Capability.Dither);
        DepthMask(true);
        Enable(Capability.DebugOutput);
        Enable(Capability.DepthTest);
        Enable(Capability.CullFace);
    }

    private static ErrorCallback HANDLE_ERROR;
    private static void HandleError (ErrorCode code, IntPtr message) => throw new ApplicationException(Marshal.PtrToStringAnsi(message) ?? "?");

    private static Vector4[] Translate (Vector4[] vectors, Vector3 translation) => Transform(vectors, Matrix4x4.CreateTranslation(translation));
    private static Vector4[] Transform (Vector4[] vectors, Matrix4x4 transformation) => Array.ConvertAll(vectors, v => Vector4.Transform(v, transformation));

    unsafe public void Run () {
        var lastSwapTicks = 0l;
        var framecount = 0ul;
        ClearColor(0.1f, 0.1f, 0.1f, 1f);
        _ = Glfw.SetWindowFocusCallback(Window, onWindowFocusInternal = OnWindowFocusInternal);
        isFocused = Glfw.GetWindowAttribute(Window, WindowAttribute.Focused);

        var p = CreateProgram();
        var vs = CreateShader(Shader.Vertex);
        ShaderSource(vs, 1, new string[] { _VS }, null);
        CompileShader(vs);
        GetShaderiv(vs, ShaderParameter.InfoLogLength, out var logLength);
        AttachShader(p, vs);
        var fs = CreateShader(Shader.Fragment);
        ShaderSource(fs, 1, new string[] { _FS }, null);
        CompileShader(fs);
        GetShaderiv(vs, ShaderParameter.InfoLogLength, out logLength);
        AttachShader(p, fs);
        LinkProgram(p);
        GetProgramiv(p, ProgramParameter.InfoLogLength, out logLength);
        UseProgram(p);
        var ints = new int[1];
        CreateVertexArrays(1, ints);
        var vao = ints[0];
        BindVertexArray(vao);
        CreateBuffers(1, ints);
        var vertexPositionBuffer = ints[0];
        BindBuffer(BufferTarget.Array, vertexPositionBuffer);
        var quad = new Vector4[] { new(.5f, .5f, 0f, 1f), new(-.5f, .5f, 0f, 1f), new(-.5f, -.5f, 0f, 1f), new(-.5f, -.5f, 0f, 1f), new(.5f, -.5f, 0f, 1f), new(.5f, .5f, 0f, 1f) };
        var left = Translate(quad, new(-.25f, 0f, .25f));
        var right = Translate(quad, new(.25f, 0f, -.25f));
        var quads = new Vector4[12];
        Array.Copy(left, 0, quads, 0, 6);
        Array.Copy(right, 0, quads, 6, 6);
        var dataSize = Marshal.SizeOf<Vector4>() * quads.Length;
        NamedBufferStorage(vertexPositionBuffer, dataSize, IntPtr.Zero, 0x0100);
        fixed (Vector4* ptr = quads)
            NamedBufferSubData(vertexPositionBuffer, 0l, dataSize, ptr);

        var vertexPositionLocation = GetAttribLocation(p, "vertexPosition");
        EnableVertexArrayAttrib(vao, vertexPositionLocation);
        VertexAttribPointer(vertexPositionLocation, 4, AttribType.Float, false, 0, IntPtr.Zero);
        var modelLocation = GetUniformLocation(p, "model");
        var viewLocation = GetUniformLocation(p, "view");
        var projectionLocation = GetUniformLocation(p, "projection");
        var colorLocation = GetUniformLocation(p, "color");

        UniformMatrix4fv(modelLocation, 1, false, Matrix4x4.Identity);
        UniformMatrix4fv(viewLocation, 1, false, Matrix4x4.CreateTranslation(0f, 0f, -2f));
        UniformMatrix4fv(projectionLocation, 1, false, Matrix4x4.CreatePerspectiveFieldOfView((float)(Math.PI / 3), (float)_WIDTH / _HEIGHT, 0.1f, 10f));
        _ = Glfw.SetKeyCallback(Window, onKey = new KeyCallback(OnKey));
        while (!Glfw.WindowShouldClose(Window)) {
            if (!isFocused) {
                Glfw.WaitEvents();
                continue;
            }
            Viewport(0, 0, _WIDTH, _HEIGHT);
            Clear(BufferBit.Color | BufferBit.Depth);

            Uniform4f(colorLocation, .25f, .75f, .25f, 1f);
            DrawArrays(Primitive.Triangles, 0, 6);
            Uniform4f(colorLocation, .25f, .25f, .75f, 1f);
            DrawArrays(Primitive.Triangles, 6, 6);

            var delay = lastSwapTicks + 3 * EXPECTED_TICKS_PER_FRAME / 4;
            if (delay > timer.ElapsedTicks)
                DelayForRetrace(delay);
            else
                Trace($"delayed frame {framecount}");

            Glfw.SwapBuffers(Window);
            lastSwapTicks = timer.ElapsedTicks;
            ++framecount;

        }
        Glfw.DestroyWindow(Window);
    }
    private KeyCallback onKey;
    private void OnKey (IntPtr _, Keys key, int code, InputState state, ModifierKeys modifier) {
        if (state != InputState.Release)
            return;
        switch (key) {
            case Keys.F1:
                var current = IsEnabled(Capability.DepthTest);
                if (current)
                    Disable(Capability.DepthTest);
                else
                    Enable(Capability.DepthTest);
                Debug.Assert(current != IsEnabled(Capability.DepthTest));
                Glfw.SetWindowTitle(Window, IsEnabled(Capability.DepthTest) ? "DepthTest: enabled" : "DepthTest: disabled");
                break;
            case Keys.Escape:
                Glfw.SetWindowShouldClose(Window, true);
                break;
            default:
                return;
        }
        AssertNoglError();
    }
    private static void AssertNoglError () {
        var e = Glfw.GetError(out var message);
        Debug.Assert(e == ErrorCode.None, message);
    }

    private void DelayForRetrace (long delay) {
        while (timer.ElapsedTicks < delay)
            Glfw.PollEvents();
    }

    private FocusCallback onWindowFocusInternal;
    private void OnWindowFocusInternal (IntPtr _, bool focused) {
        isFocused = focused;
    }
    [STAThread]
    static void Main () {
        var e = new Engine6();
        e.Run();
    }

    public static void Trace (params object[] args) => Debug.WriteLine($"{DateTime.Now:mm:ss.fff}> {new StackFrame(1).GetMethod().Name} {string.Join(", ", args)}");
    private static void ReadHints (string filepath) {
        static void SetHint (Hint hint, bool enable) {
            Trace($"{hint}: {enable}");
            Glfw.WindowHint(hint, enable);
        }
        var hintRegex = new Regex(@"^ *(\w+) *= *(true|false) *$");
        foreach (var line in File.ReadAllLines(filepath)) {
            if (hintRegex.TryMatch(line, out var m) && Enum.TryParse<Hint>(m.Groups[1].Value, out var hint))
                SetHint(hint, m.Groups[2].Value == "true");
        }
    }
    private DebugProc debugProc;
    private void HandleDebug (int source, int type, int id, int severity, int length, string message, IntPtr _) {
        Debug.WriteLine($"Source: {source}\nType: {type}\nSeverity: {severity}\nmessage: {message}");
        throw new ApplicationException(message);
    }
    private static void MaybeToggle (Capability cap, bool requested) {
        var previous = IsEnabled(cap);
        if (requested != previous) {
            Debug.WriteLine($"{cap} is currently {previous}, requested {requested}");
            if (requested)
                Enable(cap);
            else
                Disable(cap);
            var current = IsEnabled(cap);
            Debug.WriteLineIf(current != requested, $"{cap} is still {current} instead of {requested}");
        }
        else {
            Debug.WriteLine($"{cap} is already {previous}");
        }
    }
}

public static class Extensions {
    public static bool TryMatch (this Regex self, string input, out Match match) {
        var m = self.Match(input);
        match = m.Success ? m : null;
        return m.Success;
    }
}
public enum Shader {
    Fragment = 0x8B30,
    Vertex = 0x8B31
}
[Flags]
public enum BufferBit {
    Color = 0x04000,
    Depth = 0x0100,
    Stencil = 0x0400,
}
public enum Capability {
    Dither = 0x0BD0,
    CullFace = 0x0B44,
    DebugOutput = 0x92E0,
    DepthTest = 0x0B71,
    LineSmooth = 0x0B20,
}
public enum ShaderParameter {
    ShaderType = 0x8B4F,
    DeleteStatus = 0x8B80,
    CompileStatus = 0x8B81,
    InfoLogLength = 0x8B84,
    ShaderSourceLength = 0x8B88,
}
enum ProgramParameter {
    DeleteStatus = 0x8B80,
    LinkStatus = 0x8B82,
    ValidateStatus = 0x8B83,
    InfoLogLength = 0x8B84,
    AttachedShaders = 0x8B85,
    ActiveAtomicCounterBuffers = 0x92D9,
    ActiveAttributes = 0x8B89,
    ActiveAttributeMaxLength = 0x8B8A,
    ActiveUniforms = 0x8B86,
    ActiveUniformBlocks = 0x8A36,
    ActiveUniformBlockMaxNameLength = 0x8A35,
    ActiveUniformMaxLength = 0x8B87,
    TransformFeedbackBufferMode = 0x8C7F,
    TransformFeedbackVaryings = 0x8C83,
    TransformFeedbackVaryingMaxLength = 0x8C76,
    GeometryVerticesOut = 0x8916,
    GeometryInputType = 0x8917,
    GeometryOutputType = 0x8918,
}
public enum AttribType {
    Byte = 0x1400,
    UByte = 0x1401,
    Short = 0x1402,
    UShort = 0x1403,
    Int = 0x1404,
    UInt = 0x1405,
    Float = 0x1406,
    Double = 0x140A,
}
public enum Primitive {
    Triangles = 0x4,
}
public enum BufferTarget {
    Array = 0x8892,
    //AtomicCounter = Const.ATOMIC_COUNTER_BUFFER,
    //CopyRead = Const.COPY_READ_BUFFER,
    //CopyWrite = Const.COPY_WRITE_BUFFER,
    //DispatchIndirect = Const.DISPATCH_INDIRECT_BUFFER,
    //DrawIndirect = Const.DRAW_INDIRECT_BUFFER,
    //ElementArray = Const.ELEMENT_ARRAY_BUFFER,
    //PixelPack = Const.PIXEL_PACK_BUFFER,
    //PixelUnpack = Const.PIXEL_UNPACK_BUFFER,
    //Query = Const.QUERY_BUFFER,
    //ShaderStorage = Const.SHADER_STORAGE_BUFFER,
    //Texture = Const.TEXTURE_BUFFER,
    //TransformFeedback_Buffer = Const.TRANSFORM_FEEDBACK_BUFFER,
    //Uniform = Const.UNIFORM_BUFFER,
}

