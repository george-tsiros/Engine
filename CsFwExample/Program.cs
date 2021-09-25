namespace CsFwExample {
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Drawing;
    using System.Drawing.Imaging;
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
        private Program () {
            //SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.OptimizedDoubleBuffer, true);
        }
        private long timerFrequency;
        private TrackBar slider;

        private static Rectangle BoundingRectangle (IEnumerable<TimePoint> points) {
            var (minx, maxx, miny, maxy) = (int.MaxValue, int.MinValue, int.MaxValue, int.MinValue);
            foreach (var d in points) {
                minx = Math.Min(minx, d.p.X);
                miny = Math.Min(miny, d.p.Y);
                maxx = Math.Max(maxx, d.p.X);
                maxy = Math.Max(maxy, d.p.Y);
            }
            return new Rectangle(minx, miny, maxx - minx, maxy - miny);
        }

        protected override void OnLoad (EventArgs _) {
            ClientSize = new Size(1000, 1000);
            var b = new Bitmap(ClientSize.Width, ClientSize.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            using (var g = Graphics.FromImage(b))
                g.Clear(Color.Black);
            BackgroundImage = b;
            BackgroundImageLayout = ImageLayout.Center;
            Debug.Assert(ReferenceEquals(b, BackgroundImage));
            FormBorderStyle = FormBorderStyle.FixedSingle;

            events.AddRange(FromFile("movements.bin"));

            using (var f = File.OpenRead("movements.bin")) {
                var bytes = new byte[sizeof(long)];
                var read = f.Read(bytes, 0, sizeof(long));
                if (read != sizeof(long))
                    throw new ApplicationException();
                timerFrequency = BitConverter.ToInt64(bytes, 0);
            }

            count = events.Count;
            var boundingRect = BoundingRectangle(events);

            points = new Point[count];
            for (var i = 0; i < count; i++)
                points[i] = events[i].p - (Size)boundingRect.Location;

            slider = new TrackBar();
            slider.Location = new Point(slider.Margin.Left, slider.Margin.Top);
            slider.Width = ClientSize.Width - slider.Margin.Horizontal;
            slider.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            slider.Minimum = 0;
            slider.Maximum = count - 1;
            Controls.Add(slider);
            Invalidate();
        }
        unsafe protected override void OnPaint (PaintEventArgs args) {
            Debug.Assert(BackgroundImage is Bitmap);
            var t0 = Stopwatch.GetTimestamp();
            using (var g = Graphics.FromImage(BackgroundImage))
                g.Clear(Color.Black);
            var b = (Bitmap)BackgroundImage;
            var l = b.LockBits(new Rectangle(0, 0, b.Width, b.Height), ImageLockMode.WriteOnly, b.PixelFormat);
            Debug.Assert(l.Stride == 4 * l.Width);
            var p = (uint*)l.Scan0.ToPointer();
            var w = l.Width;
            foreach (var pt in points) 
                p[w * pt.Y + pt.X] = ~0u;
            var t1 = Stopwatch.GetTimestamp();
            Debug.WriteLine((t1 - t0) / (double)Stopwatch.Frequency);
            b.UnlockBits(l);
            base.OnPaint(args);
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
                    events[index++] = new TimePoint() { ticks = t, p = currentLocation };
                    previousLocation = currentLocation;
                }
            }
            using (var writer = new BinaryWriter(File.Create("movements.bin"))) {
                writer.Write(Stopwatch.Frequency);
                foreach (var e in events) {
                    if (e.ticks == 0)
                        break;
                    writer.Write(e.ticks);
                    writer.Write(e.p.X);
                    writer.Write(e.p.Y);
                }
            }
        }


        private volatile static bool run = true;
        private static void Main () {

            //var thread = new Thread(Proc);
            //thread.Start();
            //_ = Console.ReadLine();
            //run = false;
            //thread.Join();
            Application.Run(new Program());
        }
    }
}