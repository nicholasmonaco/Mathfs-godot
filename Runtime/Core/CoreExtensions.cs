using System;
using System.Runtime.CompilerServices;
using Godot;

namespace Freya {
    public static class CoreExtensions {
        #region Vector2

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 ToVector3(this Vector2 v) => new Vector3(v.X, v.Y, 0);
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector4 ToVector4(this Vector2 v) => new Vector4(v.X, v.Y, 0, 0);

        #endregion Vector2


        #region Vector3

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 ToVector2(this Vector3 v) => new Vector2(v.X, v.Y);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector4 ToVector4(this Vector3 v) => new Vector4(v.X, v.Y, v.Z, 0);

        #endregion Vector3


        #region Vector4

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 ToVector2(this Vector4 v) => new Vector2(v.X, v.Y);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 ToVector3(this Vector4 v) => new Vector3(v.X, v.Y, v.Z);

        #endregion Vector4


        #region Quaternion

        // Creates a rotation quaternion that rotates angle degrees around axis.
        // The magnitude of axis is not and should not be considered.
        // From:
        // https://discussions.unity.com/t/whats-the-source-code-of-quaternion-fromtorotation/227717/2
        public static Quaternion AngleAxis(float angle, Vector3 axis) {
            float rad = angle * Mathfs.Deg2Rad * 0.5f;
            Vector3 axisNorm = axis.Normalized() * MathF.Sin(rad);
            return new Quaternion(axisNorm.X, axisNorm.Y, axisNorm.Z, MathF.Cos(rad));
        }

        // Inverted version of the above
        public static void ToAngleAxis(this Quaternion q, out float angle, out Vector3 axis) {
            float rad = MathF.Acos(q.W);
            angle = rad / (Mathfs.Deg2Rad * 0.5f);
            Vector3 axisNorm = new Vector3(q.X, q.Y, q.Z);
            axis = axisNorm / MathF.Sin(rad);
        }

        #endregion Quaternion

        
        #region Transform3D

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 TransformPoint(this Transform3D t, Vector3 point) {
            // This only transforms the point relative to the transform, irrelavent to anything else.
            // It does not care about the game world or node hierarchy at all.
            return t.Basis.GetRotationQuaternion() * (point * t.Basis.Scale) + t.Origin;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 InverseTransformPoint(this Transform3D t, Vector3 point) {
            // Note: This method expects the transform to be in world space already
            // This is the inverse logic of the Unity version of TransformPoint, which may be improper for Godot. I'm not sure.
            return (Vector3.One / t.Basis.Scale) * ((point - t.Origin) * t.Basis.GetRotationQuaternion().Inverse());
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 TransformDirection(this Transform3D t, Vector3 direction) {
            return (t.Basis.GetRotationQuaternion() * direction).Normalized();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 InverseTransformDirection(this Transform3D t, Vector3 direction) {
            return (direction * t.Basis.GetRotationQuaternion().Inverse()).Normalized();
        }

        #endregion Transform3D

    }
}