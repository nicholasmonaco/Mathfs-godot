using Godot;

namespace Freya {
    public struct Pose {
        public Vector3 Position;
        public Quaternion Rotation;

        public Pose(Vector3 position, Quaternion rotation) {
            this.Position = position;
            this.Rotation = rotation;
        }
    }
}