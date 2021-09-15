namespace Engine;
using System.Threading;
using Gl;
using static Gl.Calls;
using Shaders;
using System.Numerics;
using System;
using System.Diagnostics;
using GLFW;

class Stats {
    private readonly double[] values;

    public Stats (int size) {
        values = new double[size];
    }

    public int Index { get; private set; }

    public void Enter (double value) {
        values[Index] = value;
        if (++Index == values.Length)
            Index = 0;
    }

    public double GetMean () {
        var sum = 0.0;
        for (var i = 0; i < values.Length; ++i)
            sum += values[i];
        return sum / values.Length;
    }
}

class NoiseTest:GlWindowBase {
    public NoiseTest (GLFW.Monitor m) : base(m) { }
    public NoiseTest (int w, int h) : base(w, h) { }
    private VertexArray quad;
    private Sampler2D tex;
    private const int _WIDTH = 160, _HEIGHT = 120;
    private const float _XSCALE = 1000f / _WIDTH, _YSCALE = 1000f / _HEIGHT;
    private readonly byte[] pixels = new byte[_WIDTH * _HEIGHT * 4];
    private const int _THREADCOUNT = 4;
    private const int _ROWS_PER_THREAD = _HEIGHT / _THREADCOUNT;
    private FastNoiseLite[] noises;
    private CountdownEvent countdown;
    private Stats stats = new(144);
    private bool usePointers;
    unsafe private void TProcPointers (int threadIndex) {
        var ms = FramesRendered;
        var start = _ROWS_PER_THREAD * threadIndex;
        var end = start + _ROWS_PER_THREAD;
        var offset = 4 * _WIDTH * start;
        var noise = noises[threadIndex];
        fixed (byte* p = pixels)
            for (var y = start; y < end; ++y) {
                var yscaled = _YSCALE * y;
                var yscaleddelayed = yscaled + ms;
                var yscaledshifted = yscaled + _YSCALE * _HEIGHT;
                for (var x = 0; x < _WIDTH; ++x, offset += 2) {
                    var xscaled = _XSCALE * x;

                    var blue = noise.GetNoise(xscaled + ms, yscaled);
                    var green = noise.GetNoise(xscaled + _XSCALE * _WIDTH, yscaleddelayed);
                    var red = noise.GetNoise(xscaled, yscaledshifted);
                    p[offset] = (byte)(127.5f * blue + 127.5);
                    p[++offset] = (byte)(127.5f * green + 127.5);
                    p[++offset] = (byte)(127.5f * red + 127.5);
                }
            }
        _ = countdown.Signal();
    }
    private void TProcArrays (int threadIndex) {
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
                pixels[offset] = (byte)(127.5f * blue + 127.5);
                pixels[++offset] = (byte)(127.5f * green + 127.5);
                pixels[++offset] = (byte)(127.5f * red + 127.5);
            }
        }
        _ = countdown.Signal();
    }
    unsafe protected override void Init () {
        quad = new();
        quad.Assign(new VertexBuffer<Vector4>(Geometry.Quad), PassThrough.VertexPosition);
        tex = new(_WIDTH, _HEIGHT, TextureInternalFormat.Rgba8) { Min = MinFilter.Nearest, Mag = MagFilter.Nearest, Wrap = Wrap.ClampToEdge };
        noises = new FastNoiseLite[_THREADCOUNT];
        for (var i = 3; i < pixels.Length; i += 4)
            pixels[i] = byte.MaxValue;
        for (var i = 0; i < _THREADCOUNT; ++i) {
            noises[i] = new FastNoiseLite(123);
            noises[i].SetNoiseType(noiseType);
        }
        countdown = new(_THREADCOUNT);
    }
    [KeyBinding(Keys.M)]
    protected void ToggleMethod (Keys _, InputState state) {
        if (state == InputState.Release)
            usePointers = !usePointers;
    }
    private FastNoiseLite.NoiseType noiseType;
    [KeyBinding(Keys.N)]
    protected void CycleNoiseType (Keys _, InputState state) {
        if (state == InputState.Release) {
            noiseType = noiseType == FastNoiseLite.NoiseType.Value ? FastNoiseLite.NoiseType.OpenSimplex2 : noiseType + 1;
            for (var i = 0; i < noises.Length; ++i)
                noises[i].SetNoiseType(noiseType);
        }
    }
    unsafe protected override void Render (float dt) {
        var t0 = GetTicks();
        if (FramesRendered > 0) {
            countdown.Wait();
            countdown.Reset(_THREADCOUNT);
        }
        stats.Enter((GetTicks() - t0) / (double)Stopwatch.Frequency);
        if (stats.Index == 0)
            Utilities.Trace(Math.Round(1000.0 * stats.GetMean(), 6).ToString());

        fixed (byte* p = pixels)
            TextureSubImage2D(tex, 0, 0, 0, tex.Width, tex.Height, Const.BGRA, Const.UNSIGNED_BYTE, p);
        for (var i = 0; i < _THREADCOUNT; ++i) {
            var ok = ThreadPool.QueueUserWorkItem(usePointers ? TProcPointers : TProcArrays, i, true);
            if (!ok)
                throw new ApplicationException();
        }
        Viewport(0, 0, Width, Height);
        ClearColor(0f, 0f, 0f, 1f);
        Clear(BufferBit.Color | BufferBit.Depth);
        State.Program = PassThrough.Id;
        State.VertexArray = quad;
        tex.BindTo(1);
        PassThrough.Tex(1);
        DrawArrays(Primitive.Triangles, 0, 6);
    }
}
