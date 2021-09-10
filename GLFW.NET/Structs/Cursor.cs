namespace GLFW;

using System;
using System.Runtime.InteropServices;

[StructLayout(LayoutKind.Sequential)]
public struct Cursor:IEquatable<Cursor> {
    public static readonly Cursor None;
    private readonly IntPtr cursor;
    public bool Equals (Cursor other) => cursor.Equals(other.cursor);
    public override bool Equals (object obj) => obj is Cursor c && Equals(c);
    public override int GetHashCode () => cursor.GetHashCode();
    public static bool operator == (Cursor left, Cursor right) => left.Equals(right);
    public static bool operator != (Cursor left, Cursor right) => !left.Equals(right);
}
