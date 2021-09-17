namespace GLFW;

using System;
using System.Runtime.InteropServices;

[StructLayout(LayoutKind.Sequential)]
public struct Image {
    public readonly int Width, Height;
    public readonly IntPtr Pixels;
    public Image (int width, int height, IntPtr pixels) => (Width, Height, Pixels) = (width, height, pixels);
}
