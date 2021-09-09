namespace Engine;

using System;
using System.Numerics;
using Shaders;
using Gl;
using static Gl.Calls;
using static Extra;
using GLFW;

class NeinCraft:GlWindowBase {
    public NeinCraft (Monitor monitor) : base(monitor) { }
    public NeinCraft (int width, int height) : base(width, height) { }
    private bool isRunning = true;
    private VertexArray skyboxVao;
    private Sampler2D skyboxTexture;
#pragma warning disable IDE0051
    [KeyBinding(GLFW.Keys.Pause)]
    private void PauseResume (Keys _, InputState state) {
        if (state == InputState.Release)
            isRunning = !isRunning;
    }
#pragma warning restore IDE0051
    protected override void Init () {
        State.Program = SkyBox.Id;
        skyboxVao = new();
        skyboxTexture = Sampler2D.FromFile("skybox.raw");
        SkyBox.Tex(0);
        var flipped = Geometry.CubeIndices;
        Geometry.FlipWinding(flipped);
        skyboxVao.Assign(new VertexBuffer<Vector4>(Dex(TranslateInPlace(Geometry.CubeVertices, -.5f * Vector3.One), flipped)), SkyBox.VertexPosition);
        var uvflipped = Geometry.CubeUVIndices;
        Geometry.FlipWinding(uvflipped);
        skyboxVao.Assign(new VertexBuffer<Vector2>(Dex(Geometry.CubeUVVectors, uvflipped)), SkyBox.VertexUV);
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
    }
}