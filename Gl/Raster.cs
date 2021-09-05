namespace Gl {
    using System;
    using System.IO;
    using System.IO.Compression;

    sealed public class Raster {
        public int Width { get; }
        public int Height { get; }
        public int Channels { get; }
        public int BytesPerChannel { get; }
        public Raster (int w, int h, int channels, int bytesPerChannel) {
            (Width, Height, Channels, BytesPerChannel) = (w, h, channels, bytesPerChannel);
        }
        public byte[] Pixels { get; private set; }
        public static Raster FromFile (string filepath) {
            using var f = File.OpenRead(filepath);
            var width = f.ReadInt32();
            var height = f.ReadInt32();
            var channels = f.ReadInt32();
            var bytesPerChannel = f.ReadInt32();
            if (bytesPerChannel != 1)
                throw new ArgumentOutOfRangeException(nameof(bytesPerChannel), "only 1Bpp bitmaps are currently supported");
            var length = f.ReadInt32();
            var raster = new Raster(width, height, channels, bytesPerChannel) { Pixels = new byte[length] };
            using var unzip = new GZipStream(f, CompressionMode.Decompress);
            var read = unzip.Read(raster.Pixels, 0, length);
            return read == length ? raster : throw new Exception($"expected to read {length} bytes, read {read} instead");
        }
    }
}