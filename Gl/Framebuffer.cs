namespace Gl {
    using static Calls;

    public sealed class Framebuffer {
        public static implicit operator int (Framebuffer framebuffer) => framebuffer.Id;
        public int Id { get; }
        public int Width { get; }
        public int Height { get; }
        public FramebufferStatus Status => glCheckNamedFramebufferStatus(Id, FramebufferTarget.Framebuffer);
        public Framebuffer (int width, int height) {
            (Width, Height) = (width, height);
            Id = CreateFramebuffers(1)[0];
            var depthbuffer = CreateRenderbuffers(1)[0];
            glNamedRenderbufferStorage(depthbuffer, RenderbufferFormat.DepthComponent, Width, Height);
            glBindRenderbuffer(Const.RENDERBUFFER, depthbuffer);
            glNamedFramebufferRenderbuffer(Id, Attachment.Depth, Const.RENDERBUFFER, depthbuffer);
        }

        public Sampler2D Add (TextureInternalFormat format, Attachment attachment) {
            var sampler = new Sampler2D(Width, Height, format) { Mag = MagFilter.Nearest, Min = MinFilter.Nearest, Wrap = Wrap.ClampToEdge };
            glNamedFramebufferTexture(Id, attachment, sampler, 0);
            return sampler;
        }
    }
}