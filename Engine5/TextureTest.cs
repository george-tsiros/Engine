namespace Engine;

using System;
using System.Numerics;
using Gl;
using static Gl.Calls;
using Shaders;
using GLFW;

class ProcTexture:GlWindowBase {
    public ProcTexture (Monitor monitor) : base(monitor) { }
    public ProcTexture (int width, int height) : base(width, height) { }
}

class TextureTest:GlWindowBase {
    public TextureTest (Monitor monitor) : base(monitor) { }
    public TextureTest (int width, int height) : base(width, height) { }
    private VertexArray quad, uiVao;
    private Sampler2D tex, uiTexture;
    private VertexArray skyboxVao;
    private Sampler2D skyboxTexture;
    private VertexBuffer<Vector4> skyboxVertices;
    private VertexBuffer<Vector2> skyboxUV;

    private Raster ui;

    unsafe private void UploadTexture () {
        Enter(Events.Texture);
        var clientSize = GetClientSize();
        fixed(byte* p = ui.Pixels)
            TextureSubImage2D(uiTexture, 0, 0, 0, clientSize.X, clientSize.Y, TextureFormat.Bgra, Const.UNSIGNED_BYTE, p);
        Leave();
    }

    protected unsafe override void Init () {

        quad = new();
        State.Program = SimpleTexture.Id;
        var quadBuffer = new VertexBuffer<Vector4>(Quad.Vertices);
        quad.Assign(quadBuffer, SimpleTexture.VertexPosition);
        quad.Assign(new VertexBuffer<Vector2>(Quad.Uv), SimpleTexture.VertexUV);
        var models = new Matrix4x4[] { Matrix4x4.Identity };
        quad.Assign(new VertexBuffer<Matrix4x4>(models), SimpleTexture.Model, 1);
        var clientSize = GetClientSize();
        var projection = Matrix4x4.CreatePerspectiveFieldOfView((float)(Math.PI / 4), (float)clientSize.X / clientSize.Y, 1f, 100f);
        SimpleTexture.Projection(projection);

        tex = Sampler2D.FromFile("untitled.raw");
        tex.Mag = MagFilter.Linear;
        tex.Min = MinFilter.LinearMipMapLinear;
        tex.Wrap = Wrap.ClampToEdge;

        uiVao = new();
        State.Program = PassThrough.Id;
        uiVao.Assign(quadBuffer, PassThrough.VertexPosition);

        uiTexture = new(clientSize, TextureInternalFormat.Rgba8);
        uiTexture.Mag = MagFilter.Nearest;
        uiTexture.Min = MinFilter.Nearest;
        ui = new Raster(clientSize, 4, 1);
        for (var i = 0; i < ui.Pixels.Length; i++)
            ui.Pixels[i] = 64;
        State.Program = SkyBox.Id;
        skyboxVao = new();
        skyboxTexture = Sampler2D.FromFile("skybox.raw");
        skyboxTexture.Mag = MagFilter.Linear;
        skyboxTexture.Min = MinFilter.Linear;
        skyboxTexture.Wrap = Wrap.ClampToEdge;
        skyboxVertices = new(Geometry.Dex(Geometry.Translate(Cube.Vertices, -.5f * Vector3.One), Geometry.FlipWinding(Cube.Indices)));
        skyboxVao.Assign(skyboxVertices, SkyBox.VertexPosition);
        skyboxUV = new(Geometry.Dex(Cube.UvVectors, Geometry.FlipWinding(Cube.UvIndices)));
        skyboxVao.Assign(skyboxUV, SkyBox.VertexUV);
        SkyBox.Projection(projection);
    }
    private bool showUI;
    [KeyBinding(Keys.U)]
    protected void ToggleUI (Keys k, InputState state) {
        if (state == InputState.Release)
            showUI = !showUI;
    }
    protected override void Render (float dt) {
        UploadTexture();
        Viewport(new(), GetClientSize());
        Clear(BufferBit.Color | BufferBit.Depth);
        State.Program = SimpleTexture.Id;
        State.VertexArray = quad;
        State.Blend = false;
        State.DepthTest = true;
        State.DepthFunc = DepthFunction.Less;
        State.CullFace = true;
        tex.BindTo(1);
        SimpleTexture.Tex(1);
        SimpleTexture.View(Camera.LookAtMatrix);
        DrawArraysInstanced(Primitive.Triangles, 0, 6, 1);

        State.Program = SkyBox.Id;
        State.VertexArray = skyboxVao;
        State.DepthFunc = DepthFunction.LessEqual;
        skyboxTexture.BindTo(0);
        SkyBox.Tex(0);
        SkyBox.View(Camera.RotationOnly);
        DrawArrays(Primitive.Triangles, 0, 36);
        if (showUI) {
            State.Program = PassThrough.Id;
            State.VertexArray = uiVao;
            State.Blend = true;
            State.DepthTest = false;
            State.CullFace = false;
            BlendFunc(BlendSourceFactor.One, BlendDestinationFactor.OneMinusSrcAlpha);
            uiTexture.BindTo(2);
            PassThrough.Tex(2);
            DrawArrays(Primitive.Triangles, 0, 6);
        }
    }
}
