using UnityEngine;

/// static extensions for Vector2
public static class Vec2 {
    /// normalize the vector
    public static Vector2 Normalize(Vector2 vec) {
        vec.Normalize();
        return vec;
    }

    /// create a Vector3 with components (x, 0, y)
    public static Vector3 XZ(this Vector2 vec) {
        return new Vector3(vec.x, 0.0f, vec.y);
    }
}