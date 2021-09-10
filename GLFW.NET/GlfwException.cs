namespace GLFW;

public class GlfwException:Exception {

    public static string GetErrorMessage (ErrorCode code) => code switch {
        ErrorCode.NotInitialized => Strings.NotInitialized,
        ErrorCode.NoCurrentContext => Strings.NoCurrentContext,
        ErrorCode.InvalidEnum => Strings.InvalidEnum,
        ErrorCode.InvalidValue => Strings.InvalidValue,
        ErrorCode.OutOfMemory => Strings.OutOfMemory,
        ErrorCode.ApiUnavailable => Strings.ApiUnavailable,
        ErrorCode.VersionUnavailable => Strings.VersionUnavailable,
        ErrorCode.PlatformError => Strings.PlatformError,
        ErrorCode.FormatUnavailable => Strings.FormatUnavailable,
        ErrorCode.NoWindowContext => Strings.NoWindowContext,
        _ => Strings.UnknownError,
    };

    public GlfwException (ErrorCode error) : base(GetErrorMessage(error)) { }
    public GlfwException (string message) : base(message) { }
}
