namespace GLFW;

using System.Runtime.InteropServices;
using System.Security;
[SuppressUnmanagedCodeSecurity]
public static class Native {
    [DllImport(Glfw.LIBRARY, EntryPoint = "glfwGetWGLContext", CallingConvention = CallingConvention.Cdecl)]
    public static extern HGLRC GetWglContext (Window window);

    [DllImport(Glfw.LIBRARY, EntryPoint = "glfwGetWin32Window", CallingConvention = CallingConvention.Cdecl)]
    public static extern IntPtr GetWin32Window (Window window);

    [DllImport(Glfw.LIBRARY, EntryPoint = "glfwGetWin32Adapter", CallingConvention = CallingConvention.Cdecl)]
    private static extern IntPtr GetWin32AdapterInternal (Monitor monitor);

    [DllImport(Glfw.LIBRARY, EntryPoint = "glfwGetWin32Monitor", CallingConvention = CallingConvention.Cdecl)]
    private static extern IntPtr GetWin32MonitorInternal (Monitor monitor);

    /// <summary>
    ///     Gets the win32 adapter.
    /// </summary>
    /// <param name="monitor">A monitor instance.</param>
    /// <returns>dapter device name (for example \\.\DISPLAY1) of the specified monitor, or <c>null</c> if an error occurred.</returns>
    public static string GetWin32Adapter (Monitor monitor) => Util.PtrToStringUni(GetWin32AdapterInternal(monitor));

    /// <summary>
    ///     Returns the display device name of the specified monitor
    /// </summary>
    /// <param name="monitor">A monitor instance.</param>
    /// <returns>
    ///     The display device name (for example \\.\DISPLAY1\Monitor0) of the specified monitor, or <c>null</c> if an
    ///     error occurred.
    /// </returns>
    public static string GetWin32Monitor (Monitor monitor) => Util.PtrToStringUni(GetWin32MonitorInternal(monitor));
}
