namespace Shaders {
    using Gl;
    using System.Numerics;
    public static class Lines {
#pragma warning disable CS0649

        //size 1, type Vector2
        [GlAttrib("coords")]
        public static uint Coords { get; }

        //size 1, type Vector4
        [GlUniform("color")]
        private readonly static uint color;
        public static void Color (Vector4 v) => Calls.Uniform(color, v);

        public static uint Id { get; }
        static Lines () => ParsedShader.Prepare(typeof(Lines));
#pragma warning restore CS0649
    }
}