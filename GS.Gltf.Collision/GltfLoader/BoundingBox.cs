using System;
using System.Collections.Generic;
using System.Text;
using glTFLoader.Schema;
using glTFLoader;
using System.Linq;
using System.Numerics;
using GS.Gltf.Collision.SharpGltf;

namespace GS.Gltf.Collision
{
    public class BoundingBox
    {
        private const int VECTOR_3D_DIMENSION = 3;
        private const int X_DIM = 0;
        private const int Y_DIM = 1;
        private const int Z_DIM = 2;

        public float[] Min = new float[VECTOR_3D_DIMENSION];
        public float[] Max = new float[VECTOR_3D_DIMENSION];

        public Vector3 MinV;
        public Vector3 MaxV;

        public BoundingBox(List<Accessor> accessors)
        {
            var maxVectors = new List<float[]>();
            var minVectors = new List<float[]>();

            foreach (var accessor in accessors)
            {
                maxVectors.Add(accessor.Max);
                minVectors.Add(accessor.Min);
            }

            for (int dim = 0; dim < VECTOR_3D_DIMENSION; dim++)
            {
                Min[dim] = minVectors.Min(v => v[dim]);
                Max[dim] = minVectors.Max(v => v[dim]);
            }

            MaxV.X = Max[X_DIM];
            MaxV.Y = Max[Y_DIM];
            MaxV.Z = Max[Z_DIM];

            MinV.X = Min[X_DIM];
            MinV.Y = Min[Y_DIM];
            MinV.Z = Min[Z_DIM];
        }

        public BoundingBox(Accessor accessor)
        {
            MaxV.X = accessor.Max[X_DIM];
            MaxV.Y = accessor.Max[Y_DIM];
            MaxV.Z = accessor.Max[Z_DIM];

            MinV.X = accessor.Min[X_DIM];
            MinV.Y = accessor.Min[Y_DIM];
            MinV.Z = accessor.Min[Z_DIM];
        }

        public BoundingBox(float[] max, float[] min)
        {
            MaxV.X = max[X_DIM];
            MaxV.Y = max[Y_DIM];
            MaxV.Z = max[Z_DIM];

            MinV.X = min[X_DIM];
            MinV.Y = min[Y_DIM];
            MinV.Z = min[Z_DIM];
        }

        public bool IsCollideWith(BoundingBox other)
        {
            float delta = CollisionConstants.Tolerance;
            return
            (Math.Max(this.MaxV.X, other.MaxV.X) - Math.Min(this.MinV.X, other.MinV.X) + delta < this.MaxV.X - this.MinV.X + other.MaxV.X - other.MinV.X) &&
            (Math.Max(this.MaxV.Y, other.MaxV.Y) - Math.Min(this.MinV.Y, other.MinV.Y) + delta < this.MaxV.Y - this.MinV.Y + other.MaxV.Y - other.MinV.Y) &&
            (Math.Max(this.MaxV.Z, other.MaxV.Z) - Math.Min(this.MinV.Z, other.MinV.Z) + delta < this.MaxV.Z - this.MinV.Z + other.MaxV.Z - other.MinV.Z);
        }
    }
}
