namespace Engine;

using GLFW;
using System;
using System.Threading;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;

static class CursorPosition {

    [DllImport("user32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private unsafe static extern bool GetCursorPos (Point* lpPoint);

    [StructLayout(LayoutKind.Sequential)]
    struct Point { public int x, y; }
    private static Point p;
    unsafe public static void Get (ref int x, ref int y) {
        fixed (Point* pp = &p)
            _ = GetCursorPos(pp);
        x = p.x;
        y = p.y;
    }
}

class MouseInputTest {
    private static void Kk (Window window, Keys key, int scanCode, InputState state, ModifierKeys mods) {
        if (state == InputState.Release && key == Keys.Escape)
            Glfw.SetWindowShouldClose(window, true);
    }

    struct TimePoint {
        public long ticks;
        public int x, y;
    }

    private readonly TimePoint[] events = new TimePoint[10000];
    unsafe private void Proc () {

        var x = 0;
        var y = 0;
        var px = 0;
        var py = 0;
        CursorPosition.Get(ref px, ref py);
        var index = 0;
        while (run && index < 10000) {
            var t = Stopwatch.GetTimestamp();
            CursorPosition.Get(ref x, ref y);
            if (x != px || y != py) {
                events[index++] = new() { ticks = t, x = px - x, y = py - y };
                px = x;
                py = y;
            }
        }
    }
    volatile bool run = true;
    private readonly KeyCallback kk = Kk;
    private Window window;
    private void TestMouseInput () {
        Glfw.WindowHint(Hint.Decorated, 1);
        window = Glfw.CreateWindow(100, 100, "", GLFW.Monitor.None, Window.None);
        Glfw.ShowWindow(window);
        Glfw.MakeContextCurrent(window);
        _ = Glfw.SetKeyCallback(window, kk);
        Glfw.SwapInterval(1);
        Glfw.SetInputMode(window, InputMode.RawMouseMotion, 1);
        Glfw.SetInputMode(window, InputMode.Cursor, (int)CursorMode.Disabled);
        var t0 = Stopwatch.GetTimestamp();
        using var log = new BinaryWriter(File.Create("log.txt"));
        var thread = new Thread(Proc);
        thread.Start();
        Gl.Calls.ClearColor(0.1f, 0.1f, 0.1f, 1f);
        while (!Glfw.WindowShouldClose(window)) {
            Glfw.PollEvents();
            Gl.Calls.Clear(Gl.BufferBit.Color);
            Glfw.SwapBuffers(window);
            var t1 = Stopwatch.GetTimestamp();
            log.Write(t1 - t0);
            t0 = t1;
        }
        Glfw.SetInputMode(window, InputMode.Cursor, (int)CursorMode.Normal);
        Glfw.SetInputMode(window, InputMode.RawMouseMotion, 0);
        run = false;
        thread.Join();
        Glfw.DestroyWindow(window);
        using var writer = new BinaryWriter(File.Create("movements.bin"));
        foreach (var x in events) {
            if (x.ticks == 0)
                break;
            writer.Write(x.ticks);
            writer.Write(x.x);
            writer.Write(x.y);
        }
    }
}
