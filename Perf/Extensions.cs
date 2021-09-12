namespace Perf {
    using System;
    using System.IO;
    using System.Text;

    static class Extensions {
        internal static bool TryRead (this Stream self, out int i) {
            var bytes = new byte[sizeof(int)];
            var read = self.Read(bytes, 0, sizeof(int));
            var ok = read == sizeof(int);
            i = ok ? BitConverter.ToInt32(bytes, 0) : 0;
            return ok;
        }

        internal static bool TryRead (this Stream self, out long l) {
            var bytes = new byte[sizeof(long)];
            var read = self.Read(bytes, 0, sizeof(long));
            var ok = read == sizeof(long);
            l = ok ? BitConverter.ToInt64(bytes, 0) : 0l;
            return ok;
        }
        internal static bool TryRead (this Stream self, out byte b) {
            var i = self.ReadByte();
            var ok = byte.MinValue <= i && i <= byte.MaxValue;
            b = ok ? (byte)i : byte.MinValue;
            return ok;
        }

        internal static int GetInt32 (this Stream self) {
            var bytes = new byte[sizeof(int)];
            var read = self.Read(bytes, 0, sizeof(int));
            return read == sizeof(int) ? BitConverter.ToInt32(bytes, 0) : throw new ApplicationException();
        }

        internal static long GetInt64 (this Stream self) {
            var bytes = new byte[sizeof(long)];
            var read = self.Read(bytes, 0, sizeof(long));
            return read == sizeof(long) ? BitConverter.ToInt64(bytes, 0) : throw new ApplicationException();
        }

        internal static byte GetByte (this Stream self) => (byte)self.ReadByte();

        internal static string GetString (this Stream self) {
            var length = self.GetInt32();
            var bytes = new byte[length];
            if (length != self.Read(bytes, 0, length))
                throw new ApplicationException();
            return Encoding.ASCII.GetString(bytes);
        }
    }
}
