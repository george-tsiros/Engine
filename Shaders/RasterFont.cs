namespace Shaders {
    using Gl;
    using System.Numerics;
    public static class RasterFont {
#pragma warning disable CS0649

        //size 1, type Vector2
        [GlAttrib("char")]
        public static uint Char { get; }

        //size 1, type Vector2
        [GlAttrib("offset")]
        public static uint Offset { get; }

        //size 1, type Vector2
        [GlAttrib("vertex")]
        public static uint Vertex { get; }

        //size 1, type Vector2
        [GlUniform("fontSize")]
        private readonly static uint fontSize;
        public static void FontSize (Vector2 v) => Calls.Uniform(fontSize, v);

        //size 1, type Vector2
        [GlUniform("screenSize")]
        private readonly static uint screenSize;
        public static void ScreenSize (Vector2 v) => Calls.Uniform(screenSize, v);

        //size 1, type Sampler2D
        [GlUniform("font")]
        private readonly static uint font;
        public static void Font (int v) => Calls.Uniform(font, v);

        public static uint Id { get; }
        static RasterFont () => ParsedShader.Prepare(typeof(RasterFont));
#pragma warning restore CS0649
    }
}