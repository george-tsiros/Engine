namespace Gl {
    using System;
    using System.Diagnostics;
    using System.Runtime.InteropServices;
    using static Calls;

    sealed public class VertexBuffer<T> where T : unmanaged {
        public static implicit operator int (VertexBuffer<T> b) => b.Id;
        public int Id { get; }
        public int ElementSize { get; }
        public int Capacity { get; }
        private VertexBuffer () => (Id, ElementSize) = (CreateBuffers(1)[0], Marshal.SizeOf<T>());
        public VertexBuffer (int capacityInElements) : this() => glNamedBufferStorage(Id, ElementSize * (Capacity = capacityInElements), IntPtr.Zero, Const.DYNAMIC_STORAGE_BIT);
        public VertexBuffer (T[] data) : this(data.Length) => BufferData(data, data.Length, 0, 0);
        unsafe public void BufferData (T[] data, int count, int sourceOffset, int targetOffset) {
            Debug.Assert(sourceOffset + count <= data.Length);
            Debug.Assert(targetOffset + count <= Capacity);
            fixed (T* ptr = data)
                glNamedBufferSubData(Id, ElementSize * targetOffset, ElementSize * count, ptr + sourceOffset);
        }
        unsafe public void BufferData (Span<T> data, int count, int sourceOffset, int targetOffset) {
            Debug.Assert(sourceOffset + count <= data.Length);
            Debug.Assert(targetOffset + count <= Capacity);
            fixed (T* ptr = data)
                glNamedBufferSubData(Id, ElementSize * targetOffset, ElementSize * count, ptr + sourceOffset);
        }
    }
}