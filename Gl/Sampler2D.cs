namespace Gl {
    using System;
    using System.Diagnostics;
    using static Calls;

    sealed public class Sampler2D:IDisposable {
        public int Id { get; }
        public int Width { get; }
        public int Height { get; }
        public TextureInternalFormat SizedFormat { get; }
        public static implicit operator int (Sampler2D sampler) => sampler.Id;
        private MagFilter mag;
        private MinFilter min;
        private bool disposed;
        private Wrap wrap;
        public void BindTo (int t) {
            Debug.Assert(!disposed);
            State.ActiveTexture = t;
            glBindTexture(Const.TEXTURE_2D, Id);
        }
        public Wrap Wrap {
            get => wrap;
            set {
                Debug.Assert(!disposed);
                wrap = value;
                TextureWrap(Id, WrapCoordinate.WrapS, value);
                TextureWrap(Id, WrapCoordinate.WrapT, value);
            }
        }
        public MinFilter Min {
            get => min;
            set {
                Debug.Assert(!disposed);
                TextureFilter(Id, min = value);
            }
        }

        public MagFilter Mag {
            get => mag;
            set {
                Debug.Assert(!disposed);
                TextureFilter(Id, mag = value);
            }
        }
        private Sampler2D () => Id = Create2DTextures(1)[0];
        public Sampler2D (int width, int height, TextureInternalFormat sizedFormat) : this() {
            (Width, Height, SizedFormat) = (width, height, sizedFormat);
            glTextureStorage2D(Id, 1, SizedFormat, width, height);
            TextureBaseLevel(Id, 0);
            TextureMaxLevel(Id, 0);
            Wrap = Wrap.ClampToEdge;
        }
        unsafe public static Sampler2D FromFile (string filepath) {
            var raster = Raster.FromFile(filepath);
            if (raster.BytesPerChannel != 1)
                throw new Exception();

            var texture = new Sampler2D(raster.Width, raster.Height, SizedFormatWith(raster.Channels));
            fixed (byte* ptr = raster.Pixels)
                glTextureSubImage2D(texture.Id, 0, 0, 0, raster.Width, raster.Height, FormatWith(raster.Channels), Const.UNSIGNED_BYTE, ptr);
            return texture;
        }
        private static readonly TextureInternalFormat[] sizedFormats = { TextureInternalFormat.R8, TextureInternalFormat.Rg8, TextureInternalFormat.Rgb8, TextureInternalFormat.Rgba8 };
        private static readonly int[] formats = { Const.RED, Const.RG, Const.BGR, Const.BGRA };
        private static TextureInternalFormat SizedFormatWith (int channels) => 1 <= channels && channels <= 4 ? sizedFormats[channels - 1] : throw new Exception();
        private static int FormatWith (int channels) => 1 <= channels && channels <= 4 ? formats[channels - 1] : throw new Exception();

        private void Dispose (bool disposing) {
            if (!disposed) {
                if (disposing) 
                    DeleteTextures(new int[] { this });
                disposed = true;
            }
        }
        public void Dispose () {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}