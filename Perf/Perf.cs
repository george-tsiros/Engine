namespace Perf {
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Drawing.Drawing2D;
    using System.Drawing.Imaging;
    using System.IO;
    using System.Diagnostics;
    using System.Windows.Forms;

    class Perf:Form {
        private const int _IMAGE_WIDTH_ = 2000;
        private const int _SECTION_HEIGHT_ = 20;
        private readonly List<Entry> Entries = new List<Entry>();
        private readonly string[] Names;
        private readonly int[] Values;
        private readonly List<int> FrameIndices = new List<int>();
        private string NameOf (int value) {
            var i = Array.IndexOf(Values, value);
            return 0 <= i && i < Names.Length ? Names[i] : throw new ApplicationException($"{value} not one of {string.Join(", ", Values)}");
        }
        private int ValueOf (string name) {
            var i = Array.IndexOf(Names, name);
            return 0 <= i && i < Values.Length ? Values[i] : throw new ApplicationException($"{name} not one of {string.Join(", ", Names)}");
        }

        private Perf (string filepath) {
            using (var stream = File.OpenRead(filepath)) {
                var enumCount = stream.GetInt32();
                Names = new string[enumCount];
                Values = new int[enumCount];
                for (var i = 0; i < Names.Length; ++i) {
                    Values[i] = stream.GetByte();
                    Names[i] = stream.GetString();
                }
                while (Entry.FromStream(stream) is Entry entry)
                    Entries.Add(entry);
            }

            if (Entries.Count == 0) {
                Text = "no entries";
                return;
            }

            var depth = 0;

            for (var i = 0; i < Entries.Count; i++) {
                var entry = Entries[i];
                var isEnter = entry.IsEnter;
                if (isEnter && depth == 0)
                    FrameIndices.Add(i);
                depth += isEnter ? 1 : -1;
            }
            Load += Load_self;
        }
        private PictureBox picture;
        private IntegralUpDown frameNum;

        private void Load_self (object sender, EventArgs args) {
            Load -= Load_self;
            ClientSize = new Size(_IMAGE_WIDTH_ + 50, 500);
            Location = new Point(10, 10);
            frameNum = new IntegralUpDown() { Width = 200, Minimum = 1, Maximum = FrameIndices.Count, Value = 1 };
            Controls.Add(frameNum);
            picture = new PictureBox() { Location = new Point(10, 50), ClientSize = new Size(_IMAGE_WIDTH_, 200) };
            Controls.Add(picture);
            frameNum.ValueChanged += ValueChanged_frameNum;
        }


        private static readonly (int r, int g, int b)[] colors = {
            (200, 131, 65),
            (142, 92, 201),
            (100, 172, 72),
            (210, 69, 149),
            (74, 172, 141),
            (206, 76, 58),
            (98, 134, 202),
            (154, 150, 63),
            (192, 125, 190),
            (192, 94, 111),
        };
        private readonly Brush[] brushes = Array.ConvertAll(colors, c => new SolidBrush(Color.FromArgb(c.r, c.g, c.b)));
        private void ValueChanged_frameNum (object sender, EventArgs args) {
            var frameIndex = frameNum.Value - 1;
            Debug.Assert(0 <= frameIndex && frameIndex < Entries.Count);
            Text = $"frame {frameNum.Value} first event index: {FrameIndices[frameIndex]}";
            var previousImage = picture.Image;
            var (totalDuration, maxDepth) = DurationAndMaxDepthOfFrame(frameIndex);
            var factor = (double)_IMAGE_WIDTH_ / totalDuration; // pixels/tick
            var newBitmap = new Bitmap(_IMAGE_WIDTH_, maxDepth * _SECTION_HEIGHT_);
            using (var g = Graphics.FromImage(newBitmap)) {
                g.Clear(Color.Black);
                g.SmoothingMode = SmoothingMode.None;
                g.InterpolationMode = InterpolationMode.NearestNeighbor;
                var eventIndex = FrameIndices[frameIndex];
                var t0 = Entries[eventIndex].Ticks;
                var stk = new Stack<Entry>();
                brushIndex = 0;
                do {
                    var entry = Entries[eventIndex++];
                    if (entry.IsEnter) {
                        stk.Push(entry);
                    } else {
                        var entering = stk.Pop();
                        var x0 = (int)((entering.Ticks - t0) * factor);
                        var width = (int)((entry.Ticks - entering.Ticks) * factor);
                        var y0 = stk.Count * _SECTION_HEIGHT_;
                        g.FillRectangle(NextBrush, x0, y0, width, _SECTION_HEIGHT_);

                    }
                } while (stk.Count != 0);
            }
            picture.ClientSize = new Size(_IMAGE_WIDTH_, newBitmap.Height);
            picture.Image = newBitmap;

            previousImage?.Dispose();
        }

        private int brushIndex;
        private Brush NextBrush {
            get {
                ++brushIndex;
                if (brushIndex == brushes.Length)
                    brushIndex = 0;
                return brushes[brushIndex];
            }
        }

        private (long duration, int maxDepth) DurationAndMaxDepthOfFrame (int frameIndex) {
            var maxDepth = 0;
            var depth = 0;
            var eventIndex = FrameIndices[frameIndex];
            var t0 = Entries[eventIndex].Ticks;
            do {
                var isEnter = Entries[eventIndex++].IsEnter;
                depth += isEnter ? +1 : -1;
                maxDepth = Math.Max(depth, maxDepth);
            } while (depth != 0);
            return (Entries[eventIndex - 1].Ticks - t0, maxDepth);
        }

        class Entry {
            public readonly long Ticks;
            public readonly int E;
            private Entry (long ticks, int e) => (Ticks, E) = (ticks, e);
            public static Entry FromStream (Stream stream) => stream.TryRead(out long l) && stream.TryRead(out byte b) ? new Entry(l, b) : null;
            public bool IsEnter => (E & 0x80) == 0;
        }

        [STAThread]
        static void Main (string[] args) {
            Application.EnableVisualStyles();
            using (var f = new Perf(args[0]))
                _ = f.ShowDialog();
        }
    }
}
