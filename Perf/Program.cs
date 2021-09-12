namespace Perf {
    using System;
    using System.Threading;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Drawing.Drawing2D;
    using System.Drawing.Imaging;
    using System.IO;
    using System.Text;
    using System.Text.RegularExpressions;

    class Program {
        private enum Kind:byte {
            Stamp,
            Enter,
            Leave,
        }
        /*
        Stamp:
        [Kind.Stamp][int64][strlen][str]
        Enter:
        [Kind.Enter][int64][strlen][str]
        Leave:
        [Kind.Leave][int64]
        */
        private class Entry {
            public Kind Kind { get; }
            public long Ticks { get; }
            public string Message { get; } = null;
            public Entry (Kind k, long t, string m = null) => (Kind, Ticks, Message) = (k, t, m);
            private static readonly byte[] _buffer = new byte[255];
            private static volatile int _synclock = 0;
            private static bool ReadExactly (Stream stream, byte[] buffer, int count) => stream.Read(buffer, 0, count) == count;
            public static Entry FromStream (Stream stream) {
                if (Interlocked.CompareExchange(ref _synclock, 1, 0) != 0)
                    throw new ApplicationException("lock failed");

                var i = stream.ReadByte();
                if (i < byte.MinValue || byte.MaxValue < i)
                    return null;
                var k = (Kind)i;
                if (!ReadExactly(stream, _buffer, sizeof(long)))
                    throw new ApplicationException($"failed to read {sizeof(long)} bytes");
                var t = BitConverter.ToInt64(_buffer, 0);
                string m = null;
                switch (k) {
                    case Kind.Enter:
                    case Kind.Stamp:
                        var b = (byte)stream.ReadByte();
                        if (!ReadExactly(stream, _buffer, b))
                            throw new ApplicationException($"failed to read {b} bytes");
                        m = Encoding.ASCII.GetString(_buffer, 0, b);
                        break;
                    case Kind.Leave:
                        break;
                    default:
                        throw new ApplicationException($"did not expect byte {i}");
                }
                var e = new Entry(k, t, m);
                _synclock = 0;
                return e;
            }
        }
        private static int Main (string[] args) {
            try {
                var entries = DoUnsafe(args[0]);

                return 0;
            }
            catch (Exception e) {
                Console.WriteLine(e.Message);
                return -1;
            }
            finally {
                _ = Console.ReadLine();
            }
        }

        private static List<Entry> DoUnsafe (string filepath) {
            using (var f = File.OpenRead(filepath))
                return DoUnsafe(f);
        }

        private static List<Entry> DoUnsafe (Stream reader) {
            var entries = new List<Entry>();
            while (Entry.FromStream(reader) is Entry e)
                entries.Add(e);
            return entries;
        }
    }
}
