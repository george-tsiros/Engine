namespace Engine;

using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Diagnostics;
using System.Numerics;
public static class Extra {

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    private static void PushAscii (Span<byte> a, ref long int64, ref int offset) {
        int64 = Math.DivRem(int64, 10, out var d);
        a[--offset] = (byte)(d + '0');
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static int ToChars (long int64, Span<byte> bytes) {
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


    public static Vector2 ByteToVector (byte b) => new(b & 0xf, b >> 4);

    public static float ModuloTwoPi (ref float angle, float delta) {
        angle += delta;
        while (angle < 0)
            angle += 2 * (float)Math.PI;
        while (angle > 2 * Math.PI)
            angle -= 2 * (float)Math.PI;
        return angle;
    }
    public static double ModuloTwoPi (ref double angle, double delta) {
        angle += delta;
        while (angle < 0)
            angle += 2 * Math.PI;
        while (angle > 2 * Math.PI)
            angle -= 2 * Math.PI;
        return angle;
    }
    public static float Clamp (ref float angle, float delta, float min, float max) => angle = (float)Math.Max(min, Math.Min(angle + delta, max));
    public static double Clamp (ref double angle, double delta, double min, double max) => angle = Math.Max(min, Math.Min(angle + delta, max));

    public static (float min, float max) Extrema (float[] ycoords) {
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

    public static T[] Dex<T> (T[] array, int[] indices) where T : struct {
        T[] b = new T[indices.Length];
        Dex(array, indices, b);
        return b;
    }
    public static void Dex<T> (T[] vertices, int[] indices, T[] dex) where T : struct {
        Debug.Assert(dex.Length == indices.Length);
        for (var i = 0; i < indices.Length; ++i)
            dex[i] = vertices[indices[i]];
    }
    public static Vector4[] ScaleInPlace (Vector4[] v, Vector4 s) {
        for (var i = 0; i < v.Length; ++i)
            v[i] *= s;
        return v;
    }
    public static Vector4[] ScaleInPlace (Vector4[] v, float f) => ScaleInPlace(v, new Vector4(f, f, f, 1));
    public static Vector4[] ScaleInPlace (Vector4[] v, Vector3 f) => ScaleInPlace(v, new Vector4(f, 1));

    public static Vector4[] TranslateInPlace (Vector4[] v, Vector3 d) {
        var t = new Vector4(d, 0);
        for (var i = 0; i < v.Length; ++i)
            v[i] += t;
        return v;
    }
}

