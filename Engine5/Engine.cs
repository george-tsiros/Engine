namespace Engine;

using GLFW;
using System;
using System.Reflection;
using System.IO;
using System.Text.RegularExpressions;

class Engine {
    private static FieldInfo GetEnumFieldInfo (Assembly a, string s) {
        var parts = s.Split('.');
        if (parts.Length != 2)
            return null;
        const BindingFlags publicStaticIgnoreCase = BindingFlags.Public | BindingFlags.Static | BindingFlags.IgnoreCase;
        return a.GetType($"GLFW.{parts[0]}", false, true)?.GetField(parts[1], publicStaticIgnoreCase);
    }

    private static bool TryGetHintValue (string hintValue, out int value) {
        if (bool.TryParse(hintValue, out var b)) {
            value = b ? 1 : 0;
            return true;
        }
        if (int.TryParse(hintValue, out value))
            return true;
        if (GetEnumFieldInfo(typeof(Glfw).Assembly, hintValue) is FieldInfo fi) {
            value = (int)fi.GetValue(null);
            return true;
        }
        value = 0;
        return false;
    }

    private static void SetHintsFrom (string filepath) {
        var hintRegex = new Regex(@"^ *(\w+) *= *(true|false|\d+|(\w+)\.(\w+)) *$");
        foreach (var line in File.ReadAllLines(filepath))
            if (hintRegex.TryMatch(line, out var m) && Enum.TryParse<Hint>(m.Groups[1].Value, true, out var hint)) {
                if (TryGetHintValue(m.Groups[2].Value, out var i))
                    Glfw.WindowHint(hint, i);
                else
                    throw new ApplicationException($"could not get an int out of '{m.Groups[2].Value}' for {hint}");
            }
    }

    [STAThread]
    static void Main () {
        if (File.Exists("hints.txt"))
            SetHintsFrom("hints.txt");
        using var gl = new NeinCraft(1280, 720);
        //using var gl = new Boxes(1280, 720);
        gl.Run();
    }
}
