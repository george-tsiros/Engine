namespace Engine;

using System;

public enum TextureParameter {
    DepthStencilTextureMode = 0x90EA,
    BaseLevel = 0x813C,
    BorderColor = 0x1004,
    CompareMode = 0x884C,
    CompareFunc = 0x884D,
    LodBias = 0x8501,
    MagFilter = 0x2800,
    MaxLevel = 0x813D,
    MaxLod = 0x813B,
    MinFilter = 0x2801,
    MinLod = 0x813A,
    SwizzleR = 0x8E42,
    SwizzleG = 0x8E43,
    SwizzleB = 0x8E44,
    SwizzleA = 0x8E45,
    SwizzleRGBA = 0x8E46,
    WrapS = 0x2802,
    WrapT = 0x2803,
    WrapR = 0x8072,
}

public enum MagFilter {
    Nearest = 0x2600,
    Linear = 0x2601,
}

public enum WrapCoordinate {
    WrapR = 0x8072,
    WrapS = 0x2802,
    WrapT = 0x2803,
}

public enum Wrap {
    ClampToEdge = 0x812F,
    ClampToBorder = 0x812D,
}

public enum MinFilter {
    Nearest = 0x2600,
    Linear = 0x2601,
    NearestMipMapNearest = 0x2700,
    LinearMipMapNearest = 0x2701,
    NearestMipMapLinear = 0x2702,
    LinearMipMapLinear = 0x2703,
}

public enum TextureInternalFormat {
    R8 = 0x8229,
    R16 = 0x822A,
    Rg8 = 0x822B,
    Rg16 = 0x822C,
    Rgb8 = 0x8051,
    Rgb16 = 0x8054,
    Rgba8 = 0x8058,
    Rgba16 = 0x805B,
}

public enum VariableKind { Uniform, Attrib }

public enum Primitive {
    Points = 0x0,
    LineStrip = 0x3,
    LineLoop = 0x2,
    Lines = 0x1,
    LineStripAdjacency = 0xB,
    LinesAdjacency = 0xA,
    TriangleStrip = 0x5,
    TriangleFan = 0x6,
    Triangles = 0x4,
    TrianglesAdjacency = 0xC,
    TriangleStripAdjacency = 0xD,
}

public enum AttribType {
    Byte = 0x1400,
    UByte = 0x1401,
    Short = 0x1402,
    UShort = 0x1403,
    Int = 0x1404,
    UInt = 0x1405,
    Float = 0x1406,
    Double = 0x140A,
}

public enum UniformType {
    Double = 0x140A,
    Float = 0x1406,
    Int = 0x1404,
    Mat2d = 0x8F46,
    Mat2x3 = 0x8B65,
    Mat2x3d = 0x8F49,
    Mat2x4 = 0x8B66,
    Mat2x4d = 0x8F4A,
    Mat3d = 0x8F47,
    Mat3x2 = 0x8B67,
    Mat3x2d = 0x8F4B,
    Mat3x4 = 0x8B68,
    Mat3x4d = 0x8F4C,
    Mat4d = 0x8F48,
    Mat4x2 = 0x8B69,
    Mat4x2d = 0x8F4D,
    Mat4x3 = 0x8B6A,
    Mat4x3d = 0x8F4E,
    Matrix2x2 = 0x8B5A,
    Matrix3x3 = 0x8B5B,
    Matrix4x4 = 0x8B5C,
    Sampler2D = 0x8B5E,
    UInt = 0x1405,
    Vec2d = 0x8FFC,
    Vec2ui = 0x8DC6,
    Vec3d = 0x8FFD,
    Vec3ui = 0x8DC7,
    Vec4d = 0x8FFE,
    Vec4ui = 0x8DC8,
    Vector2 = 0x8B50,
    Vector2i = 0x8B53,
    Vector3 = 0x8B51,
    Vector3i = 0x8B54,
    Vector4 = 0x8B52,
    Vector4i = 0x8B55,
}

public enum AttributeType {
    Bool = 0x8B56,
    Double = 0x140A,
    Float = 0x1406,
    Int = 0x1404,
    Mat2d = 0x8F46,
    Mat2x3 = 0x8B65,
    Mat2x3d = 0x8F49,
    Mat2x4 = 0x8B66,
    Mat2x4d = 0x8F4A,
    Mat3d = 0x8F47,
    Mat3x2 = 0x8B67,
    Mat3x2d = 0x8F4B,
    Mat3x4 = 0x8B68,
    Mat3x4d = 0x8F4C,
    Mat4d = 0x8F48,
    Mat4x2 = 0x8B69,
    Mat4x2d = 0x8F4D,
    Mat4x3 = 0x8B6A,
    Mat4x3d = 0x8F4E,
    Matrix2x2 = 0x8B5A,
    Matrix3x3 = 0x8B5B,
    Matrix4x4 = 0x8B5C,
    Sampler1D = 0x8B5D,
    Sampler2D = 0x8B5E,
    Sampler3D = 0x8B5F,
    UInt = 0x1405,
    Vec2b = 0x8B57,
    Vec2d = 0x8FFC,
    Vec2ui = 0x8DC6,
    Vec3b = 0x8B58,
    Vec3d = 0x8FFD,
    Vec3ui = 0x8DC7,
    Vec4b = 0x8B59,
    Vec4d = 0x8FFE,
    Vec4ui = 0x8DC8,
    Vector2 = 0x8B50,
    Vector2i = 0x8B53,
    Vector3 = 0x8B51,
    Vector3i = 0x8B54,
    Vector4 = 0x8B52,
    Vector4i = 0x8B55,
}

public enum BufferTarget {
    Array = 0x8892,
    AtomicCounter = 0x92C0,
    CopyRead = 0x8F36,
    CopyWrite = 0x8F37,
    DispatchIndirect = 0x90EE,
    DrawIndirect = 0x8F3F,
    ElementArray = 0x8893,
    PixelPack = 0x88EB,
    PixelUnpack = 0x88EC,
    Query = 0x9192,
    ShaderStorage = 0x90D2,
    Texture = 0x8C2A,
    TransformFeedbackBuffer = 0x8C8E,
    Uniform = 0x8A11,
}


public enum Capability {
    Blend = 0xBE2,
    CullFace = 0xB44,
    DebugOutput = 0x92E0,
    DebugOutputSynchronous = 0x8242,
    DepthTest = 0xB71,
    Dither = 0xBD0,
    LineSmooth = 0xB20,
    StencilTest = 0xB90,
}

public enum DepthFunc {
    Never = 0x200,
    Less = 0x201,
    Equal = 0x202,
    LessEqual = 0x203,
    Greater = 0x204,
    NotEqual = 0x205,
    GreaterEqual = 0x206,
    Always = 0x207,
}

[Flags]
public enum BufferBit {
    Color = 0x4000,
    Depth = 0x100,
    Stencil = 0x400,
}

public enum DebugSource {
    Api = 0x8246,
    WindowSystem = 0x8247,
    ShaderCompiler = 0x8248,
    ThirdParty = 0x8249,
    SourceApplication = 0x824A,
    Other = 0x824B,
}

public enum DebugType {
    Error = 0x824C,
    DeprecatedBehavior = 0x824D,
    UndefinedBehavior = 0x824E,
    Portability = 0x824F,
    Performance = 0x8250,
    Marker = 0x8268,
    PushGroup = 0x8269,
    PopGroup = 0x826A,
    Other = 0x8251,
}

public enum DebugSeverity {
    Low = 0x9148,
    Medium = 0x9147,
    High = 0x9146,
}

[Flags]
public enum Direction {
    None = 0,
    PosX = 1 << 0,
    NegX = 1 << 1,
    PosY = 1 << 2,
    NegY = 1 << 3,
    PosZ = 1 << 4,
    NegZ = 1 << 5,
}

public enum CheckFramebuffer {
    DrawFramebuffer = 0x8CA9,
    Framebuffer = 0x8D40,
    ReadFramebuffer = 0x8CA8,
}

public enum ShaderParameter {
    ShaderType = 0x8B4F,
    DeleteStatus = 0x8B80,
    CompileStatus = 0x8B81,
    InfoLogLength = 0x8B84,
    ShaderSourceLength = 0x8B88,
}

public enum FramebufferTarget {
    Read = 0x8CA8,
    Draw = 0x8CA9,
    Framebuffer = 0x8D40,
}

public enum FramebufferStatus:int {
    Undefined = 0x8219,
    Complete = 0x8CD5,
    IncompleteAttachment = 0x8CD6,
    IncompleteMissingAttachment = 0x8CD7,
    IncompleteDimensionsExt = 0x8CD9,
    IncompleteFormatsExt = 0x8CDA,
    IncompleteDrawBuffer = 0x8CDB,
    IncompleteReadBuffer = 0x8CDC,
    Unsupported = 0x8CDD,
    IncompleteMultisample = 0x8D56,
    IncompleteLayerTargets = 0x8DA8,
    IncompleteLayerCount = 0x8DA9,
}

public enum Attachment {
    Depth = 0x8D00,
    Stencil = 0x8D20,
    DepthStencil = 0x821A,
    Color0 = 0x8CE0,
    Color1 = 0x8CE1,
    Color2 = 0x8CE2,
    Color3 = 0x8CE3,
    Color4 = 0x8CE4,
    Color5 = 0x8CE5,
    Color6 = 0x8CE6,
    Color7 = 0x8CE7,
    Color8 = 0x8CE8,
    Color10 = 0x8CEA,
    Color11 = 0x8CEB,
    Color12 = 0x8CEC,
    Color13 = 0x8CED,
}

public enum ProgramParameter {
    DeleteStatus = 0x8B80,
    LinkStatus = 0x8B82,
    ValidateStatus = 0x8B83,
    InfoLogLength = 0x8B84,
    AttachedShaders = 0x8B85,
    ActiveAtomicCounterBuffers = 0x92D9,
    ActiveAttributes = 0x8B89,
    ActiveAttributeMaxLength = 0x8B8A,
    ActiveUniforms = 0x8B86,
    ActiveUniformBlocks = 0x8A36,
    ActiveUniformBlockMaxNameLength = 0x8A35,
    ActiveUniformMaxLength = 0x8B87,
    TransformFeedbackBufferMode = 0x8C7F,
    TransformFeedbackVaryings = 0x8C83,
    TransformFeedbackVaryingMaxLength = 0x8C76,
    GeometryVerticesOut = 0x8916,
    GeometryInputType = 0x8917,
    GeometryOutputType = 0x8918,
}

public enum ProgramInterface {
    Uniform = 0x92E1,
    ProgramInput = 0x92E3,
    ProgramOutput = 0x92E4,
    VertexSubroutine = 0x92E8,
    FragmentSubroutine = 0x92EC,
    VertexSubroutineUniform = 0x92EE,
    FragmentSubroutineUniform = 0x92F2,
}

public enum InterfaceParameter {
    ActiveResources = 0x92F5,
    MaxNameLength = 0x92F6,
}

public enum RenderbufferFormat {
    DepthComponent = 0x1902,
    R8 = 0x8229,
    Rg8 = 0x822B,
    Rgb8 = 0x8051,
    Rgba8 = 0x8058,
    R16 = 0x822A,
    Rg16 = 0x822C,
    Rgb16 = 0x8054,
    Rgba16 = 0x805B,
}

public enum ShaderType {
    Fragment = 0x8B30,
    Vertex = 0x8B31,

}