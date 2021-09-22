namespace CsFwExample {
    using System.Runtime.InteropServices;
    using System.Drawing;

    static class CursorPosition {

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private unsafe static extern bool GetCursorPos (Point* lpPoint);

        private static Point p;
        unsafe public static void Get (out int x, out int y) {
            fixed (Point* pp = &p)
                _ = GetCursorPos(pp);
            x = p.X;
            y = p.Y;
        }
    }
}