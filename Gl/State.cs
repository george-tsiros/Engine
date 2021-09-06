namespace Gl {
    using System.Diagnostics;
    using static Calls;

    public sealed class State {
        private static void MaybeToggle (Capability cap, bool requested) {
            var previous = glIsEnabled(cap);
            if (requested != previous) {
                (requested ? Gl.Calls.glEnable : Gl.Calls.glDisable)(cap);
                Debug.Assert(glIsEnabled(cap) == requested);
            }
        }

        public static int ActiveTexture {
            get => (int)(GetInteger(Const.ACTIVE_TEXTURE) - Const.TEXTURE0);
            set {
                if (ActiveTexture != value)
                    glActiveTexture(Const.TEXTURE0 + value);
            }
        }

        public static bool DepthTest {
            get => glIsEnabled(Capability.DepthTest);
            set => MaybeToggle(Capability.DepthTest, value);
        }
        public static bool LineSmooth {
            get => glIsEnabled(Capability.LineSmooth);
            set => MaybeToggle(Capability.LineSmooth, value);
        }
        public static bool Dither {
            get => glIsEnabled(Capability.Dither);
            set => MaybeToggle(Capability.Dither, value);
        }
        public static bool DebugOutput {
            get => glIsEnabled(Capability.DebugOutput);
            set => MaybeToggle(Capability.DebugOutput, value);
        }
        public static bool CullFace {
            get => glIsEnabled(Capability.CullFace);
            set => MaybeToggle(Capability.CullFace, value);
        }

        public static DepthFunc DepthFunc {
            get => (DepthFunc)GetInteger(Const.DEPTH_FUNC);
            set {
                if (DepthFunc != value)
                    glDepthFunc(value);
            }
        }
        public static int Framebuffer {
            get => (int)GetInteger(Const.FRAMEBUFFER_BINDING);
            set {
                if (Framebuffer != value)
                    glBindFramebuffer(Const.FRAMEBUFFER, value);
            }
        }
        public static int Program {
            get => (int)GetInteger(Const.CURRENT_PROGRAM);
            set {
                if (value != Program)
                    glUseProgram(value);
            }
        }
        public static int ArrayBuffer {
            get => (int)GetInteger(Const.ARRAY_BUFFER_BINDING);
            set {
                if (value != ArrayBuffer)
                    glBindBuffer(BufferTarget.Array, value);
            }
        }
        public static int VertexArray {
            get => (int)GetInteger(Const.VERTEX_ARRAY_BINDING);
            set {
                if (value != VertexArray)
                    glBindVertexArray(value);
            }
        }
    }
}