namespace Gl;

using System;
using System.IO;
using System.IO.Compression;

public class Raster:IDisposable {

    public readonly int Width, Height, Channels, BytesPerChannel, Stride;
    
    public byte[] Pixels;
    public Raster (int width, int height, int channels, int bytesPerChannel) {
        (Width, Height, Channels, BytesPerChannel) = (width, height, channels, bytesPerChannel);
        Pixels = new byte[Width * Height * Channels * BytesPerChannel];
        Stride = Width * Channels * BytesPerChannel;
    }

    private static Raster FromStream (FileStream f) {
        using var r = new BinaryReader(f, System.Text.Encoding.UTF8, true);
        var width = r.ReadInt32();
        var height = r.ReadInt32();
        var channels = r.ReadInt32();
        var bytesPerChannel = r.ReadInt32();
        if (bytesPerChannel != 1)
            throw new ArgumentOutOfRangeException(nameof(bytesPerChannel), "only 1Bpp bitmaps are currently supported");
        var length = r.ReadInt32();
        return new Raster(width, height, channels, bytesPerChannel);
    }
    
    public static Raster FromFile (string filepath) {
        using var f = File.OpenRead(filepath);
        var raster = Raster.FromStream(f);
        using var unzip = new DeflateStream(f, CompressionMode.Decompress);
        var read = unzip.Read(raster.Pixels, 0, raster.Pixels.Length);
        return read == raster.Pixels.Length ? raster : throw new ApplicationException($"{filepath}: expected to read {raster.Pixels.Length} bytes, read {read} instead");
    }

    private bool disposed;
    protected virtual void Dispose (bool _) {
        if (!disposed) {
            Pixels = null;
            disposed = true;
        }
    }
    public void Dispose () {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
}
