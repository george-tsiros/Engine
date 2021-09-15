namespace Engine;

using System;
using System.Collections.Generic;
using System.Numerics;
using static Extra;
using Gl;
using GLFW;

sealed class Camera {
    private static readonly Dictionary<Keys, Vector3> _DIRECTIONS = new() {
        { Keys.Z, -Vector3.UnitX },
        { Keys.X, Vector3.UnitZ },
        { Keys.C, Vector3.UnitX },
        { Keys.D, -Vector3.UnitZ },
        { Keys.LeftShift, Vector3.UnitY },
        { Keys.LeftControl, -Vector3.UnitY },
    };
    public bool Key (Keys key, InputState state) {
        if (state != InputState.Repeat && _DIRECTIONS.TryGetValue(key, out var d)) {
            if (state == InputState.Press)
                direction += d;
            else
                direction -= d;
            return true;
        }
        return false;
    }

    public Vector3 Location;
    private Vector3 direction = Vector3.Zero;
    private float yaw = 0f, pitch = 0f;
    public Camera (Vector3 location) => Location = location;
    public Matrix4x4 RotationOnly => CreateLookAt(Vector3.Zero, yaw, pitch);

    private static Matrix4x4 CreateLookAt (Vector3 location, float yaw, float pitch) {
        var rotationAboutY = Matrix4x4.CreateRotationY(yaw);
        var straightForward = Vector3.Transform(-Vector3.UnitZ, rotationAboutY);
        var right = Vector3.Transform(Vector3.UnitX, rotationAboutY);
        var rotationAboutRight = Matrix4x4.CreateFromAxisAngle(right, pitch);
        var forward = Vector3.Transform(straightForward, rotationAboutRight);
        var up = Vector3.Transform(Vector3.UnitY, rotationAboutRight);
        return Matrix4x4.CreateLookAt(location, location + forward, up);
    }

    public Matrix4x4 LookAtMatrix => CreateLookAt(Location, yaw, pitch);

    public void Move (float dt) {
        if (dt > 0f && direction.LengthSquared() > .1f)
            Location += Vector3.Transform(Vector3.Normalize(direction) * dt, Matrix4x4.CreateRotationY(yaw));
    }
    public void Mouse (Vector2i v) {
        var scaled = 0.001f * new Vector2(v.X, v.Y);
        _ = ModuloTwoPi(ref yaw, -scaled.X);
        _ = Clamp(ref pitch, scaled.Y, (float)(-.4 * Math.PI), (float)(.4 * Math.PI));
    }
}
