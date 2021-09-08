namespace Gl;

using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using static Calls;

public class VertexBuffer<T>:IDisposable where T : unmanaged {
    public static implicit operator int (VertexBuffer<T> b) => b.Id;
    public int Id { get; } = CreateBuffers(1)[0];
    public int ElementSize { get; } = Marshal.SizeOf<T>();
    public int Capacity { get; }
    private bool disposed;
    public VertexBuffer (int capacityInElements) => glNamedBufferStorage(Id, ElementSize * (Capacity = capacityInElements), IntPtr.Zero, Const.DYNAMIC_STORAGE_BIT);
    public VertexBuffer (T[] data) : this(data.Length) => BufferData(data, data.Length, 0, 0);
    unsafe public void BufferData (T[] data, int count, int sourceOffset, int targetOffset) {
        Debug.Assert(!disposed);
        Debug.Assert(sourceOffset + count <= data.Length);
        Debug.Assert(targetOffset + count <= Capacity);
        fixed (T* ptr = data)
            glNamedBufferSubData(Id, ElementSize * targetOffset, ElementSize * count, ptr + sourceOffset);
    }
    unsafe public void BufferData (Span<T> data, int count, int sourceOffset, int targetOffset) {
        Debug.Assert(!disposed);
        Debug.Assert(sourceOffset + count <= data.Length);
        Debug.Assert(targetOffset + count <= Capacity);
        fixed (T* ptr = data)
            glNamedBufferSubData(Id, ElementSize * targetOffset, ElementSize * count, ptr + sourceOffset);
    }
    void Dispose (bool _) {
        if (!disposed) {
            DeleteBuffers(new int[] { Id });
            disposed = true;
        }
    }

    public void Dispose () {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
}
