namespace Shaders {
    using Gl;
    using System.Numerics;
    public static class PassThrough {
#pragma warning disable CS0649

        //size 1, type Vector4
        [GlAttrib("vertexPosition")]
        public static uint VertexPosition { get; }

        //size 1, type Sampler2D
        [GlUniform("tex")]
        private readonly static uint tex;
        public static void Tex (int v) => Calls.Uniform(tex, v);

        public static uint Id { get; }
        static PassThrough () => ParsedShader.Prepare(typeof(PassThrough));
#pragma warning restore CS0649
    }
}