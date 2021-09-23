namespace CsFwExample {
    using System;
    using System.IO;

    struct TimePoint {
        public long ticks;
        public int x, y;
        unsafe public static TimePoint FromFileStream (FileStream f) {
            const int structSize = sizeof(long) + 2 * sizeof(int);
            var bytes = new byte[structSize];
            var read = f.Read(bytes, 0, structSize);
            if (read != structSize)
                throw new ApplicationException($"read {read} instead of {structSize} bytes");
            var ticks = BitConverter.ToInt64(bytes, 0);
            var dx = BitConverter.ToInt32(bytes, sizeof(long));
            var dy = BitConverter.ToInt32(bytes, sizeof(long) + sizeof(int));
            return new TimePoint { ticks = ticks, x = dx, y = dy };
        }
    }
}