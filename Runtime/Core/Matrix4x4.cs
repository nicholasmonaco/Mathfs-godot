// Matrix4x4.cs 
// Originally from Sylves math reference
// Adapted for Godot by Nikeshayde

using Godot;
using System;

namespace Freya {
        public struct Matrix4x4 : IEquatable<Matrix4x4> {
        public float m00 { get { return column0.X; } set { column0.X = value; } }
        public float m10 { get { return column0.Y; } set { column0.Y = value; } }
        public float m20 { get { return column0.Z; } set { column0.Z = value; } }
        public float m30 { get { return column0.W; } set { column0.W = value; } }
        public float m01 { get { return column1.X; } set { column1.X = value; } }
        public float m11 { get { return column1.Y; } set { column1.Y = value; } }
        public float m21 { get { return column1.Z; } set { column1.Z = value; } }
        public float m31 { get { return column1.W; } set { column1.W = value; } }
        public float m02 { get { return column2.X; } set { column2.X = value; } }
        public float m12 { get { return column2.Y; } set { column2.Y = value; } }
        public float m22 { get { return column2.Z; } set { column2.Z = value; } }
        public float m32 { get { return column2.W; } set { column2.W = value; } }
        public float m03 { get { return column3.X; } set { column3.X = value; } }
        public float m13 { get { return column3.Y; } set { column3.Y = value; } }
        public float m23 { get { return column3.Z; } set { column3.Z = value; } }
        public float m33 { get { return column3.W; } set { column3.W = value; } }

        public Vector4 column0;
        public Vector4 column1;
        public Vector4 column2;
        public Vector4 column3;

        public Matrix4x4(Vector4 column0, Vector4 column1, Vector4 column2, Vector4 column3) {
            this.column0 = column0;
            this.column1 = column1;
            this.column2 = column2;
            this.column3 = column3;
        }

        public static implicit operator Basis(Matrix4x4 m) => new Basis(new Vector3(m.m00, m.m10, m.m20), new Vector3(m.m01, m.m11, m.m21), new Vector3(m.m02, m.m12, m.m22));
        public static implicit operator Matrix4x4(Basis t) => new Matrix4x4(t.Column0.ToVector4(), t.Column1.ToVector4(), t.Column2.ToVector4(), new Vector4(0, 0, 0, 1));

        public static implicit operator Transform3D(Matrix4x4 m) => new Transform3D((Basis)m, new Vector3(m.m03, m.m13, m.m23));
        public static implicit operator Matrix4x4(Transform3D t) {
            var m = (Matrix4x4)t.Basis;
            m.column3 = new Vector4(t.Origin.X, t.Origin.Y, t.Origin.Z, 1);
            return m;
        }

        public static implicit operator Godot.Transform2D(Matrix4x4 m) => new Godot.Transform2D(new Vector2(m.m00, m.m10), new Vector2(m.m01, m.m11), new Vector2(m.m03, m.m13));
        public static implicit operator Matrix4x4(Godot.Transform2D t) => new Matrix4x4(new Vector4(t.X.X, t.X.Y, 0, 0), new Vector4(t.Y.X, t.Y.Y, 0, 0), new Vector4(0, 0, 1, 0), new Vector4(0, 0, 0, 1));

        //public float this[int index] { get; set; }
        //public float this[int row, int column] { get; set; }

        public static Matrix4x4 zero => new Matrix4x4(Vector4.Zero, Vector4.Zero, Vector4.Zero, Vector4.Zero);
        public static Matrix4x4 identity => new Matrix4x4(new Vector4(1, 0, 0, 0), new Vector4(0, 1, 0, 0), new Vector4(0, 0, 1, 0), new Vector4(0, 0, 0, 1));
        public Matrix4x4 transpose => new Matrix4x4(GetRow(0), GetRow(1), GetRow(2), GetRow(3));
        public Quaternion rotation {
            get {
                // Orthogonalize
                Vector3 mx = MultiplyVector(Vector3.Right).Normalized();
                Vector3 my = CoreUtil.ProjectOnPlane(MultiplyVector(Vector3.Up), mx).Normalized();
                Vector3 mz = CoreUtil.ProjectOnPlane(CoreUtil.ProjectOnPlane(MultiplyVector(Vector3.Back), mx), my).Normalized();
                var isReflection = mx.Dot(my.Cross(mz)) < 0;
                if(isReflection) mx *= -1;

                // https://www.euclideanspace.com/maths/geometry/rotations/conversions/matrixToQuaternion/
                // I believe this is known as shepherds method.
                float tr = mx.X + my.Y + mz.Z;
                if(tr > 0) {
                    float S = MathF.Sqrt(tr + 1.0f) * 2; // S=4*qw 
                    var qw = 0.25f * S;
                    var qx = (my.Z - mz.Y) / S;
                    var qy = (mz.X - mx.Z) / S;
                    var qz = (mx.Y - my.X) / S;
                    return new Quaternion(qx, qy, qz, qw);
                } else if((mx.X > my.Y) & (mx.X > mz.Z)) {
                    float S = MathF.Sqrt(1.0f + mx.X - my.Y - mz.Z) * 2; // S=4*qx 
                    var qw = (my.Z - mz.Y) / S;
                    var qx = 0.25f * S;
                    var qy = (my.X + mx.Y) / S;
                    var qz = (mz.X + mx.Z) / S;
                    return new Quaternion(qx, qy, qz, qw);
                } else if(my.Y > mz.Z) {
                    float S = MathF.Sqrt(1.0f + my.Y - mx.X - mz.Z) * 2; // S=4*qy
                    var qw = (mz.X - mx.Z) / S;
                    var qx = (my.X + mx.Y) / S;
                    var qy = 0.25f * S;
                    var qz = (mz.Y + my.Z) / S;
                    return new Quaternion(qx, qy, qz, qw);
                } else {
                    float S = MathF.Sqrt(1.0f + mz.Z - mx.X - my.Y) * 2; // S=4*qz
                    var qw = (mx.Y - my.X) / S;
                    var qx = (mz.X + mx.Z) / S;
                    var qy = (mz.Y + my.Z) / S;
                    var qz = 0.25f * S;
                    return new Quaternion(qx, qy, qz, qw);
                }
            }
        }
        public Vector3 lossyScale {
            get {
                // Orthogonalize so this matches rotation, above
                Vector3 mx = MultiplyVector(Vector3.Right);
                Vector3 my = CoreUtil.ProjectOnPlane(MultiplyVector(Vector3.Up), mx);
                Vector3 mz = CoreUtil.ProjectOnPlane(CoreUtil.ProjectOnPlane(MultiplyVector(Vector3.Back), mx), my);
                var isReflection = mx.Dot(my.Cross(mz)) < 0;
                if(isReflection) {
                    return new Vector3(-mx.Length(), my.Length(), mz.Length());
                } else {
                    return new Vector3(mx.Length(), my.Length(), mz.Length());
                }
            }
        }
        public bool isIdentity => this == identity;
        public float determinant {
            get {
                // Copied from https://stackoverflow.com/a/44446912/14738198
                var A2323 = m22 * m33 - m23 * m32;
                var A1323 = m21 * m33 - m23 * m31;
                var A1223 = m21 * m32 - m22 * m31;
                var A0323 = m20 * m33 - m23 * m30;
                var A0223 = m20 * m32 - m22 * m30;
                var A0123 = m20 * m31 - m21 * m30;

                var det = m00 * (m11 * A2323 - m12 * A1323 + m13 * A1223)
                    - m01 * (m10 * A2323 - m12 * A0323 + m13 * A0223)
                    + m02 * (m10 * A1323 - m11 * A0323 + m13 * A0123)
                    - m03 * (m10 * A1223 - m11 * A0223 + m12 * A0123);

                return det;
            }
        }
        //public FrustumPlanes decomposeProjection { get; }
        public Matrix4x4 inverse {
            get {
                // Copied from https://stackoverflow.com/a/44446912/14738198
                var A2323 = m22 * m33 - m23 * m32;
                var A1323 = m21 * m33 - m23 * m31;
                var A1223 = m21 * m32 - m22 * m31;
                var A0323 = m20 * m33 - m23 * m30;
                var A0223 = m20 * m32 - m22 * m30;
                var A0123 = m20 * m31 - m21 * m30;
                var A2313 = m12 * m33 - m13 * m32;
                var A1313 = m11 * m33 - m13 * m31;
                var A1213 = m11 * m32 - m12 * m31;
                var A2312 = m12 * m23 - m13 * m22;
                var A1312 = m11 * m23 - m13 * m21;
                var A1212 = m11 * m22 - m12 * m21;
                var A0313 = m10 * m33 - m13 * m30;
                var A0213 = m10 * m32 - m12 * m30;
                var A0312 = m10 * m23 - m13 * m20;
                var A0212 = m10 * m22 - m12 * m20;
                var A0113 = m10 * m31 - m11 * m30;
                var A0112 = m10 * m21 - m11 * m20;

                var det = m00 * (m11 * A2323 - m12 * A1323 + m13 * A1223)
                    - m01 * (m10 * A2323 - m12 * A0323 + m13 * A0223)
                    + m02 * (m10 * A1323 - m11 * A0323 + m13 * A0123)
                    - m03 * (m10 * A1223 - m11 * A0223 + m12 * A0123);
                det = 1 / det;

                return new Matrix4x4() {
                    m00 = det * (m11 * A2323 - m12 * A1323 + m13 * A1223),
                    m01 = det * -(m01 * A2323 - m02 * A1323 + m03 * A1223),
                    m02 = det * (m01 * A2313 - m02 * A1313 + m03 * A1213),
                    m03 = det * -(m01 * A2312 - m02 * A1312 + m03 * A1212),
                    m10 = det * -(m10 * A2323 - m12 * A0323 + m13 * A0223),
                    m11 = det * (m00 * A2323 - m02 * A0323 + m03 * A0223),
                    m12 = det * -(m00 * A2313 - m02 * A0313 + m03 * A0213),
                    m13 = det * (m00 * A2312 - m02 * A0312 + m03 * A0212),
                    m20 = det * (m10 * A1323 - m11 * A0323 + m13 * A0123),
                    m21 = det * -(m00 * A1323 - m01 * A0323 + m03 * A0123),
                    m22 = det * (m00 * A1313 - m01 * A0313 + m03 * A0113),
                    m23 = det * -(m00 * A1312 - m01 * A0312 + m03 * A0112),
                    m30 = det * -(m10 * A1223 - m11 * A0223 + m12 * A0123),
                    m31 = det * (m00 * A1223 - m01 * A0223 + m02 * A0123),
                    m32 = det * -(m00 * A1213 - m01 * A0213 + m02 * A0113),
                    m33 = det * (m00 * A1212 - m01 * A0212 + m02 * A0112),
                };
            }
        }

        public static float Determinant(Matrix4x4 m) => m.determinant;
        //public static Matrix4x4 Frustum(float left, float right, float bottom, float top, float zNear, float zFar);
        //public static Matrix4x4 Frustum(FrustumPlanes fp);
        public static Matrix4x4 Inverse(Matrix4x4 m) => m.inverse;
        //public static bool Inverse3DAffine(Matrix4x4 input, ref Matrix4x4 result);
        //public static Matrix4x4 LookAt(Vector3 from, Vector3 to, Vector3 up);
        //public static Matrix4x4 Ortho(float left, float right, float bottom, float top, float zNear, float zFar);
        //public static Matrix4x4 Perspective(float fov, float aspect, float zNear, float zFar);
        public static Matrix4x4 Rotate(Quaternion q) {
            var qx = q.X;
            var qy = q.Y;
            var qz = q.Z;
            var qw = q.W;
            return new Matrix4x4() {
                m00 = 1 - 2 * qy * qy - 2 * qz * qz,
                m01 = 2 * qx * qy - 2 * qz * qw,
                m02 = 2 * qx * qz + 2 * qy * qw,
                m10 = 2 * qx * qy + 2 * qz * qw,
                m11 = 1 - 2 * qx * qx - 2 * qz * qz,
                m12 = 2 * qy * qz - 2 * qx * qw,
                m20 = 2 * qx * qz - 2 * qy * qw,
                m21 = 2 * qy * qz + 2 * qx * qw,
                m22 = 1 - 2 * qx * qx - 2 * qy * qy,
                m33 = 1,
            };
        }
        public static Matrix4x4 Scale(Vector3 vector) => new Matrix4x4(new Vector4(vector.X, 0, 0, 0), new Vector4(0, vector.Y, 0, 0), new Vector4(0, 0, vector.Z, 0), new Vector4(0, 0, 0, 1));
        public static Matrix4x4 Translate(Vector3 vector) => new Matrix4x4(new Vector4(1, 0, 0, 0), new Vector4(0, 1, 0, 0), new Vector4(0, 0, 1, 0), new Vector4(vector.X, vector.Y, vector.Z, 1));
        public static Matrix4x4 Transpose(Matrix4x4 m) => m.transpose;
        public static Matrix4x4 TRS(Vector3 pos, Quaternion q, Vector3 s) => Translate(pos) * Rotate(q) * Scale(s);
        public override bool Equals(object other) {
            if(other is Matrix4x4 m) {
                return Equals(m);
            }
            return false;
        }
        public bool Equals(Matrix4x4 other) => column0 == other.column0 && column1 == other.column1 && column2 == other.column2 && column3 == other.column3;

        public Vector4 GetColumn(int index) {
            switch(index) {
                case 0: return column0;
                case 1: return column1;
                case 2: return column2;
                case 3: return column3;
                default: throw new IndexOutOfRangeException();
            }
        }

        public override int GetHashCode() => (column0, column1, column2, column3).GetHashCode();
        public Vector4 GetRow(int index) {
            switch(index) {
                case 0: return new Vector4(m00, m01, m02, m03);
                case 1: return new Vector4(m10, m11, m12, m13);
                case 2: return new Vector4(m20, m21, m22, m23);
                case 3: return new Vector4(m30, m31, m32, m33);
                default: throw new IndexOutOfRangeException();
            }
        }
        public Vector3 MultiplyPoint(Vector3 point) => MultiplyPoint3x4(point) /
            (point.X * m30 + point.Y * m31 + point.Z * m32 + m33);
        public Vector3 MultiplyPoint3x4(Vector3 point) => new Vector3(
            point.X * m00 + point.Y * m01 + point.Z * m02 + m03,
            point.X * m10 + point.Y * m11 + point.Z * m12 + m13,
            point.X * m20 + point.Y * m21 + point.Z * m22 + m23
            );
        public Vector3 MultiplyVector(Vector3 vector) => new Vector3(
            vector.X * m00 + vector.Y * m01 + vector.Z * m02,
            vector.X * m10 + vector.Y * m11 + vector.Z * m12,
            vector.X * m20 + vector.Y * m21 + vector.Z * m22
            );
        public void SetColumn(int index, Vector4 column) {
            switch(index) {
                case 0: column0 = column; return;
                case 1: column1 = column; return;
                case 2: column2 = column; return;
                case 3: column3 = column; return;
                default: throw new IndexOutOfRangeException();
            }
        }
        public void SetRow(int index, Vector4 row) {
            switch(index) {
                case 0: (m00, m01, m02, m03) = (row.X, row.Y, row.Z, row.W); return;
                case 1: (m10, m11, m12, m13) = (row.X, row.Y, row.Z, row.W); return;
                case 2: (m20, m21, m22, m23) = (row.X, row.Y, row.Z, row.W); return;
                case 3: (m30, m31, m32, m33) = (row.X, row.Y, row.Z, row.W); return;
                default: throw new IndexOutOfRangeException();
            }
        }
        public void SetTRS(Vector3 pos, Quaternion q, Vector3 s) {
            var m = TRS(pos, q, s);
            column0 = m.column0;
            column1 = m.column1;
            column2 = m.column2;
            column3 = m.column3;
        }
        public override string ToString() => $"{m00}\t{m01}\t{m02}\t{m03}\n{m10}\t{m11}\t{m12}\t{m13}\n{m20}\t{m21}\t{m22}\t{m23}\n{m30}\t{m31}\t{m32}\t{m33}";
        public string ToString(string format) => ToString();
        //public Plane TransformPlane(Plane plane);
        //public bool ValidTRS();

        public static Vector4 operator *(Matrix4x4 lhs, Vector4 vector) => new Vector4(
            vector.X * lhs.m00 + vector.Y * lhs.m01 + vector.Z * lhs.m02 + vector.W * lhs.m03,
            vector.X * lhs.m10 + vector.Y * lhs.m11 + vector.Z * lhs.m12 + vector.W * lhs.m13,
            vector.X * lhs.m20 + vector.Y * lhs.m21 + vector.Z * lhs.m22 + vector.W * lhs.m23,
            vector.X * lhs.m30 + vector.Y * lhs.m31 + vector.Z * lhs.m32 + vector.W * lhs.m33
            );
        public static Matrix4x4 operator *(Matrix4x4 lhs, Matrix4x4 rhs) => new Matrix4x4 {
            m00 = lhs.m00 * rhs.m00 + lhs.m01 * rhs.m10 + lhs.m02 * rhs.m20 + lhs.m03 * rhs.m30,
            m10 = lhs.m10 * rhs.m00 + lhs.m11 * rhs.m10 + lhs.m12 * rhs.m20 + lhs.m13 * rhs.m30,
            m20 = lhs.m20 * rhs.m00 + lhs.m21 * rhs.m10 + lhs.m22 * rhs.m20 + lhs.m23 * rhs.m30,
            m30 = lhs.m30 * rhs.m00 + lhs.m31 * rhs.m10 + lhs.m32 * rhs.m20 + lhs.m33 * rhs.m30,
            m01 = lhs.m00 * rhs.m01 + lhs.m01 * rhs.m11 + lhs.m02 * rhs.m21 + lhs.m03 * rhs.m31,
            m11 = lhs.m10 * rhs.m01 + lhs.m11 * rhs.m11 + lhs.m12 * rhs.m21 + lhs.m13 * rhs.m31,
            m21 = lhs.m20 * rhs.m01 + lhs.m21 * rhs.m11 + lhs.m22 * rhs.m21 + lhs.m23 * rhs.m31,
            m31 = lhs.m30 * rhs.m01 + lhs.m31 * rhs.m11 + lhs.m32 * rhs.m21 + lhs.m33 * rhs.m31,
            m02 = lhs.m00 * rhs.m02 + lhs.m01 * rhs.m12 + lhs.m02 * rhs.m22 + lhs.m03 * rhs.m32,
            m12 = lhs.m10 * rhs.m02 + lhs.m11 * rhs.m12 + lhs.m12 * rhs.m22 + lhs.m13 * rhs.m32,
            m22 = lhs.m20 * rhs.m02 + lhs.m21 * rhs.m12 + lhs.m22 * rhs.m22 + lhs.m23 * rhs.m32,
            m32 = lhs.m30 * rhs.m02 + lhs.m31 * rhs.m12 + lhs.m32 * rhs.m22 + lhs.m33 * rhs.m32,
            m03 = lhs.m00 * rhs.m03 + lhs.m01 * rhs.m13 + lhs.m02 * rhs.m23 + lhs.m03 * rhs.m33,
            m13 = lhs.m10 * rhs.m03 + lhs.m11 * rhs.m13 + lhs.m12 * rhs.m23 + lhs.m13 * rhs.m33,
            m23 = lhs.m20 * rhs.m03 + lhs.m21 * rhs.m13 + lhs.m22 * rhs.m23 + lhs.m23 * rhs.m33,
            m33 = lhs.m30 * rhs.m03 + lhs.m31 * rhs.m13 + lhs.m32 * rhs.m23 + lhs.m33 * rhs.m33,
        };
        public static bool operator ==(Matrix4x4 lhs, Matrix4x4 rhs) => lhs.Equals(rhs);
        public static bool operator !=(Matrix4x4 lhs, Matrix4x4 rhs) => !(lhs == rhs);
    }
}