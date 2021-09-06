//#define __USE_FUNCTION_POINTERS
namespace Gl {
    using System;
    using System.Reflection;
    using System.Numerics;
    using System.Runtime.InteropServices;
    using System.Collections.Generic;
    public static class Calls {
#if __USE_FUNCTION_POINTERS
#else
#endif
#if __USE_FUNCTION_POINTERS
        public static readonly delegate*<int, void> glActiveTexture;
#else
#pragma warning disable CS0649
        public static readonly glActiveTexture glActiveTexture;
        public static readonly glAttachShader glAttachShader;
        public static readonly glBindBuffer glBindBuffer;
        public static readonly glBindFramebuffer glBindFramebuffer;
        public static readonly glBindRenderbuffer glBindRenderbuffer;
        public static readonly glBindTexture glBindTexture;
        public static readonly glBindTextureUnit glBindTextureUnit;
        public static readonly glBindVertexArray glBindVertexArray;
        public static readonly glBufferData glBufferData;
        public static readonly glBufferSubData glBufferSubData;
        public static readonly glCheckNamedFramebufferStatus glCheckNamedFramebufferStatus;
        public static readonly glClear glClear;
        public static readonly glClearColor glClearColor;
        public static readonly glCompileShader glCompileShader;
        private static readonly glCountedArray glCreateBuffers;
        private static readonly glCountedArray glCreateFramebuffers;
        private static readonly glCountedArray glCreateRenderbuffers;
        private static readonly glCountedArray glCreateVertexArrays;
        private static readonly glDeleteArray glDeleteBuffers;
        private static readonly glDeleteArray glDeleteTextures;
        public static readonly glCreateProgram glCreateProgram;
        public static readonly glCreateShader glCreateShader;
        private static readonly glCreateTextures glCreateTextures;
        public static readonly glCullFace glCullFace;
        public static readonly glDebugMessageCallback glDebugMessageCallback;
        public static readonly glDeleteProgram glDeleteProgram;
        public static readonly glDeleteShader glDeleteShader;
        public static readonly glDepthFunc glDepthFunc;
        public static readonly glEnableDisable glDisable;
        public static readonly glEnableDisable glEnable;
        public static readonly glDrawArrays glDrawArrays;
        public static readonly glDrawArraysIndirect glDrawArraysIndirect;
        public static readonly glDrawArraysInstanced glDrawArraysInstanced;
        public static readonly glDrawArraysInstancedBaseInstance glDrawArraysInstancedBaseInstance;
        public static readonly glDrawBuffers glDrawBuffers;
        public static readonly glDrawElements glDrawElements;
        public static readonly glDrawElementsInstanced glDrawElementsInstanced;
        public static readonly glMultiDrawArrays glMultiDrawArrays;
        public static readonly glMultiDrawArraysIndirect glMultiDrawArraysIndirect;
        public static readonly glEnableDisableVertexArrayAttrib glDisableVertexArrayAttrib;
        public static readonly glEnableDisableVertexArrayAttrib glEnableVertexArrayAttrib;
        public static readonly glFinish glFinish;
        public static readonly glFlush glFlush;
        public static readonly glFramebufferRenderbuffer glFramebufferRenderbuffer;
        public static readonly glFramebufferTexture glFramebufferTexture;
        public static readonly glNamedFramebufferTexture glNamedFramebufferTexture;
        public static readonly glGenerateMipmap glGenerateMipmap;
        public static readonly glGenerateTextureMipmap glGenerateTextureMipmap;
        public static readonly glGet_InfoLog glGetProgramInfoLog;
        public static readonly glGet_InfoLog glGetShaderInfoLog;
        private static readonly glGetActiveAttrib glGetActiveAttrib;
        private static readonly glGetActiveUniform glGetActiveUniform;
        private static readonly glGetIntegerv glGetIntegerv;
        public static readonly glGetLocation glGetAttribLocation;
        public static readonly glGetLocation glGetFragDataLocation;
        public static readonly glGetLocation glGetUniformLocation;
        public static readonly glGetProgramInterfaceiv glGetProgramInterfaceiv;
        public static readonly glGetProgramiv glGetProgramiv;
        public static readonly glGetShaderiv glGetShaderiv;
        public static readonly glGetTextureParameterfv glGetTextureParameterfv;
        private static readonly glGetTextureParameteriv glGetTextureParameteriv;
        public static readonly glIsBuffer glIsBuffer;
        public static readonly glIsEnabled glIsEnabled;
        public static readonly glIsProgram glIsProgram;
        public static readonly glIsShader glIsShader;
        public static readonly glIsTexture glIsTexture;
        public static readonly glIsVertexArray glIsVertexArray;
        public static readonly glLineWidth glLineWidth;
        public static readonly glLinkProgram glLinkProgram;
        public static readonly glNamedBufferStorage glNamedBufferStorage;
        public static readonly glNamedRenderbufferStorage glNamedRenderbufferStorage;
        public static readonly glNamedFramebufferRenderbuffer glNamedFramebufferRenderbuffer;
        public static readonly glNamedBufferSubData glNamedBufferSubData;
        public static readonly glReadBuffer glReadBuffer;
        public static readonly glReadnPixels glReadnPixels;
        public static readonly glReadPixels glReadPixels;
        public static readonly glRenderbufferStorage glRenderbufferStorage;
        public static readonly glShaderSource glShaderSource;
        public static readonly glTexImage2D glTexImage2D;
        public static readonly glTexSubImage2D glTexSubImage2D;
        public static readonly glTextureParameterf glTextureParameterf;
        private static readonly glTextureParameteri glTextureParameteri;
        public static readonly glTextureStorage2D glTextureStorage2D;
        public static readonly glTextureSubImage2D glTextureSubImage2D;
        private static readonly glUniform2f glUniform2f;
        public static readonly glUniform2i glUniform2i;
        private static readonly glUniform3f glUniform3f;
        public static readonly glDepthMask glDepthMask;
        private static readonly glUniform4f glUniform4f;
        private static readonly glUniformf glUniform1f;
        public static readonly glUniformfv glUniform2fv;
        public static readonly glUniformfv glUniform3fv;
        public static readonly glUniformfv glUniform4fv;
        private static readonly glUniformi glUniform1i;
        public static readonly glUniformiv glUniform2iv;
        private static readonly glUniformMatrix4fv glUniformMatrix4fv;
        public static readonly glUseProgram glUseProgram;
        public static readonly glVertexAttribDivisor glVertexAttribDivisor;
        public static readonly glVertexAttribPointer glVertexAttribPointer;
        public static readonly glViewport glViewport;
        public static readonly glNamedFramebufferDrawBuffers glNamedFramebufferDrawBuffers;
#pragma warning restore CS0649
#endif
        unsafe public static int GetInteger (int id) {
            var i = 0;
            glGetIntegerv(id, &i);
            return i;
        }
        public static void TextureBaseLevel (int texture, int level) => glTextureParameteri(texture, Const.TEXTURE_BASE_LEVEL, level);
        public static void TextureMaxLevel (int texture, int level) => glTextureParameteri(texture, Const.TEXTURE_MAX_LEVEL, level);
        public static void TextureFilter (int texture, MagFilter filter) => glTextureParameteri(texture, Const.TEXTURE_MAG_FILTER, (int)filter);
        public static void TextureFilter (int texture, MinFilter filter) => glTextureParameteri(texture, Const.TEXTURE_MIN_FILTER, (int)filter);
        public static void TextureWrap (int texture, WrapCoordinate c, Wrap w) => glTextureParameteri(texture, (int)c, (int)w);

        public static void Uniform (int uniform, Matrix4x4 m) => glUniformMatrix4fv(uniform, 1, false, m);
        public static void Uniform (int uniform, Vector4 v) => glUniform4f(uniform, v.X, v.Y, v.Z, v.W);
        public static void Uniform (int uniform, Vector3 v) => glUniform3f(uniform, v.X, v.Y, v.Z);
        public static void Uniform (int uniform, Vector2 v) => glUniform2f(uniform, v.X, v.Y);
        public static void Uniform (int uniform, float f) => glUniform1f(uniform, f);
        public static void Uniform (int uniform, int i) => glUniform1i(uniform, i);

        public static int[] CreateVertexArrays (int count) => Create(count, glCreateVertexArrays);
        public static int[] CreateBuffers (int count) => Create(count, glCreateBuffers);
        public static int[] CreateFramebuffers (int count) => Create(count, glCreateFramebuffers);
        public static int[] CreateRenderbuffers (int count) => Create(count, glCreateRenderbuffers);
        public static void DeleteTextures (int[] textures) => glDeleteTextures(textures.Length, textures);
        public static void DeleteBuffers (int[] buffers) => glDeleteBuffers(buffers.Length, buffers);
        unsafe public static void GetActiveUniform (int program, int index, int maxUniformNameLength, out int length, out int size, out UniformType type, out string name) {
            Span<byte> bytes = stackalloc byte[maxUniformNameLength + 1];
            fixed (byte* ptr = bytes)
                glGetActiveUniform(program, index, maxUniformNameLength, out length, out size, out type, ptr);
            name = System.Text.Encoding.ASCII.GetString(bytes.Slice(0, length));
        }
        unsafe public static void GetActiveAttrib (int program, int index, int maxAttribNameLength, out int length, out int size, out AttributeType type, out string name) {
            Span<byte> bytes = stackalloc byte[maxAttribNameLength + 1];
            fixed (byte* ptr = bytes)
                glGetActiveAttrib(program, index, maxAttribNameLength, out length, out size, out type, ptr);
            name = System.Text.Encoding.ASCII.GetString(bytes.Slice(0, length));
        }
        private static int[] Create (int count, glCountedArray f) {
            var names = new int[count];
            f(count, names);
            return names;
        }
        public static int[] Create2DTextures (int count) {
            var ints = new int[count];
            glCreateTextures(Const.TEXTURE_2D, count, ints);
            return ints;
        }
        public static T GetTextureParameteri<T> (int texture, TextureParameter parameter) where T : Enum {
            var t = new int[1];
            glGetTextureParameteriv(texture, parameter, t);
            return (T)Enum.ToObject(typeof(T), t[0]);
        }

        private static IntPtr GetProcAddress (string name) {
            var ptr = GLFW.Glfw.GetProcAddress(name);
            return ptr != IntPtr.Zero ? ptr : throw new Exception($"failed to get proc address of {name}");
        }

#if __USE_FUNCTION_POINTERS
        private static Delegate GetDelegate (FieldInfo f) => Marshal.GetDelegateForFunctionPointer(GetProcAddress(f.Name), f.FieldType) ?? throw new Exception($"failed to create delegate for {f.Name}");
        static Calls () { 
            foreach (var f in typeof(Calls).GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static))
                if (f.GetValue(null) is null)
                    f.SetValue(null, GetDelegate(f));
                else
                    throw new Exception($"{f.Name} already set");
        }
#else
        private static Delegate GetDelegate (FieldInfo f) => Marshal.GetDelegateForFunctionPointer(GetProcAddress(f.Name), f.FieldType) ?? throw new Exception($"failed to create delegate for {f.Name}");
        static Calls () {
            foreach (var f in typeof(Calls).GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static))
                if (f.GetValue(null) is null)
                    f.SetValue(null, GetDelegate(f));
                else
                    throw new Exception($"{f.Name} already set");

            var fields = typeof(Calls).GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
            var pointers = new HashSet<long>();
            foreach (var field in fields) {
                if (field.GetValue(null) is MulticastDelegate d) {
                    if (!pointers.Add(d.GetMethodInfo().MethodHandle.Value.ToInt64()))
                        Utilities.Trace(field.Name);
                }
            }
        }
#endif
    }
}