namespace CsFwExample {
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Drawing;
    using System.Windows.Forms;
    using System.Collections.Generic;
    using System.Threading;

    class Program:Form {
        private static IEnumerable<TimePoint> FromFile (string filename) {
            using (var f = File.OpenRead(filename)) {
                _ = f.Seek(sizeof(long), SeekOrigin.Begin);
                while (f.Position != f.Length)
                    yield return TimePoint.FromFileStream(f);
            }
        }

        private int count;
        private List<TimePoint> events = new List<TimePoint>();
        private Point[] points;
        private Program () { }
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

            var (minx, maxx, miny, maxy) = (int.MaxValue, int.MinValue, int.MaxValue, int.MinValue);
            count = events.Count;
            foreach (var d in events) {
                minx = Math.Min(minx, d.x);
                miny = Math.Min(miny, d.y);
                maxx = Math.Max(maxx, d.x);
                maxy = Math.Max(maxy, d.y);
            }
            var (w, h) = (maxx - minx, maxy - miny);
            points = new Point[count];
            for (var i = 0; i < count; i++) {
                var e = events[i];
                points[i] = new Point(e.x - minx, e.y - miny);
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

            var previousLocation = CursorPosition.Get();
            var index = 0;
            while (run && index < capacity) {
                var t = Stopwatch.GetTimestamp();
                var currentLocation = CursorPosition.Get();
                if (previousLocation != currentLocation) {
                    events[index++] = new TimePoint() { ticks = t, x = currentLocation.X, y = currentLocation.Y };
                    previousLocation = currentLocation;
                }
            }
            using (var writer = new BinaryWriter(File.Create("movements.bin"))) {
                writer.Write(Stopwatch.Frequency);
                foreach (var e in events) {
                    if (e.ticks == 0)
                        break;
                    writer.Write(e.ticks);
                    writer.Write(e.x);
                    writer.Write(e.y);
                }
            }
        }
        volatile static bool run = true;
        internal static void Main () {
            //const double framerate = 100.0;
            //var ticksPerSecond = (double)Stopwatch.Frequency;
            //var ticksPerFrame = ticksPerSecond / framerate;
            //var secondsPerFrame = 1.0 / framerate;
            //using (var f = File.OpenRead("movements.bin")) {
            //    var bytes = new byte[sizeof(long)];
            //    var read = f.Read(bytes, 0, sizeof(long));
            //    if (read != sizeof(long))
            //        throw new ApplicationException();
            //    var timerFrequency = BitConverter.ToInt64(bytes, 0);
            //    Debug.Assert(timerFrequency == Stopwatch.Frequency);
            //}

            //var events = new List<TimePoint>(FromFile("movements.bin"));
            //Debug.Assert(events.Count != 0);
            //var t0 = events[0].ticks;
            //var frametime = 0.0;
            //var (px, py) = (0, 0);
            //var cursorAtFrame = new List<Point>();
            //foreach (var e in events) {
            //    var deltaTicks = e.ticks - t0;
            //    frametime += deltaTicks / ticksPerSecond;
            //    if (frametime >= secondsPerFrame) {
            //        cursorAtFrame.Add(new Point(px, py));
            //        frametime -= secondsPerFrame;
            //    }
            //    (px, py) = (px + e.dx, py + e.dy);
            //}

            //var thread = new Thread(Proc);
            //thread.Start();
            //_ = Console.ReadLine();
            //run = false;
            //thread.Join();
            Application.Run(new Program());
        }
    }
}