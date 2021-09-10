namespace GLFW;

using System;
using System.Runtime.InteropServices;

internal static class Util {
    public static string PtrToStringUni (IntPtr ptr) => ptr != IntPtr.Zero ? Marshal.PtrToStringUni(ptr) : "";
}
