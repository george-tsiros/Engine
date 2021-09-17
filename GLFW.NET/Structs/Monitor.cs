namespace GLFW;
using System;

using System.Drawing;
using System.Runtime.InteropServices;

[StructLayout(LayoutKind.Sequential)]
public struct Monitor:IEquatable<Monitor> {
    public static readonly Monitor None;
    private readonly IntPtr handle;
    public bool Equals (Monitor other) => handle.Equals(other.handle);
    public override bool Equals (object ob) => ob is Monitor m && Equals(m);
    public override int GetHashCode () => handle.GetHashCode();
    public static bool operator == (Monitor left, Monitor right) => left.Equals(right);
    public static bool operator != (Monitor left, Monitor right) => !left.Equals(right);
    public override string ToString () => handle.ToString();

    /// <summary>
    ///     Gets the position, in screen coordinates of the valid work are for the monitor.
    /// </summary>
    /// <seealso cref="Glfw.GetMonitorWorkArea" />
    public Rectangle WorkArea {
        get {
            Glfw.GetMonitorWorkArea(handle, out var x, out var y, out var width, out var height);
            return new Rectangle(x, y, width, height);
        }
    }

    /// <summary>
    ///     Gets the content scale of this monitor.
    ///     <para>The content scale is the ratio between the current DPI and the platform's default DPI.</para>
    /// </summary>
    /// <seealso cref="Glfw.GetMonitorContentScale" />

    public PointF ContentScale {
        get {
            Glfw.GetMonitorContentScale(handle, out var x, out var y);
            return new PointF(x, y);
        }
    }

    /// <summary>
    ///     Gets or sets a user-defined pointer to associate with the window.
    /// </summary>
    /// <seealso cref="Glfw.GetMonitorUserPointer" />
    /// <seealso cref="Glfw.SetMonitorUserPointer" />
    public IntPtr UserPointer {
        get => Glfw.GetMonitorUserPointer(handle);
        set => Glfw.SetMonitorUserPointer(handle, value);
    }
}
