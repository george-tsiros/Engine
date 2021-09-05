namespace Engine {
    using System;

    sealed class Engine {
        [STAThread]
        static void Main () {
            using var gl = new NeinCraft(1280, 720);
            //using var gl = new Boxes(1280, 720);
            gl.Run();
        }
    }
}
