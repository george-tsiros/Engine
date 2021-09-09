namespace Shaders;
using System.Collections.Generic;
using System.Numerics;
using System;
using Gl;
public abstract class Vao {
    public int Id { get; } = Gl.Calls.CreateVertexArray();
    protected abstract void Draw ();
    protected abstract int Program { get; }
    public void Render () {
        State.VertexArray = Id;
        State.Program = Program;
        Draw();
    }

    protected static void Assign<T> (int vao, VertexBuffer<T> buffer, int location, int divisor = 0) where T : unmanaged {
        State.VertexArray = vao;
        Calls.BindBuffer(BufferTarget.Array, buffer);
        var (size, type) = SizeAndTypeOf(typeof(T));
        if (size > 4)
            for (var i = 0; i < 4; ++i)
                Attrib(vao, location + i, 4, type, 16 * sizeof(float), 4 * i * sizeof(float), divisor);
        else
            Attrib(vao, location, size, type, 0, 0, divisor);
    }
    private static void Attrib (int id, int location, int size, AttribType type, int stride, int offset, int divisor) {
        Calls.EnableVertexArrayAttrib(id, location);
        Calls.VertexAttribPointer(location, size, type, false, stride, offset);
        Calls.VertexAttribDivisor(location, divisor);
    }

    private static (int size, AttribType type) SizeAndTypeOf (Type type) => _TYPES.TryGetValue(type, out var i) ? i : throw new ArgumentException($"unsupported type {type.Name}", nameof(type));
    private static readonly Dictionary<Type, (int, AttribType)> _TYPES = new() {
        { typeof(float), (1, AttribType.Float) },
        { typeof(double), (1, AttribType.Double) },
        { typeof(int), (1, AttribType.Int) },
        { typeof(uint), (1, AttribType.UInt) },
        { typeof(Vector2), (2, AttribType.Float) },
        { typeof(Vector3), (3, AttribType.Float) },
        { typeof(Vector4), (4, AttribType.Float) },
        { typeof(Vector2i), (2, AttribType.Int) },
        { typeof(Vector3i), (3, AttribType.Int) },
        { typeof(Matrix4x4), (16, AttribType.Float) },
    };
}

public partial class PlotVao:Vao {
    private static int program;
    protected override int Program => program;

    private Notifiable<float> a;
    private Notifiable<float> b;
    private Notifiable<Vector4> color;

    public void SetA (float value) => a.Value = value;
    public void SetB (float value) => b.Value = value;
    public void SetColor (Vector4 value) => color.Value = value;
    public void AssignX (VertexBuffer<float> b) => Assign(Id, b, Plot.X);
    public void AssignY (VertexBuffer<float> b) => Assign(Id, b, Plot.Y);
}
partial class PlotVao {
    protected override void Draw () {
        State.DepthTest = false;
        if (a.Changed)
            Plot.A(a);
        if (b.Changed)
            Plot.B(b);
        if (color.Changed)
            Plot.Color(color);
        Calls.DrawArrays(Primitive.LineStrip, 0, 1);
    }
}
public class PassThroughVao:Vao {
    private static int program;
    protected override int Program => program;

    private Notifiable<int> tex;
    public void Tex (int value) => tex.Value = value;
    public void AssignVertexPosition (VertexBuffer<Vector4> b) => Assign(Id, b, PassThrough.VertexPosition);

    protected override void Draw () {
        if (tex.Changed)
            PassThrough.Tex(tex);
        Calls.DrawArrays(Primitive.Triangles, 0, 6);
    }
}

public class SkyboxVao:Vao {
    private static int program;
    protected override int Program => program;

    private Notifiable<Matrix4x4> projection;
    private Notifiable<Matrix4x4> view;
    private Notifiable<int> tex;

    public void Tex (int value) => tex.Value = value;
    public void VertexPosition (VertexBuffer<Vector4> b) => Assign(Id, b, SkyBox.VertexPosition);
    public void VertexUV (VertexBuffer<Vector2> b) => Assign(Id, b, SkyBox.VertexUV);

    protected override void Draw () {
        State.DepthFunc = DepthFunction.LessEqual;
        State.DepthTest = true;
        if (tex.Changed)
            SkyBox.Tex(tex);
        if (projection.Changed)
            SkyBox.Projection(projection);
        if (view.Changed)
            SkyBox.View(view);
        Calls.DrawArrays(Primitive.Triangles, 0, 36);
    }
}
