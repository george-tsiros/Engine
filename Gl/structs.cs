namespace Gl {
    using System.Runtime.InteropServices;

    //[StructLayout(LayoutKind.Sequential)]
    //public readonly struct Triangle4 {
    //    public readonly Vector4 A, B, C;
    //    public Triangle4 (in Vector4 a, in Vector4 b, in Vector4 c) => (A, B, C) = (a, b, c);
    //    //public Triangle4 (in BepuPhysics.Collidables.Triangle t) => (A, B, C) = (new(t.A, 1), new(t.B, 1), new(t.C, 1));
    //    public static Triangle4 InvertWinding (in Triangle4 t) => new(t.A, t.C, t.B);
    //}

    [StructLayout(LayoutKind.Sequential)]
    public readonly struct Vector2i {
        public readonly int X, Y;
        public Vector2i (in int x, in int y) => (X, Y) = (x, y);
        public static Vector2i operator + (Vector2i a, Vector2i b) => new(a.X + b.X, a.Y + b.Y);
        public static Vector2i operator - (Vector2i a, Vector2i b) => new(a.X - b.X, a.Y - b.Y);
    }

    [StructLayout(LayoutKind.Sequential)]
    public readonly struct Vector3i {
        public readonly int X, Y, Z;
        public Vector3i (in int x, in int y, in int z) => (X, Y, Z) = (x, y, z);
        public static Vector3i operator + (Vector3i a, Vector3i b) => new(a.X + b.X, a.Y + b.Y, a.Z + b.Z);
        public static Vector3i operator - (Vector3i a, Vector3i b) => new(a.X - b.X, a.Y - b.Y, a.Z - b.Z);
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct ArraysCommand {
        public uint VerticesCount, InstancesCount, FirstVertexIndex, BaseInstanceIndex;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct ElementsCommand {
        public uint VerticesCount, InstancesCount, FirstVertexIndex, BaseVertexIndex, BaseInstanceIndex;
    }
}