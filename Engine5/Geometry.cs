namespace Engine;

using System.Collections.Generic;
using System.Diagnostics;
using System.Numerics;
using Gl;

static class Geometry {

    public static void FlipWinding (int[] indices) {
        var triangleCount = indices.Length / 3;
        Debug.Assert(indices.Length % 3 == 0);
        for (var i = 0; i < indices.Length; i += 3) {
            var x = indices[i + 1];
            indices[i + 1] = indices[i + 2];
            indices[i + 2] = x;
        }
    }

    public static Vector4[] CreateNormals (Vector4[] vertices, int[] indices) {
        var indexCount = indices.Length;
        var faceCount = indexCount / 3;
        Debug.Assert(indexCount % 3 == 0);
        var faces = new Vector3i[faceCount];
        for (var i = 0; i < faceCount; ++i)
            faces[i] = new Vector3i(indices[3 * i], indices[3 * i + 1], indices[3 * i + 2]);
        var faceNormals = new Vector3[faces.Length];
        for (var i = 0; i < faces.Length; ++i)
            faceNormals[i] = FaceNormal(vertices, faces[i]);
        var vertexCount = vertices.Length;
        var stack = new Stack<int>();
        var vertexNormals = new Vector4[vertexCount];
        for (var vertexIndex = 0; vertexIndex < vertexCount; ++vertexIndex) {
            for (var i = 0; i < indexCount; ++i)
                if (indices[i] == vertexIndex)
                    stack.Push(i / 3);
            var normal = Vector3.Zero;
            while (stack.Count > 0)
                normal += faceNormals[stack.Pop()];
            vertexNormals[vertexIndex] = new(Vector3.Normalize(normal), 0);
        }
        return vertexNormals;
    }
    private static Vector3 FaceNormal (Vector4[] vertices, Vector3i face) {
        var a = vertices[face.X].Xyz();
        var b = vertices[face.Y].Xyz();
        var c = vertices[face.Z].Xyz();
        var ab = b - a;
        var bc = c - b;
        return Vector3.Normalize(Vector3.Cross(ab, bc));
    }

    public static Vector4[] CreateNormals (Vector4[] vertices) {
        Debug.Assert(vertices.Length % 3 == 0);
        var normals = new Vector4[vertices.Length];
        for (var i = 0; i < vertices.Length; i += 3) {
            var a = vertices[i + 0];
            var b = vertices[i + 1];
            var c = vertices[i + 2];
            var ab = b - a;
            var bc = c - b;
            var n = Vector4.Normalize(new(Vector3.Cross(ab.Xyz(), bc.Xyz()), 0));
            normals[i + 0] = n;
            normals[i + 1] = n;
            normals[i + 2] = n;
        }
        return normals;
    }

    public static Vector4[] Quad => new Vector4[] {
            new(-1, -1, 0, 1),
            new(+1, -1, 0, 1),
            new(+1, +1, 0, 1),
            new(+1, +1, 0, 1),
            new(-1, +1, 0, 1),
            new(-1, -1, 0, 1),
        };

    public static Vector2[] QuadUV => new Vector2[] {
            new(0, 0),
            new(1, 0),
            new(1, 1),
            new(1, 1),
            new(0, 1),
            new(0, 0),
        };

    public static Vector2[] QuadUV2 => new Vector2[] {
            new(0, 0),
            new(1, 0),
            new(1, 1),
            new(0, 0),
            new(1, 1),
            new(0, 1),
        };

    public static Vector2i[] QuadUVi => new Vector2i[] {
            new(0, 0),
            new(1, 0),
            new(1, 1),
            new(0, 0),
            new(1, 1),
            new(0, 1),
        };

    public static int[] QuadUVIndices => new int[] { 0, 1, 2, 0, 2, 3, };
    /*
       4-----5
      /     /
     /     / |
    7-----6  |
    |  0  |  |1
    |     | /
    |     |/
    3-----2
    01234567890123456789012345678901234567890
    ┌─────────┬─────────┬─────────┬─────────┐
    │         │         │         │         │
    │         │         │         │         │
    │         │         │         │         │
    ├─────────┼─────────┼─────────┼─────────┤
    │         │         │         │         │
    │         │         │         │         │
    │         │         │         │         │
    ├─────────┼─────────┼─────────┼─────────┤
    │         │         │         │         │
    │         │         │         │         │
    │         │         │         │         │
    ├─────────┼─────────┼─────────┼─────────┤
    │         │         │         │         │
    │         │         │         │         │
    │         │         │         │         │
    └─────────┴─────────┴─────────┴─────────┘

    ┌─────────┬─────────┬─────────┬─────────┐
    │0        │1        │2        │3        │4
    │         │   top   │         │         │
    │         │         │         │         │
    ├─────────┼─────────┼─────────┼─────────┤
    │5        │6        │7        │8        │9
    │  left   │  near   │  right  │   far   │
    │         │         │         │         │
    ├─────────┼─────────┼─────────┼─────────┤
    │10       │11       │12       │13       │14
    │         │ bottom  │         │         │
    │         │         │         │         │
    ├─────────┼─────────┼─────────┼─────────┤
    │15       │16       │17       │18       │19
    │         │         │         │         │
    │         │         │         │         │
    └─────────┴─────────┴─────────┴─────────┘

    */
    public static Vector4[] CubeVertices => new Vector4[] { new(0, 0, 0, 1), new(1, 0, 0, 1), new(1, 0, 1, 1), new(0, 0, 1, 1), new(0, 1, 0, 1), new(1, 1, 0, 1), new(1, 1, 1, 1), new(0, 1, 1, 1), };
    public static Vector2[] CubeUVVectors => new Vector2[] {
            new(0.00f, 0.00f),
            new(0.25f, 0.00f),
            new(0.50f, 0.00f),
            new(0.75f, 0.00f),
            new(1.00f, 0.00f),

            new(0.00f, 0.25f),
            new(0.25f, 0.25f),
            new(0.50f, 0.25f),
            new(0.75f, 0.25f),
            new(1.00f, 0.25f),

            new(0.00f, 0.50f),
            new(0.25f, 0.50f),
            new(0.50f, 0.50f),
            new(0.75f, 0.50f),
            new(1.00f, 0.50f),

            new(0.00f, 0.75f),
            new(0.25f, 0.75f),
            new(0.50f, 0.75f),
            new(0.75f, 0.75f),
            new(1.00f, 0.75f),

            new(0.00f, 1.00f),
            new(0.25f, 1.00f),
            new(0.50f, 1.00f),
            new(0.75f, 1.00f),
            new(1.00f, 1.00f),

        };
    public static int[] CubeIndices => new int[] {
            1, 5, 6, 6, 2, 1, // right
            0, 3, 7, 7, 4, 0, // left
            4, 7, 6, 6, 5, 4, // top
            0, 1, 2, 2, 3, 0, // bottom
            2, 6, 7, 7, 3, 2, // near
            0, 4, 5, 5, 1, 0, // far
        };
    public static int[] CubeUVIndices => new int[] {
            13, 8, 7, 7 , 12, 13,
            10, 11, 6, 6, 5, 10,
            6, 7, 2, 2, 1, 6,
            16, 17, 12, 12, 11, 16,
            12, 7, 6, 6, 11, 12,
            14, 9, 8, 8, 13, 14,
        };
    public static Vector4[] CubeNormals => new Vector4[] { Vector4.UnitX, -Vector4.UnitX, Vector4.UnitY, -Vector4.UnitY, Vector4.UnitZ, -Vector4.UnitZ, };
    public static int[] CubeNormalIndices => new int[] { 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 2, 2, 2, 2, 2, 2, 3, 3, 3, 3, 3, 3, 4, 4, 4, 4, 4, 4, 5, 5, 5, 5, 5, 5, };
}
