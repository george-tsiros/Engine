namespace Engine {
    using System;

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    sealed class KeyBindingAttribute:Attribute {
        public GLFW.Keys[] Key { get; }
        public string Description { get; }
        public KeyBindingAttribute (GLFW.Keys key, string description = null) => (Key, Description) = (new GLFW.Keys[] { key }, description);
        public KeyBindingAttribute (params GLFW.Keys[] keys) => (Key, Description) = (keys, null);
    }
}

