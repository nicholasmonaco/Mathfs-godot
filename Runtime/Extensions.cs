// by Freya Holmér (https://github.com/FreyaHolmer/Mathfs)

using System;
using System.Runtime.CompilerServices;

using Vector2 = Godot.Vector2;
using Vector3 = Godot.Vector3;
using Vector4 = Godot.Vector4;
using Vector2Int = Godot.Vector2I;
using Vector3Int = Godot.Vector3I;
using Quaternion = Godot.Quaternion;

using Transform = Godot.Transform3D;
using Color = Godot.Color;
using Rect = Godot.Rect2;
using Bounds = Godot.Aabb;

using Ndot;

namespace Freya {

	/// <summary>Various extensions for floats, vectors and colors</summary>
	public static class MathfsExtensions {

		const MethodImplOptions INLINE = MethodImplOptions.AggressiveInlining;

		#region Vector rotation and angles

		/// <summary>Returns the angle of this vector, in radians</summary>
		/// <param name="v">The vector to get the angle of. It does not have to be normalized</param>
		/// <seealso cref="Mathfs.DirToAng"/>
		[MethodImpl( INLINE )] public static float Angle( this Vector2 v ) => MathF.Atan2( v.Y, v.X );

		/// <summary>Rotates the vector 90 degrees clockwise (negative Z axis rotation)</summary>
		[MethodImpl( INLINE )] public static Vector2 Rotate90CW( this Vector2 v ) => new Vector2( v.Y, -v.X );

		/// <summary>Rotates the vector 90 degrees counter-clockwise (positive Z axis rotation)</summary>
		[MethodImpl( INLINE )] public static Vector2 Rotate90CCW( this Vector2 v ) => new Vector2( -v.Y, v.X );

		/// <summary>Rotates the vector around <c>pivot</c> with the given angle (in radians)</summary>
		/// <param name="v">The vector to rotate</param>
		/// <param name="pivot">The point to rotate around</param>
		/// <param name="angRad">The angle to rotate by, in radians</param>
		[MethodImpl( INLINE )] public static Vector2 RotateAround( this Vector2 v, Vector2 pivot, float angRad ) => Rotate( v - pivot, angRad ) + pivot;

		/// <summary>Rotates the vector around <c>(0,0)</c> with the given angle (in radians)</summary>
		/// <param name="v">The vector to rotate</param>
		/// <param name="angRad">The angle to rotate by, in radians</param>
		public static Vector2 Rotate( this Vector2 v, float angRad ) {
			float ca = MathF.Cos( angRad );
			float sa = MathF.Sin( angRad );
			return new Vector2( ca * v.X - sa * v.Y, sa * v.X + ca * v.Y );
		}

		/// <summary>Converts an angle in degrees to radians</summary>
		/// <param name="angDegrees">The angle, in degrees, to convert to radians</param>
		[MethodImpl( INLINE )] public static float DegToRad( this float angDegrees ) => angDegrees * Mathfs.Deg2Rad;

		/// <summary>Converts an angle in radians to degrees</summary>
		/// <param name="angRadians">The angle, in radians, to convert to degrees</param>
		[MethodImpl( INLINE )] public static float RadToDeg( this float angRadians ) => angRadians * Mathfs.Rad2Deg;

		/// <summary>Extracts the quaternion components into a Vector4</summary>
		/// <param name="q">The quaternion to get the components of</param>
		[MethodImpl( INLINE )] public static Vector4 ToVector4( this Quaternion q ) => new Vector4( q.X, q.Y, q.Z, q.W );

		/// <summary>Converts to a rotation vector (axis-angle where the angle is embedded in the magnitude, in radians)</summary>
		/// <param name="q">The quaternion to get the rotation vector of</param>
		[MethodImpl( INLINE )] public static Vector3 ToRotationVector( this Quaternion q ) {
			q.ToAngleAxis( out float angDeg, out Vector3 axis );
			return axis * ( angDeg * Mathfs.Deg2Rad );
		}

		#endregion

		#region Swizzling

		/// <summary>Returns X and Y as a Vector2, equivalent to <c>new Vector2(v.X,v.Y)</c></summary>
		[MethodImpl( INLINE )] public static Vector2 XY( this Vector2 v ) => new Vector2( v.X, v.Y );

		/// <summary>Returns Y and X as a Vector2, equivalent to <c>new Vector2(v.Y,v.X)</c></summary>
		[MethodImpl( INLINE )] public static Vector2 YX( this Vector2 v ) => new Vector2( v.Y, v.X );

		/// <summary>Returns X and Z as a Vector2, equivalent to <c>new Vector2(v.X,v.Z)</c></summary>
		[MethodImpl( INLINE )] public static Vector2 XZ( this Vector3 v ) => new Vector2( v.X, v.Z );

		/// <summary>Returns this vector as a Vector3, slotting X into X, and Y into Z, and the input value y into Y.
		/// Equivalent to <c>new Vector3(v.X,y,v.Y)</c></summary>
		[MethodImpl( INLINE )] public static Vector3 XZtoXYZ( this Vector2 v, float y = 0 ) => new Vector3( v.X, y, v.Y );

		/// <summary>Returns this vector as a Vector3, slotting X into X, and Y into Y, and the input value z into Z.
		/// Equivalent to <c>new Vector3(v.X,v.Y,z)</c></summary>
		[MethodImpl( INLINE )] public static Vector3 XYtoXYZ( this Vector2 v, float z = 0 ) => new Vector3( v.X, v.Y, z );

		/// <summary>Sets X to 0</summary>
		[MethodImpl( INLINE )] public static Vector2 FlattenX( this Vector2 v ) => new Vector2( 0f, v.Y );

		/// <summary>Sets Y to 0</summary>
		[MethodImpl( INLINE )] public static Vector2 FlattenY( this Vector2 v ) => new Vector2( v.X, 0f );

		/// <summary>Sets X to 0</summary>
		[MethodImpl( INLINE )] public static Vector3 FlattenX( this Vector3 v ) => new Vector3( 0f, v.Y, v.Z );

		/// <summary>Sets Y to 0</summary>
		[MethodImpl( INLINE )] public static Vector3 FlattenY( this Vector3 v ) => new Vector3( v.X, 0f, v.Z );

		/// <summary>Sets Z to 0</summary>
		[MethodImpl( INLINE )] public static Vector3 FlattenZ( this Vector3 v ) => new Vector3( v.X, v.Y, 0f );

		#endregion

		#region Vector directions & magnitudes

		/// <summary>Returns the chebyshev magnitude of this vector</summary>
		[MethodImpl( INLINE )] public static float ChebyshevMagnitude( this Vector3 v ) => Mathfs.Max( Abs( v.X ), Abs( v.Y ), Abs( v.Z ) );

		/// <summary>Returns the taxicab/rectilinear magnitude of this vector</summary>
		[MethodImpl( INLINE )] public static float TaxicabMagnitude( this Vector3 v ) => Abs( v.X ) + Abs( v.Y ) + Abs( v.Z );

		/// <inheritdoc cref="ChebyshevMagnitude(Vector3)"/>
		[MethodImpl( INLINE )] public static float ChebyshevMagnitude( this Vector2 v ) => Mathfs.Max( Abs( v.X ), Abs( v.Y ) );

		/// <inheritdoc cref="TaxicabMagnitude(Vector3)"/>
		[MethodImpl( INLINE )] public static float TaxicabMagnitude( this Vector2 v ) => Abs( v.X ) + Abs( v.Y );

		/// <summary>Returns a vector with the same direction, but with the given magnitude.
		/// Equivalent to <c>v.normalized*mag</c></summary>
		[MethodImpl( INLINE )] public static Vector2 WithMagnitude( this Vector2 v, float mag ) => v.Normalized() * mag;

		/// <inheritdoc cref="WithMagnitude(Vector2,float)"/>
		[MethodImpl( INLINE )] public static Vector3 WithMagnitude( this Vector3 v, float mag ) => v.Normalized() * mag;

		/// <summary>Returns a vector with the same direction, but extending the magnitude by the given amount</summary>
		[MethodImpl( INLINE )] public static Vector2 AddMagnitude( this Vector2 v, float extraMagnitude ) => v * ( 1 + extraMagnitude / v.Length() );

		/// <inheritdoc cref="AddMagnitude(Vector2,float)"/>
		[MethodImpl( INLINE )] public static Vector3 AddMagnitude( this Vector3 v, float extraMagnitude ) => v * ( 1 + extraMagnitude / v.Length() );

		/// <summary>Returns the vector going from one position to another, also known as the displacement.
		/// Equivalent to <c>target-v</c></summary>
		[MethodImpl( INLINE )] public static Vector2 To( this Vector2 v, Vector2 target ) => target - v;

		/// <summary>Returns the vector going from one position to another, also known as the displacement.
		/// Equivalent to <c>target-v</c></summary>
		[MethodImpl( INLINE )] public static Vector3 To( this Vector3 v, Vector3 target ) => target - v;

		/// <summary>Returns the normalized direction from this vector to the target.
		/// Equivalent to <c>(target-v).normalized</c> or <c>v.To(target).normalized</c></summary>
		[MethodImpl( INLINE )] public static Vector2 DirTo( this Vector2 v, Vector2 target ) => ( target - v ).Normalized();

		/// <summary>Returns the normalized direction from this vector to the target.
		/// Equivalent to <c>(target-v).normalized</c> or <c>v.To(target).normalized</c></summary>
		[MethodImpl( INLINE )] public static Vector3 DirTo( this Vector3 v, Vector3 target ) => ( target - v ).Normalized();

		/// <summary>Mirrors this vector around another point. Equivalent to rotating the vector 180° around the point</summary>
		/// <param name="p">The point to mirror</param>
		/// <param name="pivot">The point to mirror around</param>
		[MethodImpl( INLINE )] public static Vector2 MirrorAround( this Vector2 p, Vector2 pivot ) => new(2 * pivot.X - p.X, 2 * pivot.Y - p.Y);

		/// <summary>Mirrors this vector around an x coordinate</summary>
		/// <param name="p">The point to mirror</param>
		/// <param name="xPivot">The x coordinate to mirror around</param>
		[MethodImpl( INLINE )] public static Vector2 MirrorAroundX( this Vector2 p, float xPivot ) => new(2 * xPivot - p.X, p.Y);

		/// <summary>Mirrors this vector around a y coordinate</summary>
		/// <param name="p">The point to mirror</param>
		/// <param name="yPivot">The y coordinate to mirror around</param>
		[MethodImpl( INLINE )] public static Vector2 MirrorAroundY( this Vector2 p, float yPivot ) => new(p.X, 2 * yPivot - p.Y);

		/// <inheritdoc cref="MirrorAroundX(Vector2,float)"/>
		[MethodImpl( INLINE )] public static Vector3 MirrorAroundX( this Vector3 p, float xPivot ) => new(2 * xPivot - p.X, p.Y, p.Z);

		/// <inheritdoc cref="MirrorAroundY(Vector2,float)"/>
		[MethodImpl( INLINE )] public static Vector3 MirrorAroundY( this Vector3 p, float yPivot ) => new(p.X, 2 * yPivot - p.Y, p.Z);

		/// <summary>Mirrors this vector around a y coordinate</summary>
		/// <param name="p">The point to mirror</param>
		/// <param name="zPivot">The z coordinate to mirror around</param>
		[MethodImpl( INLINE )] public static Vector3 MirrorAroundZ( this Vector3 p, float zPivot ) => new(p.X, p.Y, 2 * zPivot - p.Z);

		/// <inheritdoc cref="MirrorAround(Vector2,Vector2)"/>
		[MethodImpl( INLINE )] public static Vector3 MirrorAround( this Vector3 p, Vector3 pivot ) => new(2 * pivot.X - p.X, 2 * pivot.Y - p.Y, 2 * pivot.Z - p.Z);

		/// <summary>Scale the point <c>p</c> around <c>pivot</c> by <c>scale</c></summary>
		/// <param name="p">The point to scale</param>
		/// <param name="pivot">The pivot to scale around</param>
		/// <param name="scale">The scale to scale by</param>
		[MethodImpl( INLINE )] public static Vector2 ScaleAround( this Vector2 p, Vector2 pivot, Vector2 scale ) => new(pivot.X + ( p.X - pivot.X ) * scale.X, pivot.Y + ( p.Y - pivot.Y ) * scale.Y);

		/// <inheritdoc cref="ScaleAround(Vector2,Vector2,Vector2)"/>
		[MethodImpl( INLINE )] public static Vector3 ScaleAround( this Vector3 p, Vector3 pivot, Vector3 scale ) => new(pivot.X + ( p.X - pivot.X ) * scale.X, pivot.Y + ( p.Y - pivot.Y ) * scale.Y, pivot.Z + ( p.Z - pivot.Z ) * scale.Z);

		/// <summary>Projects the vector perpendicularly onto the other vector B</summary>
		/// <param name="a">The vector to project with</param>
		/// <param name="b">The vector to project perpendicularly against. The resulting vector is along this vector</param>
		public static Vector3 Project( this Vector3 a, Vector3 b ) {
			float denom = b.Dot( b );
			if( Mathfs.Approximately( denom, 0 ) )
				throw new DivideByZeroException( "Can't project to a vector with 0 length" );
			return b * ( a.Dot( b ) / denom );
		}

		/// <summary>Projects the vector perpendicularly *from* the initial vector, onto the other vector B</summary>
		/// <param name="a">The vector to project perpendicularly from</param>
		/// <param name="b">The vector to project against. The resulting vector is along this vector</param>
		public static Vector3 ProjectPerpFrom( this Vector3 a, Vector3 b ) {
			float denom = a.Dot( b );
			if( Mathfs.Approximately( denom, 0 ) )
				throw new DivideByZeroException( "Can't project to a vector with 0 length" );
			return b * ( a.Dot( a ) / denom );
		}

		#endregion

		#region Quaternions

		/// <summary>Rotates 180° around the extrinsic pre-rotation X axis, sometimes this is interpreted as a world space rotation, as opposed to rotating around its own axes</summary>
		[MethodImpl( INLINE )] public static Quaternion Rotate180AroundExtrX( this Quaternion q ) => new(q.W, -q.Z, q.Y, -q.X);

		/// <summary>Rotates 180° around the extrinsic pre-rotation Y axis, sometimes this is interpreted as a world space rotation, as opposed to rotating around its own axes</summary>
		[MethodImpl( INLINE )] public static Quaternion Rotate180AroundExtrY( this Quaternion q ) => new(q.Z, q.W, -q.X, -q.Y);

		/// <summary>Rotates 180° around the extrinsic pre-rotation Z axis, sometimes this is interpreted as a world space rotation, as opposed to rotating around its own axes</summary>
		[MethodImpl( INLINE )] public static Quaternion Rotate180AroundExtrZ( this Quaternion q ) => new(-q.Y, q.X, q.W, -q.Z);

		/// <summary>Rotates 180° around its local X axis</summary>
		[MethodImpl( INLINE )] public static Quaternion Rotate180AroundSelfX( this Quaternion q ) => new(q.W, q.Z, -q.Y, -q.X);

		/// <summary>Rotates 180° around its local Y axis</summary>
		[MethodImpl( INLINE )] public static Quaternion Rotate180AroundSelfY( this Quaternion q ) => new(-q.Z, q.W, q.X, -q.Y);

		/// <summary>Rotates 180° around its local Z axis</summary>
		[MethodImpl( INLINE )] public static Quaternion Rotate180AroundSelfZ( this Quaternion q ) => new(q.Y, -q.X, q.W, -q.Z);

		/// <summary>Rotates this quaternion 180° so that its Y and Z axes are swapped</summary>
		[MethodImpl( INLINE )] public static Quaternion SwapYZ( this Quaternion q ) {
			return new Quaternion(
				Mathfs.RSQRT2 * ( q.Y - q.Z ),
				Mathfs.RSQRT2 * ( q.W - q.X ),
				Mathfs.RSQRT2 * ( q.W + q.X ),
				Mathfs.RSQRT2 * ( -q.Y - q.Z )
			);
		}

		/// <summary>Rotates this quaternion 180° so that its Z and X axes are swapped</summary>
		[MethodImpl( INLINE )] public static Quaternion SwapZX( this Quaternion q ) {
			return new Quaternion(
				Mathfs.RSQRT2 * ( q.W + q.Y ),
				Mathfs.RSQRT2 * ( q.Z - q.X ),
				Mathfs.RSQRT2 * ( q.W - q.Y ),
				Mathfs.RSQRT2 * ( -q.X - q.Z )
			);
		}

		/// <summary>Rotates this quaternion 180° so that its X and Y axes are swapped</summary>
		[MethodImpl( INLINE )] public static Quaternion SwapXY( this Quaternion q ) {
			return new Quaternion(
				Mathfs.RSQRT2 * ( q.W - q.Z ),
				Mathfs.RSQRT2 * ( q.W + q.Z ),
				Mathfs.RSQRT2 * ( q.X - q.Y ),
				Mathfs.RSQRT2 * ( -q.X - q.Y )
			);
		}

		/// <summary>Returns an 180° rotated version of this quaternion around the given axis</summary>
		/// <param name="q">The quaternion to rotate</param>
		/// <param name="axis">The axis to rotate around</param>
		/// <param name="space">The rotation space of the axis, if it should be intrinsic/self/local or extrinsic/"world"</param>
		public static Quaternion Rotate180Around( this Quaternion q, Axis axis, RotationSpace space = RotationSpace.Self ) {
			return axis switch {
				Axis.X => space == RotationSpace.Self ? Rotate180AroundSelfX( q ) : Rotate180AroundExtrX( q ),
				Axis.Y => space == RotationSpace.Self ? Rotate180AroundSelfY( q ) : Rotate180AroundExtrY( q ),
				Axis.Z => space == RotationSpace.Self ? Rotate180AroundSelfZ( q ) : Rotate180AroundExtrZ( q ),
				_      => throw new ArgumentOutOfRangeException( nameof(axis), $"Invalid axis: {axis}. Expected 0, 1 or 2" )
			};
		}

		/// <summary>Returns the quaternion rotated around the given axis by the given angle in radians</summary>
		/// <param name="q">The quaternion to rotate</param>
		/// <param name="axis">The axis to rotate around</param>
		/// <param name="angRad">The angle to rotate by (in radians)</param>
		/// <param name="space">The rotation space of the axis, if it should be intrinsic/self/local or extrinsic/"world"</param>
		public static Quaternion RotateAround( this Quaternion q, Axis axis, float angRad, RotationSpace space = RotationSpace.Self ) {
			float aHalf = angRad / 2;
			float c = MathF.Cos( aHalf );
			float s = MathF.Sin( aHalf );
			float xc = q.X * c;
			float yc = q.Y * c;
			float zc = q.Z * c;
			float wc = q.W * c;
			float xs = q.X * s;
			float ys = q.Y * s;
			float zs = q.Z * s;
			float ws = q.W * s;

			return space switch {
				RotationSpace.Self => axis switch {
					Axis.X => new Quaternion( xc + ws, yc + zs, zc - ys, wc - xs ),
					Axis.Y => new Quaternion( xc - zs, yc + ws, zc + xs, wc - ys ),
					Axis.Z => new Quaternion( xc + ys, yc - xs, zc + ws, wc - zs ),
					_      => throw new ArgumentOutOfRangeException( nameof(axis) )
				},
				RotationSpace.Extrinsic => axis switch {
					Axis.X => new Quaternion( xc + ws, yc - zs, zc + ys, wc - xs ),
					Axis.Y => new Quaternion( xc + zs, yc + ws, zc - xs, wc - ys ),
					Axis.Z => new Quaternion( xc - ys, yc + xs, zc + ws, wc - zs ),
					_      => throw new ArgumentOutOfRangeException( nameof(axis) )
				},
				_ => throw new ArgumentOutOfRangeException( nameof(space) )
			};
		}

		/// <summary>Returns the quaternion rotated around the given axis by 90°</summary>
		/// <param name="q">The quaternion to rotate</param>
		/// <param name="axis">The axis to rotate around</param>
		/// <param name="space">The rotation space of the axis, if it should be intrinsic/self/local or extrinsic/"world"</param>
		public static Quaternion Rotate90Around( this Quaternion q, Axis axis, RotationSpace space = RotationSpace.Self ) {
			const float v = Mathfs.RSQRT2; // cos(90°/2) = sin(90°/2)
			float x = q.X;
			float y = q.Y;
			float z = q.Z;
			float w = q.W;

			return space switch {
				RotationSpace.Self => axis switch {
					Axis.X => new Quaternion( v * ( x + w ), v * ( y + z ), v * ( z - y ), v * ( w - x ) ),
					Axis.Y => new Quaternion( v * ( x - z ), v * ( y + w ), v * ( z + x ), v * ( w - y ) ),
					Axis.Z => new Quaternion( v * ( x + y ), v * ( y - x ), v * ( z + w ), v * ( w - z ) ),
					_      => throw new ArgumentOutOfRangeException( nameof(axis) )
				},
				RotationSpace.Extrinsic => axis switch {
					Axis.X => new Quaternion( v * ( x + w ), v * ( y - z ), v * ( z + y ), v * ( w - x ) ),
					Axis.Y => new Quaternion( v * ( x + z ), v * ( y + w ), v * ( z - x ), v * ( w - y ) ),
					Axis.Z => new Quaternion( v * ( x - y ), v * ( y + x ), v * ( z + w ), v * ( w - z ) ),
					_      => throw new ArgumentOutOfRangeException( nameof(axis) )
				},
				_ => throw new ArgumentOutOfRangeException( nameof(space) )
			};
		}

		/// <summary>Returns the quaternion rotated around the given axis by -90°</summary>
		/// <param name="q">The quaternion to rotate</param>
		/// <param name="axis">The axis to rotate around</param>
		/// <param name="space">The rotation space of the axis, if it should be intrinsic/self/local or extrinsic/"world"</param>
		public static Quaternion RotateNeg90Around( this Quaternion q, Axis axis, RotationSpace space = RotationSpace.Self ) {
			const float v = Mathfs.RSQRT2; // cos(90°/2) = sin(90°/2)
			float x = q.X;
			float y = q.Y;
			float z = q.Z;
			float w = q.W;

			return space switch {
				RotationSpace.Self => axis switch {
					Axis.X => new Quaternion( v * ( x - w ), v * ( y - z ), v * ( z + y ), v * ( w + x ) ),
					Axis.Y => new Quaternion( v * ( x + z ), v * ( y - w ), v * ( z - x ), v * ( w + y ) ),
					Axis.Z => new Quaternion( v * ( x - y ), v * ( y + x ), v * ( z - w ), v * ( w + z ) ),
					_      => throw new ArgumentOutOfRangeException( nameof(axis) )
				},
				RotationSpace.Extrinsic => axis switch {
					Axis.X => new Quaternion( v * ( x - w ), v * ( y + z ), v * ( z - y ), v * ( w + x ) ),
					Axis.Y => new Quaternion( v * ( x - z ), v * ( y - w ), v * ( z + x ), v * ( w + y ) ),
					Axis.Z => new Quaternion( v * ( x + y ), v * ( y - x ), v * ( z - w ), v * ( w + z ) ),
					_      => throw new ArgumentOutOfRangeException( nameof(axis) )
				},
				_ => throw new ArgumentOutOfRangeException( nameof(space) )
			};
		}

		/// <summary>Returns the given axis of this rotation (assumes this quaternion is normalized)</summary>
		public static Vector3 GetAxis( this Quaternion q, Axis axis ) {
			return axis switch {
				Axis.X => q.Right(),
				Axis.Y => q.Up(),
				Axis.Z => q.Forward(),
				_      => throw new ArgumentOutOfRangeException( nameof(axis) )
			};
		}

		/// <summary>Returns the X axis of this rotation (assumes this quaternion is normalized)</summary>
		[MethodImpl( INLINE )] public static Vector3 Right( this Quaternion q ) =>
			new(
				q.X * q.X - q.Y * q.Y - q.Z * q.Z + q.W * q.W,
				2 * ( q.X * q.Y + q.Z * q.W ),
				2 * ( q.X * q.Z - q.Y * q.W )
			);

		/// <summary>Returns the Y axis of this rotation (assumes this quaternion is normalized)</summary>
		[MethodImpl( INLINE )] public static Vector3 Up( this Quaternion q ) =>
			new(
				2 * ( q.X * q.Y - q.Z * q.W ),
				-q.X * q.X + q.Y * q.Y - q.Z * q.Z + q.W * q.W,
				2 * ( q.X * q.W + q.Y * q.Z )
			);

		/// <summary>Returns the Z axis of this rotation (assumes this quaternion is normalized)</summary>
		[MethodImpl( INLINE )] public static Vector3 Forward( this Quaternion q ) =>
			new(
				2 * ( q.X * q.Z + q.Y * q.W ),
				2 * ( q.Y * q.Z - q.X * q.W ),
				-q.X * q.X - q.Y * q.Y + q.Z * q.Z + q.W * q.W
			);

		/// <summary>Converts this quaternion to a rotation matrix</summary>
		public static Matrix4x4 ToMatrix( this Quaternion q ) {
			// you could just use Matrix4x4.Rotate( q ) but that's not as fun as doing this math myself
			float xx = q.X * q.X;
			float yy = q.Y * q.Y;
			float zz = q.Z * q.Z;
			float ww = q.W * q.W;
			float xy = q.X * q.Y;
			float yz = q.Y * q.Z;
			float zw = q.Z * q.W;
			float wx = q.W * q.X;
			float xz = q.X * q.Z;
			float yw = q.Y * q.W;

			return new Matrix4x4 {
				m00 = xx - yy - zz + ww, // X
				m10 = 2 * ( xy + zw ),
				m20 = 2 * ( xz - yw ),
				m01 = 2 * ( xy - zw ), // Y
				m11 = -xx + yy - zz + ww,
				m21 = 2 * ( wx + yz ),
				m02 = 2 * ( xz + yw ), // Z
				m12 = 2 * ( yz - wx ),
				m22 = -xx - yy + zz + ww,
				m33 = 1
			};
		}

		/// <summary>Returns the natural logarithm of a quaternion</summary>
		public static Quaternion Ln( this Quaternion q ) {
			double vMagSq = (double)q.X * q.X + (double)q.Y * q.Y + (double)q.Z * q.Z;
			double vMag = Math.Sqrt( vMagSq );
			double qMag = Math.Sqrt( vMagSq + (double)q.W * q.W );
			double theta = Math.Atan2( vMag, q.W );
			double scV = vMag < 0.001 ? Mathfs.SincRcp( theta ) / qMag : theta / vMag;
			return new Quaternion(
				(float)( scV * q.X ),
				(float)( scV * q.Y ),
				(float)( scV * q.Z ),
				(float)Math.Log( qMag )
			);
		}

		/// <summary>Returns the natural logarithm of a unit quaternion</summary>
		public static Quaternion LnUnit( this Quaternion q ) {
			double vMagSq = (double)q.X * q.X + (double)q.Y * q.Y + (double)q.Z * q.Z;
			double vMag = Math.Sqrt( vMagSq );
			double theta = Math.Atan2( vMag, q.W );
			double scV = vMag < 0.001 ? Mathfs.SincRcp( theta ) : theta / vMag;
			return new Quaternion(
				(float)( scV * q.X ),
				(float)( scV * q.Y ),
				(float)( scV * q.Z ),
				0f
			);
		}

		/// <summary>Returns the natural exponent of a quaternion</summary>
		public static Quaternion Exp( this Quaternion q ) {
			double vMag = Math.Sqrt( (double)q.X * q.X + (double)q.Y * q.Y + (double)q.Z * q.Z );
			double sc = Math.Exp( q.W );
			double scV = sc * Mathfs.Sinc( vMag );
			return new Quaternion( (float)( scV * q.X ), (float)( scV * q.Y ), (float)( scV * q.Z ), (float)( sc * Math.Cos( vMag ) ) );
		}

		/// <summary>Returns the natural exponent of a pure imaginary quaternion</summary>
		public static Quaternion ExpPureIm( this Quaternion q ) {
			double vMag = Math.Sqrt( (double)q.X * q.X + (double)q.Y * q.Y + (double)q.Z * q.Z );
			double scV = Mathfs.Sinc( vMag );
			return new Quaternion( (float)( scV * q.X ), (float)( scV * q.Y ), (float)( scV * q.Z ), (float)Math.Cos( vMag ) );
		}

		/// <summary>Returns the quaternion raised to a real power</summary>
		public static Quaternion Pow( this Quaternion q, float x ) {
			return x switch {
				0f => new Quaternion( 0, 0, 0, 1 ),
				1f => q,
				_  => q.Ln().Mul( x ).Exp()
			};
		}

		/// <summary>Returns the unit quaternion raised to a real power</summary>
		public static Quaternion PowUnit( this Quaternion q, float x ) {
			return x switch {
				0f => new Quaternion( 0, 0, 0, 1 ),
				1f => q,
				_  => q.LnUnit().Mul( x ).ExpPureIm()
			};
		}

		/// <summary>Returns the imaginary part of a quaternion as a vector</summary>
		public static Vector3 Imag( this Quaternion q ) => new Vector3( q.X, q.Y, q.Z );

		/// <summary>Returns the squared magnitude of this quaternion</summary>
		public static float SqrMagnitude( this Quaternion q ) => (float)( (double)q.W * q.W + (double)q.X * q.X + (double)q.Y * q.Y + (double)q.Z * q.Z );

		/// <summary>Returns the magnitude of this quaternion</summary>
		public static float Magnitude( this Quaternion q ) => MathF.Sqrt( q.SqrMagnitude() );

		/// <summary>Multiplies a quaternion by a scalar</summary>
		/// <param name="q">The quaternion to multiply</param>
		/// <param name="c">The scalar value to multiply with</param>
		public static Quaternion Mul( this Quaternion q, float c ) => new Quaternion( c * q.X, c * q.Y, c * q.Z, c * q.W );

		/// <summary>Adds a quaternion to an existing quaternion</summary>
		public static Quaternion Add( this Quaternion a, Quaternion b ) => new Quaternion( a.X + b.X, a.Y + b.Y, a.Z + b.Z, a.W + b.W );

		/// <summary>Subtracts a quaternion from an existing quaternion</summary>
		public static Quaternion Sub( this Quaternion a, Quaternion b ) => new Quaternion( a.X - b.X, a.Y - b.Y, a.Z - b.Z, a.W - b.W );

		/// <summary>The conjugate of a quaternion</summary>
		/// <param name="q">The quaternion to conjugate</param>
		public static Quaternion Conjugate( this Quaternion q ) => new Quaternion( -q.X, -q.Y, -q.Z, q.W );

		/// <inheritdoc cref="Quaternion.Inverse(Quaternion)"/>
		public static Quaternion Inverse( this Quaternion q ) => q.Inverse();

		/// <summary>The inverse of a unit quaternion, equivalent to the quaternion conjugate</summary>
		public static Quaternion InverseUnit( this Quaternion q ) {
			return new Quaternion( -q.X, -q.Y, -q.Z, q.W );
		}

		/// <summary>The inverse of a pure imaginary unit quaternion, where w is assumed to be 0</summary>
		public static Quaternion InversePureIm( this Quaternion q ) {
			float sqMag = q.X * q.X + q.Y * q.Y + q.Z * q.Z;
			return new Quaternion( -q.X / sqMag, -q.Y / sqMag, -q.Z / sqMag, 0 );
		}

		/// <summary>Add to the magnitude of this quaternion</summary>
		public static Quaternion AddMagnitude( this Quaternion q, float amount ) => amount == 0f ? q : q.Mul( 1 + amount / q.Magnitude() );

		#endregion

		#region Transform extensions

		/// <summary>Transforms a rotation from local space to world space</summary>
		/// <param name="tf">The transform to use</param>
		/// <param name="quat">The local space rotation</param>
		public static Quaternion TransformRotation( this Transform tf, Quaternion quat ) => tf.Basis.GetRotationQuaternion() * quat;

		/// <summary>Transforms a rotation from world space to local space</summary>
		/// <param name="tf">The transform to use</param>
		/// <param name="quat">The world space rotation</param>
		public static Quaternion InverseTransformRotation( this Transform tf, Quaternion quat ) => tf.Basis.GetRotationQuaternion().Inverse() * quat;

		/// <summary>Transforms a ray from world space to local space</summary>
		/// <param name="tf">The transform to use</param>
		/// <param name="ray">The world space ray</param>
		public static Ray InverseTransformRay( this Transform tf, Ray ray ) => new(tf.InverseTransformPoint( ray.Origin ), tf.InverseTransformDirection( ray.Direction ));

		/// <summary>Transforms a ray from local space to world space</summary>
		/// <param name="tf">The transform to use</param>
		/// <param name="ray">The local space ray</param>
		public static Ray TransformRay( this Transform tf, Ray ray ) => new(tf.TransformPoint( ray.Origin ), tf.TransformDirection( ray.Direction ));

		#endregion

		#region Color manipulation

		/// <summary>Returns the same color, but with the specified alpha value</summary>
		/// <param name="c">The source color</param>
		/// <param name="a">The new alpha value</param>
		[MethodImpl( INLINE )] public static Color WithAlpha( this Color c, float a ) => new Color( c.R, c.G, c.B, a );

		/// <summary>Returns the same color and alpha, but with RGB multiplied by the given value</summary>
		/// <param name="c">The source color</param>
		/// <param name="m">The multiplier for the RGB channels</param>
		[MethodImpl( INLINE )] public static Color MultiplyRGB( this Color c, float m ) => new Color( c.R * m, c.G * m, c.B * m, c.A );

		/// <summary>Returns the same color and alpha, but with the RGB values multiplief by another color</summary>
		/// <param name="c">The source color</param>
		/// <param name="m">The color to multiply RGB by</param>
		[MethodImpl( INLINE )] public static Color MultiplyRGB( this Color c, Color m ) => new Color( c.R * m.R, c.G * m.G, c.B * m.B, c.A );

		/// <summary>Returns the same color, but with the alpha channel multiplied by the given value</summary>
		/// <param name="c">The source color</param>
		/// <param name="m">The multiplier for the alpha</param>
		[MethodImpl( INLINE )] public static Color MultiplyA( this Color c, float m ) => new Color( c.R, c.G, c.B, c.A * m );

		/// <summary>Converts this color to the nearest 32 bit hex string, including the alpha channel.
		/// A pure red color of (1,0,0,1) returns "FF0000FF"</summary>
		/// <param name="c">The color to get the hex string of</param>
		/// <returns></returns>
		[MethodImpl( INLINE )] public static string ToHexString( this Color c ) => c.ToHtml();

		#endregion

		#region Rect

		/// <summary>Expands the rectangle to encapsulate the point <c>p</c></summary>
		/// <param name="r">The rectangle to expand</param>
		/// <param name="p">The point to encapsulate</param>
		public static Rect Encapsulate( this Rect r, Vector2 p ) {
			r.Position = new Vector2(
				MathF.Min( r.Position.X, p.X ),
				MathF.Min( r.Position.Y, p.Y )
			);
			r.End = new Vector2(
				MathF.Max( r.End.X, p.X ),
				MathF.Max( r.End.Y, p.Y )
			);
			return r;
		}

		/// <summary>Interpolates a position within this rectangle, given a normalized position</summary>
		/// <param name="r">The rectangle to get a position within</param>
		/// <param name="tPos">The normalized position within this rectangle</param>
		public static Vector2 Lerp( this Rect r, Vector2 tPos ) =>
			new(
				Mathfs.Lerp( r.Position.X, r.End.X, tPos.X ),
				Mathfs.Lerp( r.Position.Y, r.End.Y, tPos.Y )
			);

		/// <summary>The x axis range of this rectangle</summary>
		/// <param name="rect">The rectangle to get the x range of</param>
		public static FloatRange RangeX( this Rect rect ) => ( rect.Position.X, rect.End.X );

		/// <summary>The y axis range of this rectangle</summary>
		/// <param name="rect">The rectangle to get the y range of</param>
		public static FloatRange RangeY( this Rect rect ) => ( rect.Position.Y, rect.End.Y );

		/// <summary>Places the center of this rectangle at its position,
		/// useful together with the constructor to define it by center instead of by corner</summary>
		public static Rect ByCenter( this Rect r ) => new Rect( r.Position - r.Size / 2f , r.Size );

		#endregion

		#region Simple float and int operations

		/// <summary>Returns true if v is between or equal to <c>min</c> &amp; <c>max</c></summary>
		/// <seealso cref="Between(float,float,float)"/>
		[MethodImpl( INLINE )] public static bool Within( this float v, float min, float max ) => v >= min && v <= max;

		/// <summary>Returns true if v is between or equal to <c>min</c> &amp; <c>max</c></summary>
		/// <seealso cref="Between(int,int,int)"/>
		[MethodImpl( INLINE )] public static bool Within( this int v, int min, int max ) => v >= min && v <= max;

		/// <summary>Returns true if v is between, but not equal to, <c>min</c> &amp; <c>max</c></summary>
		/// <seealso cref="Within(float,float,float)"/>
		[MethodImpl( INLINE )] public static bool Between( this float v, float min, float max ) => v > min && v < max;

		/// <summary>Returns true if v is between, but not equal to, <c>min</c> &amp; <c>max</c></summary>
		/// <seealso cref="Within(int,int,int)"/>
		[MethodImpl( INLINE )] public static bool Between( this int v, int min, int max ) => v > min && v < max;

		/// <summary>Clamps the value to be at least <c>min</c></summary>
		[MethodImpl( INLINE )] public static float AtLeast( this float v, float min ) => v < min ? min : v;

		/// <summary>Clamps the value to be at least <c>min</c></summary>
		[MethodImpl( INLINE )] public static int AtLeast( this int v, int min ) => v < min ? min : v;

		/// <summary>Clamps the value to be at most <c>max</c></summary>
		[MethodImpl( INLINE )] public static float AtMost( this float v, float max ) => v > max ? max : v;

		/// <summary>Clamps the value to be at most <c>max</c></summary>
		[MethodImpl( INLINE )] public static int AtMost( this int v, int max ) => v > max ? max : v;

		/// <summary>Squares the value. Equivalent to <c>v*v</c></summary>
		[MethodImpl( INLINE )] public static float Square( this float v ) => v * v;

		/// <summary>Cubes the value. Equivalent to <c>v*v*v</c></summary>
		[MethodImpl( INLINE )] public static float Cube( this float v ) => v * v * v;

		/// <summary>Squares the value. Equivalent to <c>v*v</c></summary>
		[MethodImpl( INLINE )] public static int Square( this int v ) => v * v;

		/// <summary>The next integer, modulo <c>length</c>. Behaves the way you want with negative values for stuff like array index access etc</summary>
		[MethodImpl( INLINE )] public static int NextMod( this int value, int length ) => ( value + 1 ).Mod( length );

		/// <summary>The previous integer, modulo <c>length</c>. Behaves the way you want with negative values for stuff like array index access etc</summary>
		[MethodImpl( INLINE )] public static int PrevMod( this int value, int length ) => ( value - 1 ).Mod( length );

		#endregion

		#region String extensions

		public static string ToValueTableString( this string[,] m ) {
			int rowCount = m.GetLength( 0 );
			int colCount = m.GetLength( 1 );
			string[] r = new string[rowCount];
			for( int i = 0; i < rowCount; i++ )
				r[i] = "";

			for( int c = 0; c < colCount; c++ ) {
				string endBit = c == colCount - 1 ? "" : ", ";

				int colWidth = 4; // min width
				string[] columnEntries = new string[rowCount];
				for( int row = 0; row < rowCount; row++ ) {
					string s = m[row, c].StartsWith( '-' ) ? "" : " ";
					columnEntries[row] = $"{s}{m[row, c]}{endBit}";
					colWidth = Mathfs.Max( colWidth, columnEntries[row].Length );
				}

				for( int row = 0; row < rowCount; row++ ) {
					r[row] += columnEntries[row].PadRight( colWidth, ' ' );
				}
			}

			return string.Join( '\n', r );
		}

		#endregion

		#region Matrix extensions

		public static float AverageScale( this Matrix4x4 m ) {
			return (
				( m.GetColumn( 0 ).ToVector3() ).Length() +
				( m.GetColumn( 1 ).ToVector3() ).Length() +
				( m.GetColumn( 2 ).ToVector3() ).Length()
			) / 3;
		}

		public static Matrix4x1 MultiplyColumnVector( this Matrix4x4 m, Matrix4x1 v ) =>
			new Matrix4x1(
				m.m00 * v.m0 + m.m01 * v.m1 + m.m02 * v.m2 + m.m03 * v.m3,
				m.m10 * v.m0 + m.m11 * v.m1 + m.m12 * v.m2 + m.m13 * v.m3,
				m.m20 * v.m0 + m.m21 * v.m1 + m.m22 * v.m2 + m.m23 * v.m3,
				m.m30 * v.m0 + m.m31 * v.m1 + m.m32 * v.m2 + m.m33 * v.m3
			);

		public static Vector2Matrix4x1 MultiplyColumnVector( this Matrix4x4 m, Vector2Matrix4x1 v ) => new(m.MultiplyColumnVector( v.X ), m.MultiplyColumnVector( v.Y ));
		public static Vector3Matrix4x1 MultiplyColumnVector( this Matrix4x4 m, Vector3Matrix4x1 v ) => new(m.MultiplyColumnVector( v.X ), m.MultiplyColumnVector( v.Y ), m.MultiplyColumnVector( v.Z ));
		public static Vector4Matrix4x1 MultiplyColumnVector( this Matrix4x4 m, Vector4Matrix4x1 v ) => new(m.MultiplyColumnVector( v.X ), m.MultiplyColumnVector( v.Y ), m.MultiplyColumnVector( v.Z ), m.MultiplyColumnVector( v.W ));

		/// <summary>Transforms a ray by this matrix</summary>
		/// <param name="mtx">The matrix to use</param>
		/// <param name="ray">The ray to transform</param>
		public static Ray MultiplyRay( this Matrix4x4 mtx, Ray ray ) => new(mtx.MultiplyPoint3x4( ray.Origin ), mtx.MultiplyVector( ray.Direction ));

		#endregion

		#region Extension method counterparts of the static Mathfs functions - lots of boilerplate in here

		#region Math operations

		/// <inheritdoc cref="Mathfs.Sqrt(float)"/>
		[MethodImpl( INLINE )] public static float Sqrt( this float value ) => MathF.Sqrt( value );

		/// <inheritdoc cref="Mathfs.Sqrt(Vector2)"/>
		[MethodImpl( INLINE )] public static Vector2 Sqrt( this Vector2 value ) => Mathfs.Sqrt( value );

		/// <inheritdoc cref="Mathfs.Sqrt(Vector3)"/>
		[MethodImpl( INLINE )] public static Vector3 Sqrt( this Vector3 value ) => Mathfs.Sqrt( value );

		/// <inheritdoc cref="Mathfs.Sqrt(Vector4)"/>
		[MethodImpl( INLINE )] public static Vector4 Sqrt( this Vector4 value ) => Mathfs.Sqrt( value );

		/// <inheritdoc cref="Mathfs.Cbrt(float)"/>
		[MethodImpl( INLINE )] public static float Cbrt( this float value ) => MathF.Cbrt( value );

		/// <inheritdoc cref="Mathfs.Pow(float, float)"/>
		[MethodImpl( INLINE )] public static float Pow( this float value, float exponent ) => MathF.Pow( value, exponent );

		/// <summary>Calculates exact positive integer powers</summary>
		/// <param name="value"></param>
		/// <param name="pow">A positive integer power</param>
		[MethodImpl( INLINE )] public static int Pow( this int value, int pow ) {
			if( pow < 0 )
				throw new ArithmeticException( "int.Pow(int) doesn't support negative powers" );
			checked {
				switch( pow ) {
					case 0: return 1;
					case 1: return value;
					case 2: return value * value;
					case 3: return value * value * value;
					default:
						if( value == 2 )
							return 1 << pow;
						// from: https://stackoverflow.com/questions/383587/how-do-you-do-integer-exponentiation-in-c
						int ret = 1;
						while( pow != 0 ) {
							if( ( pow & 1 ) == 1 )
								ret *= value;
							value *= value;
							pow >>= 1;
						}

						return ret;
				}
			}
		}

		#endregion

		#region Absolute Values

		/// <inheritdoc cref="Mathfs.Abs(float)"/>
		[MethodImpl( INLINE )] public static float Abs( this float value ) => MathF.Abs( value );

		/// <inheritdoc cref="Mathfs.Abs(int)"/>
		[MethodImpl( INLINE )] public static int Abs( this int value ) => Mathfs.Abs( value );

		/// <inheritdoc cref="Mathfs.Abs(Vector2)"/>
		[MethodImpl( INLINE )] public static Vector2 Abs( this Vector2 v ) => Mathfs.Abs( v );

		/// <inheritdoc cref="Mathfs.Abs(Vector3)"/>
		[MethodImpl( INLINE )] public static Vector3 Abs( this Vector3 v ) => Mathfs.Abs( v );

		/// <inheritdoc cref="Mathfs.Abs(Vector4)"/>
		[MethodImpl( INLINE )] public static Vector4 Abs( this Vector4 v ) => Mathfs.Abs( v );

		#endregion

		#region Clamping

		/// <inheritdoc cref="Mathfs.Clamp(float,float,float)"/>
		[MethodImpl( INLINE )] public static float Clamp( this float value, float min, float max ) => Mathfs.Clamp( value, min, max );

		/// <inheritdoc cref="Mathfs.Clamp(Vector2,Vector2,Vector2)"/>
		[MethodImpl( INLINE )] public static Vector2 Clamp( this Vector2 v, Vector2 min, Vector2 max ) => Mathfs.Clamp( v, min, max );

		/// <inheritdoc cref="Mathfs.Clamp(Vector3,Vector3,Vector3)"/>
		[MethodImpl( INLINE )] public static Vector3 Clamp( this Vector3 v, Vector3 min, Vector3 max ) => Mathfs.Clamp( v, min, max );

		/// <inheritdoc cref="Mathfs.Clamp(Vector4,Vector4,Vector4)"/>
		[MethodImpl( INLINE )] public static Vector4 Clamp( this Vector4 v, Vector4 min, Vector4 max ) => Mathfs.Clamp( v, min, max );

		/// <inheritdoc cref="Mathfs.Clamp(int,int,int)"/>
		[MethodImpl( INLINE )] public static int Clamp( this int value, int min, int max ) => Mathfs.Clamp( value, min, max );

		/// <inheritdoc cref="Mathfs.Clamp01(float)"/>
		[MethodImpl( INLINE )] public static float Clamp01( this float value ) => Mathfs.Clamp01( value );

		/// <inheritdoc cref="Mathfs.Clamp01(Vector2)"/>
		[MethodImpl( INLINE )] public static Vector2 Clamp01( this Vector2 v ) => Mathfs.Clamp01( v );

		/// <inheritdoc cref="Mathfs.Clamp01(Vector3)"/>
		[MethodImpl( INLINE )] public static Vector3 Clamp01( this Vector3 v ) => Mathfs.Clamp01( v );

		/// <inheritdoc cref="Mathfs.Clamp01(Vector4)"/>
		[MethodImpl( INLINE )] public static Vector4 Clamp01( this Vector4 v ) => Mathfs.Clamp01( v );

		/// <inheritdoc cref="Mathfs.ClampNeg1to1(float)"/>
		[MethodImpl( INLINE )] public static float ClampNeg1to1( this float value ) => Mathfs.ClampNeg1to1( value );

		/// <inheritdoc cref="Mathfs.ClampNeg1to1(float)"/>
		[MethodImpl( INLINE )] public static double ClampNeg1to1( this double value ) => Mathfs.ClampNeg1to1( value );

		/// <inheritdoc cref="Mathfs.ClampNeg1to1(Vector2)"/>
		[MethodImpl( INLINE )] public static Vector2 ClampNeg1to1( this Vector2 v ) => Mathfs.ClampNeg1to1( v );

		/// <inheritdoc cref="Mathfs.ClampNeg1to1(Vector3)"/>
		[MethodImpl( INLINE )] public static Vector3 ClampNeg1to1( this Vector3 v ) => Mathfs.ClampNeg1to1( v );

		/// <inheritdoc cref="Mathfs.ClampNeg1to1(Vector4)"/>
		[MethodImpl( INLINE )] public static Vector4 ClampNeg1to1( this Vector4 v ) => Mathfs.ClampNeg1to1( v );

		#endregion

		#region Min & Max

		/// <inheritdoc cref="Mathfs.Min(Vector2)"/>
		[MethodImpl( INLINE )] public static float Min( this Vector2 v ) => Mathfs.Min( v );

		/// <inheritdoc cref="Mathfs.Min(Vector3)"/>
		[MethodImpl( INLINE )] public static float Min( this Vector3 v ) => Mathfs.Min( v );

		/// <inheritdoc cref="Mathfs.Min(Vector4)"/>
		[MethodImpl( INLINE )] public static float Min( this Vector4 v ) => Mathfs.Min( v );

		/// <inheritdoc cref="Mathfs.Max(Vector2)"/>
		[MethodImpl( INLINE )] public static float Max( this Vector2 v ) => Mathfs.Max( v );

		/// <inheritdoc cref="Mathfs.Max(Vector3)"/>
		[MethodImpl( INLINE )] public static float Max( this Vector3 v ) => Mathfs.Max( v );

		/// <inheritdoc cref="Mathfs.Max(Vector4)"/>
		[MethodImpl( INLINE )] public static float Max( this Vector4 v ) => Mathfs.Max( v );

		#endregion

		#region Signs & Rounding

		/// <inheritdoc cref="Mathfs.Sign(float)"/>
		[MethodImpl( INLINE )] public static float Sign( this float value ) => Mathfs.Sign( value );

		/// <inheritdoc cref="Mathfs.Sign(Vector2)"/>
		[MethodImpl( INLINE )] public static Vector2 Sign( this Vector2 value ) => Mathfs.Sign( value );

		/// <inheritdoc cref="Mathfs.Sign(Vector3)"/>
		[MethodImpl( INLINE )] public static Vector3 Sign( this Vector3 value ) => Mathfs.Sign( value );

		/// <inheritdoc cref="Mathfs.Sign(Vector4)"/>
		[MethodImpl( INLINE )] public static Vector4 Sign( this Vector4 value ) => Mathfs.Sign( value );

		/// <inheritdoc cref="Mathfs.Sign(int)"/>
		[MethodImpl( INLINE )] public static int Sign( this int value ) => Mathfs.Sign( value );

		/// <inheritdoc cref="Mathfs.SignAsInt(float)"/>
		[MethodImpl( INLINE )] public static int SignAsInt( this float value ) => Mathfs.SignAsInt( value );

		/// <inheritdoc cref="Mathfs.SignWithZero(float,float)"/>
		[MethodImpl( INLINE )] public static float SignWithZero( this float value, float zeroThreshold = 0.000001f ) => Mathfs.SignWithZero( value, zeroThreshold );

		/// <inheritdoc cref="Mathfs.SignWithZero(Vector2,float)"/>
		[MethodImpl( INLINE )] public static Vector2 SignWithZero( this Vector2 value, float zeroThreshold = 0.000001f ) => Mathfs.SignWithZero( value, zeroThreshold );

		/// <inheritdoc cref="Mathfs.SignWithZero(Vector3,float)"/>
		[MethodImpl( INLINE )] public static Vector3 SignWithZero( this Vector3 value, float zeroThreshold = 0.000001f ) => Mathfs.SignWithZero( value, zeroThreshold );

		/// <inheritdoc cref="Mathfs.SignWithZero(Vector4,float)"/>
		[MethodImpl( INLINE )] public static Vector4 SignWithZero( this Vector4 value, float zeroThreshold = 0.000001f ) => Mathfs.SignWithZero( value, zeroThreshold );

		/// <inheritdoc cref="Mathfs.SignWithZero(int)"/>
		[MethodImpl( INLINE )] public static int SignWithZero( this int value ) => Mathfs.SignWithZero( value );

		/// <inheritdoc cref="Mathfs.SignWithZeroAsInt(float,float)"/>
		[MethodImpl( INLINE )] public static int SignWithZeroAsInt( this float value, float zeroThreshold = 0.000001f ) => Mathfs.SignWithZeroAsInt( value, zeroThreshold );

		/// <inheritdoc cref="Mathfs.Floor(float)"/>
		[MethodImpl( INLINE )] public static float Floor( this float value ) => Mathfs.Floor( value );

		/// <inheritdoc cref="Mathfs.Floor(Vector2)"/>
		[MethodImpl( INLINE )] public static Vector2 Floor( this Vector2 value ) => Mathfs.Floor( value );

		/// <inheritdoc cref="Mathfs.Floor(Vector2)"/>
		[MethodImpl( INLINE )] public static Vector3 Floor( this Vector3 value ) => Mathfs.Floor( value );

		/// <inheritdoc cref="Mathfs.Floor(Vector2)"/>
		[MethodImpl( INLINE )] public static Vector4 Floor( this Vector4 value ) => Mathfs.Floor( value );

		/// <inheritdoc cref="Mathfs.FloorToInt(float)"/>
		[MethodImpl( INLINE )] public static int FloorToInt( this float value ) => Mathfs.FloorToInt( value );

		/// <inheritdoc cref="Mathfs.FloorToInt(Vector2)"/>
		[MethodImpl( INLINE )] public static Vector2Int FloorToInt( this Vector2 value ) => Mathfs.FloorToInt( value );

		/// <inheritdoc cref="Mathfs.FloorToInt(Vector2)"/>
		[MethodImpl( INLINE )] public static Vector3Int FloorToInt( this Vector3 value ) => Mathfs.FloorToInt( value );

		/// <inheritdoc cref="Mathfs.Ceil(float)"/>
		[MethodImpl( INLINE )] public static float Ceil( this float value ) => Mathfs.Ceil( value );

		/// <inheritdoc cref="Mathfs.Ceil(Vector2)"/>
		[MethodImpl( INLINE )] public static Vector2 Ceil( this Vector2 value ) => Mathfs.Ceil( value );

		/// <inheritdoc cref="Mathfs.Ceil(Vector2)"/>
		[MethodImpl( INLINE )] public static Vector3 Ceil( this Vector3 value ) => Mathfs.Ceil( value );

		/// <inheritdoc cref="Mathfs.Ceil(Vector2)"/>
		[MethodImpl( INLINE )] public static Vector4 Ceil( this Vector4 value ) => Mathfs.Ceil( value );

		/// <inheritdoc cref="Mathfs.CeilToInt(float)"/>
		[MethodImpl( INLINE )] public static int CeilToInt( this float value ) => Mathfs.CeilToInt( value );

		/// <inheritdoc cref="Mathfs.CeilToInt(Vector2)"/>
		[MethodImpl( INLINE )] public static Vector2Int CeilToInt( this Vector2 value ) => Mathfs.CeilToInt( value );

		/// <inheritdoc cref="Mathfs.CeilToInt(Vector2)"/>
		[MethodImpl( INLINE )] public static Vector3Int CeilToInt( this Vector3 value ) => Mathfs.CeilToInt( value );

		/// <inheritdoc cref="Mathfs.Round(float,MidpointRounding)"/>
		[MethodImpl( INLINE )] public static float Round( this float value, MidpointRounding midpointRounding = MidpointRounding.ToEven ) => Mathfs.Round( value, midpointRounding );

		/// <inheritdoc cref="Mathfs.Round(Vector2,MidpointRounding)"/>
		[MethodImpl( INLINE )] public static Vector2 Round( this Vector2 value, MidpointRounding midpointRounding = MidpointRounding.ToEven ) => Mathfs.Round( value, midpointRounding );

		/// <inheritdoc cref="Mathfs.Round(Vector2,MidpointRounding)"/>
		[MethodImpl( INLINE )] public static Vector3 Round( this Vector3 value, MidpointRounding midpointRounding = MidpointRounding.ToEven ) => Mathfs.Round( value, midpointRounding );

		/// <inheritdoc cref="Mathfs.Round(Vector2,MidpointRounding)"/>
		[MethodImpl( INLINE )] public static Vector4 Round( this Vector4 value, MidpointRounding midpointRounding = MidpointRounding.ToEven ) => Mathfs.Round( value, midpointRounding );

		/// <inheritdoc cref="Mathfs.Round(float,MidpointRounding)"/>
		[MethodImpl( INLINE )] public static float Round( this float value, float snapInterval, MidpointRounding midpointRounding = MidpointRounding.ToEven ) => Mathfs.Round( value, snapInterval, midpointRounding );

		/// <inheritdoc cref="Mathfs.Round(Vector2,MidpointRounding)"/>
		[MethodImpl( INLINE )] public static Vector2 Round( this Vector2 value, float snapInterval, MidpointRounding midpointRounding = MidpointRounding.ToEven ) => Mathfs.Round( value, snapInterval, midpointRounding );

		/// <inheritdoc cref="Mathfs.Round(Vector2,float,MidpointRounding)"/>
		[MethodImpl( INLINE )] public static Vector3 Round( this Vector3 value, float snapInterval, MidpointRounding midpointRounding = MidpointRounding.ToEven ) => Mathfs.Round( value, snapInterval, midpointRounding );

		/// <inheritdoc cref="Mathfs.Round(Vector2,float,MidpointRounding)"/>
		[MethodImpl( INLINE )] public static Vector4 Round( this Vector4 value, float snapInterval, MidpointRounding midpointRounding = MidpointRounding.ToEven ) => Mathfs.Round( value, snapInterval, midpointRounding );

		/// <inheritdoc cref="Mathfs.RoundToInt(float,MidpointRounding)"/>
		[MethodImpl( INLINE )] public static int RoundToInt( this float value, MidpointRounding midpointRounding = MidpointRounding.ToEven ) => Mathfs.RoundToInt( value, midpointRounding );

		/// <inheritdoc cref="Mathfs.RoundToInt(Vector2,MidpointRounding)"/>
		[MethodImpl( INLINE )] public static Vector2Int RoundToInt( this Vector2 value, MidpointRounding midpointRounding = MidpointRounding.ToEven ) => Mathfs.RoundToInt( value, midpointRounding );

		/// <inheritdoc cref="Mathfs.RoundToInt(Vector2,MidpointRounding)"/>
		[MethodImpl( INLINE )] public static Vector3Int RoundToInt( this Vector3 value, MidpointRounding midpointRounding = MidpointRounding.ToEven ) => Mathfs.RoundToInt( value, midpointRounding );

		#endregion

		#region Range Repeating

		/// <inheritdoc cref="Mathfs.Frac(float)"/>
		[MethodImpl( INLINE )] public static float Frac( this float x ) => Mathfs.Frac( x );

		/// <inheritdoc cref="Mathfs.Frac(Vector2)"/>
		[MethodImpl( INLINE )] public static Vector2 Frac( this Vector2 v ) => Mathfs.Frac( v );

		/// <inheritdoc cref="Mathfs.Frac(Vector3)"/>
		[MethodImpl( INLINE )] public static Vector3 Frac( this Vector3 v ) => Mathfs.Frac( v );

		/// <inheritdoc cref="Mathfs.Frac(Vector4)"/>
		[MethodImpl( INLINE )] public static Vector4 Frac( this Vector4 v ) => Mathfs.Frac( v );

		/// <inheritdoc cref="Mathfs.Repeat(float,float)"/>
		[MethodImpl( INLINE )] public static float Repeat( this float value, float length ) => Mathfs.Repeat( value, length );

		/// <inheritdoc cref="Mathfs.Mod(int,int)"/>
		[MethodImpl( INLINE )] public static int Mod( this int value, int length ) => Mathfs.Mod( value, length );

		#endregion

		#region Smoothing & Easing Curves

		/// <inheritdoc cref="Mathfs.Smooth01(float)"/>
		[MethodImpl( INLINE )] public static float Smooth01( this float x ) => Mathfs.Smooth01( x );

		/// <inheritdoc cref="Mathfs.Smoother01(float)"/>
		[MethodImpl( INLINE )] public static float Smoother01( this float x ) => Mathfs.Smoother01( x );

		/// <inheritdoc cref="Mathfs.SmoothCos01(float)"/>
		[MethodImpl( INLINE )] public static float SmoothCos01( this float x ) => Mathfs.SmoothCos01( x );

		#endregion

		#region Value & Vector interpolation

		/// <inheritdoc cref="Mathfs.Remap(float,float,float,float,float)"/>
		[MethodImpl( INLINE )] public static float Remap( this float value, float iMin, float iMax, float oMin, float oMax ) => Mathfs.Remap( iMin, iMax, oMin, oMax, value );

		/// <inheritdoc cref="Mathfs.RemapClamped(float,float,float,float,float)"/>
		[MethodImpl( INLINE )] public static float RemapClamped( this float value, float iMin, float iMax, float oMin, float oMax ) => Mathfs.RemapClamped( iMin, iMax, oMin, oMax, value );

		/// <inheritdoc cref="Mathfs.Remap(FloatRange,FloatRange,float)"/>
		[MethodImpl( INLINE )] public static float Remap( this float value, FloatRange inRange, FloatRange outRange ) => Mathfs.Remap( inRange.a, inRange.b, outRange.a, outRange.b, value );

		/// <inheritdoc cref="Mathfs.RemapClamped(FloatRange,FloatRange,float)"/>
		[MethodImpl( INLINE )] public static float RemapClamped( this float value, FloatRange inRange, FloatRange outRange ) => Mathfs.RemapClamped( inRange.a, inRange.b, outRange.a, outRange.b, value );

		/// <inheritdoc cref="Mathfs.Remap(float,float,float,float,int)"/>
		[MethodImpl( INLINE )] public static float Remap( this int value, float iMin, float iMax, float oMin, float oMax ) => Mathfs.Remap( iMin, iMax, oMin, oMax, value );

		/// <inheritdoc cref="Mathfs.RemapClamped(float,float,float,float,float)"/>
		[MethodImpl( INLINE )] public static float RemapClamped( this int value, float iMin, float iMax, float oMin, float oMax ) => Mathfs.RemapClamped( iMin, iMax, oMin, oMax, value );

		/// <inheritdoc cref="Mathfs.Lerp(float,float,float)"/>
		[MethodImpl( INLINE )] public static float Lerp( this float t, float a, float b ) => Mathfs.Lerp( a, b, t );

		/// <inheritdoc cref="Mathfs.InverseLerp(float,float,float)"/>
		[MethodImpl( INLINE )] public static float InverseLerp( this float value, float a, float b ) => Mathfs.InverseLerp( a, b, value );

		/// <inheritdoc cref="Mathfs.LerpClamped(float,float,float)"/>
		[MethodImpl( INLINE )] public static float LerpClamped( this float t, float a, float b ) => Mathfs.LerpClamped( a, b, t );

		/// <inheritdoc cref="Mathfs.InverseLerpClamped(float,float,float)"/>
		[MethodImpl( INLINE )] public static float InverseLerpClamped( this float value, float a, float b ) => Mathfs.InverseLerpClamped( a, b, value );

		/// <inheritdoc cref="Mathfs.Remap(Vector2,Vector2,Vector2,Vector2,Vector2)"/>
		[MethodImpl( INLINE )] public static Vector2 Remap( this Vector2 v, Vector2 iMin, Vector2 iMax, Vector2 oMin, Vector2 oMax ) => Mathfs.Remap( iMin, iMax, oMin, oMax, v );

		/// <inheritdoc cref="Mathfs.Remap(Vector3,Vector3,Vector3,Vector3,Vector3)"/>
		[MethodImpl( INLINE )] public static Vector3 Remap( this Vector3 v, Vector3 iMin, Vector3 iMax, Vector3 oMin, Vector3 oMax ) => Mathfs.Remap( iMin, iMax, oMin, oMax, v );

		/// <inheritdoc cref="Mathfs.Remap(Vector4,Vector4,Vector4,Vector4,Vector4)"/>
		[MethodImpl( INLINE )] public static Vector4 Remap( this Vector4 v, Vector4 iMin, Vector4 iMax, Vector4 oMin, Vector4 oMax ) => Mathfs.Remap( iMin, iMax, oMin, oMax, v );

		/// <inheritdoc cref="Mathfs.Remap(Rect,Rect,Vector2)"/>
		[MethodImpl( INLINE )] public static Vector2 Remap( this Vector2 iPos, Rect iRect, Rect oRect ) => Mathfs.Remap( iRect.Position, iRect.End, oRect.Position, oRect.End, iPos );

		/// <inheritdoc cref="Mathfs.Remap(Bounds,Bounds,Vector3)"/>
		[MethodImpl( INLINE )] public static Vector3 Remap( this Vector3 iPos, Bounds iBounds, Bounds oBounds ) => Mathfs.Remap( iBounds.Position, iBounds.End, oBounds.Position, oBounds.End, iPos );

		/// <inheritdoc cref="Mathfs.Eerp(float,float,float)"/>
		[MethodImpl( INLINE )] public static float Eerp( this float t, float a, float b ) => Mathfs.Eerp( a, b, t );

		/// <inheritdoc cref="Mathfs.InverseEerp(float,float,float)"/>
		[MethodImpl( INLINE )] public static float InverseEerp( this float v, float a, float b ) => Mathfs.InverseEerp( a, b, v );

		#endregion

		#region Vector Math

		/// <inheritdoc cref="Mathfs.GetDirAndMagnitude(Vector2)"/>
		[MethodImpl( INLINE )] public static (Vector2 dir, float magnitude ) GetDirAndMagnitude( this Vector2 v ) => Mathfs.GetDirAndMagnitude( v );

		/// <inheritdoc cref="Mathfs.GetDirAndMagnitude(Vector3)"/>
		[MethodImpl( INLINE )] public static (Vector3 dir, float magnitude ) GetDirAndMagnitude( this Vector3 v ) => Mathfs.GetDirAndMagnitude( v );

		/// <inheritdoc cref="Mathfs.ClampMagnitude(Vector2,float,float)"/>
		[MethodImpl( INLINE )] public static Vector2 ClampMagnitude( this Vector2 v, float min, float max ) => Mathfs.ClampMagnitude( v, min, max );

		/// <inheritdoc cref="Mathfs.ClampMagnitude(Vector3,float,float)"/>
		[MethodImpl( INLINE )] public static Vector3 ClampMagnitude( this Vector3 v, float min, float max ) => Mathfs.ClampMagnitude( v, min, max );

		#endregion

		#endregion


	}

}