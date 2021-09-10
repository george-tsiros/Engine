namespace GLFW;
using System;
using System.Drawing;
/// <summary>
///     Arguments used when a window content scaling is changed.
/// </summary>
public class ContentScaleEventArgs:EventArgs {
    /// <summary>
    /// </summary>
    /// <param name="xScale">The new scale on the x-axis.</param>
    /// <param name="yScale">The new scale on the y-axis.</param>
    public ContentScaleEventArgs (float xScale, float yScale) {
        XScale = xScale;
        YScale = yScale;
    }
    /// <summary>
    ///     Gets the new scale on the x-axis.
    /// </summary>
    public float XScale { get; }
    /// <summary>
    ///     Gets the new scale on the y-axis.
    /// </summary>
    public float YScale { get; }
}
/// <summary>
///     Arguments supplied with char input events.
/// </summary>
/// <seealso cref="System.EventArgs" />
public class CharEventArgs:EventArgs {
    /// <summary>
    ///     Initializes a new instance of the <see cref="CharEventArgs" /> class.
    /// </summary>
    /// <param name="codePoint">A UTF-32 code point.</param>
    /// <param name="mods">The modifier keys present.</param>
    public CharEventArgs (uint codePoint, ModifierKeys mods) {
        CodePoint = codePoint;
        ModifierKeys = mods;
    }
    /// <summary>
    ///     Gets the Unicode character for the code point.
    /// </summary>
    /// <value>
    ///     The character.
    /// </value>
    public string Char => char.ConvertFromUtf32(unchecked((int)CodePoint));
    /// <summary>
    ///     Gets the platform independent code point.
    ///     <para>This value can be treated as a UTF-32 code point.</para>
    /// </summary>
    /// <value>
    ///     The code point.
    /// </value>
    public uint CodePoint { get; }
    /// <summary>
    ///     Gets the modifier keys.
    /// </summary>
    /// <value>
    ///     The modifier keys.
    /// </value>
    public ModifierKeys ModifierKeys { get; }
}
/// <summary>
///     Arguments supplied with file drag-drop events.
/// </summary>
/// <seealso cref="EventArgs" />
public class FileDropEventArgs:EventArgs {
    /// <summary>
    ///     Initializes a new instance of the <see cref="FileDropEventArgs" /> class.
    /// </summary>
    /// <param name="filenames">The dropped filenames.</param>
    public FileDropEventArgs (string[] filenames) { Filenames = filenames; }
    /// <summary>
    ///     Gets the filenames of the dropped files.
    /// </summary>
    /// <value>
    ///     The filenames.
    /// </value>
    public string[] Filenames { get; }
}
/// <summary>
///     Arguments supplied with keyboard events.
/// </summary>
/// <seealso cref="EventArgs" />
public class KeyEventArgs:EventArgs {
    /// <summary>
    ///     Initializes a new instance of the <see cref="KeyEventArgs" /> class.
    /// </summary>
    /// <param name="key">The key.</param>
    /// <param name="scanCode">The platform scan code of the key.</param>
    /// <param name="state">The state of the key.</param>
    /// <param name="mods">The modifier keys.</param>
    public KeyEventArgs (Keys key, int scanCode, InputState state, ModifierKeys mods) {
        Key = key;
        ScanCode = scanCode;
        State = state;
        Modifiers = mods;
    }
    /// <summary>
    ///     Gets the key whose state change raised the event.
    /// </summary>
    /// <value>
    ///     The key.
    /// </value>
    public Keys Key { get; }
    /// <summary>
    ///     Gets the modifier keys at the time of the event.
    /// </summary>
    /// <value>
    ///     The modifiers.
    /// </value>
    public ModifierKeys Modifiers { get; }
    /// <summary>
    ///     Gets the platform scan code for the key.
    /// </summary>
    /// <value>
    ///     The scan code.
    /// </value>
    public int ScanCode { get; }
    /// <summary>
    ///     Gets the state of the key.
    /// </summary>
    /// <value>
    ///     The state.
    /// </value>
    public InputState State { get; }
}
/// <summary>
///     Arguments supplied with maximize events.
/// </summary>
public class MaximizeEventArgs:EventArgs {
    /// <summary>
    ///     Initializes a new instance of the <see cref="MaximizeEventArgs" /> class.
    /// </summary>
    /// <param name="maximized"><c>true</c> it maximized, otherwise <c>false</c>.</param>
    public MaximizeEventArgs (bool maximized) { IsMaximized = maximized; }
    /// <summary>
    ///     Gets value indicating if window was maximized, or being restored.
    /// </summary>
    public bool IsMaximized { get; }
}
/// <summary>
///     Arguments supplied with mouse button events.
/// </summary>
/// <seealso cref="EventArgs" />
public class MouseButtonEventArgs:EventArgs {
    /// <summary>
    ///     Initializes a new instance of the <see cref="MouseButtonEventArgs" /> class.
    /// </summary>
    /// <param name="button">The mouse button.</param>
    /// <param name="state">The state of the <paramref name="button" />.</param>
    /// <param name="modifiers">The modifier keys.</param>
    public MouseButtonEventArgs (MouseButton button, InputState state, ModifierKeys modifiers) {
        Button = button;
        Action = state;
        Modifiers = modifiers;
    }
    /// <summary>
    ///     Gets the state of the mouse button when the event was raised.
    /// </summary>
    /// <value>
    ///     The action.
    /// </value>
    public InputState Action { get; }
    /// <summary>
    ///     Gets the mouse button that raised the event.
    /// </summary>
    /// <value>
    ///     The button.
    /// </value>
    public MouseButton Button { get; }
    /// <summary>
    ///     Gets the key modifiers present when mouse button was pressed.
    /// </summary>
    /// <value>
    ///     The modifiers.
    /// </value>
    public ModifierKeys Modifiers { get; }
}
/// <summary>
///     Arguments supplied with mouse movement events.
/// </summary>
/// <seealso cref="EventArgs" />
public class MouseMoveEventArgs:EventArgs {
    /// <summary>
    ///     Initializes a new instance of the <see cref="MouseMoveEventArgs" /> class.
    /// </summary>
    /// <param name="x">
    ///     The cursor x-coordinate, relative to the left edge of the client area, or the amount of movement on
    ///     x-axis if this is scroll event.
    /// </param>
    /// <param name="y">
    ///     The cursor y-coordinate, relative to the left edge of the client area, or the amount of movement on
    ///     y-axis if this is scroll event.
    /// </param>
    public MouseMoveEventArgs (double x, double y) {
        X = x;
        Y = y;
    }
    /// <summary>
    ///     Gets the position of the mouse, relative to the screen.
    /// </summary>
    /// <value>
    ///     The position.
    /// </value>
    public Point Position => new Point(Convert.ToInt32(X), Convert.ToInt32(Y));
    /// <summary>
    ///     Gets the cursor x-coordinate, relative to the left edge of the client area, or the amount of movement on
    ///     x-axis if this is scroll event.
    /// </summary>
    /// <value>
    ///     The location on the x-axis.
    /// </value>
    public double X { get; }
    /// <summary>
    ///     Gets the cursor y-coordinate, relative to the left edge of the client area, or the amount of movement on
    ///     y-axis if this is scroll event.
    /// </summary>
    /// <value>
    ///     The location on the y-axis.
    /// </value>
    public double Y { get; }
}
/// <summary>
///     Arguments supplied with size changing events.
/// </summary>
/// <seealso cref="EventArgs" />
public class SizeChangeEventArgs:EventArgs {
    /// <summary>
    ///     Gets the new size.
    /// </summary>
    /// <value>
    ///     The size.
    /// </value>
    public Size Size { get; }
    /// <summary>
    ///     Initializes a new instance of the <see cref="SizeChangeEventArgs" /> class.
    /// </summary>
    /// <param name="width">The new width.</param>
    /// <param name="height">The new height.</param>
    public SizeChangeEventArgs (int width, int height) : this(new Size(width, height)) { }
    /// <summary>
    ///     Initializes a new instance of the <see cref="SizeChangeEventArgs" /> class.
    /// </summary>
    /// <param name="size">The new size.</param>
    public SizeChangeEventArgs (Size size) { Size = size; }
}
