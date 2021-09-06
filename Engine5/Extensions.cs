namespace Engine;

using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

public static class Extensions {
    public static bool TryMatch (this Regex self, string input, out Match match) {
        var m = self.Match(input);
        match = m.Success ? m : null;
        return m.Success;

    }
    public static unsafe void Write (this Stream self, ulong value) {
        Span<byte> bytes = stackalloc byte[sizeof(ulong)];
        fixed (byte* b = bytes)
            *(ulong*)b = value;
        self.Write(bytes);
    }
    public static unsafe void Write (this Stream self, long value) {
        Span<byte> bytes = stackalloc byte[sizeof(long)];
        fixed (byte* b = bytes)
            *(long*)b = value;
        self.Write(bytes);
    }
    public static unsafe void Write (this Stream self, int value) {
        Span<byte> bytes = stackalloc byte[sizeof(int)];
        fixed (byte* b = bytes)
            *(int*)b = value;
        self.Write(bytes);
    }
    public static void Write (this Stream self, string value) {
        var maxByteCount = Encoding.ASCII.GetMaxByteCount(value.Length);
        Span<byte> bytes = stackalloc byte[maxByteCount + 1];
        var count = Encoding.ASCII.GetBytes(value, bytes.Slice(1));
        bytes[0] = (byte)count;
        self.Write(bytes.Slice(0, count + 1));
    }
    public static int ReadInt32 (Stream self) {
        Span<byte> bytes = stackalloc byte[sizeof(int)];
        _ = self.Read(bytes);
        return BitConverter.ToInt32(bytes);
    }
    public static ulong ReadUInt64 (Stream self) {
        Span<byte> bytes = stackalloc byte[sizeof(ulong)];
        _ = self.Read(bytes);
        return BitConverter.ToUInt64(bytes);
    }
    public static string ReadString (Stream self) {
        var expected = self.ReadByte();
        if (expected < 0)
            throw new Exception("expected a byte");
        if (expected == 0)
            return "";
        if (expected > byte.MaxValue)
            throw new Exception($"did not expect {expected}");
        Span<byte> bytes = stackalloc byte[expected];
        var actual = self.Read(bytes);

        return expected == actual ? Encoding.ASCII.GetString(bytes) : throw new Exception($"expected {expected} bytes but could only read {actual}");
    }
}
