namespace Engine;

using System;
using System.Numerics;
using Gl;
using static Gl.Calls;
using Shaders;
using GLFW;

class ProcTextureTest:GlWindowBase {
    public ProcTextureTest (Monitor monitor) : base(monitor) { }
    public ProcTextureTest (int width, int height) : base(width, height) { }
    protected override void Init () {

    }
    protected override void Render (float dt) {
        Viewport(new(), GetClientSize());
        Clear(BufferBit.Color | BufferBit.Depth);
    }
}
