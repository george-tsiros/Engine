namespace Engine;

using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Reflection;

static class Extensions {
    internal static FieldInfo GetEnumFieldInfo (this Assembly self, string s) {
        var parts = s.Split('.');
        if (parts.Length != 2)
            return null;
        const BindingFlags publicStaticIgnoreCase = BindingFlags.Public | BindingFlags.Static | BindingFlags.IgnoreCase;
        return self.GetType($"GLFW.{parts[0]}", false, true)?.GetField(parts[1], publicStaticIgnoreCase);
    }

    internal static bool TryMatch (this Regex self, string input, out Match match) {
        var m = self.Match(input);
        match = m.Success ? m : null;
        return m.Success;
    }

    internal static void Write (this Stream self, long int64) {
        Span<byte> bytes = stackalloc byte[20];
        var offset = Extra.ToChars(int64, bytes);
        self.Write(bytes.Slice(offset));
    }

    internal static void Write (this Stream self, string value) {
        var maxByteCount = Encoding.ASCII.GetMaxByteCount(value.Length);
        Span<byte> bytes = stackalloc byte[maxByteCount + 1];
        var count = Encoding.ASCII.GetBytes(value, bytes.Slice(1));
        bytes[0] = (byte)count;
        self.Write(bytes.Slice(0, count + 1));
    }
}