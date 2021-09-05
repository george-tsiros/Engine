namespace Gl {
    using System;
    using System.IO;
    using System.Numerics;

    public static class Extensions {
        public static Vector3 Xyz (this Vector4 self) => new(self.X, self.Y, self.Z);

        public static float Float (this Random r, float min, float max) => (float)(r.NextDouble() * (max - min) + min);
        public static Vector3 Vector3 (this Random r, Vector3 min, Vector3 max) => new(r.Float(min.X, max.X), r.Float(min.Y, max.Y), r.Float(min.Z, max.Z));
        public static int ReadInt32 (this Stream self) {
            Span<byte> bb = stackalloc byte[sizeof(int)];
            _ = self.Read(bb);
            return BitConverter.ToInt32(bb);
        }
        public static double ReadDouble (this Stream self) {
            Span<byte> bb = stackalloc byte[sizeof(double)];
            _ = self.Read(bb);
            return BitConverter.ToDouble(bb);
        }
    }
}