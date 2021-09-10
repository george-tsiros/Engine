namespace Engine;
using System;
using System.IO;
using System.Text;

sealed class Perf:IDisposable {
#if !DEBUG
    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
#endif
    private static void PushAscii (Span<byte> a, ref long int64, ref int offset) {
        int64 = Math.DivRem(int64, 10, out var d);
        a[--offset] = (byte)(d + '0');
    }
#if !DEBUG
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
#endif
    private static int ToChars (long int64, Span<byte> bytes) {
        var isNegative = int64 < 0l;
        if (isNegative)
            int64 = -int64;
        var offset = 20;
        do
            PushAscii(bytes, ref int64, ref offset);
        while (int64 != 0);
        if (isNegative)
            bytes[--offset] = (byte)'-';
        return offset;
    }

    private bool disposed;
    private readonly Stream stream;
    public Perf (string filepath) {
        stream = File.Create(filepath);
    }
#if !DEBUG
    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
#endif
    public void Log (long int64, string str) {
        var l = str.Length;
        Span<byte> bytes = stackalloc byte[l + 22];
        bytes[20] = (byte)' ';
        bytes[l + 21] = (byte)'\n';
        var offset = ToChars(int64, bytes);
        _ = Encoding.ASCII.GetBytes(str, bytes.Slice(21));
        stream.Write(bytes.Slice(offset));
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
