using glTFLoader.Schema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("GS.Gltf.Collision.Tests")]
namespace GS.Gltf.Collision
{
    
    public class BoundingBox
    {
        private const int VECTOR_3D_DIMENSION = 3;
        private const int X_DIM = 0;
        private const int Y_DIM = 1;
        private const int Z_DIM = 2;

        //delete 
        private float[] Min = new float[VECTOR_3D_DIMENSION];
        private float[] Max = new float[VECTOR_3D_DIMENSION];

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

        public BoundingBox(List<Vector3> points)
        {
            var Xs = new List<float>();
            var Ys = new List<float>();
            var Zs = new List<float>();

            foreach (var point in points)
            {
                Xs.Add(point.X);
                Ys.Add(point.Y);
                Zs.Add(point.Z);
            }

            MaxV.X = Xs.Max();
            MaxV.Y = Ys.Max();
            MaxV.Z = Zs.Max();

            MinV.X = Xs.Min();
            MinV.Y = Ys.Min();
            MinV.Z = Zs.Min();
        }

        internal BoundingBox(Accessor accessor)
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

        public BoundingBox(Vector3 max, Vector3 min)
        {
            MaxV = max;
            MinV = min;
        }

        internal bool IsCollideWith(BoundingBox other)
        {
            float delta = CollisionConstants.Tolerance;
            return
            (Math.Max(this.MaxV.X, other.MaxV.X) - Math.Min(this.MinV.X, other.MinV.X) + delta < this.MaxV.X - this.MinV.X + other.MaxV.X - other.MinV.X) &&
            (Math.Max(this.MaxV.Y, other.MaxV.Y) - Math.Min(this.MinV.Y, other.MinV.Y) + delta < this.MaxV.Y - this.MinV.Y + other.MaxV.Y - other.MinV.Y) &&
            (Math.Max(this.MaxV.Z, other.MaxV.Z) - Math.Min(this.MinV.Z, other.MinV.Z) + delta < this.MaxV.Z - this.MinV.Z + other.MaxV.Z - other.MinV.Z);
        }

        internal BoundingBox GetCollisionBoundingBox(BoundingBox other)
        {
            if (this.MaxV == other.MaxV && this.MinV == other.MinV)
            {
                return new BoundingBox(MaxV, MinV); //equal BB
            }
            if ((this.MaxV.X > other.MaxV.X) && (this.MaxV.Y > other.MaxV.Y) && (this.MaxV.Z > other.MaxV.Z) &&
               (this.MinV.X < other.MinV.X) && (this.MinV.Y < other.MinV.Y) && (this.MinV.Z < other.MinV.Z))
            {
                return new BoundingBox(this.MaxV, this.MinV); //this include other
            }
            if ((other.MaxV.X >= this.MaxV.X) && (other.MaxV.Y >= this.MaxV.Y) && (other.MaxV.Z >= this.MaxV.Z) &&
               (other.MinV.X <= this.MinV.X) && (other.MinV.Y <= this.MinV.Y) && (other.MinV.Z <= this.MinV.Z))
            {
                return new BoundingBox(this.MaxV, this.MinV); //other include this
            }
            var firstDist = Vector3.Distance(this.MaxV, other.MinV);
            var secondDist = Vector3.Distance(this.MinV, other.MaxV);

            if (firstDist >= secondDist)
            {
                return new BoundingBox(other.MaxV,this.MinV);
            }
            else
            {
                return new BoundingBox(this.MaxV, other.MinV);
            }

        }

        internal BoundingBox GetBigCollisionBoundingBox(BoundingBox other)
        {
            if (this.MaxV == other.MaxV && this.MinV == other.MinV)
            {
                return new BoundingBox(MaxV, MinV); //equal BB
            }

            var MaxMax = Vector3.Max(this.MaxV, other.MaxV);
            var MinMin = Vector3.Min(this.MinV, other.MinV);

            return new BoundingBox(MaxMax, MinMin);
        }
    }
}
