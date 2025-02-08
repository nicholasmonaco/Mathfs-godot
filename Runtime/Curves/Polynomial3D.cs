// by Freya Holmér (https://github.com/FreyaHolmer/Mathfs)

using System;
using System.Runtime.CompilerServices;

using Vector2 = Godot.Vector2;
using Vector3 = Godot.Vector3;
using Bounds = Godot.Aabb;

namespace Freya {

	[Serializable]
	public struct Polynomial3D : IPolynomialCubic<Polynomial3D, Vector3>, IParamCurve3Diff<Vector3> {

		const MethodImplOptions INLINE = MethodImplOptions.AggressiveInlining;

		/// <inheritdoc cref="Polynomial.NaN"/>
		public static readonly Polynomial3D NaN = new Polynomial3D { x = Polynomial.NaN, y = Polynomial.NaN, z = Polynomial.NaN };

		public Polynomial x, y, z;

		public Polynomial3D( Polynomial x, Polynomial y, Polynomial z ) => ( this.x, this.y, this.z ) = ( x, y, z );

		/// <inheritdoc cref="Polynomial(float,float,float,float)"/>
		public Polynomial3D( Vector3 c0, Vector3 c1, Vector3 c2, Vector3 c3 ) {
			this.x = new Polynomial( c0.X, c1.X, c2.X, c3.X );
			this.y = new Polynomial( c0.Y, c1.Y, c2.Y, c3.Y );
			this.z = new Polynomial( c0.Z, c1.Z, c2.Z, c3.Z );
		}

		/// <inheritdoc cref="Polynomial(float,float,float)"/>
		public Polynomial3D( Vector3 c0, Vector3 c1, Vector3 c2 ) {
			this.x = new Polynomial( c0.X, c1.X, c2.X, 0 );
			this.y = new Polynomial( c0.Y, c1.Y, c2.Y, 0 );
			this.z = new Polynomial( c0.Z, c1.Z, c2.Z, 0 );
		}

		/// <inheritdoc cref="Polynomial(float,float)"/>
		public Polynomial3D( Vector3 c0, Vector3 c1 ) {
			this.x = new Polynomial( c0.X, c1.X, 0, 0 );
			this.y = new Polynomial( c0.Y, c1.Y, 0, 0 );
			this.z = new Polynomial( c0.Z, c1.Z, 0, 0 );
		}

		/// <inheritdoc cref="Polynomial(Matrix4x1)"/>
		public Polynomial3D( Vector3Matrix4x1 coefficients ) => ( x, y, z ) = ( new Polynomial( coefficients.X ), new Polynomial( coefficients.Y ), new Polynomial( coefficients.Z ) );

		/// <inheritdoc cref="Polynomial(Matrix4x1)"/>
		public Polynomial3D( Vector3Matrix3x1 coefficients ) => ( x, y, z ) = ( new Polynomial( coefficients.X ), new Polynomial( coefficients.Y ), new Polynomial( coefficients.Z ) );

		#region IPolynomialCubic

		public Vector3 C0 {
			[MethodImpl( INLINE )] get => new(x.c0, y.c0, z.c0);
			[MethodImpl( INLINE )] set => ( x.c0, y.c0, z.c0 ) = ( value.X, value.Y, value.Z );
		}
		public Vector3 C1 {
			[MethodImpl( INLINE )] get => new(x.c1, y.c1, z.c1);
			[MethodImpl( INLINE )] set => ( x.c1, y.c1, z.c1 ) = ( value.X, value.Y, value.Z );
		}
		public Vector3 C2 {
			[MethodImpl( INLINE )] get => new(x.c2, y.c2, z.c2);
			[MethodImpl( INLINE )] set => ( x.c2, y.c2, z.c2 ) = ( value.X, value.Y, value.Z );
		}
		public Vector3 C3 {
			[MethodImpl( INLINE )] get => new(x.c3, y.c3, z.c3);
			[MethodImpl( INLINE )] set => ( x.c3, y.c3, z.c3 ) = ( value.X, value.Y, value.Z );
		}

		public Polynomial this[ int i ] {
			get { return i switch { 0 => x, 1         => y, 2         => z, _         => throw new IndexOutOfRangeException( "Polynomial3D component index has to be either 0, 1, or 2" ) }; }
			set => _ = i switch { 0   => x = value, 1 => y = value, 2 => z = value, _ => throw new IndexOutOfRangeException() };
		}

		[MethodImpl( INLINE )] public Vector3 GetCoefficient( int degree ) =>
			degree switch {
				0 => C0,
				1 => C1,
				2 => C2,
				3 => C3,
				_ => throw new IndexOutOfRangeException( "Polynomial coefficient degree/index has to be between 0 and 3" )
			};

		[MethodImpl( INLINE )] public void SetCoefficient( int degree, Vector3 value ) {
			_ = degree switch {
				0 => C0 = value,
				1 => C1 = value,
				2 => C2 = value,
				3 => C3 = value,
				_ => throw new IndexOutOfRangeException( "Polynomial coefficient degree/index has to be between 0 and 3" )
			};
		}

		public Vector3 Eval( float t ) {
			float t2 = t * t;
			float t3 = t2 * t;
			return new Vector3(
				x.c3 * t3 + x.c2 * t2 + x.c1 * t + x.c0,
				y.c3 * t3 + y.c2 * t2 + y.c1 * t + y.c0,
				z.c3 * t3 + z.c2 * t2 + z.c1 * t + z.c0
			);
		}

		[MethodImpl( INLINE )] public Vector3 Eval( float t, int n ) => Differentiate( n ).Eval( t );

		[MethodImpl( INLINE )] public Polynomial3D Differentiate( int n = 1 ) => new(x.Differentiate( n ), y.Differentiate( n ), z.Differentiate( n ));

		public Polynomial3D ScaleParameterSpace( float factor ) {
			// ReSharper disable once CompareOfFloatsByEqualityOperator
			if( factor == 1f )
				return this;
			float factor2 = factor * factor;
			float factor3 = factor2 * factor;
			return new Polynomial3D(
				new Polynomial( x.c0, x.c1 / factor, x.c2 / factor2, x.c3 / factor3 ),
				new Polynomial( y.c0, y.c1 / factor, y.c2 / factor2, y.c3 / factor3 ),
				new Polynomial( z.c0, z.c1 / factor, z.c2 / factor2, z.c3 / factor3 )
			);
		}

		public Polynomial3D Compose( float g0, float g1 ) => new(x.Compose( g0, g1 ), y.Compose( g0, g1 ), z.Compose( g0, g1 ));

		#endregion

		/// <inheritdoc cref="Polynomial.FitCubicFrom0(float,float,float,float,float,float,float)"/>
		public static Polynomial3D FitCubicFrom0( float x1, float x2, float x3, Vector3 y0, Vector3 y1, Vector3 y2, Vector3 y3 ) {
			// precalcs
			float i12 = x2 - x1;
			float i13 = x3 - x1;
			float i23 = x3 - x2;
			float x1x2 = x1 * x2;
			float x1x3 = x1 * x3;
			float x2x3 = x2 * x3;
			float x1x2x3 = x1 * x2x3;
			float x0plusx1plusx2 = x1 + x2;
			float x0plusx1plusx3 = x1 + x3;
			float x2plusx3 = x2 + x3;
			float x1plusx2plusx3 = x1 + x2plusx3;
			float x1x2plusx1x3plusx2x3 = ( x1x2 + x1x3 + x2x3 );

			// scale factors
			Vector3 scl0 = y0 / -( x1 * x2 * x3 );
			Vector3 scl1 = y1 / +( x1 * i12 * i13 );
			Vector3 scl2 = y2 / -( x2 * i12 * i23 );
			Vector3 scl3 = y3 / +( x3 * i13 * i23 );

			// polynomial form
			Vector3 c0 = new(
				-( scl0.X * x1x2x3 ),
				-( scl0.Y * x1x2x3 ),
				-( scl0.Z * x1x2x3 )
			);
			Vector3 c1 = new(
				scl0.X * x1x2plusx1x3plusx2x3 + scl1.X * x2x3 + scl2.X * x1x3 + scl3.X * x1x2,
				scl0.Y * x1x2plusx1x3plusx2x3 + scl1.Y * x2x3 + scl2.Y * x1x3 + scl3.Y * x1x2,
				scl0.Z * x1x2plusx1x3plusx2x3 + scl1.Z * x2x3 + scl2.Z * x1x3 + scl3.Z * x1x2
			);
			Vector3 c2 = new(
				-( scl0.X * x1plusx2plusx3 + scl1.X * x2plusx3 + scl2.X * x0plusx1plusx3 + scl3.X * x0plusx1plusx2 ),
				-( scl0.Y * x1plusx2plusx3 + scl1.Y * x2plusx3 + scl2.Y * x0plusx1plusx3 + scl3.Y * x0plusx1plusx2 ),
				-( scl0.Z * x1plusx2plusx3 + scl1.Z * x2plusx3 + scl2.Z * x0plusx1plusx3 + scl3.Z * x0plusx1plusx2 )
			);
			Vector3 c3 = new(
				scl0.X + scl1.X + scl2.X + scl3.X,
				scl0.Y + scl1.Y + scl2.Y + scl3.Y,
				scl0.Z + scl1.Z + scl2.Z + scl3.Z
			);

			return new Polynomial3D( c0, c1, c2, c3 );
		}


		/// <inheritdoc cref="Polynomial2D.GetBounds01"/>
		public Bounds GetBounds01() => FloatRange.ToBounds( x.OutputRange01, y.OutputRange01, z.OutputRange01 );

		/// <inheritdoc cref="Polynomial.Split01"/>
		public (Polynomial3D pre, Polynomial3D post) Split01( float u ) {
			( Polynomial xPre, Polynomial xPost ) = x.Split01( u );
			( Polynomial yPre, Polynomial yPost ) = y.Split01( u );
			( Polynomial zPre, Polynomial zPost ) = z.Split01( u );
			return ( new Polynomial3D( xPre, yPre, zPre ), new Polynomial3D( xPost, yPost, zPost ) );
		}

		#region IParamCurve3Diff interface implementations

		public int Degree => Mathfs.Max( x.Degree, y.Degree, z.Degree );
		public Vector3 EvalDerivative( float t ) => Differentiate().Eval( t );
		public Vector3 EvalSecondDerivative( float t ) => Differentiate( 2 ).Eval( t );
		public Vector3 EvalThirdDerivative( float t = 0 ) => Differentiate( 3 ).Eval( 0 );

		#endregion

		#region Project Point

		/// <inheritdoc cref="Polynomial2D.ProjectPoint(Vector2,int,int)"/>
		public Vector3 ProjectPoint( Vector3 point, int initialSubdivisions = 16, int refinementIterations = 4 ) => ProjectPoint( point, out _, initialSubdivisions, refinementIterations );

		struct PointProjectSample {
			public float t;
			public float distDeltaSq;
			public Vector3 f;
			public Vector3 fp;
		}

		static PointProjectSample[] pointProjectGuesses = { default, default, default };

		/// <inheritdoc cref="Polynomial2D.ProjectPoint(Vector2,out float,int,int)"/>
		public Vector3 ProjectPoint( Vector3 point, out float t, int initialSubdivisions = 16, int refinementIterations = 4 ) {
			// define a bezier relative to the test point
			Polynomial3D curve = this;
			curve.x.c0 -= point.X; // constant coefficient defines the start position
			curve.y.c0 -= point.Y;
			curve.z.c0 -= point.Z;
			Vector3 curveStart = curve.Eval( 0 );
			Vector3 curveEnd = curve.Eval( 1 );

			PointProjectSample SampleDistSqDelta( float tSmp ) {
				PointProjectSample s = new PointProjectSample { t = tSmp };
				( s.f, s.fp ) = ( curve.Eval( tSmp ), curve.EvalDerivative( tSmp ) );
				s.distDeltaSq = s.f.Dot( s.fp );
				return s;
			}

			// find initial candidates
			int candidatesFound = 0;
			PointProjectSample prevSmp = SampleDistSqDelta( 0 );

			for( int i = 1; i < initialSubdivisions; i++ ) {
				float ti = i / ( initialSubdivisions - 1f );
				PointProjectSample smp = SampleDistSqDelta( ti );
				if( Mathfs.SignAsInt( smp.distDeltaSq ) != Mathfs.SignAsInt( prevSmp.distDeltaSq ) ) {
					pointProjectGuesses[candidatesFound++] = SampleDistSqDelta( ( prevSmp.t + smp.t ) / 2 );
					if( candidatesFound == 3 ) break; // no more than three possible candidates because of the polynomial degree
				}

				prevSmp = smp;
			}

			// refine each guess w. Newton-Raphson iterations
			void Refine( ref PointProjectSample smp ) {
				Vector3 fpp = curve.EvalSecondDerivative( smp.t );
				float tNew = smp.t - smp.f.Dot( smp.fp ) / ( smp.f.Dot( fpp ) + smp.fp.Dot( smp.fp ) );
				smp = SampleDistSqDelta( tNew );
			}

			for( int p = 0; p < candidatesFound; p++ )
				for( int i = 0; i < refinementIterations; i++ )
					Refine( ref pointProjectGuesses[p] );

			// Now find closest. First include the endpoints
			float sqDist0 = curveStart.LengthSquared(); // include endpoints
			float sqDist1 = curveEnd.LengthSquared();
			bool firstClosest = sqDist0 < sqDist1;
			float tClosest = firstClosest ? 0 : 1;
			Vector3 ptClosest = ( firstClosest ? curveStart : curveEnd ) + point;
			float distSqClosest = firstClosest ? sqDist0 : sqDist1;

			// then check internal roots
			for( int i = 0; i < candidatesFound; i++ ) {
				float pSqmag = pointProjectGuesses[i].f.LengthSquared();
				if( pSqmag < distSqClosest ) {
					distSqClosest = pSqmag;
					tClosest = pointProjectGuesses[i].t;
					ptClosest = pointProjectGuesses[i].f + point;
				}
			}

			t = tClosest;
			return ptClosest;
		}

		#endregion

		#region Typecasting & Operators

		public static Polynomial3D operator /( Polynomial3D p, float v ) => new(p.C0 / v, p.C1 / v, p.C2 / v, p.C3 / v);
		public static Polynomial3D operator *( Polynomial3D p, float v ) => new(p.C0 * v, p.C1 * v, p.C2 * v, p.C3 * v);
		public static Polynomial3D operator *( float v, Polynomial3D p ) => p * v;

		public override string ToString() {
			string s = "";
			s += x + "\n";
			s += y + "\n";
			s += z;
			return s;
		}

		public static explicit operator Polynomial2D( Polynomial3D p ) => new(p.x, p.y);
		public static explicit operator Vector3Matrix3x1( Polynomial3D poly ) => new(poly.C0, poly.C1, poly.C2);
		public static explicit operator Vector3Matrix4x1( Polynomial3D poly ) => new(poly.C0, poly.C1, poly.C2, poly.C3);
		public static explicit operator BezierQuad3D( Polynomial3D poly ) => poly.Degree < 3 ? new BezierQuad3D( CharMatrix.quadraticBezierInverse * (Vector3Matrix3x1)poly ) : throw new InvalidCastException( "Cannot cast a cubic polynomial to a quadratic curve" );
		public static explicit operator BezierCubic3D( Polynomial3D poly ) => new(CharMatrix.cubicBezierInverse * (Vector3Matrix4x1)poly);
		public static explicit operator CatRomCubic3D( Polynomial3D poly ) => new(CharMatrix.cubicCatmullRomInverse * (Vector3Matrix4x1)poly);
		public static explicit operator HermiteCubic3D( Polynomial3D poly ) => new(CharMatrix.cubicHermiteInverse * (Vector3Matrix4x1)poly);
		public static explicit operator UBSCubic3D( Polynomial3D poly ) => new(CharMatrix.cubicUniformBsplineInverse * (Vector3Matrix4x1)poly);

		#endregion

	}

}