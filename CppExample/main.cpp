#include <windows.h>

LRESULT CALLBACK WndProc(HWND hWnd, UINT message, WPARAM wParam, LPARAM lParam) {
    switch (message) {
    case WM_DESTROY:
        PostQuitMessage(0);
        break;
    case WM_KEYUP:
        if (wParam == VK_ESCAPE)
            PostQuitMessage(0);
        break;
    default:
        return DefWindowProc(hWnd, message, wParam, lParam);
    }
    return 0;
}

int WINAPI WinMain(__in HINSTANCE hInstance, __in_opt HINSTANCE hPrevInstance, __in LPSTR lpCmdLine, __in int nShowCmd) {
    WNDCLASS wc = { 0 };
    wc.lpfnWndProc = WndProc;
    wc.hInstance = hInstance;
    wc.lpszClassName = TEXT("PlainWindow");
    wc.style = CS_HREDRAW | CS_VREDRAW;
    if (!RegisterClass(&wc))
        return 1;
    HWND window = CreateWindow(wc.lpszClassName, wc.lpszClassName, WS_OVERLAPPEDWINDOW, 0, 0, 640, 480, 0, 0, hInstance, 0);
    ShowWindow(window, nShowCmd);
    UpdateWindow(window);
    MSG msg = { 0 };
again:
    switch (GetMessage(&msg, NULL, 0, 0)) {
    case -1: {
        DWORD lastError = GetLastError();
        DebugBreak();
    }
    case 0:
        break;
    default:
        DispatchMessage(&msg);
        goto again; // what about it?
    }

    UnregisterClass(wc.lpszClassName, hInstance);
    return 0;
}
