namespace Engine;

using System.Diagnostics;
using System;
using System.IO;
using System.Text;

public enum DataType:byte {
    Null = 0,
    Byte,
    Int16,
    Int32,
    Int64,
    UInt16,
    UInt32,
    UInt64,
    Single,
    Double,
    String,
    Array,
}

public class FancyStream:IDisposable {
    private Stream stream;
    public FancyStream (string filepath) => stream = File.Create(filepath);
    public void Write (byte b) {
        stream.WriteByte((byte)DataType.Byte);
        stream.WriteByte(b);
    }

    unsafe public void Write (short i16) {
        stream.WriteByte((byte)DataType.Int16);
        Span<byte> bytes = stackalloc byte[sizeof(short)];
        fixed (byte* p = bytes)
            *(short*)p = i16;
        stream.Write(bytes);
    }

    unsafe public void Write (int i32) {
        stream.WriteByte((byte)DataType.Int32);
        Span<byte> bytes = stackalloc byte[sizeof(int)];
        fixed (byte* p = bytes)
            *(int*)p = i32;
        stream.Write(bytes);
    }

    unsafe public void Write (long i64) {
        stream.WriteByte((byte)DataType.Int64);
        Span<byte> bytes = stackalloc byte[sizeof(long)];
        fixed (byte* p = bytes)
            *(long*)p = i64;
        stream.Write(bytes);
    }

    unsafe public void Write (ushort u16) {
        stream.WriteByte((byte)DataType.UInt16);
        Span<byte> bytes = stackalloc byte[sizeof(ushort)];
        fixed (byte* p = bytes)
            *(ushort*)p = u16;
        stream.Write(bytes);
    }

    public void Write (uint u32) {
        stream.WriteByte((byte)DataType.UInt32);
        WriteRaw(u32);
    }

    unsafe private void WriteRaw (uint u32) {
        Span<byte> bytes = stackalloc byte[sizeof(uint)];
        fixed (byte* p = bytes)
            *(uint*)p = u32;
        stream.Write(bytes);
    }

    unsafe public void Write (ulong u64) {
        stream.WriteByte((byte)DataType.UInt64);
        Span<byte> bytes = stackalloc byte[sizeof(ulong)];
        fixed (byte* p = bytes)
            *(ulong*)p = u64;
        stream.Write(bytes);
    }

    public void Write (string s) {
        stream.WriteByte((byte)DataType.String);
        Debug.Assert(s.Length <= byte.MaxValue);
        var expected = (byte)s.Length;
        stream.WriteByte(expected);
        Span<byte> bytes = stackalloc byte[expected];
        var actual = Encoding.ASCII.GetBytes(s, bytes);
        Debug.Assert(actual == expected);
        stream.Write(bytes);
    }

    unsafe public void Write (float f32) {
        stream.WriteByte((byte)DataType.Single);
        Span<byte> bytes = stackalloc byte[sizeof(float)];
        fixed (byte* p = bytes)
            *(float*)p = f32;
        stream.Write(bytes);
    }

    unsafe public void Write (double f64) {
        stream.WriteByte((byte)DataType.Double);
        Span<byte> bytes = stackalloc byte[sizeof(double)];
        fixed (byte* p = bytes)
            *(double*)p = f64;
        stream.Write(bytes);
    }

    public void Write (ReadOnlySpan<byte> b) {
        stream.WriteByte((byte)DataType.Array);
        stream.WriteByte((byte)DataType.Byte);
        WriteRaw((uint)b.Length);
        if (b.Length > 0)
            stream.Write(b);
    }

    unsafe public void Write (ReadOnlySpan<short> i16) {
        stream.WriteByte((byte)DataType.Array);
        stream.WriteByte((byte)DataType.Int16);
        WriteRaw((uint)i16.Length);
        if (i16.Length > 0)
            fixed (short* p = i16)
                stream.Write(new ReadOnlySpan<byte>(p, i16.Length * sizeof(short)));
    }

    unsafe public void Write (ReadOnlySpan<int> i32) {
        stream.WriteByte((byte)DataType.Array);
        stream.WriteByte((byte)DataType.Int32);
        WriteRaw((uint)i32.Length);
        if (i32.Length > 0)
            fixed (int* p = i32)
                stream.Write(new ReadOnlySpan<byte>(p, i32.Length * sizeof(int)));
    }

    unsafe public void Write (ReadOnlySpan<long> i64) {
        stream.WriteByte((byte)DataType.Array);
        stream.WriteByte((byte)DataType.Int64);
        WriteRaw((uint)i64.Length);
        if (i64.Length > 0)
            fixed (long* p = i64)
                stream.Write(new ReadOnlySpan<byte>(p, i64.Length * sizeof(long)));
    }

    unsafe public void Write (ReadOnlySpan<ushort> u16) {
        stream.WriteByte((byte)DataType.Array);
        stream.WriteByte((byte)DataType.UInt16);
        WriteRaw((uint)u16.Length);
        if (u16.Length > 0)
            fixed (ushort* p = u16)
                stream.Write(new ReadOnlySpan<byte>(p, u16.Length * sizeof(ushort)));
    }

    unsafe public void Write (ReadOnlySpan<uint> u32) {
        stream.WriteByte((byte)DataType.Array);
        stream.WriteByte((byte)DataType.UInt32);
        WriteRaw((uint)u32.Length);
        if (u32.Length > 0)
            fixed (uint* p = u32)
                stream.Write(new ReadOnlySpan<byte>(p, u32.Length * sizeof(uint)));
    }

    unsafe public void Write (ReadOnlySpan<ulong> u64) {
        stream.WriteByte((byte)DataType.Array);
        stream.WriteByte((byte)DataType.UInt64);
        WriteRaw((uint)u64.Length);
        if (u64.Length > 0)
            fixed (ulong* p = u64)
                stream.Write(new ReadOnlySpan<byte>(p, u64.Length * sizeof(ulong)));
    }

    unsafe public void Write (ReadOnlySpan<float> f32) {
        stream.WriteByte((byte)DataType.Array);
        stream.WriteByte((byte)DataType.Single);
        WriteRaw((uint)f32.Length);
        if (f32.Length > 0)
            fixed (float* p = f32)
                stream.Write(new ReadOnlySpan<byte>(p, f32.Length * sizeof(float)));
    }

    unsafe public void Write (ReadOnlySpan<double> f64) {
        stream.WriteByte((byte)DataType.Array);
        stream.WriteByte((byte)DataType.Double);
        WriteRaw((uint)f64.Length);
        if (f64.Length > 0)
            fixed (double* p = f64)
                stream.Write(new ReadOnlySpan<byte>(p, f64.Length * sizeof(double)));
    }

    private void Dispose (bool _) {
        stream?.Dispose();
        stream = null;
    }

    public void Dispose () {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
}
