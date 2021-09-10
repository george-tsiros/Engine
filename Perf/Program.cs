namespace Perf {
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Text;

    enum DataType:byte {
        Null = 0,
        Byte,
        Int16,
        Int32,
        Int64,
        UInt16,
        UInt32,
        UInt64,
        Single,
        Double,
        String,
        Object,
        Array,
    }
    class FancyStream:IDisposable {
        private bool disposed;
        private readonly Stream stream;
        private readonly byte[] buffer = new byte[255];
        protected virtual void Dispose (bool disposing) {
            if (disposed)
                return;
            disposed = true;
            if (disposing)
                stream.Dispose();
        }
        public FancyStream (string filepath) => stream = File.OpenRead(filepath);
        private DataType next;
        public DataType Next {
            get {
                if (stream.Position == stream.Length)
                    return DataType.Null;
                var b = stream.ReadByte();
                _ = stream.Seek(-1, SeekOrigin.Current);
                return 0 <= b && b <= byte.MaxValue ? (DataType)(byte)b : throw new ApplicationException($"{b} not valid byte");
            }
        }
        //private void UpdateNext () {
        //    if (stream.Position == stream.Length)
        //        return;
        //    var b = stream.ReadByte();
        //    _ = stream.Seek(-1, SeekOrigin.Current);
        //    next = 0 <= b && b <= byte.MaxValue ? (DataType)(byte)b : throw new ApplicationException($"{b} not valid byte");
        //}
        private void Expect (DataType expected) {
            var actual = (DataType)GetByte();
            if (actual != expected)
                throw new ApplicationException($"expected {expected} found {actual}");
        }
        private byte GetByte () {
            var i = stream.ReadByte();
            return 0 <= i && i <= byte.MaxValue ? (byte)i : throw new ApplicationException($"{i} not a valid byte");
        }

        public byte ReadByte () {
            Expect(DataType.Byte);
            return GetByte();
        }

        public string ReadString () {
            Expect(DataType.String);
            var expected = GetByte();
            if (expected == 0)
                return "";
            var bytes = new byte[expected];
            var read = stream.Read(bytes, 0, expected);
            return read == expected ? Encoding.ASCII.GetString(bytes) : throw new ApplicationException($"expected {expected} bytes, read {read} instead");
        }

        public int ReadInt32 () {
            Expect(DataType.Int32);
            var read = stream.Read(buffer, 0, sizeof(int));
            return read == sizeof(int) ? BitConverter.ToInt32(buffer, 0) : throw new ApplicationException($"expected {sizeof(int)} bytes, read {read} instead");
        }
        public long ReadInt64 () {
            Expect(DataType.Int64);
            var read = stream.Read(buffer, 0, sizeof(long));
            return read == sizeof(long) ? BitConverter.ToInt64(buffer, 0) : throw new ApplicationException($"expected {sizeof(long)} bytes, read {read} instead");
        }
        public void Dispose () {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
    class Program {
        private static int Main (string[] args) {
            return args.Length == 1 && File.Exists(args[0]) ? Do(args[0]) : -1;
        }

        private static int Do (string filepath) {
            try {
                DoUnsafe(filepath);
                return 0;
            }
            catch (Exception e) {
                Console.WriteLine(e.Message);
                return -1;
            }
        }

        private static void DoUnsafe (string filepath) {
            using (var f = new FancyStream(filepath))
                DoUnsafe(f);
        }

        private static void DoUnsafe (FancyStream f) {
            while (f.Next != DataType.Null)
                switch (f.Next) {
                    case DataType.Null:
                        throw new ApplicationException();
                    case DataType.Byte:
                        Console.WriteLine($"byte {f.ReadByte()}");
                        break;
                    //case DataType.Int16:
                    //    break;
                    //case DataType.Int32:
                    //    break;
                    case DataType.Int64:
                        Console.WriteLine($"long {f.ReadInt64()}");
                        break;
                    case DataType.UInt16:
                        break;
                    case DataType.UInt32:
                        break;
                    case DataType.UInt64:
                        break;
                    case DataType.Single:
                        break;
                    case DataType.Double:
                        break;
                    case DataType.String:
                        Console.WriteLine($"string '{f.ReadString()}'");
                        break;
                    case DataType.Object:
                        break;
                    //case DataType.Array:
                    //    break;
                    default:
                        throw new ApplicationException();
                }
        }
    }
}
