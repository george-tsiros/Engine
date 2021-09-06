namespace ShaderGen {
    using System;
    using System.IO;
    using System.Runtime.InteropServices;
    using Gl;
    using static Gl.Calls;
    class ShaderGen {
        private static bool CreateFrom (string[] args) {
            if (args.Length != 2)
                throw new ArgumentException($"must have length 1, not {args.Length}", nameof(args));
            var missing = Array.FindIndex(args, f => !Directory.Exists(f));
            if (missing >= 0)
                throw new DirectoryNotFoundException(args[missing]);

            GLFW.Glfw.WindowHint(GLFW.Hint.ContextVersionMajor, 4);
            GLFW.Glfw.WindowHint(GLFW.Hint.ContextVersionMinor, 6);
            GLFW.Glfw.WindowHint(GLFW.Hint.OpenglDebugContext, GLFW.Constants.True);
            GLFW.Glfw.WindowHint(GLFW.Hint.OpenglForwardCompatible, GLFW.Constants.True);
            GLFW.Glfw.WindowHint(GLFW.Hint.OpenglProfile, GLFW.Profile.Core);
            GLFW.Glfw.WindowHint(GLFW.Hint.Visible, GLFW.Constants.False);

            var window = GLFW.Glfw.CreateWindow(256, 256, string.Empty, GLFW.Monitor.None, GLFW.Window.None);
            GLFW.Glfw.MakeContextCurrent(window);
            try {
                foreach (var vertexShaderFilepath in Directory.EnumerateFiles(args[0], "*.vert")) {
                    var shaderName = UppercaseFirst(Path.GetFileNameWithoutExtension(vertexShaderFilepath));
                    var fragmentShaderFilepath = Path.Combine(Path.GetDirectoryName(vertexShaderFilepath), shaderName + ".frag");
                    if (File.Exists(fragmentShaderFilepath)) {
                        var vertexShaderSource = File.ReadAllText(vertexShaderFilepath);
                        var fragmentShaderSource = File.ReadAllText(fragmentShaderFilepath);
                        var program = Utilities.ProgramFromStrings(vertexShaderSource, fragmentShaderSource);
                        using (var f = new StreamWriter(Path.Combine(args[1], shaderName + ".cs")) { })
                            DoProgram(program, shaderName, f);
                        glDeleteProgram(program);
                    }
                }
            }
            catch (TypeInitializationException ex) {
                Utilities.Trace($"'{ex.Message}' for '{ex.TypeName}'");
                if (ex.InnerException is MarshalDirectiveException inner)
                    Utilities.Trace($"({inner.Message})");
                return false;
            }
            finally {
                GLFW.Glfw.DestroyWindow(window);
            }
            return true;
        }
        private static string UppercaseFirst (string str) {
            var chars = str.ToCharArray();
            chars[0] = char.ToUpper(chars[0]);
            return new string(chars);
        }
        private static bool IsPrimitive (UniformType type) => type == UniformType.Double || type == UniformType.Float || type == UniformType.Int || type == UniformType.UInt;

        private static string UniformTypeToTypeName (UniformType type) {
            if (IsPrimitive(type))
                return type.ToString().ToLower();
            if (type == UniformType.Sampler2D)
                return "int";
            return type.ToString();
        }
        //static bool IsReserved (string name) => name == "float" || name == "double" || name == "byte" || name == "char";
        private static bool IsPrimitive (string name) => name == "float" || name == "double" || name == "byte" || name == "char";
        private static void DoProgram (int program, string className, StreamWriter f) {
            f.Write($@"namespace Shaders {{
    using Gl;
    using System.Numerics;
    public static class {className} {{
#pragma warning disable CS0649
");
            glGetProgramiv(program, ProgramParameter.ActiveAttributes, out int attrCount);
            glGetProgramiv(program, ProgramParameter.ActiveAttributeMaxLength, out int maxAttrLength);
            for (var i = 0; i < attrCount; ++i) {
                GetActiveAttrib(program, i, maxAttrLength, out _, out var size, out var type, out var name);
                if (name.StartsWith("gl_"))
                    continue;
                f.Write($@"
        //size {size}, type {type}
        [GlAttrib(""{name}"")]
        public static int {UppercaseFirst(name)} {{ get; }}
");
            }
            glGetProgramiv(program, ProgramParameter.ActiveUniforms, out int uniformCount);
            glGetProgramiv(program, ProgramParameter.ActiveUniformMaxLength, out int maxUniformLength);
            for (var i = 0; i < uniformCount; ++i) {
                GetActiveUniform(program, i, maxUniformLength, out _, out var size, out var type, out var name);
                if (name.StartsWith("gl_"))
                    continue;
                var fieldName = IsPrimitive(name) ? "@" + name : name;
                var rawTypeName = type.ToString();
                f.Write($@"
        //size {size}, type {type}
        [GlUniform(""{name}"")]
        private readonly static int {fieldName};
        public static void {UppercaseFirst(name)} ({UniformTypeToTypeName(type)} v) => Calls.Uniform({fieldName}, v);
");
            }
            f.Write($@"
        public static int Id {{ get; }}
        static {className} () => ParsedShader.Prepare(typeof({className}));
#pragma warning restore CS0649
    }}
}}");
        }

        [STAThread]
        public static int Main (string[] args) {
            try {
                return CreateFrom(args) ? 0 : -1;
            }
            catch (Exception ex) {
                Utilities.Trace(ex.Message);
                return -1;
            }
        }
    }
}