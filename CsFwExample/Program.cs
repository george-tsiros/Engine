namespace CsFwExample {
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Drawing;
    using System.Windows.Forms;
    using System.Collections.Generic;

    class Program:Form {
        private static IEnumerable<TimePoint> FromFile (string filename) {
            using (var f = File.OpenRead("movements.bin")) {
                f.Seek(sizeof(long), SeekOrigin.Begin);
                while (f.Position != f.Length)
                    yield return TimePoint.FromFileStream(f);
            }
        }

        private int count;
        private List<TimePoint> events = new List<TimePoint>();
        private Point[] points;
        private Program () {        }
        private long timerFrequency;
        private TrackBar slider;
        protected override void OnLoad (EventArgs _) {
            ClientSize = new Size(1000, 1000);
            events.AddRange(FromFile("movements.bin"));

            using (var f = File.OpenRead("movements.bin")) {
                var bytes = new byte[sizeof(long)];
                var read = f.Read(bytes, 0, sizeof(long));
                if (read != sizeof(long))
                    throw new ApplicationException();
                timerFrequency = BitConverter.ToInt64(bytes, 0);
            }

            var (minx, maxx, miny, maxy, x, y) = (0, 0, 0, 0, 0, 0);
            count = events.Count;
            foreach (var d in events) {
                x += d.dx;
                y += d.dy;
                if (x < minx)
                    minx = x;
                if (maxx < x)
                    maxx = x;
                if (y < miny)
                    miny = y;
                if (maxy < y)
                    maxy = y;
            }
            var (w, h) = (maxx - minx, maxy - miny);
            points = new Point[count];
            (x, y) = (0, 0);
            for (var i = 0; i < count; i++) {
                var d = events[i];
                x += d.dx;
                y += d.dy;
                points[i] = new Point(x - minx, y - miny);
            }
            slider = new TrackBar();
            slider.Location = new Point(slider.Margin.Left, slider.Margin.Top);
            slider.Width = ClientSize.Width - slider.Margin.Horizontal;
            slider.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            slider.Minimum = 0;
            slider.Maximum = count - 1;
            Controls.Add(slider);
        }
        protected override void OnPaint (PaintEventArgs e) {
            e.Graphics.Clear(Color.Black);
            
            e.Graphics.DrawLines(Pens.White, points);
            base.OnPaint(e);
        }

        unsafe private static void Proc () {
            const int capacity = 10000;
            var events = new TimePoint[capacity];

            CursorPosition.Get(out var px, out var py);
            var index = 0;
            while (run && index < capacity) {
                var t = Stopwatch.GetTimestamp();
                CursorPosition.Get(out var x, out var y);
                var (dx, dy) = (px - x, py - y);
                if (dx != 0 || dy != 0) {
                    events[index++] = new TimePoint() { ticks = t, dx = dx, dy = dy };
                    px = x;
                    py = y;
                }
            }
            using (var writer = new BinaryWriter(File.Create("movements.bin"))) {
                writer.Write(Stopwatch.Frequency);
                foreach (var e in events) {
                    if (e.ticks == 0)
                        break;
                    writer.Write(e.ticks);
                    writer.Write(e.dx);
                    writer.Write(e.dy);
                }
            }
        }
        volatile static bool run = true;
        internal static void Main () {
            const double framerate = 100.0;
            var performanceCounterFrequency = (double)Stopwatch.Frequency;
            var ticksPerFrame = performanceCounterFrequency / framerate;
            using (var f = File.OpenRead("movements.bin")) {
                var bytes = new byte[sizeof(long)];
                var read = f.Read(bytes, 0, sizeof(long));
                if (read != sizeof(long))
                    throw new ApplicationException();
                var timerFrequency = BitConverter.ToInt64(bytes, 0);
                Debug.Assert(timerFrequency == Stopwatch.Frequency);
            }
            var events = new List<TimePoint>(FromFile("movements.bin"));
            Debug.Assert(events.Count != 0);
            var t0 = events[0].ticks;
            var frametime = 0.0;
            var i = 0;
            for (; ; ) {
                if (i >= events.Count)
                    break;
                var e = events[i];
                var deltaTicks = e.ticks - t0;

            }

            //var thread = new Thread(Proc);
            //thread.Start();
            //_ = Console.ReadLine();
            //run = false;
            //thread.Join();
            Application.Run(new Program());
        }
    }
}