namespace Gl {
    using System;
    using static Calls;

    sealed public class Sampler2D {
        public uint Id { get; }
        public int Width { get; }
        public int Height { get; }
        public TextureInternalFormat SizedFormat { get; }
        public static implicit operator uint (Sampler2D sampler) => sampler.Id;
        public void BindTo (uint t) {
            State.ActiveTexture = t;
            glBindTexture(Const.TEXTURE_2D, Id);
        }
        private Wrap wrap;
        public Wrap Wrap {
            get => wrap;
            set {
                wrap = value;
                TextureWrap(Id, WrapCoordinate.WrapS, value);
                TextureWrap(Id, WrapCoordinate.WrapT, value);
            }
        }
        private MinFilter min;
        public MinFilter Min {
            get => min;
            set => TextureFilter(Id, min = value);
        }
        private MagFilter mag;
        public MagFilter Mag {
            get => mag;
            set => TextureFilter(Id, mag = value);
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
    }
}