using System.Runtime.CompilerServices;
using Godot;

namespace Freya {
    public static class CoreExtensions {
        // Vector2
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 ToVector3(this Vector2 v) => new Vector3(v.X, v.Y, 0);
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector4 ToVector4(this Vector2 v) => new Vector4(v.X, v.Y, 0, 0);

        // Vector3
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 ToVector2(this Vector3 v) => new Vector2(v.X, v.Y);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector4 ToVector4(this Vector3 v) => new Vector4(v.X, v.Y, v.Z, 0);

        // Vector4
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 ToVector2(this Vector4 v) => new Vector2(v.X, v.Y);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 ToVector3(this Vector4 v) => new Vector3(v.X, v.Y, v.Z);
    }
}