#define __BINARY__

namespace Engine;
using System;
using System.Diagnostics;
using System.IO;
#if !DEBUG
using System.Runtime.CompilerServices;
#endif
using System.Text;

sealed class Perf<T>:IDisposable where T : struct, Enum {

    public Perf (string filepath) {
        if (typeof(T).GetEnumUnderlyingType() != typeof(int))
            throw new ApplicationException($"enum {typeof(T).Name} has underlying type {typeof(T).GetEnumUnderlyingType().Name}, expected {typeof(int).Name} ");
        stream = File.Create(filepath);
        var names = Enum.GetNames<T>();
        stream.WriteRaw(names.Length);
        foreach (var name in names) {
            stream.WriteByte((byte)(int)Enum.Parse(typeof(T), name));
            stream.WriteRaw(name.Length);
            stream.Write(Encoding.ASCII.GetBytes(name));
        }
    }

#if !DEBUG
    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
#endif
    unsafe public void Leave () {
        Span<byte> bytes = stackalloc byte[sizeof(long) + sizeof(byte)];
        fixed (byte* p = bytes) {
            *(long*)p = Stopwatch.GetTimestamp();
            p[sizeof(long)] = 0;
        }
        stream.Write(bytes);
    }
#if !DEBUG
    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
#endif
    unsafe public void Enter (int id) {
        Span<byte> bytes = stackalloc byte[sizeof(long) + sizeof(byte)];
        fixed (byte* p = bytes) {
            *(long*)p = Stopwatch.GetTimestamp();
            p[sizeof(long)] = (byte)id;
        }
        stream.Write(bytes);
    }

    private bool disposed;
    private readonly Stream stream;

    private void Dispose (bool disposing) {
        if (disposed)
            return;
        if (disposing)
            stream.Dispose();
        disposed = true;
    }

    public void Dispose () {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
}
