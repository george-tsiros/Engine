namespace CsFwExample {
    using System;
    using System.Drawing;
    using System.IO;

    struct TimePoint {
        public long ticks;
        public Point p;
        unsafe public static TimePoint FromFileStream (FileStream f) {
            const int structSize = sizeof(long) + 2 * sizeof(int);
            var bytes = new byte[structSize];
            var read = f.Read(bytes, 0, structSize);
            if (read != structSize)
                throw new ApplicationException($"read {read} instead of {structSize} bytes");
            var ticks = BitConverter.ToInt64(bytes, 0);
            var x = BitConverter.ToInt32(bytes, sizeof(long));
            var y = BitConverter.ToInt32(bytes, sizeof(long) + sizeof(int));
            return new TimePoint { ticks = ticks, p = new Point(x, y) };
        }
    }
}