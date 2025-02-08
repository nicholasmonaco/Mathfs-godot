// by Freya Holmér (https://github.com/FreyaHolmer/Mathfs)
// Do not manually edit - this file is generated by MathfsCodegen.cs

using System;
using System.Runtime.CompilerServices;

using Godot;

namespace Freya {

	/// <summary>An optimized uniform 4D Cubic catmull-rom segment, with 4 control points</summary>
	[Serializable] public struct CatRomCubic4D : IParamSplineSegment<Polynomial4D,Vector4Matrix4x1> {

		const MethodImplOptions INLINE = MethodImplOptions.AggressiveInlining;

		[Export] Vector4Matrix4x1 pointMatrix;
		[NonSerialized] Polynomial4D curve;
		[NonSerialized] bool validCoefficients;

		/// <summary>Creates a uniform 4D Cubic catmull-rom segment, from 4 control points</summary>
		/// <param name="p0">The first control point of the catmull-rom curve. Note that this point is not included in the curve itself, and only helps to shape it</param>
		/// <param name="p1">The second control point, and the start of the catmull-rom curve</param>
		/// <param name="p2">The third control point, and the end of the catmull-rom curve</param>
		/// <param name="p3">The last control point of the catmull-rom curve. Note that this point is not included in the curve itself, and only helps to shape it</param>
		public CatRomCubic4D( Vector4 p0, Vector4 p1, Vector4 p2, Vector4 p3 ) : this(new Vector4Matrix4x1(p0, p1, p2, p3)){}
		/// <summary>Creates a uniform 4D Cubic catmull-rom segment, from 4 control points</summary>
		/// <param name="pointMatrix">The matrix containing the control points of this spline</param>
		public CatRomCubic4D( Vector4Matrix4x1 pointMatrix ) => (this.pointMatrix,curve,validCoefficients) = (pointMatrix,default,false);

		public Polynomial4D Curve {
			get {
				if( validCoefficients )
					return curve; // no need to update
				validCoefficients = true;
				return curve = new Polynomial4D(
					P1,
					(-P0+P2)/2,
					P0-(5/2f)*P1+2*P2-(1/2f)*P3,
					-(1/2f)*P0+(3/2f)*P1-(3/2f)*P2+(1/2f)*P3
				);
			}
		}
		public Vector4Matrix4x1 PointMatrix {[MethodImpl( INLINE )] get => pointMatrix; [MethodImpl( INLINE )] set => _ = ( pointMatrix = value, validCoefficients = false ); }
		/// <summary>The first control point of the catmull-rom curve. Note that this point is not included in the curve itself, and only helps to shape it</summary>
		public Vector4 P0{ [MethodImpl( INLINE )] get => pointMatrix.m0; [MethodImpl( INLINE )] set => _ = ( pointMatrix.m0 = value, validCoefficients = false ); }
		/// <summary>The second control point, and the start of the catmull-rom curve</summary>
		public Vector4 P1{ [MethodImpl( INLINE )] get => pointMatrix.m1; [MethodImpl( INLINE )] set => _ = ( pointMatrix.m1 = value, validCoefficients = false ); }
		/// <summary>The third control point, and the end of the catmull-rom curve</summary>
		public Vector4 P2{ [MethodImpl( INLINE )] get => pointMatrix.m2; [MethodImpl( INLINE )] set => _ = ( pointMatrix.m2 = value, validCoefficients = false ); }
		/// <summary>The last control point of the catmull-rom curve. Note that this point is not included in the curve itself, and only helps to shape it</summary>
		public Vector4 P3{ [MethodImpl( INLINE )] get => pointMatrix.m3; [MethodImpl( INLINE )] set => _ = ( pointMatrix.m3 = value, validCoefficients = false ); }
		/// <summary>Get or set a control point position by index. Valid indices from 0 to 3</summary>
		public Vector4 this[ int i ] {
			get => i switch { 0 => P0, 1 => P1, 2 => P2, 3 => P3, _ => throw new ArgumentOutOfRangeException( nameof(i), $"Index has to be in the 0 to 3 range, and I think {i} is outside that range you know" ) };
			set { switch( i ){ case 0: P0 = value; break; case 1: P1 = value; break; case 2: P2 = value; break; case 3: P3 = value; break; default: throw new ArgumentOutOfRangeException( nameof(i), $"Index has to be in the 0 to 3 range, and I think {i} is outside that range you know" ); }}
		}
		public static bool operator ==( CatRomCubic4D a, CatRomCubic4D b ) => a.pointMatrix == b.pointMatrix;
		public static bool operator !=( CatRomCubic4D a, CatRomCubic4D b ) => !( a == b );
		public bool Equals( CatRomCubic4D other ) => P0.Equals( other.P0 ) && P1.Equals( other.P1 ) && P2.Equals( other.P2 ) && P3.Equals( other.P3 );
		public override bool Equals( object obj ) => obj is CatRomCubic4D other && pointMatrix.Equals( other.pointMatrix );
		public override int GetHashCode() => pointMatrix.GetHashCode();
		public override string ToString() => $"({pointMatrix.m0}, {pointMatrix.m1}, {pointMatrix.m2}, {pointMatrix.m3})";

		public static explicit operator BezierCubic4D( CatRomCubic4D s ) =>
			new BezierCubic4D(
				s.P1,
				-(1/6f)*s.P0+s.P1+(1/6f)*s.P2,
				(1/6f)*s.P1+s.P2-(1/6f)*s.P3,
				s.P2
			);
		public static explicit operator HermiteCubic4D( CatRomCubic4D s ) =>
			new HermiteCubic4D(
				s.P1,
				(-s.P0+s.P2)/2,
				s.P2,
				(-s.P1+s.P3)/2
			);
		public static explicit operator UBSCubic4D( CatRomCubic4D s ) =>
			new UBSCubic4D(
				(7/6f)*s.P0-(2/3f)*s.P1+(5/6f)*s.P2-(1/3f)*s.P3,
				-(1/3f)*s.P0+(11/6f)*s.P1-(2/3f)*s.P2+(1/6f)*s.P3,
				(1/6f)*s.P0-(2/3f)*s.P1+(11/6f)*s.P2-(1/3f)*s.P3,
				-(1/3f)*s.P0+(5/6f)*s.P1-(2/3f)*s.P2+(7/6f)*s.P3
			);
		/// <summary>Returns a linear blend between two catmull-rom curves</summary>
		/// <param name="a">The first spline segment</param>
		/// <param name="b">The second spline segment</param>
		/// <param name="t">A value from 0 to 1 to blend between <c>a</c> and <c>b</c></param>
		public static CatRomCubic4D Lerp( CatRomCubic4D a, CatRomCubic4D b, float t ) =>
			new(
				CoreUtil.LerpUnclamped( a.P0, b.P0, t ),
				CoreUtil.LerpUnclamped( a.P1, b.P1, t ),
				CoreUtil.LerpUnclamped( a.P2, b.P2, t ),
				CoreUtil.LerpUnclamped( a.P3, b.P3, t )
			);
	}
}
