namespace Gl;

using System;
using System.Text;
using System.Reflection;
using System.Numerics;
using System.Diagnostics;

unsafe public delegate void DebugProc (int sourceEnum, int typeEnum, int id, int severityEnum, int length, byte* message, void* userParam);
unsafe public static class Calls {
#pragma warning disable CS0649
    private static readonly delegate* unmanaged[Cdecl]<int, void> glActiveTexture;
    private static readonly delegate* unmanaged[Cdecl]<Capability, bool> glIsEnabled;
    private static readonly delegate* unmanaged[Cdecl]<int, byte*, int> glGetAttribLocation;
    private static readonly delegate* unmanaged[Cdecl]<int, byte*, int> glGetUniformLocation;
    private static readonly delegate* unmanaged[Cdecl]<int> glCreateProgram;
    private static readonly delegate* unmanaged[Cdecl]<ShaderType, int> glCreateShader;
    private static readonly delegate* unmanaged[Cdecl]<int, int, void> glAttachShader;
    private static readonly delegate* unmanaged[Cdecl]<BufferTarget, int, void> glBindBuffer;
    private static readonly delegate* unmanaged[Cdecl]<int, int, void> glBindFramebuffer;
    private static readonly delegate* unmanaged[Cdecl]<int, int, void> glBindTexture;
    private static readonly delegate* unmanaged[Cdecl]<int, void> glBindVertexArray;
    private static readonly delegate* unmanaged[Cdecl]<BufferBit, void> glClear;
    private static readonly delegate* unmanaged[Cdecl]<float, float, float, float, void> glClearColor;
    private static readonly delegate* unmanaged[Cdecl]<int, void> glCompileShader;
    private static readonly delegate* unmanaged[Cdecl]<int, int*, void> glCreateBuffers;
    private static readonly delegate* unmanaged[Cdecl]<int, int*, void> glCreateVertexArrays;
    private static readonly delegate* unmanaged[Cdecl]<int, int*, void> glDeleteBuffers;
    private static readonly delegate* unmanaged[Cdecl]<int, int*, void> glDeleteTextures;
    private static readonly delegate* unmanaged[Cdecl]<int, int*, void> glDeleteVertexArrays;
    private static readonly delegate* unmanaged[Cdecl]<int, int, int*, void> glCreateTextures;
    private static readonly delegate* unmanaged[Cdecl]<DebugProc, IntPtr, void> glDebugMessageCallback;
    private static readonly delegate* unmanaged[Cdecl]<int, void> glDeleteShader;
    private static readonly delegate* unmanaged[Cdecl]<int, void> glDeleteProgram;
    private static readonly delegate* unmanaged[Cdecl]<DepthFunction, void> glDepthFunc;
    private static readonly delegate* unmanaged[Cdecl]<Primitive, int, int, void> glDrawArrays;
    private static readonly delegate* unmanaged[Cdecl]<Primitive, int, int, int, void> glDrawArraysInstanced;
    private static readonly delegate* unmanaged[Cdecl]<Capability, void> glEnable;
    private static readonly delegate* unmanaged[Cdecl]<Capability, void> glDisable;
    private static readonly delegate* unmanaged[Cdecl]<int, int, void> glEnableVertexArrayAttrib;
    private static readonly delegate* unmanaged[Cdecl]<bool, void> glDepthMask;
    private static readonly delegate* unmanaged[Cdecl]<int, ProgramParameter, int*, void> glGetProgramiv;
    private static readonly delegate* unmanaged[Cdecl]<int, ShaderParameter, int*, void> glGetShaderiv;
    private static readonly delegate* unmanaged[Cdecl]<int, int, int*, byte*, void> glGetProgramInfoLog;
    private static readonly delegate* unmanaged[Cdecl]<int, int, int*, byte*, void> glGetShaderInfoLog;
    private static readonly delegate* unmanaged[Cdecl]<int, int, int, int*, int*, UniformType*, byte*, void> glGetActiveUniform;
    private static readonly delegate* unmanaged[Cdecl]<int, int, int, int*, int*, AttribType*, byte*, void> glGetActiveAttrib;
    private static readonly delegate* unmanaged[Cdecl]<int, int*, void> glGetIntegerv;
    private static readonly delegate* unmanaged[Cdecl]<int, void> glLinkProgram;
    private static readonly delegate* unmanaged[Cdecl]<int, long, void*, int, void> glNamedBufferStorage;
    private static readonly delegate* unmanaged[Cdecl]<int, long, long, void*, void> glNamedBufferSubData;
    private static readonly delegate* unmanaged[Cdecl]<int, int, byte**, int*, void> glShaderSource;
    private static readonly delegate* unmanaged[Cdecl]<int, int, int, void> glTextureParameteri;
    private static readonly delegate* unmanaged[Cdecl]<int, int, TextureInternalFormat, int, int, void> glTextureStorage2D;
    private static readonly delegate* unmanaged[Cdecl]<int, int, int, int, int, int, int, int, void*, void> glTextureSubImage2D;
    private static readonly delegate* unmanaged[Cdecl]<int, int, void> glUniform1i;
    private static readonly delegate* unmanaged[Cdecl]<int, float, void> glUniform1f;
    private static readonly delegate* unmanaged[Cdecl]<int, float, float, void> glUniform2f;
    private static readonly delegate* unmanaged[Cdecl]<int, float, float, float, float, void> glUniform4f;
    private static readonly delegate* unmanaged[Cdecl]<int, long, bool, Matrix4x4, void> glUniformMatrix4fv;
    private static readonly delegate* unmanaged[Cdecl]<int, void> glUseProgram;
    private static readonly delegate* unmanaged[Cdecl]<int, int, void> glVertexAttribDivisor;
    private static readonly delegate* unmanaged[Cdecl]<int, int, AttribType, bool, int, long, void> glVertexAttribPointer;
    private static readonly delegate* unmanaged[Cdecl]<int, int, int, int, void> glViewport;
    private static readonly delegate* unmanaged[Cdecl]<void> glFlush;
    private static readonly delegate* unmanaged[Cdecl]<void> glFinish;
    private static readonly delegate* unmanaged[Cdecl]<int, int, void> glBlendFunc;

    private static readonly delegate* unmanaged[Cdecl]<int, bool> glIsBuffer;
    private static readonly delegate* unmanaged[Cdecl]<int, bool> glIsProgram;
    private static readonly delegate* unmanaged[Cdecl]<int, bool> glIsShader;
    private static readonly delegate* unmanaged[Cdecl]<int, bool> glIsTexture;
    private static readonly delegate* unmanaged[Cdecl]<int, bool> glIsVertexArray;
    private static readonly delegate* unmanaged[Cdecl]<int, FramebufferTarget, FramebufferStatus> glCheckNamedFramebufferStatus;
    private static readonly delegate* unmanaged[Cdecl]<int, byte*, int> glGetFragDataLocation;
    private static readonly delegate* unmanaged[Cdecl]<int, int, void> glBindRenderbuffer;
    private static readonly delegate* unmanaged[Cdecl]<int, int, void> glBindTextureUnit;
    private static readonly delegate* unmanaged[Cdecl]<int, long, void*, int, void> glBufferData;
    private static readonly delegate* unmanaged[Cdecl]<int, long, long, void*, void> glBufferSubData;
    private static readonly delegate* unmanaged[Cdecl]<int, int*, void> glCreateFramebuffers;
    private static readonly delegate* unmanaged[Cdecl]<int, int*, void> glCreateRenderbuffers;
    private static readonly delegate* unmanaged[Cdecl]<int, int*, void> glDeleteFramebuffers;
    private static readonly delegate* unmanaged[Cdecl]<int, void> glCullFace;
    private static readonly delegate* unmanaged[Cdecl]<Primitive, long, void> glDrawArraysIndirect;
    private static readonly delegate* unmanaged[Cdecl]<int, Attachment*, void> glDrawBuffers;
    private static readonly delegate* unmanaged[Cdecl]<int, int, void> glDisableVertexArrayAttrib;
    private static readonly delegate* unmanaged[Cdecl]<int, int, int, int, int, int, int, int, void*, void> glTexImage2D;
    private static readonly delegate* unmanaged[Cdecl]<int, int, int, int, int, int, int, int, void*, void> glTexSubImage2D;
    private static readonly delegate* unmanaged[Cdecl]<int, int, float, void> glTextureParameterf;
    private static readonly delegate* unmanaged[Cdecl]<int, int, int, void> glUniform2i;
    private static readonly delegate* unmanaged[Cdecl]<int, float, float, float, void> glUniform3f;
    private static readonly delegate* unmanaged[Cdecl]<int, int, int*, void> glUniform1iv;
    private static readonly delegate* unmanaged[Cdecl]<int, int, float*, void> glUniform1fv;
    private static readonly delegate* unmanaged[Cdecl]<int, int, int*, void> glUniform2iv;
    private static readonly delegate* unmanaged[Cdecl]<int, int, float*, void> glUniform2fv;
    private static readonly delegate* unmanaged[Cdecl]<int, TextureParameter, float*, void> glGetTextureParameterfv;
    private static readonly delegate* unmanaged[Cdecl]<int, TextureParameter, int*, void> glGetTextureParameteriv;
#pragma warning restore CS0649

    public static bool IsEnabled (Capability cap) => glIsEnabled(cap);
    public static void ActiveTexture (int i) => glActiveTexture(i);
    public static void AttachShader (int p, int s) => glAttachShader(p, s);
    public static void BindBuffer (BufferTarget target, int buffer) => glBindBuffer(target, buffer);
    public static void BindFramebuffer (int target, int buffer) => glBindFramebuffer((int)target, buffer);
    public static void BindTexture (int target, int texture) => glBindTexture(target, texture);
    public static void BindVertexArray (int vao) => glBindVertexArray(vao);
    public static void BlendFunc (BlendSourceFactor sfactor, BlendDestinationFactor dfactor) => glBlendFunc((int)sfactor, (int)dfactor);
    public static void Clear (BufferBit mask) => glClear(mask);
    public static void ClearColor (float r, float g, float b, float a) => glClearColor(r, g, b, a);
    public static void CompileShader (int s) => glCompileShader(s);
    public static int CreateProgram () => glCreateProgram();
    public static int CreateShader (ShaderType shaderType) => glCreateShader(shaderType);
    public static void DebugMessageCallback (DebugProc proc, IntPtr userParam) => glDebugMessageCallback(proc, userParam);
    public static void DeleteBuffer (int i) => glDeleteBuffers(1, &i);
    public static void DeleteProgram (int program) => glDeleteProgram(program);
    public static void DeleteShader (int shader) => glDeleteShader(shader);
    public static void DeleteTexture (int texture) => glDeleteTextures(1, &texture);
    public static void DeleteVertexArray (int vao) => glDeleteVertexArrays(1, &vao);
    public static void DepthFunc (DepthFunction f) => glDepthFunc(f);
    public static void DepthMask (bool enabled) => glDepthMask(enabled);
    public static void Disable (Capability c) => glDisable(c);
    public static void DrawArrays (Primitive mode, int first, int count) => glDrawArrays(mode, first, count);
    public static void DrawArraysInstanced (Primitive mode, int firstIndex, int indicesPerInstance, int instancesCount) => glDrawArraysInstanced(mode, firstIndex, indicesPerInstance, instancesCount);
    public static void Enable (Capability c) => glEnable(c);
    public static void EnableVertexArrayAttrib (int id, int i) => glEnableVertexArrayAttrib(id, i);
    public static void LinkProgram (int p) => glLinkProgram(p);
    public static void NamedBufferStorage (int buffer, long size, IntPtr data, int flags) => glNamedBufferStorage(buffer, size, data.ToPointer(), flags);
    public static void NamedBufferSubData (int buffer, long offset, long size, void* data) => glNamedBufferSubData(buffer, offset, size, data);
    public static void TextureBaseLevel (int texture, int level) => glTextureParameteri(texture, Const.TEXTURE_BASE_LEVEL, level);
    public static void TextureFilter (int texture, MagFilter filter) => glTextureParameteri(texture, Const.TEXTURE_MAG_FILTER, (int)filter);
    public static void TextureFilter (int texture, MinFilter filter) => glTextureParameteri(texture, Const.TEXTURE_MIN_FILTER, (int)filter);
    public static void TextureMaxLevel (int texture, int level) => glTextureParameteri(texture, Const.TEXTURE_MAX_LEVEL, level);
    public static void TextureStorage2D (int texture, int levels, TextureInternalFormat sizedFormat, int width, int height) => glTextureStorage2D(texture, levels, sizedFormat, width, height);
    public static void TextureSubImage2D (int texture, int level, int xOffset, int yOffset, int width, int height, TextureFormat format, int type, void* pixels) => glTextureSubImage2D(texture, level, xOffset, yOffset, width, height, (int)format, type, pixels);
    public static void TextureWrap (int texture, WrapCoordinate c, Wrap w) => glTextureParameteri(texture, (int)c, (int)w);
    public static void Uniform (int uniform, float f) => glUniform1f(uniform, f);
    public static void Uniform (int uniform, int i) => glUniform1i(uniform, i);
    public static void Uniform (int uniform, Matrix4x4 m) => glUniformMatrix4fv(uniform, 1, false, m);
    public static void Uniform (int uniform, Vector2 v) => glUniform2f(uniform, v.X, v.Y);
    public static void Uniform (int uniform, Vector4 v) => glUniform4f(uniform, v.X, v.Y, v.Z, v.W);
    public static void UseProgram (int p) => glUseProgram(p);
    public static void VertexAttribDivisor (int index, int divisor) => glVertexAttribDivisor(index, divisor);
    public static void VertexAttribPointer (int index, int size, AttribType type, bool normalized, int stride, long ptr) => glVertexAttribPointer(index, size, type, normalized, stride, ptr);
    public static void Viewport (Vector2i position, Vector2i size) => glViewport(position.X, position.Y, size.X, size.Y);
    public static void Flush () => glFlush();
    public static void Finish () => glFinish();

    public static int GetAttribLocation (int program, string name) => GetLocation(program, name, glGetAttribLocation);
    public static int GetUniformLocation (int program, string name) => GetLocation(program, name, glGetUniformLocation);

    private static int GetLocation (int program, string name, delegate* unmanaged[Cdecl]<int, byte*, int> f) {
        Span<byte> bytes = stackalloc byte[name.Length + 1];
        var l = Encoding.ASCII.GetBytes(name, bytes);
        Debug.Assert(l == name.Length);
        bytes[name.Length] = 0;
        fixed (byte* p = bytes)
            return f(program, p);
    }

    public static int CreateBuffer () => Create(glCreateBuffers);
    public static int CreateVertexArray () => Create(glCreateVertexArrays);

    private static int Create (delegate* unmanaged[Cdecl]<int, int*, void> f) {
        int i;
        f(1, &i);
        return i;
    }

    public static void ShaderSource (int id, string source) {
        var bytes = new byte[source.Length + 1];
        var l = Encoding.ASCII.GetBytes(source, bytes);
        Debug.Assert(source.Length == l);
        bytes[source.Length] = 0;
        fixed (byte* strPtr = bytes)
            glShaderSource(id, 1, &strPtr, null);
    }

    public static int GetProgram (int id, ProgramParameter p) { // I wish I could join this with GetShader
        int i;
        glGetProgramiv(id, p, &i);
        return i;
    }

    public static int GetShader (int id, ShaderParameter p) {
        int i;
        glGetShaderiv(id, p, &i);
        return i;
    }

    public static (int size, AttribType type, string name) GetActiveAttrib (int id, int index) {
        var maxLength = GetProgram(id, ProgramParameter.ActiveAttributeMaxLength);
        int length, size;
        AttribType type;
        Span<byte> bytes = stackalloc byte[maxLength];
        fixed (byte* p = bytes)
            glGetActiveAttrib(id, index, maxLength, &length, &size, &type, p);
        var n = length > 0 ? Encoding.ASCII.GetString(bytes.Slice(0, length)) : "";
        return (size, type, n);
    }

    public static (int size, UniformType type, string name) GetActiveUniform (int id, int index) {
        var maxLength = GetProgram(id, ProgramParameter.ActiveUniformMaxLength);
        int length, size;
        UniformType type;
        Span<byte> bytes = stackalloc byte[maxLength];
        fixed (byte* p = bytes)
            glGetActiveUniform(id, index, maxLength, &length, &size, &type, p);
        var n = length > 0 ? Encoding.ASCII.GetString(bytes.Slice(0, length)) : "";
        return (size, type, n);
    }

    public static string GetProgramInfoLog (int id) {
        int actualLogLength = GetProgram(id, ProgramParameter.InfoLogLength);
        var bufferLength = Math.Min(1024, actualLogLength);
        Span<byte> bytes = stackalloc byte[bufferLength];
        fixed (byte* p = bytes)
            glGetProgramInfoLog(id, bufferLength, null, p);
        return Encoding.ASCII.GetString(bytes);
    }

    public static string GetShaderInfoLog (int id) {
        int actualLogLength = GetShader(id, ShaderParameter.InfoLogLength);
        var bufferLength = Math.Min(1024, actualLogLength);
        Span<byte> bytes = stackalloc byte[bufferLength];
        fixed (byte* p = bytes)
            glGetShaderInfoLog(id, bufferLength, null, p);
        return Encoding.ASCII.GetString(bytes);
    }

    unsafe public static int GetIntegerv (IntParameter p) {
        int i;
        glGetIntegerv((int)p, &i);
        return i;
    }

    public static int CreateTexture2D () {
        int i;
        glCreateTextures(Const.TEXTURE_2D, 1, &i);
        return i;
    }

    private static IntPtr GetProcAddress (string name) {
        var ptr = GLFW.Glfw.GetProcAddress(name);
        return ptr != IntPtr.Zero ? ptr : throw new ApplicationException($"failed to get proc address of {name}");
    }

    static Calls () {
        foreach (var f in typeof(Calls).GetFields(BindingFlags.NonPublic | BindingFlags.Static))
            if ((IntPtr)f.GetValue(null) == IntPtr.Zero)
                f.SetValue(null, GetProcAddress(f.Name));
            else
                throw new ApplicationException($"{f.Name} already set");
    }
}
