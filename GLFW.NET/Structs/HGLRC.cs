namespace GLFW;

using System.Runtime.InteropServices;


[StructLayout(LayoutKind.Sequential)]
public struct HGLRC:IEquatable<HGLRC> {
    public static readonly HGLRC None;
    private readonly IntPtr handle;
    public bool Equals (HGLRC other) => handle.Equals(other.handle);
    public override bool Equals (object obj) => obj is HGLRC hglrc && Equals(hglrc);
    public override int GetHashCode () => handle.GetHashCode();
    public static bool operator == (HGLRC left, HGLRC right) => left.Equals(right);
    public static bool operator != (HGLRC left, HGLRC right) => !left.Equals(right);
    public override string ToString () => handle.ToString();
    public static implicit operator IntPtr (HGLRC hglrc) => hglrc.handle;
}
