namespace Engine;

using System;
using System.IO;
using System.Diagnostics;
using System.Runtime.InteropServices;

class Engine {

    [STAThread]
    static void Main () {
        Debug.Assert(Marshal.SizeOf<WindowsStuff.OsVersionInfoExW>() == 5 * sizeof(int) + 128 * sizeof(ushort) + 3 * sizeof(short) + 2 * sizeof(byte));


        if (File.Exists("hints.txt"))
            Extra.SetHintsFrom("hints.txt");
        using var f = new NeinCraft(GLFW.Glfw.PrimaryMonitor);
        f.Run();
    }
}
