using System;

using Vector2 = Godot.Vector2;
using Vector3 = Godot.Vector3;

namespace Freya {

	/// <summary>An orthonormal affine 2D transformation</summary>
	[Serializable]
	public struct Transform2D {

		public float origin_x, origin_y;
		public float axisX_x, axisX_y;

		public Vector2 Origin {
			get => new(origin_x, origin_y);
			set => ( origin_x, origin_y ) = ( value.X, value.Y );
		}
		public Vector2 AxisX {
			get => new(axisX_x, axisX_y);
			set => ( axisX_x, axisX_y ) = ( value.X, value.Y );
		}
		public Vector2 AxisY {
			get => new(AxisY_x, AxisY_y);
			set => ( axisX_x, axisX_y ) = ( +value.Y, -value.X );
		}
		public float AxisY_x => -axisX_y;
		public float AxisY_y => +axisX_x;

		/// <summary>Creates an orthonormal affine 2D transformation</summary>
		public Transform2D( Vector2 origin, Vector2 axisX ) {
			this.origin_x = origin.X;
			this.origin_y = origin.Y;
			this.axisX_x = axisX.X;
			this.axisX_y = axisX.Y;
		}

		/// <summary>Transforms a local point to a world space point</summary>
		/// <param name="pt">The local space point to transform</param>
		public Vector2 TransformPoint( Vector2 pt ) {
			return new( // unrolled for performance
				origin_x + axisX_x * pt.X + AxisY_x * pt.Y,
				origin_y + axisX_y * pt.X + AxisY_y * pt.Y
			);
		}

		/// <inheritdoc cref="TransformPoint(Vector2)"/>
		public Vector2 TransformPoint( float x, float y ) {
			return new( // unrolled for performance
				origin_x + axisX_x * x + AxisY_x * y,
				origin_y + axisX_y * x + AxisY_y * y
			);
		}

		/// <summary>Transforms a local vector to a world space vector, not taking position into account</summary>
		/// <param name="vec">The local space vector to transform</param>
		public Vector2 TransformVector( Vector2 vec ) {
			return new( // unrolled for performance
				axisX_x * vec.X + AxisY_x * vec.Y,
				axisX_y * vec.X + AxisY_y * vec.Y
			);
		}

		/// <inheritdoc cref="TransformVector(Vector2)"/>
		public Vector2 TransformVector( float x, float y ) {
			return new( // unrolled for performance
				axisX_x * x + AxisY_x * y,
				axisX_y * x + AxisY_y * y
			);
		}

		/// <summary>Transform a world space point to a local point</summary>
		/// <param name="pt">World space point</param>
		public Vector2 InverseTransformPoint( Vector2 pt ) {
			float rx = pt.X - origin_x;
			float ry = pt.Y - origin_y;
			return new(
				axisX_x * rx + axisX_y * ry,
				AxisY_x * rx + AxisY_y * ry
			);
		}

		/// <summary>Transform a world space vector to a local vector</summary>
		/// <param name="vec">World space vector</param>
		public Vector2 InverseTransformVector( Vector2 vec ) {
			return new(
				axisX_x * vec.X + axisX_y * vec.Y,
				AxisY_x * vec.X + AxisY_y * vec.Y
			);
		}

		public static Transform2D operator *( Transform2D a, Transform2D b ) {
			return new Transform2D(
				a.TransformPoint( b.origin_x, b.origin_y ),
				a.TransformVector( b.axisX_x, b.axisX_y )
			);
		}

	}

}