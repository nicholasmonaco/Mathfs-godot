using System;
using System.Runtime.CompilerServices;

using Godot;

namespace Freya {
    public static class CoreUtil {
        // Time
        public static float DeltaTime => 4; // todo;

        // LerpUnclamped
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float LerpUnclamped(float a, float b, float t) {
            return a + (b - a) * t;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 LerpUnclamped(Vector2 a, Vector2 b, float t) {
            return new Vector2(
                a.X + (b.X - a.X) * t,
                a.Y + (b.Y - a.Y) * t
            );
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 LerpUnclamped(Vector3 a, Vector3 b, float t) {
            return new Vector3(
                a.X + (b.X - a.X) * t,
                a.Y + (b.Y - a.Y) * t,
                a.Z + (b.Z - a.Z) * t
            );
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector4 LerpUnclamped(Vector4 a, Vector4 b, float t) {
            return new Vector4(
                a.X + (b.X - a.X) * t,
                a.Y + (b.Y - a.Y) * t,
                a.Z + (b.Z - a.Z) * t,
                a.W + (b.W - a.W) * t
            );
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Quaternion LerpUnclamped(Quaternion a, Quaternion b, float t) {
            return new Quaternion(
                a.X + (b.X - a.X) * t,
                a.Y + (b.Y - a.Y) * t,
                a.Z + (b.Z - a.Z) * t,
                a.W + (b.W - a.W) * t
            );
        }


        // LookRotation
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Quaternion LookRotation(Vector3 forward) {
            return LookRotation(forward, Vector3.Up);
        }

        // From: http://answers.unity3d.com/questions/467614/what-is-the-source-code-of-quaternionlookrotation.html
        public static Quaternion LookRotation(Vector3 forward, Vector3 up) {
            forward = forward.Normalized();
            Vector3 right = up.Cross(forward).Normalized();
            up = forward.Cross(right);
            
            float m00 = right.X;
            float m01 = right.Y;
            float m02 = right.Z;
            float m10 = up.X;
            float m11 = up.Y;
            float m12 = up.Z;
            float m20 = forward.X;
            float m21 = forward.Y;
            float m22 = forward.Z;

            float num8 = (m00 + m11) + m22;
            Quaternion quaternion = new Quaternion();

            if (num8 > 0f) {
                float num = (float)Math.Sqrt(num8 + 1f);
                quaternion.W = num * 0.5f;
                num = 0.5f / num;
                quaternion.X = (m12 - m21) * num;
                quaternion.Y = (m20 - m02) * num;
                quaternion.Z = (m01 - m10) * num;
                return quaternion;
            }

            if ((m00 >= m11) && (m00 >= m22)) {
                float num7 = (float)Math.Sqrt(((1f + m00) - m11) - m22);
                float num4 = 0.5f / num7;
                quaternion.X = 0.5f * num7;
                quaternion.Y = (m01 + m10) * num4;
                quaternion.Z = (m02 + m20) * num4;
                quaternion.W = (m12 - m21) * num4;
                return quaternion;
            }

            if (m11 > m22) {
                float num6 = (float)Math.Sqrt(((1f + m11) - m00) - m22);
                float num3 = 0.5f / num6;
                quaternion.X = (m10 + m01) * num3;
                quaternion.Y = 0.5f * num6;
                quaternion.Z = (m21 + m12) * num3;
                quaternion.W = (m20 - m02) * num3;
                return quaternion;
            }

            float num5 = (float)Math.Sqrt(((1f + m22) - m00) - m11);
            float num2 = 0.5f / num5;
            quaternion.X = (m20 + m02) * num2;
            quaternion.Y = (m21 + m12) * num2;
            quaternion.Z = 0.5f * num5;
            quaternion.W = (m01 - m10) * num2;
            return quaternion;
        }


        // Orthonormalization
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 Orthonormalize(Vector3 tangent, Vector3 normal) {
            return (tangent - tangent.Dot(normal) * normal).Normalized();
        }

        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 OrthoNormalVector(Vector3 v) {
            Vector3 tangent = new Vector3(
                -v.X * v.Z, 
                -v.Y * v.Z, 
                v.X * v.X + v.Y * v.Y
            );
            return (tangent - tangent.Dot(v) * v).Normalized();
        }

        // SlerpUnclamped
        public static Vector3 SlerpUnclamped(Vector3 lhs, Vector3 rhs, float t) {
            float lhsMag = lhs.Length();
            float rhsMag = rhs.Length();

            if (lhsMag < Mathfs.Epsilon || rhsMag < Mathfs.Epsilon) {
                return LerpUnclamped(lhs, rhs, t);
            }

            float lerpedMagnitude = LerpUnclamped(lhsMag, rhsMag, t);

            float dot = lhs.Dot(rhs) / (lhsMag * rhsMag);
            // Direction is almost the same
            if (dot > 1.0F - Mathfs.Epsilon) {
                return LerpUnclamped(lhs, rhs, t);
            }
            // Directions are almost opposite
            else if (dot < -1.0F + Mathfs.Epsilon) {
                Vector3 lhsNorm = lhs / lhsMag;
                Vector3 axis = OrthoNormalVector(lhsNorm);
                Matrix3x3 m = Matrix3x3.FromAxisAngle(axis, Mathfs.PI * t);
                Vector3 slerped = m * lhsNorm;
                slerped *= lerpedMagnitude;
                return slerped;
            }
            // normal case
            else {
                Vector3 axis = lhs.Cross(rhs);
                Vector3 lhsNorm = lhs / lhsMag;
                axis = axis.Normalized();
                float angle = MathF.Acos(dot) * t;

                Matrix3x3 m = Matrix3x3.FromAxisAngle(axis, angle);
                Vector3 slerped = m * lhsNorm;
                slerped *= lerpedMagnitude;
                return slerped;
            }
        }


        public static Quaternion SlerpUnclamped(Quaternion q1, Quaternion q2, float t) {
            float dot = q1.Dot(q2);

            // dot = cos(theta)
            // If (dot < 0), q1 and q2 are more than 90 degrees apart,
            // so we can invert one to reduce spinning
            Quaternion tmpQuat;
            if (dot < 0.0f) {
                dot = -dot;
                tmpQuat = new Quaternion(
                    -q2.X,
                    -q2.Y,
                    -q2.Z,
                    -q2.W
                );
            }
            else tmpQuat = q2;

            if (dot < 0.95f) {
                float angle = MathF.Acos(dot);
                float sinadiv, sinat, sinaomt;
                sinadiv = 1.0f / MathF.Sin(angle);
                sinat = MathF.Sin(angle * t);
                sinaomt = MathF.Sin(angle * (1.0f - t));
                
                tmpQuat.X = (q1.X * sinaomt + tmpQuat.X * sinat) * sinadiv;
                tmpQuat.Y = (q1.Y * sinaomt + tmpQuat.Y * sinat) * sinadiv;
                tmpQuat.Z = (q1.Z * sinaomt + tmpQuat.Z * sinat) * sinadiv;
                tmpQuat.W = (q1.W * sinaomt + tmpQuat.W * sinat) * sinadiv;
                return tmpQuat;
            }
            // If the angle is small, use linear interpolation
            else {
                return LerpUnclamped(q1, tmpQuat, t);
            }
        }


        // Vector Projection
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 ProjectOnPlane(Vector3 vector, Vector3 planeNormal) {
            var sqrMag = planeNormal.Dot(planeNormal);
            if(sqrMag < Mathfs.Epsilon)
                return vector;
            else {
                var dot = vector.Dot(planeNormal);
                return new Vector3(
                    vector.X - planeNormal.X * dot / sqrMag,
                    vector.Y - planeNormal.Y * dot / sqrMag,
                    vector.Z - planeNormal.Z * dot / sqrMag);
            }
        }

    }
}