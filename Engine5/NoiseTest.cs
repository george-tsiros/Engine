namespace Engine;
using System.Threading;
using Gl;
using static Gl.Calls;
using Shaders;
using System.Numerics;
using System;

class NoiseTest:GlWindowBase {
    public NoiseTest (GLFW.Monitor m) : base(m) => Assert();
    public NoiseTest (int w, int h) : base(w, h) => Assert();
    private static void Assert () {
        if (_HEIGHT % _THREADCOUNT != 0)
            throw new ApplicationException();
    }
    private VertexArray quad;
    private Sampler2D tex;
    private const int _WIDTH = 256, _HEIGHT = 180;
    private const float _XSCALE = 1000f / _WIDTH, _YSCALE = 1000f / _HEIGHT;
    private readonly byte[] bytes = new byte[_WIDTH * _HEIGHT * 4];
    private const int _THREADCOUNT = 4;
    private const int _ROWS_PER_THREAD = _HEIGHT / _THREADCOUNT;
    private FastNoiseLite[] noises;
    private CountdownEvent countdown;

    private void ProcArrays (int threadIndex) {
        var ms = FramesRendered;
        var start = _ROWS_PER_THREAD * threadIndex;
        var end = start + _ROWS_PER_THREAD;
        var offset = 4 * _WIDTH * start;
        var noise = noises[threadIndex];
        for (var y = start; y < end; ++y) {
            var yscaled = _YSCALE * y;
            var yscaleddelayed = yscaled + ms;
            var yscaledshifted = yscaled + _YSCALE * _HEIGHT;
            for (var x = 0; x < _WIDTH; ++x, offset += 2) {
                var xscaled = _XSCALE * x;

                var blue = noise.GetNoise(xscaled + ms, yscaled);
                var green = noise.GetNoise(xscaled + _XSCALE * _WIDTH, yscaleddelayed);
                var red = noise.GetNoise(xscaled, yscaledshifted);
                bytes[offset] = (byte)(127.5f * blue + 127.5);
                bytes[++offset] = (byte)(127.5f * green + 127.5);
                bytes[++offset] = (byte)(127.5f * red + 127.5);
            }
        }
        _ = countdown.Signal();
    }

    unsafe protected override void Init () {
        quad = new();
        quad.Assign(new VertexBuffer<Vector4>(Quad.Vertices), PassThrough.VertexPosition);
        tex = new(new(_WIDTH, _HEIGHT), TextureInternalFormat.Rgba8) { Min = MinFilter.Nearest, Mag = MagFilter.Nearest, Wrap = Wrap.ClampToEdge };
        noises = new FastNoiseLite[_THREADCOUNT];
        for (var i = 3; i < bytes.Length; i += 4)
            bytes[i] = byte.MaxValue;
        for (var i = 0; i < _THREADCOUNT; ++i)
            noises[i] = new FastNoiseLite(123);
        countdown = new(_THREADCOUNT);
        StartThreads();
    }

    unsafe protected override void Render (float dt) {
        countdown.Wait();
        countdown.Reset(_THREADCOUNT);
        fixed (byte* p = bytes)
            TextureSubImage2D(tex, 0, 0, 0, tex.Width, tex.Height, TextureFormat.Bgra, Const.UNSIGNED_BYTE, p);

        Viewport(new(), GetClientSize());
        ClearColor(0f, 0f, 0f, 1f);
        Clear(BufferBit.Color | BufferBit.Depth);
        State.Program = PassThrough.Id;
        State.VertexArray = quad;
        tex.BindTo(1);
        PassThrough.Tex(1);
        DrawArrays(Primitive.Triangles, 0, 6);
        StartThreads();
    }

    private unsafe void StartThreads () {
        for (var i = 0; i < _THREADCOUNT; ++i) {
            var ok = ThreadPool.QueueUserWorkItem(ProcArrays, i, true);
            if (!ok)
                throw new ApplicationException();
        }
    }
}
