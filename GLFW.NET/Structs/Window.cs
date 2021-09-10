namespace GLFW;

using System.Runtime.InteropServices;


[StructLayout(LayoutKind.Sequential)]
public struct Window:IEquatable<Window> {
    public static readonly Window None;
    private readonly IntPtr handle;
    public bool Equals (Window other) => handle.Equals(other.handle);
    public override bool Equals (object obj) => obj is Window w && Equals(w);
    public override int GetHashCode () => handle.GetHashCode();
    public static bool operator == (Window left, Window right) => left.Equals(right);
    public static bool operator != (Window left, Window right) => !left.Equals(right);
    public override string ToString () => handle.ToString();

    public static implicit operator IntPtr (Window window) { return window.handle; }
    public static explicit operator Window (IntPtr handle) => new Window(handle);
    public Window (IntPtr handle) => this.handle = handle;

    /// <summary>Gets or sets the opacity of the window in the range of <c>0.0f</c> and <c>1.0f</c> inclusive.</summary>
    public float Opacity {
        get => Glfw.GetWindowOpacity(handle);
        set => Glfw.SetWindowOpacity(handle, Math.Min(1.0f, Math.Max(0.0f, value)));
    }
}