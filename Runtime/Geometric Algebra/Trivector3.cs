using Vector3 = Godot.Vector3;

namespace Freya {

	public struct Trivector3 {
		public static readonly Trivector3 zero = new Trivector3( 0 );
		public float xyz;
		public Trivector3( float xyz ) => this.xyz = xyz;
		public static Trivector3 operator +( Trivector3 a, Trivector3 b ) => new Trivector3( a.xyz + b.xyz );
		public static Trivector3 operator *( Trivector3 a, float b ) => new Trivector3( a.xyz * b );
		public static Trivector3 operator *( float a, Trivector3 b ) => b * a;

		public static Bivector3 operator *( Trivector3 a, Vector3 b ) => new Bivector3( a.xyz * b.X, a.xyz * b.Y, a.xyz * b.Z );
		public static Bivector3 operator *( Vector3 a, Trivector3 b ) => new Bivector3( a.X * b.xyz, a.Y * b.xyz, a.Z * b.xyz );
		public static Vector3 operator *( Bivector3 a, Trivector3 b ) => new Vector3( -a.yz * b.xyz, -a.zx * b.xyz, -a.xy * b.xyz );
		public static Vector3 operator *( Trivector3 a, Bivector3 b ) => new Vector3( -a.xyz * b.yz, -a.xyz * b.zx, -a.xyz * b.xy );

		public static float operator *( Trivector3 a, Trivector3 b ) => -a.xyz * b.xyz;

	}

}