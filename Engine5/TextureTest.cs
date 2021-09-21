namespace Engine;

using System;
using System.Numerics;
using Gl;
using static Gl.Calls;
using Shaders;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using GLFW;
using System.Diagnostics;
using System.IO;

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

    private Bitmap ui;
    private readonly Font font = new("ubuntu mono", 12f, GraphicsUnit.Pixel);
    private readonly Brush translucentBrush = new SolidBrush(Color.FromArgb(64, 0, 0, 0));

    unsafe private void UploadTexture () {
        Enter(Events.LockBits);
        var lockBits = ui.LockBits(new(new(), ui.Size), System.Drawing.Imaging.ImageLockMode.ReadOnly, ui.PixelFormat);
        Enter(Events.Texture);
        TextureSubImage2D(uiTexture, 0, 0, 0, Width, Height, TextureFormat.Bgra, Const.UNSIGNED_BYTE, lockBits.Scan0.ToPointer());
        Leave();
        ui.UnlockBits(lockBits);
        Leave();
    }

    protected unsafe override void Init () {

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
        ui = new(Width, Height, PixelFormat.Format32bppArgb);

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
    }

    protected override void Render (float dt) {
        Enter(Events.Graphics);
        using (var g = Graphics.FromImage(ui)) {
            Enter(Events.Text);
            g.CompositingMode = CompositingMode.SourceCopy;
            g.FillRectangle(translucentBrush, 0, 0, 200, 100);
            g.CompositingMode = CompositingMode.SourceOver;
            g.TextRenderingHint = TextRenderingHint.SingleBitPerPixelGridFit;
            g.DrawString(FramesRendered.ToString(), font, Brushes.White, 0f, 0f);
            Leave();
        }
        Leave();
        UploadTexture();
        Viewport(0, 0, Width, Height);
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
