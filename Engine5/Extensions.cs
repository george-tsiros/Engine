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

    unsafe internal static void WriteRaw (this Stream self, int int32) {
        Span<byte> bytes = stackalloc byte[sizeof(int)];
        fixed (byte* p = bytes)
            *(int*)p = int32;
        self.Write(bytes);
    }
    unsafe internal static void WriteRaw (this Stream self, long int64) {
        Span<byte> bytes = stackalloc byte[sizeof(long)];
        fixed (byte* p = bytes)
            *(long*)p = int64;
        self.Write(bytes);
    }
}