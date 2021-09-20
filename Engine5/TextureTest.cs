#define __RENDER_SKYBOX
namespace Engine;

using System;
using System.Numerics;
using Gl;
using static Gl.Calls;
using Shaders;
using GLFW;

class ProcTexture:GlWindowBase {
    public ProcTexture (GLFW.Monitor monitor) : base(monitor) { }
    public ProcTexture (int width, int height) : base(width, height) { }
}

class TextureTest:GlWindowBase {
    public TextureTest (GLFW.Monitor monitor) : base(monitor) { }
    public TextureTest (int width, int height) : base(width, height) { }
    private VertexArray quad, uiVao;
    private Sampler2D tex, uiTexture;
#if __RENDER_SKYBOX
    private VertexArray skyboxVao;
    private Sampler2D skyboxTexture;
    private VertexBuffer<Vector4> skyboxVertices;
    private VertexBuffer<Vector2> skyboxUV;
#endif

    private Raster ui;
    private readonly Raster font = Raster.FromFile("font.raw");
    unsafe private void UploadTexture () {
        fixed (byte* p = ui.Pixels)
            TextureSubImage2D(uiTexture, 0, 0, 0, Width, Height, TextureFormat.Bgra, Const.UNSIGNED_BYTE, p);
    }
    protected override void Init () {

        quad = new();
        State.Program = SimpleTexture.Id;
        var quadBuffer = new VertexBuffer<Vector4>(Geometry.Quad);
        quad.Assign(quadBuffer, SimpleTexture.VertexPosition);
        quad.Assign(new VertexBuffer<Vector2>(Geometry.QuadUV), SimpleTexture.VertexUV);
        var models = new Matrix4x4[] { Matrix4x4.Identity };
        quad.Assign(new VertexBuffer<Matrix4x4>(models), SimpleTexture.Model, 1);
        var projection = Matrix4x4.CreatePerspectiveFieldOfView((float)(Math.PI / 4), (float)Width / Height, 1f, 100f);
        SimpleTexture.Projection(projection);

        tex = Sampler2D.FromFile("untitled.raw");
        tex.Mag = MagFilter.Linear;
        tex.Min = MinFilter.LinearMipMapLinear;
        tex.Wrap = Wrap.ClampToEdge;

        uiVao = new();
        State.Program = PassThrough.Id;
        uiVao.Assign(quadBuffer, PassThrough.VertexPosition);

        uiTexture = new(Width, Height, TextureInternalFormat.Rgba8);
        uiTexture.Mag = MagFilter.Nearest;
        uiTexture.Min = MinFilter.Nearest;
        ui = new(Width, Height, 4, 1);
        for (var (y, count) = (0, Math.Min(ui.Stride, font.Stride)); y < font.Height; y++)
            Array.Copy(font.Pixels, font.Stride * y, ui.Pixels, ui.Stride * y, count);
        UploadTexture();
#if __RENDER_SKYBOX
        State.Program = SkyBox.Id;
        skyboxVao = new();
        skyboxTexture = Sampler2D.FromFile("skybox.raw");
        skyboxTexture.Mag = MagFilter.Linear;
        skyboxTexture.Min = MinFilter.Linear;
        skyboxTexture.Wrap = Wrap.ClampToEdge;
        skyboxVertices = new(Geometry.Dex(Geometry.Translate(Geometry.CubeVertices, -.5f * Vector3.One), Geometry.FlipWinding(Geometry.CubeIndices)));
        skyboxVao.Assign(skyboxVertices, SkyBox.VertexPosition);
        skyboxUV = new(Geometry.Dex(Geometry.CubeUVVectors, Geometry.FlipWinding(Geometry.CubeUVIndices)));
        skyboxVao.Assign(skyboxUV, SkyBox.VertexUV);
        SkyBox.Projection(projection);
#endif
    }

    private BlendSourceFactor sfactor = BlendSourceFactor.One;
    private BlendDestinationFactor dfactor = BlendDestinationFactor.OneMinusSrcAlpha;

    [KeyBinding(Keys.F2, Keys.F3)]
    protected void CycleFactors (Keys k, InputState s) {
        if (s != InputState.Release)
            return;
        switch (k) {
            case Keys.F2:
                Utilities.CycleThrough(ref sfactor);
                Utilities.Trace($"{nameof(sfactor)}: {sfactor}");
                break;
            case Keys.F3:
                Utilities.CycleThrough(ref dfactor);
                Utilities.Trace($"{nameof(dfactor)}: {dfactor}");
                break;
        }
    }
    private bool showUI = true;
    [KeyBinding(Keys.GraveAccent)]
    protected void ToggleUI (Keys k, InputState s) {
        if (s == InputState.Release)
            showUI = !showUI;
    }
    protected override void Render (float dt) {
        Viewport(0, 0, Width, Height);
        Clear(BufferBit.Color | BufferBit.Depth);
        State.VertexArray = quad;
        State.Program = SimpleTexture.Id;
        State.Blend = false;
        State.DepthTest = true;
        State.DepthFunc = DepthFunction.Less;
        State.CullFace = true;
        tex.BindTo(1);
        SimpleTexture.Tex(1);
        SimpleTexture.View(Camera.LookAtMatrix);
        DrawArraysInstanced(Primitive.Triangles, 0, 6, 1);
#if __RENDER_SKYBOX
        State.Program = SkyBox.Id;
        State.VertexArray = skyboxVao;
        State.DepthFunc = DepthFunction.LessEqual;
        skyboxTexture.BindTo(0);
        SkyBox.Tex(0);
        SkyBox.View(Camera.RotationOnly);
        DrawArrays(Primitive.Triangles, 0, 36);
#endif
        if (showUI) {
            State.VertexArray = uiVao;
            State.Program = PassThrough.Id;
            State.Blend = true;
            State.DepthTest = false;
            State.CullFace = false;
            BlendFunc(sfactor, dfactor);
            uiTexture.BindTo(2);
            PassThrough.Tex(2);
            DrawArrays(Primitive.Triangles, 0, 6);
        }
    }
}
