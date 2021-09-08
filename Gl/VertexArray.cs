namespace Gl;

using System;
using System.Numerics;
using System.Collections.Generic;
using static Calls;
public class NotString {
    public readonly byte[] chars;
    public NotString (string s) => chars = System.Text.Encoding.ASCII.GetBytes(s);
    public static explicit operator NotString(string s) => new(s);
}

public class VertexArray {
    public static readonly NotString aaaaaaaa = (NotString)"asdgf";
    public static implicit operator int (VertexArray b) => b.Id;
    public int Id { get; } = CreateVertexArrays(1)[0];
    public void Assign<T> (VertexBuffer<T> buffer, int location, int divisor = 0) where T : unmanaged => Assign(this, buffer, location, divisor);

    private static void Assign<T> (int vao, VertexBuffer<T> buffer, int location, int divisor = 0) where T : unmanaged {
        State.VertexArray = vao;
        glBindBuffer(BufferTarget.Array, buffer);
        Attrib<T>(vao, location, divisor);
    }
    private static void Attrib<T> (int vao, int location, int divisor) where T : unmanaged {
        var (size, type) = SizeAndTypeOf(typeof(T));
        if (size > 4)
            for (var i = 0; i < 4; ++i)
                Attrib(vao, location + i, 4, type, 16 * sizeof(float), 4 * i * sizeof(float), divisor);
        else
            Attrib(vao, location, size, type, 0, 0, divisor);
    }

    private static void Attrib (int id, int location, int size, AttribType type, int stride, int offset, int divisor) {
        glEnableVertexArrayAttrib(id, location);
        glVertexAttribPointer(location, size, type, false, stride, new(offset));
        glVertexAttribDivisor(location, divisor);
    }

    private static (int size, AttribType type) SizeAndTypeOf (Type type) => _TYPES.TryGetValue(type, out var i) ? i : throw new ArgumentException($"unsupported type {type.Name}", nameof(type));
    private static readonly Dictionary<Type, (int, AttribType)> _TYPES = new() {
        { typeof(float), (1, AttribType.Float) },
        { typeof(double), (1, AttribType.Double) },
        { typeof(int), (1, AttribType.Int) },
        { typeof(uint), (1, AttribType.UInt) },
        { typeof(Vector2), (2, AttribType.Float) },
        { typeof(Vector3), (3, AttribType.Float) },
        { typeof(Vector4), (4, AttribType.Float) },
        { typeof(Vector2i), (2, AttribType.Int) },
        { typeof(Vector3i), (3, AttribType.Int) },
        { typeof(Matrix4x4), (16, AttribType.Float) },
    };
}
