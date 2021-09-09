namespace ShaderGen;

using System;
using System.IO;
using System.Runtime.InteropServices;
using Gl;
using static Gl.Calls;
using static Gl.Utilities;
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
                    var program = ProgramFromStrings(vertexShaderSource, fragmentShaderSource);
                    using (var f = new StreamWriter(Path.Combine(args[1], shaderName + ".cs")) { })
                        DoProgram(program, shaderName, f);
                    DeleteProgram(program);
                }
            }
        }
        catch (TypeInitializationException ex) {
            Trace($"'{ex.Message}' for '{ex.TypeName}'");
            if (ex.InnerException is MarshalDirectiveException inner)
                Trace($"({inner.Message})");
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
        Trace($"emitting {className}");
        f.Write($@"namespace Shaders;
using Gl;
using System.Numerics;
public static class {className} {{
#pragma warning disable CS0649
");
        int attrCount = GetProgram(program, ProgramParameter.ActiveAttributes);
        Trace($"{attrCount} active attributes");
        for (var i = 0; i < attrCount; ++i) {
            var x = GetActiveAttrib(program, i);
            if (x.name.StartsWith(" gl_")) {
                Trace($"skipping '{x.name}'");
                continue;
            }
            Trace($"'{x.name}': size {x.size}, type {x.type}");
            f.Write($@"
    //size {x.size}, type {x.type}
    [GlAttrib(""{x.name}"")]
    public static int {UppercaseFirst(x.name)} {{ get; }}
");
        }

        int uniformCount = GetProgram(program, ProgramParameter.ActiveUniforms);
        Trace($"{uniformCount} active uniforms");
        for (var i = 0; i < uniformCount; ++i) {
            var y = GetActiveUniform(program, i);
            if (y.name.StartsWith("gl_")) {
                Trace($"skipping '{y.name}'");
                continue;
            }
            Trace($"'{y.name}': size {y.size}, type {y.type}");
            var fieldName = IsPrimitive(y.name) ? "@" + y.name : y.name;
            var rawTypeName = y.type.ToString();
            f.Write($@"
    //size {y.size}, type {y.type}
    [GlUniform(""{y.name}"")]
    private readonly static int {fieldName};
    public static void {UppercaseFirst(y.name)} ({UniformTypeToTypeName(y.type)} v) => Calls.Uniform({fieldName}, v);
");
        }
        f.Write($@"
    public static int Id {{ get; }}
    static {className} () => ParsedShader.Prepare(typeof({className}));
#pragma warning restore CS0649
}}");
        Trace(dashes);
    }
    private static readonly string dashes = new('-', 100);
    [STAThread]
    public static int Main (string[] args) {
        try {
            return CreateFrom(args) ? 0 : -1;
        }
        catch (Exception ex) {
            Trace(ex.Message);
            return -1;
        }
    }
}
