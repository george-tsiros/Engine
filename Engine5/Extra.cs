namespace Engine;

using System;
using System.Reflection;
using System.IO;
using System.Runtime.CompilerServices;
using System.Diagnostics;
using System.Numerics;
using System.Text.RegularExpressions;

static class Extra {

    private static bool TryGetHintValue (string hintValue, out int value) {
        if (bool.TryParse(hintValue, out var b)) {
            value = b ? 1 : 0;
            return true;
        }
        if (int.TryParse(hintValue, out value))
            return true;
        if (typeof(GLFW.Glfw).Assembly.GetEnumFieldInfo(hintValue) is FieldInfo fi) {
            value = (int)fi.GetValue(null);
            return true;
        }
        value = 0;
        return false;
    }

    internal static void SetHintsFrom (string filepath) {
        var hintRegex = new Regex(@"^ *(\w+) *= *(true|false|\d+|(\w+)\.(\w+)) *$");
        foreach (var line in File.ReadAllLines(filepath))
            if (hintRegex.TryMatch(line, out var m) && Enum.TryParse<GLFW.Hint>(m.Groups[1].Value, true, out var hint)) {
                if (TryGetHintValue(m.Groups[2].Value, out var i))
                    GLFW.Glfw.WindowHint(hint, i);
                else
                    throw new ApplicationException($"could not get an int out of '{m.Groups[2].Value}' for {hint}");
            }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    private static void PushAscii (Span<byte> a, ref long int64, ref int offset) {
        int64 = Math.DivRem(int64, 10, out var d);
        a[--offset] = (byte)(d + '0');
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    internal static int ToChars (long int64, Span<byte> bytes) {
        var isNegative = int64 < 0l;
        if (isNegative)
            int64 = -int64;
        var offset = 20;
        do
            PushAscii(bytes, ref int64, ref offset);
        while (int64 != 0);
        if (isNegative)
            bytes[--offset] = (byte)'-';
        return offset;
    }


    internal static float ModuloTwoPi (ref float angle, float delta) {
        angle += delta;
        while (angle < 0)
            angle += 2 * (float)Math.PI;
        while (angle > 2 * Math.PI)
            angle -= 2 * (float)Math.PI;
        return angle;
    }
    internal static double ModuloTwoPi (ref double angle, double delta) {
        angle += delta;
        while (angle < 0)
            angle += 2 * Math.PI;
        while (angle > 2 * Math.PI)
            angle -= 2 * Math.PI;
        return angle;
    }
    internal static float Clamp (ref float angle, float delta, float min, float max) => angle = (float)Math.Max(min, Math.Min(angle + delta, max));
    internal static double Clamp (ref double angle, double delta, double min, double max) => angle = Math.Max(min, Math.Min(angle + delta, max));

    internal static (float min, float max) Extrema (float[] ycoords) {
#if !true
            var min = new Vector<float>(float.MaxValue);
            var max = new Vector<float>(float.MinValue);
            var total = ycoords.Length;
            var vectored = total & 0xfffffff8;
            for (var i = 0; i < vectored; i += 8) {
                var y = new Vector<float>(ycoords, i);
                min = Vector.Min(min, y);
                max = Vector.Max(max, y);
            }
            return (0f, 100f);
#else
        var min = float.MaxValue;
        var max = float.MinValue;
        for (var i = 0; i < ycoords.Length; i++)
            Extrema(ycoords[i], ref min, ref max);
        return (min, max);
#endif
    }
    [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
    private static void Extrema (float l, ref float min, ref float max) {
        if (l < min)
            min = l;
        if (max < l)
            max = l;
    }

}

