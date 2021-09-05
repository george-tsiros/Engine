namespace Engine {
    using System;
    using System.IO;
    using System.Numerics;
    using Shaders;
    using Gl;
    using static Gl.Calls;
    using static Extra;
    using GLFW;
    using System.Diagnostics;

    class NeinCraft:GlWindowBase {
        public NeinCraft (Monitor monitor) : base(monitor) { }
        public NeinCraft (int width, int height) : base(width, height) { }
        private bool isRunning = true;
        //private readonly Camera camera = new(new(-10, 0, -1f));
        private VertexArray skybox;
        private Sampler2D skyTexture;
        private Sampler2D boxTexture;
        private VertexArray boxes;
        private VertexBuffer<Matrix4x4> modelBuffer;
        private readonly Matrix4x4[] modelMatrices = new Matrix4x4[_MAX_BOX_COUNT];
        private readonly Vector3[] boxLocations = new Vector3[_MAX_BOX_COUNT];
#pragma warning disable IDE0051
        [KeyBinding(GLFW.Keys.Pause)]
        private void PauseResume (Keys _, InputState state) {
            if (state == InputState.Release)
                isRunning = !isRunning;
        }
#pragma warning restore IDE0051

        private const int _MAX_BOX_COUNT = 10;
        private const string _SAVE = "boxes.txt";

        protected override void Init () {
            State.Program = SkyBox.Id;
            skybox = new();
            skyTexture = Sampler2D.FromFile("skybox.raw");
            SkyBox.Tex(0);
            var flipped = Geometry.CubeIndices;
            Geometry.FlipWinding(flipped);
            skybox.Assign(new VertexBuffer<Vector4>(Dex(TranslateInPlace(Geometry.CubeVertices, -.5f * Vector3.One), flipped)), SkyBox.VertexPosition);
            var uvflipped = Geometry.CubeUVIndices;
            Geometry.FlipWinding(uvflipped);
            skybox.Assign(new VertexBuffer<Vector2>(Dex(Geometry.CubeUVVectors, uvflipped)), SkyBox.VertexUV);
            var projection = Matrix4x4.CreatePerspectiveFieldOfView((float)(Math.PI / 4), (float)Width / Height, 2f, 2000f);
            SkyBox.Projection(projection);

            boxes = new();
            boxTexture = Sampler2D.FromFile("untitled.raw");
            State.Program = SimpleTexture.Id;
            boxes.Assign(new VertexBuffer<Vector4>(Dex(TranslateInPlace(Geometry.CubeVertices, -.5f * Vector3.One), Geometry.CubeIndices)), SimpleTexture.VertexPosition);
            boxes.Assign(new VertexBuffer<Vector2>(Dex(Geometry.CubeUVVectors, Geometry.CubeUVIndices)), SimpleTexture.VertexUV);
            modelBuffer = new(_MAX_BOX_COUNT);
            boxes.Assign(modelBuffer, SimpleTexture.Model, 1u);
            SimpleTexture.Tex(0);
            SimpleTexture.Projection(projection);
            if (File.Exists(_SAVE)) {
                var i = 0;
                foreach (var line in File.ReadAllLines(_SAVE)) {
                    if (i >= _MAX_BOX_COUNT)
                        break;
                    var parts = line.Split(',');
                    Debug.Assert(parts.Length == 3);
                    boxLocations[i++] = new(float.Parse(parts[0]), float.Parse(parts[1]), float.Parse(parts[2]));
                }
            } else {
                var r = new Random();
                for (var i = 0; i < _MAX_BOX_COUNT; ++i)
                    boxLocations[i] = new(3f * (i - _MAX_BOX_COUNT / 2), -10f + 20f * (float)r.NextDouble(), -1f);
            }
            Camera.Mouse(new((int)(500 * Math.PI), 0));
        }
        private void MoveBoxes (float dt) {
            for (var i = 0; i < _MAX_BOX_COUNT; ++i)
                MoveBox(i, dt);
        }

        private void MoveBox (int i, float dt) {
            var p = boxLocations[i];
            var y = p.Y - dt;
            if (y < -10f)
                y += 20f;
            boxLocations[i] = new Vector3(p.X, y, p.Z);
        }

        protected override void Render (float dt) {

            glViewport(0, 0, Width, Height);
            glDepthMask(true);
            glClear(BufferBit.Color | BufferBit.Depth);

            State.Program = SimpleTexture.Id;
            State.VertexArray = boxes;
            State.DepthTest = true;
            State.DepthFunc = DepthFunc.Less;

            boxTexture.BindTo(0);
            SimpleTexture.View(Camera.LookAtMatrix);
            if (dt > 0) {
                MoveBoxes(dt);
                for (var i = 0; i < _MAX_BOX_COUNT; ++i)
                    modelMatrices[i] = Matrix4x4.CreateTranslation(boxLocations[i]);
                modelBuffer.BufferData(modelMatrices, _MAX_BOX_COUNT, 0, 0);
            }
            glDrawArraysInstanced(Primitive.Triangles, 0, 36, _MAX_BOX_COUNT);

            if (renderSkyBox) {
                State.Program = SkyBox.Id;
                State.VertexArray = skybox;
                State.DepthFunc = DepthFunc.LessEqual;
                skyTexture.BindTo(0);
                SkyBox.View(Camera.RotationOnly);
                glDrawArrays(Primitive.Triangles, 0, 36);
            }
        }
        protected override void OnClose () {
            File.WriteAllLines(_SAVE, Array.ConvertAll(boxLocations, v => $"{v.X},{v.Y},{v.Z}"));
        }

        private bool renderSkyBox;
        [KeyBinding(Keys.F1, "toggle skybox rendering")]
        protected void ToggleSkyBox (Keys _, InputState state) {
            if (state == InputState.Release)
                renderSkyBox = !renderSkyBox;
        }

    }
}

