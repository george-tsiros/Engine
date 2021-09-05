namespace Engine {
    using System.Threading;
    using Gl;
    using static Gl.Calls;
    using Shaders;
    using System.Numerics;
    using System;
    using System.Diagnostics;

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
        private const int _WIDTH = 320, _HEIGHT = 200;
        private const float _XSCALE = 1000f / _WIDTH, _YSCALE = 1000f / _HEIGHT;
        private readonly byte[] pixels = new byte[_WIDTH * _HEIGHT * 4];
        private const int _THREADCOUNT = 4;
        private const int _ROWS_PER_THREAD = _HEIGHT / _THREADCOUNT;
        private FastNoiseLite[] noises;
        private CountdownEvent countdown;
        private Stats stats = new(144);
        private void TProc (ValueTuple<int, float> datum) {
            var threadIndex = datum.Item1;
            var ms = datum.Item2;
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
#if true
#else
                    var l = (byte)(255f * (red + 1f) * (green + 1f) * (blue + 1f) / 8f);
                    var l = (byte)(255f * (red + green + blue + 3f) / 6f);
#endif
                    pixels[offset] = (byte)(127.5f * blue + 127.5);
                    pixels[++offset] = (byte)(127.5f * green + 127.5);
                    pixels[++offset] = (byte)(127.5f * red + 127.5);
                }
            }
            _ = countdown.Signal();
        }
        private Raster font;
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
            font = Raster.FromFile("font.raw");
        }
        private FastNoiseLite.NoiseType noiseType;
        [KeyBinding(GLFW.Keys.N)]
        protected void CycleNoiseType (GLFW.Keys _, GLFW.InputState state) {
            if (state == GLFW.InputState.Release) {
                noiseType = noiseType == FastNoiseLite.NoiseType.Value ? FastNoiseLite.NoiseType.OpenSimplex2 : noiseType + 1;
                for (var i = 0; i < noises.Length; ++i)
                    noises[i].SetNoiseType(noiseType);
            }
        }
        unsafe protected override void Render (float dt) {
            var t0 = Timer.ElapsedTicks;
            var ms = (float)(0.1f * Timer.Elapsed.TotalMilliseconds);
            if (FrameCount > 0) {
                countdown.Wait();
                countdown.Reset(_THREADCOUNT);
            }
            stats.Enter((Timer.ElapsedTicks - t0) / (double)Stopwatch.Frequency);
            if (stats.Index == 0) {
                var time = Math.Round(1000.0 * stats.GetMean(), 6);
                Utilities.Trace(time.ToString());
            }

            fixed (byte* p = pixels)
                glTextureSubImage2D(tex, 0, 0, 0, tex.Width, tex.Height, Const.BGRA, Const.UNSIGNED_BYTE, p);
            for (var i = 0; i < _THREADCOUNT; ++i) {
                var ok = ThreadPool.QueueUserWorkItem(TProc, new ValueTuple<int, float>(i, ms), true);
                if (!ok)
                    throw new ApplicationException();
            }
            glViewport(0, 0, Width, Height);
            glClearColor(0f, 0f, 0f, 1f);
            glClear(BufferBit.Color | BufferBit.Depth);
            State.Program = PassThrough.Id;
            State.VertexArray = quad;
            tex.BindTo(1);
            PassThrough.Tex(1);
            glDrawArrays(Primitive.Triangles, 0, 6);
        }
    }
}