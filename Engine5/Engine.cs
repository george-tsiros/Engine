namespace Engine;

using System;
using System.IO;
using System.Diagnostics;
using System.Runtime.InteropServices;

class Engine {

    [STAThread]
    static void Main () {
        //MouseInputTest.TestMouseInput();

        if (File.Exists("hints.txt"))
            Extra.SetHintsFrom("hints.txt");
        using var f = new TextureTest(1280,720);
        f.Run();
    }
}
