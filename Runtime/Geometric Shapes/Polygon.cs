﻿// by Freya Holmér (https://github.com/FreyaHolmer/Mathfs)

using System;
using System.Collections.Generic;

using Vector2 = Godot.Vector2;
using Rect = Godot.Rect2;

using static Freya.Mathfs;

namespace Freya {

	/// <summary>Polygon with various math functions to test if a point is inside, calculate area, etc.</summary>
	public class Polygon {

		/// <summary>The points in this polygon</summary>
		public IReadOnlyList<Vector2> points;

		/// <summary>Creates a new 2D polygon</summary>
		/// <param name="points">The points in the polygon</param>
		public Polygon( IReadOnlyList<Vector2> points ) => this.points = points;

		/// <summary>Get a point by index. Indices cannot be out of range, as they will wrap/cycle in the polygon</summary>
		/// <param name="i">The index of the point</param>
		public Vector2 this[ int i ] => points[i.Mod( Count )];

		/// <summary>The number of points in this polygon</summary>
		public int Count => points.Count;

		/// <summary>Returns whether or not this polygon is defined clockwise</summary>
		public bool IsClockwise => SignedArea > 0;

		/// <summary>Returns the area of this polygon</summary>
		public float Area => MathF.Abs( SignedArea );

		/// <summary>Returns the signed area of this polygon</summary>
		public float SignedArea {
			get {
				int count = points.Count;
				float sum = 0f;
				for( int i = 0; i < count; i++ ) {
					Vector2 a = points[i];
					Vector2 b = points[( i + 1 ) % count];
					sum += ( b.X - a.X ) * ( b.Y + a.Y );
				}

				return sum * 0.5f;
			}
		}

		/// <summary>Returns the length of the perimeter of the polygon</summary>
		public float Perimeter {
			get {
				int count = points.Count;
				float totalDist = 0f;
				for( int i = 0; i < count; i++ ) {
					Vector2 a = points[i];
					Vector2 b = points[( i + 1 ) % count];
					float dx = a.X - b.X;
					float dy = a.Y - b.Y;
					totalDist += MathF.Sqrt( dx * dx + dy * dy ); // unrolled for speed
				}

				return totalDist;
			}
		}

		/// <summary>Returns the axis-aligned bounding box of this polygon</summary>
		public Rect Bounds {
			get {
				int count = points.Count;
				Vector2 p = points[0];
				float xMin = p.X, xMax = p.X, yMin = p.Y, yMax = p.Y;
				for( int i = 1; i < count; i++ ) {
					p = points[i];
					xMin = MathF.Min( xMin, p.X );
					xMax = MathF.Max( xMax, p.X );
					yMin = MathF.Min( yMin, p.Y );
					yMax = MathF.Max( yMax, p.Y );
				}

				return new Rect( xMin, yMin, xMax - xMin, yMax - yMin );
			}
		}

		/// <summary>Returns whether or not a point is inside the polygon</summary>
		/// <param name="point">The point to test and see if it's inside</param>
		public bool Contains( Vector2 point ) => WindingNumber( point ) != 0;

		// modified version of the code from here:
		// http://softsurfer.com/Archive/algorithm_0103/algorithm_0103.htm
		// Copyright 2000 softSurfer, 2012 Dan Sunday. This code may be freely used and modified for any purpose providing that this copyright notice is included with it. SoftSurfer makes no warranty for this code, and cannot be held liable for any real or imagined damage resulting from its use. Users of this code must verify correctness for their application.
		/// <summary>Returns the winding number for this polygon, around a given point</summary>
		/// <param name="point">The point to check winding around</param>
		public int WindingNumber( Vector2 point ) {
			int winding = 0;
			float IsLeft( Vector2 a, Vector2 b, Vector2 p ) => SignWithZero( Determinant( a.To( p ), a.To( b ) ) );

			int count = points.Count;
			for( int i = 0; i < count; i++ ) {
				int iNext = ( i + 1 ) % count;
				if( points[i].Y <= point.Y ) {
					if( points[iNext].Y > point.Y && IsLeft( points[i], points[iNext], point ) > 0 )
						winding--;
				} else {
					if( points[iNext].Y <= point.Y && IsLeft( points[i], points[iNext], point ) < 0 )
						winding++;
				}
			}

			return winding;
		}

		/// <summary>Returns the resulting polygons when clipping this polygon by a line</summary>
		/// <param name="line">The line/plane to clip by. Points on its left side will be kept</param>
		/// <param name="clippedPolygons">The resulting array of clipped polygons (if any)</param>
		public PolygonClipper.ResultState Clip( Line2D line, out List<Polygon> clippedPolygons ) => PolygonClipper.Clip( this, line, out clippedPolygons );

		public Polygon GetMiterPolygon( float offset ) {
			List<Vector2> miterPts = new List<Vector2>();

			Line2D GetMiterLine( int i ) {
				Vector2 tangent = ( this[i + 1] - this[i] ).Normalized();
				Vector2 normal = tangent.Rotate90CCW();
				return new Line2D( this[i] + normal * offset, tangent );
			}

			// Line2D prev = GetMiterLine( -1 );
			for( int i = 0; i < Count; i++ ) {
				Line2D line = GetMiterLine( i );
				Line2D line2 = GetMiterLine( i + 1 );
				if( line.Intersect( line2, out Vector2 pt ) )
					miterPts.Add( pt );
				else {
					Godot.GD.PrintErr( $"{line.origin},{line.dir}\n{line2.origin},{line2.dir}\nPoints:{string.Join( '\n', points )}" );
					throw new Exception( "Line intersection failed" );
				}
				// prev = line;
			}

			return new Polygon( miterPts );
		}

		// from: https://en.wikipedia.org/wiki/Centroid
		/// <summary>The centroid of this polygon, also known as the center of mass</summary>
		public Vector2 Centroid {
			get {
				Vector2 centroid = Vector2.Zero;
				float signedArea = 0;
				for( int i = 0; i < Count; i++ ) {
					Vector2 a = points[i];
					Vector2 b = points[( i + 1 ) % Count];
					float det = a.X * b.Y - b.X * a.Y;
					signedArea += det;
					centroid.X += ( a.X + b.X ) * det;
					centroid.Y += ( b.Y + a.Y ) * det;
				}
				return centroid / ( 3 * signedArea );
			}
		}

		public Vector2 WeightedEdgeCenter {
			get {
				Vector2 eCenter = Vector2.Zero;
				float totalLength = 0;
				for( int i = 0; i < Count; i++ ) {
					Vector2 a = points[i];
					Vector2 b = points[( i + 1 ) % Count];
					float length = a.DistanceTo( b );
					totalLength += length;
					eCenter += ( a + b ) * length;
				}
				return eCenter / ( 2 * totalLength );
			}
		}

	}

}