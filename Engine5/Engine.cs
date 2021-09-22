namespace Engine;

using System;
using System.IO;

class Engine {

    [STAThread]
    static void Main () {
        if (File.Exists("hints.txt"))
            Extra.SetHintsFrom("hints.txt");
        using var f = new TextureTest(1280,720);
        f.Run();
    }
}
