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
int WINAPI WinMain(__in HINSTANCE hInstance, __in_opt HINSTANCE hPrevInstance, __in_opt LPSTR lpCmdLine, __in int nShowCmd) {
    MSG msg = { 0 };
    WNDCLASS wc = { 0 };
    wc.lpfnWndProc = WndProc;
    wc.hInstance = hInstance;
    wc.hbrBackground = (HBRUSH)(BLACK_BRUSH);
    wc.lpszClassName = L"oglversionchecksample";
    wc.style = CS_OWNDC;
    if (!RegisterClass(&wc))
        return 1;
    CreateWindow(wc.lpszClassName, L"openglversioncheck", WS_OVERLAPPEDWINDOW | WS_VISIBLE, 0, 0, 640, 480, 0, 0, hInstance, 0);

    while (running) {
        while (GetMessage(&msg, NULL, 0, 0) > 0)
            DispatchMessage(&msg);
        glClearColor(.1f, .1f, .1f, 1.f);
        glClear(GL_COLOR_BUFFER_BIT | GL_DEPTH_BUFFER_BIT);
    }
    wglDeleteContext(ctx);
    return 0;
}

LRESULT CALLBACK WndProc(HWND hWnd, UINT message, WPARAM wParam, LPARAM lParam) {
    switch (message) {
    case WM_DESTROY:
    {
        PostQuitMessage(0);
        running = 0;
    }
    break;
    case WM_KEYUP:
    {
        if (running && wParam == VK_ESCAPE)
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
            PFD_DRAW_TO_WINDOW | PFD_SUPPORT_OPENGL | PFD_DOUBLEBUFFER,
            PFD_TYPE_RGBA,        
            32,                   
            0, 0, 0, 0, 0, 0,
            0,
            0,
            0,
            0, 0, 0, 0,
            24,                   
            8,                    
            0,                    
            PFD_MAIN_PLANE,
            0,
            0, 0, 0
        };

        HDC hdc = GetDC(hWnd);

        int somePixelFormat = ChoosePixelFormat(hdc, &pfd);
        SetPixelFormat(hdc, somePixelFormat, &pfd);
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