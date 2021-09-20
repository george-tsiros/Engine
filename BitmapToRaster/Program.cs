namespace BitmapToRaster;

using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.IO.Compression;
using System.Runtime.InteropServices;
class BitmapToRaster {
    private static bool IsValid (string filepath) => File.Exists(filepath) && Path.GetFileName(filepath).ToLower().EndsWith(".png");
    private static void Main (string[] args) => Array.ForEach(Array.FindAll(args, IsValid), ConvertFrom);

    private static void ConvertFrom (string filepath) {

        using (var b = new Bitmap(filepath)) {
            var l = b.LockBits(new Rectangle(Point.Empty, b.Size), ImageLockMode.ReadOnly, b.PixelFormat);
            var bytes = new byte[l.Stride * l.Height];
            var channels = l.Stride / l.Width;
            const int bytesPerChannel = 1;
            Debug.Assert(l.Stride % l.Width == 0);
            Marshal.Copy(l.Scan0, bytes, 0, bytes.Length);
            b.UnlockBits(l);
            var name = Path.GetFileNameWithoutExtension(filepath);
            var dir = Path.GetDirectoryName(filepath);
            using (var f = new BinaryWriter(File.Create(Path.Combine(dir, $"{name}.raw")))) {
                Console.WriteLine($"{filepath}: {b.Width}x{b.Height}, {channels} channels, {bytes.Length} bytes");
                f.Write(b.Width);
                f.Write(b.Height);
                f.Write(channels);
                f.Write(bytesPerChannel);
                f.Write(bytes.Length);
                using (var zip = new DeflateStream(f.BaseStream, CompressionLevel.Optimal))
                    zip.Write(bytes, 0, bytes.Length);
            }
        }
    }
}
