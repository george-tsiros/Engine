namespace Gl {
    using System;
    using System.Numerics;
    using System.Collections.Generic;
    using static Calls;

    sealed public class VertexArray {
        public static implicit operator uint (VertexArray b) => b.Id;
        public uint Id { get; } = CreateVertexArrays(1)[0];
        //public void Assign<T> (VertexBuffer<T> buffer, uint location) where T : unmanaged => Assign<T>(buffer, location, 0u);
        public void Assign<T> (VertexBuffer<T> buffer, uint location, uint divisor=0u) where T : unmanaged {
            State.VertexArray = this;
            glBindBuffer(BufferTarget.Array, buffer);
            var (size, type) = SizeAndTypeOf(typeof(T));
            if (size > 4)
                for (var i = 0u; i < 4; ++i)
                    Attrib(this, location + i, 4, type, 16 * sizeof(float), 4 * i * sizeof(float), divisor);
            else
                Attrib(this, location, size, type, 0, 0, divisor);
        }
        private static void Attrib (uint id, uint location, int size, AttribType type, int stride, uint offset, uint divisor = 0u) {
            glEnableVertexArrayAttrib(id, location);
            glVertexAttribPointer(location, size, type, false, stride, new(offset));
            glVertexAttribDivisor(location, divisor);
        }

        public static (int size, AttribType type) SizeAndTypeOf (Type type) => _TYPES.TryGetValue(type, out var i) ? i : throw new ArgumentException($"unsupported type {type.Name}", nameof(type));
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
}