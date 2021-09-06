namespace Gl {
    using System;
    using System.Runtime.InteropServices;
    public delegate bool glIsBuffer (int id);
    public delegate bool glIsEnabled (Capability cap);
    public delegate bool glIsProgram (int id);
    public delegate bool glIsShader (int id);
    public delegate bool glIsTexture (int id);
    public delegate bool glIsVertexArray (int id);
    public delegate FramebufferStatus glCheckNamedFramebufferStatus (int framebuffer, FramebufferTarget target);
    public delegate int glGetLocation (int id, [MarshalAs(UnmanagedType.LPStr)] string str);
    public delegate int glCreateProgram ();
    public delegate int glCreateShader (int shaderType);
    public delegate void DebugProc (int source, int type, int id, int severity, int length, string message, IntPtr userParam);
    public delegate void glActiveTexture (int i);
    public delegate void glAttachShader (int program, int shader);
    public delegate void glBindBuffer (BufferTarget target, int buffer);
    public delegate void glBindFramebuffer (int target, int buffer);
    public delegate void glBindRenderbuffer (int target, int buffer);
    public delegate void glBindTexture (int target, int buffer);
    public delegate void glBindTextureUnit (int unit, int texture);
    public delegate void glBindVertexArray (int i);
    public delegate void glBufferData (int target, long size, IntPtr data, int usage);
    public delegate void glBufferSubData (int target, IntPtr offset, long size, IntPtr data);
    public delegate void glClear (BufferBit mask);
    public delegate void glClearColor (float r, float g, float b, float a);
    public delegate void glCompileShader (int i);
    public delegate void glCountedArray (int count, int[] names);
    public delegate void glDeleteArray (int count, int[] names);
    public delegate void glCreateTextures (int target, int count, int[] textures);
    public delegate void glCullFace (int i);
    public delegate void glDebugMessageCallback (DebugProc proc, IntPtr userParam);
    public delegate void glDeleteProgram (int i);
    public delegate void glDeleteShader (int i);
    public delegate void glDepthFunc (DepthFunc func);
    public delegate void glDrawArrays (Primitive mode, int first, int count);
    public delegate void glDrawArraysIndirect (Primitive mode, IntPtr offsetIntoIndirectBuffer);
    public delegate void glDrawArraysInstanced (Primitive mode, int firstIndex, int indicesPerInstance, int instancesCount);
    public delegate void glDrawArraysInstancedBaseInstance (Primitive mode, int firstIndex, int indicesPerInstance, int instancesCount, int baseInstance);
    public delegate void glDrawBuffers (int count, Attachment[] buffers);
    public delegate void glDrawElements (Primitive mode, int indicesCount, int type, IntPtr elementsPtrOrOffsetIntoBoundElementArray);
    public delegate void glDrawElementsInstanced (Primitive mode, int indicesCount, int type, IntPtr elementsPtrOrOffsetIntoBoundElementArray, int instancesCount);
    public delegate void glMultiDrawArrays (int mode, int[] first, int[] count, int drawCount);
    public delegate void glMultiDrawArraysIndirect (int mode, IntPtr indirect, int drawCount, int stride);
    public delegate void glEnableDisable (Capability cap);
    public delegate void glEnableDisableVertexArrayAttrib (int id, int index);
    public delegate void glFinish ();
    public delegate void glFlush ();
    public delegate void glDepthMask (bool enabled);
    public delegate void glFramebufferRenderbuffer (int target, int attachment, int renderbufferTarget, int renderbuffer);
    public delegate void glFramebufferTexture (int target, int attachment, int texture, int level);
    public delegate void glNamedFramebufferTexture (int framebuffer, Attachment attachment, int texture, int level);
    public delegate void glGenerateMipmap (int i);
    public delegate void glGenerateTextureMipmap (int texture);
    unsafe public delegate void glGet_InfoLog (int id, int bufferSize, ref int length, byte* log);
    unsafe public delegate void glGetActiveAttrib (int id, int index, int bufferSize, out int length, out int size, out AttributeType type, byte* name);
    unsafe public delegate void glGetActiveUniform (int id, int index, int bufferSize, out int length, out int size, out UniformType type, byte* name);
    unsafe public delegate void glGetIntegerv (int id, int* values);
    public delegate void glGetProgramInterfaceiv (int id, ProgramInterface programInterface, InterfaceParameter parameter, int[] paramerers);
    public delegate void glGetProgramiv (int id, ProgramParameter parameter, out int result);
    public delegate void glGetShaderiv (int id, ShaderParameter parameter, out int result);
    public delegate void glGetTextureParameterfv (int texture, TextureParameter name, float[] f);
    public delegate void glGetTextureParameteriv (int texture, TextureParameter name, int[] i);
    public delegate void glLineWidth (float width);
    public delegate void glLinkProgram (int i);
    public delegate void glNamedBufferStorage (int buffer, long size, IntPtr data, int flags);
    public delegate void glNamedRenderbufferStorage (int buffer, RenderbufferFormat format, int width, int height);
    public delegate void glNamedFramebufferRenderbuffer (int framebuffer, Attachment attachment, int renderbufferTarget, int renderbuffer);
    unsafe public delegate void glNamedBufferSubData (int buffer, long offset, long size, void* data);
    public delegate void glReadBuffer (int i);
    public delegate void glReadnPixels (int x, int y, int width, int height, int format, int type, int bufferSize, IntPtr data);
    public delegate void glReadPixels (int x, int y, int width, int height, int format, int type, IntPtr data);
    public delegate void glRenderbufferStorage (int target, int storage, int width, int height);
    public delegate void glShaderSource (int id, int count, [MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.LPStr)] string[] sources, int[] length);
    public delegate void glTexImage2D (int target, int level, int internalFormat, int width, int height, int borderZERO, int pixelFormat, int pixelType, IntPtr data);
    public delegate void glTexSubImage2D (int target, int level, int xOffset, int yOffset, int width, int height, int format, int type, IntPtr pixels);
    public delegate void glTextureParameterf (int texture, int name, float f);
    public delegate void glTextureParameteri (int texture, int name, int i);
    public delegate void glTextureStorage2D (int texture, int levels, TextureInternalFormat sizedFormat, int width, int height);
    unsafe public delegate void glTextureSubImage2D (int texture, int level, int xOffset, int yOffset, int width, int height, int format, int type, byte* pixels);
    public delegate void glUniform2i (int location, int x, int y);
    public delegate void glUniform2f (int location, float x, float y);
    public delegate void glUniform3f (int location, float x, float y, float z);
    public delegate void glUniform4f (int location, float x, float y, float z, float w);
    public delegate void glUniformi (int location, int value);
    public delegate void glUniformf (int location, float value);
    public delegate void glUniformiv (int location, int count, int[] iv);
    public delegate void glUniformfv (int location, int count, float[] fv);
    public delegate void glUniformMatrix4fv (int location, long count, bool transpose, System.Numerics.Matrix4x4 matrix);
    public delegate void glUseProgram (int i);
    public delegate void glVertexAttribDivisor (int index, int divisor);
    public delegate void glVertexAttribPointer (int index, int size, AttribType type, bool normalized, int stride, IntPtr ptr);
    public delegate void glViewport (int x, int y, int width, int height);
    public delegate void glNamedFramebufferDrawBuffers (int framebuffer, int count, Attachment[] buffers);
}