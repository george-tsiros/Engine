#include <windows.h>
#include <iostream>
#include <string>
#include <GLFW/glfw3.h>

void WriteLine(std::string str) {
    std::cout << str << std::endl;
}
int ReturnWith(std::string str, int i) {
    WriteLine(str);
    return i;
}
GLFWwindow* window;
void OnError(int errorCode, const char* message) {
    WriteLine(std::to_string(errorCode));
    WriteLine(message);
}
void OnKey(GLFWwindow* window, int key, int scanCode, int action, int modifiers) {
    if (key == GLFW_KEY_ESCAPE && action == GLFW_RELEASE)
        glfwSetWindowShouldClose(window, GLFW_TRUE);
}

int allCapabilities[] = {
    GL_DEPTH_TEST,
    GL_CULL_FACE,
    GL_DITHER,
    GL_BLEND,
    GL_LINE_SMOOTH,
    GL_STENCIL_TEST,
};

typedef void (*GlClearColor)(float, float, float, float);
typedef void (*GlClear)(int);
typedef bool (*GlIsEnabled)(int);
typedef void (*GlEnable)(int);
typedef void (*GlDisable)(int);

GlClearColor ClearColor;
GlClear Clear;
GlEnable Enable;
GlDisable Disable;
GlIsEnabled IsEnabled;
void ToggleAll() {
    for (int i = 0; i < 6; ++i) {
        auto c = allCapabilities[i];
        auto enabled = IsEnabled(c);
        if (enabled)
            Disable(c);
        else
            Enable(c);
        if (enabled == IsEnabled(c))
            DebugBreak();
    }
}
int main() {

    if (!glfwInit())
        return ReturnWith("glfwInit", -1);

    glfwWindowHint(GLFW_RESIZABLE, GLFW_FALSE);
    glfwWindowHint(GLFW_OPENGL_FORWARD_COMPAT, GLFW_TRUE);
    glfwWindowHint(GLFW_OPENGL_PROFILE, GLFW_OPENGL_CORE_PROFILE);
    glfwWindowHint(GLFW_CONTEXT_VERSION_MAJOR, 4);
    glfwWindowHint(GLFW_CONTEXT_VERSION_MINOR, 6);
    glfwWindowHint(GLFW_DEPTH_BITS, 24);
    glfwWindowHint(GLFW_DOUBLEBUFFER, GLFW_TRUE);
    window = glfwCreateWindow(800, 600, "", NULL, NULL);
    if (!window)
        return ReturnWith("create window", -1);

    glfwMakeContextCurrent(window);
    glfwSetKeyCallback(window, OnKey);
    glfwSetErrorCallback(OnError);
    ClearColor = (GlClearColor)glfwGetProcAddress("glClearColor");
    Clear = (GlClear)glfwGetProcAddress("glClear");
    IsEnabled = (GlIsEnabled)glfwGetProcAddress("glIsEnabled");
    Enable = (GlEnable)glfwGetProcAddress("glEnable");
    Disable = (GlDisable)glfwGetProcAddress("glDisable");
    while (!glfwWindowShouldClose(window)) {
        ToggleAll();
        glfwPollEvents();
        ToggleAll();
        ClearColor(.1f, .1f, .1f, 1.f);
        ToggleAll();
        Clear(GL_COLOR_BUFFER_BIT | GL_DEPTH_BUFFER_BIT);
        ToggleAll();
        glfwSwapBuffers(window);
        ToggleAll();
    }

    glfwDestroyWindow(window);
    return 0;
}