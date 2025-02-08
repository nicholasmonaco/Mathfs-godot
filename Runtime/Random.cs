// by Freya Holmér (https://github.com/FreyaHolmer/Mathfs)

using System;

using static Freya.Mathfs;

using Vector2 = Godot.Vector2;
using Vector3 = Godot.Vector3;

namespace Freya {

	/// <summary>Various methods for generating random stuff (like, actually things of the category randomization, not, "various items")</summary>
	public static class Random {

		private static System.Random InternalRandom = new System.Random();

		// 1D
		/// <summary>Returns a random value between 0 and 1</summary>
		public static float Value => InternalRandom.NextSingle();

		/// <summary>Randomly returns either -1 or 1</summary>
		public static float Sign => Value > 0.5f ? 1f : -1f;

		/// <summary>Randomly returns either -1 or 1, equivalent to <c>Random.Sign</c></summary>
		public static float Direction1D => Sign;

		/// <summary>Randomly returns a value between <c>min</c> [inclusive] and <c>max</c> [inclusive]</summary>
		/// <param name="min">The minimum value [inclusive] </param>
		/// <param name="max">The maximum value [inclusive]</param>
		public static float Range( float min, float max ) => InternalRandom.NextSingle() * (max - min) + min;
		
		/// <summary>Randomly returns a value between <c>min</c> [inclusive] and <c>max</c> [exclusive]</summary>
		/// <param name="min">The minimum value [inclusive]</param>
		/// <param name="max">The maximum value [exclusive]</param>
		public static int Range( int min, int max ) => InternalRandom.Next() * (max - min) + min;

		// 2D
		/// <summary>Returns a random point on the unit circle</summary>
		public static Vector2 OnUnitCircle => AngToDir( Value * TAU );

		/// <summary>Returns a random 2D direction, equivalent to <c>OnUnitCircle</c></summary>
		public static Vector2 Direction2D => OnUnitCircle;

		/// <summary>Returns a random point inside the unit circle</summary>
		public static Vector2 InUnitCircle { get {
			float angle = InternalRandom.NextSingle() * MathF.Tau;
            return new Vector2(MathF.Sin(angle), MathF.Cos(angle));
		} }

		/// <summary>Returns a random point inside the unit square. Values are between 0 to 1</summary>
		public static Vector2 InUnitSquare => new Vector2( Value, Value );


		// 3D

		// Godot version of OnUnitSphere from:
        // https://github.com/XJINE/Unity_RandomEx/blob/master/Assets/Packages/Extensions/RandomEx/RandomEx.cs
		/// <summary>Returns a random point on the unit sphere</summary>
		public static Vector3 OnUnitSphere { get {
			float angle1 = InternalRandom.NextSingle() * MathF.Tau;
            float angle2 = InternalRandom.NextSingle() * MathF.Tau;
            return new Vector3(
                MathF.Sin(angle1) * MathF.Cos(angle2),
                MathF.Sin(angle1) * MathF.Sin(angle2),
                MathF.Cos(angle1)
            );
		} }

		/// <summary>Returns a random 3D direction, equivalent to <c>OnUnitSphere</c></summary>
		public static Vector3 Direction3D => OnUnitSphere;

		/// <summary>Returns a random point inside the unit sphere</summary>
		public static Vector3 InUnitSphere { get {
			float angle = InternalRandom.NextSingle() * MathF.Tau;
            return new Vector3(MathF.Sin(angle), MathF.Cos(angle), MathF.Sin(InternalRandom.NextSingle())).Normalized();
		} }

		/// <summary>Returns a random point inside the unit cube. Values are between 0 to 1</summary>
		public static Vector3 InUnitCube => new Vector3( Value, Value, Value );

		// 2D orientation
		/// <summary>Returns a random angle in radians from 0 to TAU</summary>
		public static float Angle => Value * TAU;

		// 3D Orientation
		/// <summary>Returns a random uniformly distributed rotation</summary>
		// public static Quaternion Rotation => NRandom.RandomUniformRotation(); // .rotationUniform;
	}

}