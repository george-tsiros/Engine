using System.Reflection;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using GLFW;
namespace Engine;

unsafe class Gl {
    public delegate void DebugProc (int source, int type, int id, int severity, int length, string message, IntPtr userParam);
    public static readonly delegate*<int, void> glActiveTexture;
    public static readonly delegate*<int, bool> glIsBuffer;
    public static readonly delegate*<Capability, bool> glIsEnabled;
    public static readonly delegate*<int, bool> glIsProgram;
    public static readonly delegate*<int, bool> glIsShader;
    public static readonly delegate*<int, bool> glIsTexture;
    public static readonly delegate*<int, bool> glIsVertexArray;
    public static readonly delegate*<int, FramebufferTarget, FramebufferStatus> glCheckNamedFramebufferStatus;
    public static readonly delegate*<int, byte*> glGetAttribLocation;
    public static readonly delegate*<int, byte*> glGetFragDataLocation;
    public static readonly delegate*<int, byte*> glGetUniformLocation;
    public static readonly delegate*<int> glCreateProgram;
    public static readonly delegate*<ShaderType, int> glCreateShader;
    public static readonly delegate*<int, int, void> glAttachShader;
    public static readonly delegate*<BufferTarget, int, void> glBindBuffer;
    public static readonly delegate*<int, int, void> glBindFramebuffer;
    public static readonly delegate*<int, int, void> glBindRenderbuffer;
    public static readonly delegate*<int, int, void> glBindTexture;
    public static readonly delegate*<int, int, void> glBindTextureUnit;
    public static readonly delegate*<int, void> glBindVertexArray;
    public static readonly delegate*<int, long, void*, int, void> glBufferData;
    public static readonly delegate*<int, long, long, void*, void> glBufferSubData;
    public static readonly delegate*<BufferBit, void> glClear;
    public static readonly delegate*<float, float, float, float, void> glClearColor;
    public static readonly delegate*<int, void> glCompileShader;
    public static readonly delegate*<int, int*, void> glCreateBuffers;
    public static readonly delegate*<int, int*, void> glCreateFramebuffers;
    public static readonly delegate*<int, int*, void> glCreateRenderbuffers;
    public static readonly delegate*<int, int*, void> glCreateVertexArrays;
    public static readonly delegate*<int, int*, void> glDeleteBuffers;
    public static readonly delegate*<int, int, int*, void> glCreateTextures;
    public static readonly delegate*<int, void> glCullFace;
    public static readonly delegate*<DebugProc, IntPtr> glDebugMessageCallback;
    public static readonly delegate*<int, void> glDeleteShader;
    public static readonly delegate*<int, void> glDeleteProgram;
    public static readonly delegate*<DepthFunc, void> glDepthFunc;
    public static readonly delegate*<Primitive, int, int, void> glDrawArrays;
    public static readonly delegate*<Primitive, long, void> glDrawArraysIndirect;
    public static readonly delegate*<Primitive, int, int, int, void> glDrawArraysInstanced;
    public static readonly delegate*<int, Attachment*> glDrawBuffers;
    public static readonly delegate*<Capability, void> glEnable;
    public static readonly delegate*<Capability, void> glDisable;
    public static readonly delegate*<int, int, void> glEnableVertexArrayAttrib;
    public static readonly delegate*<int, int, void> glDisableVertexArrayAttrib;
    public static readonly delegate*<void> glFlush;
    public static readonly delegate*<void> glFinish;
    public static readonly delegate*<bool, void> glDepthMask;
    public static readonly delegate*<int, int, int*, byte*, void> glGetProgramInfoLog;
    public static readonly delegate*<int, int, int*, byte*, void> glGetShaderInfoLog;
    public static readonly delegate*<int, int, int, int*, int*, UniformType*, byte*, void> glGetActiveUniform;
    public static readonly delegate*<int, int, int, int*, int*, AttribType*, byte*, void> glGetActiveAttrib;
    public static readonly delegate*<int, int*, void> glGetIntegerv;
    public static readonly delegate*<int, void> glLinkProgram;
    public static readonly delegate*<int, long, void*, int> glNamedBufferStorage;
    public static readonly delegate*<int, long, long, void*, void> glNamedBufferSubData;
    public static readonly delegate*<int, int, byte**, int*, void> glShaderSource;
    public static readonly delegate*<int, int, int, int, int, int, int, int, void*, void> glTexImage2D;
    public static readonly delegate*<int, int, int, int, int, int, int, int, void*, void> glTexSubImage2D;
    public static readonly delegate*<int, int, float, void> glTextureParameterf;
    public static readonly delegate*<int, int, int, void> glTextureParameteri;
    public static readonly delegate*<int, int, TextureInternalFormat, int, int, void> glTextureStorage2D;
    public static readonly delegate*<int, int, int, int, int, int, int, int, byte*, void> glTextureSubImage2D;
    public static readonly delegate*<int, int, void> glUniform1i;
    public static readonly delegate*<int, int, int, void> glUniform2i;
    public static readonly delegate*<int, float, float, void> glUniform2f;
    public static readonly delegate*<int, float, float, float, void> glUniform3f;
    public static readonly delegate*<int, float, float, float, float, void> glUniform4f;
    public static readonly delegate*<int, int, int*, void> glUniform2iv;
    public static readonly delegate*<int, int, float*, void> glUniform2fv;
    public static readonly delegate*<int, long, bool, System.Numerics.Matrix4x4, void> glUniformMatrix4fv;
    public static readonly delegate*<int, void> glUseProgram;
    public static readonly delegate*<int, int, void> glVertexAttribDivisor;
    public static readonly delegate*<int, int, AttribType, bool, int, long, void> glVertexAttribPointer;
    public static readonly delegate*<int, int, int, int, void> glViewport;
    static Gl () {
        foreach (var fi in typeof(Gl).GetFields(BindingFlags.Static | BindingFlags.Public)) {
            var p = Glfw.GetProcAddress(fi.Name);
            if (p == IntPtr.Zero)
                throw new ApplicationException($"{fi.Name} null");
            fi.SetValue(null, p);
        }
    }
}

class Engine {

    private static void GetConsts (string filepath, Dictionary<string, int> constants) {
        using var f = new StreamReader(filepath);
        var r = new Regex(@"^ {4}public const int ([A-Zx\d_]+) = 0x([A-F\d]+);$", RegexOptions.Compiled);
        while (!f.EndOfStream) {
            var line = f.ReadLine();
            if (string.IsNullOrWhiteSpace(line))
                continue;
            var m = r.Match(line);
            if (m.Success) {
                var n = m.Groups[1].Value;
                var i = int.Parse(m.Groups[2].Value, System.Globalization.NumberStyles.AllowHexSpecifier);
                constants.Add(n, i);
            }
        }
    }
    private static string FormatFilename (string filepath, string pattern) => string.Format(pattern, Path.GetFileNameWithoutExtension(filepath), Path.GetExtension(filepath));
    private static void ReplaceConsts (string filepath, Dictionary<string, int> constants) {
        var r = new Regex(@"^ {4}([a-zA-Z\d]+) = Const\.(\w+),$", RegexOptions.Compiled);
        using var output = new StreamWriter(FormatFilename(filepath, "{0}.txt"), false, System.Text.Encoding.ASCII);
        using var input = new StreamReader(filepath);
        while (!input.EndOfStream) {
            var line = input.ReadLine();
            var m = r.Match(line);
            if (m.Success && constants.TryGetValue(m.Groups[2].Value, out var value))
                line = $"    {m.Groups[1].Value} = 0x{value:X},";

            output.WriteLine(line);
        }
    }
    [STAThread]
    unsafe static void Main () {
        if (Glfw.Init()) {
            _ = Glfw.SetErrorCallback((e, m) => throw new ApplicationException(Marshal.PtrToStringAnsi(m) ?? "?"));


            Glfw.WindowHint(Hint.ContextVersionMajor, 4);
            Glfw.WindowHint(Hint.ContextVersionMinor, 6);
            Glfw.WindowHint(Hint.Doublebuffer, true);
            Glfw.WindowHint(Hint.OpenglDebugContext, true);
            Glfw.WindowHint(Hint.OpenglForwardCompatible, true);
            Glfw.WindowHint(Hint.Resizable, false);
            Glfw.WindowHint(Hint.Visible, true);
            Glfw.WindowHint(Hint.Focused, true);
            var window = Glfw.CreateWindow(640, 480, "", GLFW.Monitor.None, Window.None);
            Glfw.MakeContextCurrent(window);

            _ = Glfw.SetKeyCallback(window, (w, k, c, s, m) => Glfw.SetWindowShouldClose(window, true));
            while (!Glfw.WindowShouldClose(window)) {
                Glfw.PollEvents();
                Gl.glClearColor(0.1f, 0.1f, 0.1f, 1f);
                Gl.glClear(BufferBit.Color | BufferBit.Depth);
                Glfw.SwapBuffers(window);
            }

            Glfw.Terminate();
        }
        _ = Console.ReadLine();
    }
}
