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
    struct TimePoint {
        public long ticks;
        public int x, y;
    }

    unsafe private static void Proc () {
        const int capacity = 10000;
        var events = new TimePoint[capacity];

        var x = 0;
        var y = 0;
        var px = 0;
        var py = 0;
        CursorPosition.Get(ref px, ref py);
        var index = 0;
        while (run && index < capacity) {
            var t = Stopwatch.GetTimestamp();
            CursorPosition.Get(ref x, ref y);
            if (x != px || y != py) {
                events[index++] = new() { ticks = t, x = px - x, y = py - y };
                px = x;
                py = y;
            }
        }
        using var writer = new BinaryWriter(File.Create("movements.bin"));
        foreach (var e in events) {
            if (e.ticks == 0)
                break;
            writer.Write(e.ticks);
            writer.Write(e.x);
            writer.Write(e.y);
        }
    }
    volatile static bool run = true;
    internal static void TestMouseInput () {
        var thread = new Thread(Proc);
        thread.Start();
        _ = Console.ReadLine();
        run = false;
        thread.Join();
    }
}
