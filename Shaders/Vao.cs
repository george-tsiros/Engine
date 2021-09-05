namespace Shaders {
    using System.Numerics;
    using Gl;
    public abstract class Vao {
        public VertexArray V { get; } = new();
        public void Render () {
            State.VertexArray = V;
            State.Program = Program;
            Draw();
        }
        protected abstract void Draw ();
        protected abstract uint Program { get; }
    }
    public class PlotVAO:Vao {
        public PlotVAO (VertexBuffer<float> x, VertexBuffer<float> y) : base() {
            V.Assign(x, Plot.X);
            V.Assign(y, Plot.Y);
        }
        protected override uint Program => Plot.Id;
        private Notifiable<float> a, b;
        private Notifiable<Vector4> color;
        public void A (float value) => a.Value = value;
        public void B (float value) => b.Value = value;
        public void Color (Vector4 value) => color.Value = value;
        public void X (VertexBuffer<float> b) => V.Assign(b, Plot.X);
        public void Y (VertexBuffer<float> b) => V.Assign(b, Plot.Y);
        protected override void Draw () {
            State.DepthTest = false;
            if (a.Changed)
                Plot.A(a);
            if (b.Changed)
                Plot.B(b);
            if (color.Changed)
                Plot.Color(color);
            Calls.glDrawArrays(Primitive.LineStrip, 0, 1);
        }
    }
    public class PassThroughVao:Vao {
        protected override uint Program => PassThrough.Id;
        private Notifiable<int> tex;
        public void Tex (int value) => tex.Value = value;
        public void VertexPosition (VertexBuffer<Vector4> b) => V.Assign(b, PassThrough.VertexPosition);
        protected override void Draw () {
            if (tex.Changed)
                PassThrough.Tex(tex);
            Calls.glDrawArrays(Primitive.Triangles, 0, 6);
        }
    }

    public class SkyboxVao:Vao {
        protected override uint Program => SkyBox.Id;
        private Notifiable<Matrix4x4> projection;
        private Notifiable<Matrix4x4> view;
        private Notifiable<int> tex;
        public void Tex (int value) => tex.Value = value;
        public void VertexPosition (VertexBuffer<Vector4> b) => V.Assign(b, SkyBox.VertexPosition);
        public void VertexUV (VertexBuffer<Vector2> b) => V.Assign(b, SkyBox.VertexUV);
        protected override void Draw () {
            State.VertexArray = V;
            State.DepthFunc = DepthFunc.LessEqual;
            if (tex.Changed)
                SkyBox.Tex(tex);
            if (projection.Changed)
                SkyBox.Projection(projection);
            if (view.Changed)
                SkyBox.View(view);
            Calls.glDrawArrays(Primitive.Triangles, 0, 36);
        }
    }
}