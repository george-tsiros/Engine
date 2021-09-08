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

    public static void Write (this Stream self, long int64) {
        Span<byte> bytes = stackalloc byte[20];
        var offset = Extra.ToChars(int64, bytes);
        self.Write(bytes.Slice(offset));
    }

    public static void Write (this Stream self, string value) {
        var maxByteCount = Encoding.ASCII.GetMaxByteCount(value.Length);
        Span<byte> bytes = stackalloc byte[maxByteCount + 1];
        var count = Encoding.ASCII.GetBytes(value, bytes.Slice(1));
        bytes[0] = (byte)count;
        self.Write(bytes.Slice(0, count + 1));
    }
}