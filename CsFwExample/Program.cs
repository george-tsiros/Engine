namespace CsFwExample;

using System;
using System.Runtime.InteropServices;
using GLFW;

enum Capability
{
    DepthTest = 0x0B71,
    CullFace = 0x0B44,
    Dither = 0x0BD0,
    Blend = 0x0BE2,
    LineSmooth = 0x0B20,
    StencilTest = 0x0B90
}

class Program
{
    delegate void ClearColor(float r, float g, float b, float a);
    delegate void Clear(int mask);
    delegate bool IsEnabled(Capability capability);
    delegate void Enable(Capability capability);
    delegate void Disable(Capability capability);

    static ClearColor glClearColor;
    static Clear glClear;
    static IsEnabled glIsEnabled;
    static Enable glEnable;
    static Disable glDisable;
    static ErrorCallback onError;
    static void OnError(ErrorCode code, IntPtr ptr) => throw new ApplicationException(Marshal.PtrToStringAnsi(ptr) ?? "?");

    static KeyCallback onKey;
    static void OnKey(IntPtr window, Keys key, int scanCode, InputState state, ModifierKeys mods)
    {
        if (state == InputState.Release && key == Keys.Escape)
            Glfw.SetWindowShouldClose(glfwWindow, true);
    }
    const int GL_DEPTH_BUFFER_BIT = 0x0100, GL_COLOR_BUFFER_BIT = 0x04000;
    const int _WIDTH = 800, _HEIGHT = 600;

    static Window glfwWindow;
    static void GetDelegateFor<T>(ref T field, string name) where T : Delegate => field = Marshal.GetDelegateForFunctionPointer<T>(Glfw.GetProcAddress(name));
    static void Dump(int i)
    {
        Console.Write($"{framecount,-5} {i,-5}");
        foreach (var c in AllCapabilities)
        {
            Console.ForegroundColor = glIsEnabled(c) ? ConsoleColor.Green : ConsoleColor.Red;
            Console.Write(c.ToString());
            Console.Write(' ');
        }
        Console.ResetColor();
        Console.WriteLine();
    }
    static int lastState;
    static void MaybeDump(int i)
    {
        var currentState = GetState();
        if (currentState != lastState)
        {
            Dump(i);
            lastState = currentState;
        }
    }

    static int GetState()
    {
        var state = 0;
        for (var i = 0; i < AllCapabilities.Length; ++i)
            if (glIsEnabled(AllCapabilities[i]))
                state |= 1 << i;
        return state;
    }

    static readonly Capability[] AllCapabilities = new Capability[] {
            Capability.Blend,
            Capability.CullFace, 
            //Capability.LineSmooth, 
            Capability.StencilTest, 
            //Capability.Dither, 
            Capability.DepthTest,
        };
    static long framecount = 0l;
    [STAThread]
    static void Main()
    {
        if (!Glfw.Init())
            throw new ApplicationException();
        onError = new ErrorCallback(OnError);
        _ = Glfw.SetErrorCallback(onError);
        Glfw.WindowHint(Hint.Resizable, false);
        Glfw.WindowHint(Hint.OpenglForwardCompatible, true);
        Glfw.WindowHint(Hint.OpenglProfile, Profile.Core);
        Glfw.WindowHint(Hint.ContextVersionMajor, 4);
        Glfw.WindowHint(Hint.ContextVersionMinor, 6);
        Glfw.WindowHint(Hint.DepthBits, 24);
        Glfw.WindowHint(Hint.Doublebuffer, true);
        glfwWindow = Glfw.CreateWindow(_WIDTH, _HEIGHT, "", Monitor.None, Window.None);
        onKey = new KeyCallback(OnKey);
        _ = Glfw.SetKeyCallback(glfwWindow, onKey);
        Glfw.MakeContextCurrent(glfwWindow);
        GetDelegateFor(ref glClear, nameof(glClear));
        GetDelegateFor(ref glClearColor, nameof(glClearColor));
        GetDelegateFor(ref glEnable, nameof(glEnable));
        GetDelegateFor(ref glDisable, nameof(glDisable));
        GetDelegateFor(ref glIsEnabled, nameof(glIsEnabled));
        Glfw.ShowWindow(glfwWindow);
        glDisable(Capability.Dither);
        MaybeDump(-1);
        while (!Glfw.WindowShouldClose(glfwWindow))
        {
            MaybeDump(0);
            if (framecount >= 0xFF && (framecount & 0xFF) == 0)
                Toggle(AllCapabilities[(framecount >> 8) % AllCapabilities.Length]);
            MaybeDump(1);
            Glfw.PollEvents();
            MaybeDump(2);
            glClearColor(.1f, .1f, .1f, 1f);
            MaybeDump(3);
            glClear(GL_DEPTH_BUFFER_BIT | GL_COLOR_BUFFER_BIT);
            MaybeDump(4);
            Glfw.SwapBuffers(glfwWindow);
            MaybeDump(5);
            ++framecount;
        }
        Glfw.DestroyWindow(glfwWindow);
        Glfw.Terminate();
        _ = Console.ReadLine();
    }
    static void Toggle(Capability c)
    {
        var enabled = glIsEnabled(c);
        Console.WriteLine($"{framecount,-5} toggling {c}, currently {enabled}");
        if (enabled)
            glDisable(c);
        else
            glEnable(c);
    }
}
