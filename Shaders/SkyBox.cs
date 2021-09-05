namespace Shaders {
    using Gl;
    using System.Numerics;
    public static class SkyBox {
#pragma warning disable CS0649

        //size 1, type Vector4
        [GlAttrib("vertexPosition")]
        public static uint VertexPosition { get; }

        //size 1, type Vector2
        [GlAttrib("vertexUV")]
        public static uint VertexUV { get; }

        //size 1, type Matrix4x4
        [GlUniform("projection")]
        private readonly static uint projection;
        public static void Projection (Matrix4x4 v) => Calls.Uniform(projection, v);

        //size 1, type Matrix4x4
        [GlUniform("view")]
        private readonly static uint view;
        public static void View (Matrix4x4 v) => Calls.Uniform(view, v);

        //size 1, type Sampler2D
        [GlUniform("tex")]
        private readonly static uint tex;
        public static void Tex (int v) => Calls.Uniform(tex, v);

        public static uint Id { get; }
        static SkyBox () => ParsedShader.Prepare(typeof(SkyBox));
#pragma warning restore CS0649
    }
}