namespace Gl {
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Reflection;

    public static class Utilities {
#if DEBUG
        public static void Trace (params object[] args) => System.Diagnostics.Debug.WriteLine($"{DateTime.Now:mm:ss.fff}> {new System.Diagnostics.StackFrame(1).GetMethod().Name} {string.Join(", ", args)}");
#else
        public static void Trace (params object[] args) => Console.WriteLine($"{DateTime.Now:mm:ss.fff}> {new System.Diagnostics.StackFrame(1).GetMethod().Name} {string.Join(", ", args)}");
#endif
        public static T GetAttribute<T> (MemberInfo self, bool inherit) where T : Attribute {
            var eh = self.GetCustomAttributes(typeof(T), inherit);
            return eh.Length == 1 && eh[0] is T attr ? attr : null;
        }
        public static bool TryGetAttribute<T> (MemberInfo self,[NotNullWhen(true)] out T attribute, bool inherit) where T : Attribute {
            attribute = GetAttribute<T>(self, inherit);
            return attribute != null;
        }
        public static FieldInfo GetBackingField (Type type, PropertyInfo prop, BindingFlags flags = BindingFlags.Instance) => type.GetField($"<{prop.Name}>k__BackingField", BindingFlags.NonPublic | flags);

        public static bool TryGetBackingField (Type type, PropertyInfo prop, out FieldInfo eh, BindingFlags flags = BindingFlags.Instance) => (eh = GetBackingField(type, prop, flags)) != null;
        public static uint ShaderFromString (int type, string source) {
            var vs = Calls.glCreateShader(type);
            Calls.glShaderSource(vs, 1, new string[] { source }, null);
            Calls.glCompileShader(vs);
            Calls.glGetShaderiv(vs, ShaderParameter.InfoLogLength, out int length);
            if (length > 0)
                ThrowThing(Calls.glGetShaderInfoLog, vs, length);
            return vs;
        }
        public static uint ProgramFromStrings (string vertexSource, string fragmentSource) {
            var vertexShader = ShaderFromString(Const.VERTEX_SHADER, vertexSource);
            var fragmentShader = ShaderFromString(Const.FRAGMENT_SHADER, fragmentSource);
            var program = Calls.glCreateProgram();
            Calls.glAttachShader(program, vertexShader);
            Calls.glAttachShader(program, fragmentShader);
            Calls.glLinkProgram(program);
            Calls.glGetProgramiv(program, ProgramParameter.InfoLogLength, out var logLength);
            if (logLength > 0)
                ThrowThing(Calls.glGetProgramInfoLog, program, logLength);
            Calls.glDeleteShader(vertexShader);
            Calls.glDeleteShader(fragmentShader);
            return program;
        }
        unsafe public static void ThrowThing (glGet_InfoLog f, uint name, int length) {
            Span<byte> bytes = stackalloc byte[length + 1];
            fixed (byte* ptr = bytes)
                f(name, bytes.Length, ref length, ptr);
            throw new Exception(System.Text.Encoding.ASCII.GetString(bytes.Slice(0, length)));
        }
    }
}