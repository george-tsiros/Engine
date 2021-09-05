#include <windows.h>
#include <GL/GL.h>
#include <gl/GLU.h>
#pragma comment (lib, "opengl32.lib")

LRESULT CALLBACK WndProc(HWND hWnd, UINT message, WPARAM wParam, LPARAM lParam);
int running = 1;
enum Capability {
    DepthTest = GL_DEPTH_TEST,
    CullFace = GL_CULL_FACE,
    Dither = GL_DITHER,
    Blend = GL_BLEND,
    LineSmooth = GL_LINE_SMOOTH,
    StencilTest = GL_STENCIL_TEST,
};
HGLRC ctx;
int WinMain(__in HINSTANCE hInstance, __in_opt HINSTANCE hPrevInstance, __in_opt LPSTR lpCmdLine, __in int nShowCmd) {
    MSG msg = { 0 };
    WNDCLASS wc = { 0 };
    wc.lpfnWndProc = WndProc;
    wc.hInstance = hInstance;
    wc.hbrBackground = (HBRUSH)(COLOR_BACKGROUND);
    wc.lpszClassName = L"oglversionchecksample";
    wc.style = CS_OWNDC;
    if (!RegisterClass(&wc))
        return 1;
    CreateWindowW(wc.lpszClassName, L"openglversioncheck", WS_OVERLAPPEDWINDOW | WS_VISIBLE, 0, 0, 640, 480, 0, 0, hInstance, 0);

    while (running) {
        while (GetMessage(&msg, NULL, 0, 0) > 0)
            DispatchMessage(&msg);
        glClearColor(.1f, .1f, .1f, 1.f);
        glClear(GL_COLOR_BUFFER_BIT | GL_DEPTH_BUFFER_BIT);
    }
    wglDeleteContext(ctx);

    return 0;
}
Capability AllCapabilities[] = {
    Capability::Blend,
    Capability::CullFace,
    Capability::LineSmooth,
    Capability::StencilTest,
    Capability::Dither,
    Capability::DepthTest,
};
LRESULT CALLBACK WndProc(HWND hWnd, UINT message, WPARAM wParam, LPARAM lParam) {
    switch (message) {
    case WM_KEYUP:
    {
        running = 0;
       //GLboolean enabled = glIsEnabled(GL_DEPTH_TEST);
       //if (enabled)
       //    glDisable(GL_DEPTH_TEST);
       //else
       //    glEnable(GL_DEPTH_TEST);
       //if (enabled == glIsEnabled(GL_DEPTH_TEST))
       //    DebugBreak();
    }
    break;
    case WM_CREATE:
    {
        PIXELFORMATDESCRIPTOR pfd =
        {
            sizeof(PIXELFORMATDESCRIPTOR),
            1,
            PFD_DRAW_TO_WINDOW | PFD_SUPPORT_OPENGL | PFD_DOUBLEBUFFER,    //Flags
            PFD_TYPE_RGBA,        // The kind of framebuffer. RGBA or palette.
            32,                   // Colordepth of the framebuffer.
            0, 0, 0, 0, 0, 0,
            0,
            0,
            0,
            0, 0, 0, 0,
            24,                   // Number of bits for the depthbuffer
            8,                    // Number of bits for the stencilbuffer
            0,                    // Number of Aux buffers in the framebuffer.
            PFD_MAIN_PLANE,
            0,
            0, 0, 0
        };

        HDC hdc = GetDC(hWnd);

        int  aPixelFormat;
        aPixelFormat = ChoosePixelFormat(hdc, &pfd);
        SetPixelFormat(hdc, aPixelFormat, &pfd);
        ctx = wglCreateContext(hdc);
        wglMakeCurrent(hdc, ctx);
        ReleaseDC(hWnd, hdc);
    }
    break;
    default:
        return DefWindowProc(hWnd, message, wParam, lParam);
    }
    return 0;
}