namespace Engine;

using GLFW;
using System;
using System.IO;

class Engine {
    [STAThread]
    static void Main () {
        if (File.Exists("hints.txt"))
            Extra.SetHintsFrom("hints.txt");
        using var gl = new NeinCraft(Glfw.PrimaryMonitor);
        gl.Run();
    }
}
