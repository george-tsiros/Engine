namespace Engine;

using System;
using System.Numerics;
using Gl;
using static Extra;
using static Gl.Calls;
using Shaders;

class ProcTexture:GlWindowBase {
    public ProcTexture (GLFW.Monitor monitor) : base(monitor) { }
    public ProcTexture (int width, int height) : base(width, height) { }
}

class TextureTest:GlWindowBase {
    public TextureTest (GLFW.Monitor monitor) : base(monitor) { }
    public TextureTest (int width, int height) : base(width, height) { }
    private VertexArray quad;
    private Sampler2D tex;
#if __RENDER_SKYBOX
        private VertexArray skybox;
        private Sampler2D skyTexture;
#endif
    //private readonly Camera camera = new(new(0, 0, 10));

    protected override void Init () {
        var projection = Matrix4x4.CreatePerspectiveFieldOfView((float)(Math.PI / 4), (float)Width / Height, 1f, 100f);

        quad = new();
        tex = Sampler2D.FromFile("skybox.raw");
        tex.Mag = MagFilter.Linear;
        tex.Min = MinFilter.LinearMipMapLinear;
        tex.Wrap = Wrap.ClampToEdge;
        State.Program = SimpleTexture.Id;
        var quadBuffer = new VertexBuffer<Vector4>(Geometry.Quad);
        quad.Assign(quadBuffer, SimpleTexture.VertexPosition);
        quad.Assign(new VertexBuffer<Vector2>(Geometry.QuadUV), SimpleTexture.VertexUV);
        var models = new Matrix4x4[] { Matrix4x4.Identity };
        quad.Assign(new VertexBuffer<Matrix4x4>(models), SimpleTexture.Model, 1);
        SimpleTexture.Projection(projection);

#if __RENDER_SKYBOX
            State.Program = SkyBox.Id;
            skybox = new();
            skyTexture = Sampler2D.FromFile("skybox.raw");
            skyTexture.Mag = MagFilter.Linear;
            skyTexture.Min = MinFilter.Linear;
            skyTexture.Wrap = Wrap.ClampToEdge;
            SkyBox.Tex(0);
            var flipped = Geometry.CubeIndices;
            Geometry.FlipWinding(flipped);
            skybox.Assign(new VertexBuffer<Vector4>(Dex(TranslateInPlace(Geometry.CubeVertices, -.5f * Vector3.One), flipped)), SkyBox.VertexPosition);
            var uvflipped = Geometry.CubeUVIndices;
            Geometry.FlipWinding(uvflipped);
            skybox.Assign(new VertexBuffer<Vector2>(Dex(Geometry.CubeUVVectors, uvflipped)), SkyBox.VertexUV);
            SkyBox.Projection(projection);
#endif
    }

    protected override void Render (float dt) {
        Viewport(0, 0, Width, Height);
        Clear(BufferBit.Color | BufferBit.Depth);
        State.VertexArray = quad;
        State.Program = SimpleTexture.Id;
        State.DepthTest = true;
        State.DepthFunc = DepthFunction.Less;
        State.CullFace = true;
        tex.BindTo(1);
        SimpleTexture.Tex(1);
        SimpleTexture.View(Camera.LookAtMatrix);
        DrawArraysInstanced(Primitive.Triangles, 0, 6, 1);
#if __RENDER_SKYBOX
            State.Program = SkyBox.Id;
            State.VertexArray = skybox;
            State.DepthFunc = DepthFunc.LessEqual;
            skyTexture.BindTo(0);
            SkyBox.View(Camera.RotationOnly);
            glDrawArrays(Primitive.Triangles, 0, 36);
#endif
    }
}
