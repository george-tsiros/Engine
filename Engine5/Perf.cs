#define __BINARY__

namespace Engine;
using System;
using System.IO;
using System.Runtime.CompilerServices;

sealed class Perf:IDisposable {
    private enum Kind:byte {
        Stamp,
        Enter,
        Leave,
    }
    /*
    Stamp:
    _    _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _
    Kind [   ticks     ] l 
    0    1 2 3 4 5 6 7 8 9 10 11 12


    [Kind.Stamp][int64][strlen][str]
    Enter:
    [Kind.Enter][int64][strlen][str]
    Leave:
    [Kind.Leave][int64]
    */
    public Perf (string filepath) => stream = File.Create(filepath);
#if !DEBUG
    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
#endif
    public void Log (long int64, string name) => Log(Kind.Stamp, int64, name);

#if !DEBUG
    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
#endif
    public void Enter (long int64, string name) => Log(Kind.Enter, int64, name);
#if !DEBUG
    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
#endif
    public void Leave (long int64) => Log(Kind.Leave, int64, null);

    private bool disposed;
    private readonly Stream stream;

#if !DEBUG
    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
#endif
    unsafe private void Log (Kind kind, long int64, string str) {
        const int int64_Offset = sizeof(byte); // 1
        const int length_Offset = int64_Offset + sizeof(long); //9
        const int string_Offset = length_Offset + sizeof(byte); //10
        var len = str?.Length ?? 0;
        Span<byte> bytes = stackalloc byte[len != 0 ? string_Offset + len : length_Offset];
        bytes[0] = (byte)kind;
        fixed (byte* p = &bytes[int64_Offset])
            *(long*)p = int64;
        if (len != 0) {
            bytes[length_Offset] = (byte)len;
            for (int i = 0, o = string_Offset; i < len; ++i, ++o)
                bytes[o] = (byte)str[i];
        }
        stream.Write(bytes);
    }

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
