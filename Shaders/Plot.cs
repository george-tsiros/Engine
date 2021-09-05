namespace Shaders {
    using Gl;
    using System.Numerics;
    public static class Plot {
#pragma warning disable CS0649

        //size 1, type Float
        [GlAttrib("x")]
        public static uint X { get; }

        //size 1, type Float
        [GlAttrib("y")]
        public static uint Y { get; }

        //size 1, type Float
        [GlUniform("a")]
        private readonly static uint a;
        public static void A (float v) => Calls.Uniform(a, v);

        //size 1, type Float
        [GlUniform("b")]
        private readonly static uint b;
        public static void B (float v) => Calls.Uniform(b, v);

        //size 1, type Vector4
        [GlUniform("color")]
        private readonly static uint color;
        public static void Color (Vector4 v) => Calls.Uniform(color, v);

        public static uint Id { get; }
        static Plot () => ParsedShader.Prepare(typeof(Plot));
#pragma warning restore CS0649
    }
}