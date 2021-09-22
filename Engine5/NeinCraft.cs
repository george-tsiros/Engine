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
    private VertexBuffer<Vector4> skyboxVertices;
    private VertexBuffer<Vector2> skyboxUV;

    private VertexArray cubeVao;
    private VertexBuffer<Vector4> cubeVertices;

    protected override void Init () {
        State.Program = SkyBox.Id;
        skyboxVao = new();
        skyboxTexture = Sampler2D.FromFile("skybox.raw");
        SkyBox.Tex(0);
        skyboxVertices = new(Geometry.Dex(Geometry.Translate(Geometry.CubeVertices, -.5f * Vector3.One), Geometry.FlipWinding(Geometry.CubeIndices)));
        skyboxVao.Assign(skyboxVertices, SkyBox.VertexPosition);
        skyboxUV = new(Geometry.Dex(Geometry.CubeUVVectors, Geometry.FlipWinding(Geometry.CubeUVIndices)));
        skyboxVao.Assign(skyboxUV, SkyBox.VertexUV);
        var clientsize = GetClientSize();

        var projection = Matrix4x4.CreatePerspectiveFieldOfView((float)(Math.PI / 4), (float)clientsize.X / clientsize.Y, 2f, 2000f);
        SkyBox.Projection(projection);

        State.Program = SolidColor.Id;
        cubeVao = new();
        cubeVertices = new(Geometry.Dex(Geometry.Translate(Geometry.CubeVertices, -.5f * Vector3.One), Geometry.CubeIndices));
        cubeVao.Assign(cubeVertices, SolidColor.VertexPosition);
        SolidColor.Color(new(1f, 1f, 1f, 1f));
        SolidColor.Projection(projection);
    }
    double theta = 0.0;
    protected override void Render (float dt) {
        Viewport(new(), GetClientSize());
        Clear(BufferBit.Color | BufferBit.Depth);
        theta += 1.8 * dt;
        if (theta > 2.0 * Math.PI)
            theta -= 2.0 * Math.PI;
        State.DepthTest = true;
        State.DepthFunc = DepthFunction.Less;
        State.Program = SolidColor.Id;
        State.VertexArray = cubeVao;
        SolidColor.View(Camera.LookAtMatrix);
        SolidColor.Model(Matrix4x4.CreateTranslation(new(2f * (float)Math.Sin(theta), 2f * (float)Math.Cos(theta), -3f)));
        DrawArrays(Primitive.Triangles, 0, 36);
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
        skyboxVertices.Dispose();
        skyboxUV.Dispose();
    }
}