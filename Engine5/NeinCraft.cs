namespace Engine;

using System;
using System.Numerics;
using Shaders;
using Gl;
using static Gl.Calls;
using GLFW;

class NeinCraft:GlWindowBase {
    public NeinCraft (Monitor monitor) : base(monitor) { }
    public NeinCraft (int width, int height) : base(width, height) { }
    private VertexArray skyboxVao;
    private Sampler2D skyboxTexture;
    private VertexBuffer<Vector4> cubeVertices;
    private VertexBuffer<Vector2> cubeUV;
    protected override void Init () {
        State.Program = SkyBox.Id;
        skyboxVao = new();
        skyboxTexture = Sampler2D.FromFile("skybox.raw");
        SkyBox.Tex(0);
        cubeVertices = new(Geometry.Dex(Geometry.Translate(Geometry.CubeVertices, -.5f * Vector3.One), Geometry.FlipWinding(Geometry.CubeIndices)));
        skyboxVao.Assign(cubeVertices, SkyBox.VertexPosition);
        cubeUV = new(Geometry.Dex(Geometry.CubeUVVectors, Geometry.FlipWinding(Geometry.CubeUVIndices)));
        skyboxVao.Assign(cubeUV, SkyBox.VertexUV);
        var projection = Matrix4x4.CreatePerspectiveFieldOfView((float)(Math.PI / 4), (float)Width / Height, 2f, 2000f);
        SkyBox.Projection(projection);
    }

    protected override void Render (float dt) {
        Viewport(0, 0, Width, Height);
        Clear(BufferBit.Color | BufferBit.Depth);

        State.DepthTest = true;
        State.Program = SkyBox.Id;
        State.VertexArray = skyboxVao;
        State.DepthFunc = DepthFunction.LessEqual;
        skyboxTexture.BindTo(0);
        SkyBox.View(Camera.RotationOnly);
        DrawArrays(Primitive.Triangles, 0, 36);
    }

    protected override void OnClose () {
        skyboxTexture.Dispose();
        skyboxVao.Dispose();
        cubeVertices.Dispose();
        cubeUV.Dispose();
    }
}