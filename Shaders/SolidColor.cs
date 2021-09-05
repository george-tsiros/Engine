namespace Shaders {
    using Gl;
    using System.Numerics;
    public static class SolidColor {
#pragma warning disable CS0649

        //size 1, type Vector4
        [GlAttrib("vertexPosition")]
        public static uint VertexPosition { get; }

        //size 1, type Vector4
        [GlUniform("color")]
        private readonly static uint color;
        public static void Color (Vector4 v) => Calls.Uniform(color, v);

        //size 1, type Matrix4x4
        [GlUniform("model")]
        private readonly static uint model;
        public static void Model (Matrix4x4 v) => Calls.Uniform(model, v);

        //size 1, type Matrix4x4
        [GlUniform("projection")]
        private readonly static uint projection;
        public static void Projection (Matrix4x4 v) => Calls.Uniform(projection, v);

        //size 1, type Matrix4x4
        [GlUniform("view")]
        private readonly static uint view;
        public static void View (Matrix4x4 v) => Calls.Uniform(view, v);

        public static uint Id { get; }
        static SolidColor () => ParsedShader.Prepare(typeof(SolidColor));
#pragma warning restore CS0649
    }
}