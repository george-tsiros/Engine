namespace Engine {
    using System;
    using System.Diagnostics;
    using System.Numerics;
    public enum DataType:byte {
        Null = 0,
        Byte,
        Int16,
        Int32,
        Int64,
        UInt16,
        UInt32,
        UInt64,
        String,
    }
    static class Extra {
        public static int ToChars (int i, Span<byte> a) {
            var isNegative = i < 0;
            if (isNegative)
                i = -i;
            var l = 0;
            do
                PushAscii(a, ref i, ref l);
            while (i != 0);
            if (isNegative)
                a[l++] = (byte)'-';
            return l;
        }

        private static void PushAscii (Span<byte> a, ref int i, ref int l) {
            var d = i % 10;
            a[l++] = (byte)(d + '0');
            i /= 10;
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

        public static void Append (Span<Vector2> vectors, ref int position, int value) {
            Span<byte> bytes = stackalloc byte[10];
            var l = ToChars(value, bytes);
            while (--l >= 0)
                Append(vectors, ref position, bytes[l]);
        }
        public static void Append (Span<Vector2> vectors, ref int position, byte b) => vectors[position++] = ByteToVector(b);
        public static void Append (Span<Vector2> vectors, ref int position, ReadOnlySpan<byte> bytes) {
            var count = bytes.Length;
            for (var i = 0; i < count; ++i)
                Append(vectors, ref position, bytes[i]);
        }

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
}

