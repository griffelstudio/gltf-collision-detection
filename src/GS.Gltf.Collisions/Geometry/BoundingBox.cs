using glTFLoader.Schema;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("GS.Gltf.Collisions.Tests")]
namespace GS.Gltf.Collisions
{

    public class BoundingBox
    {
        private const int X_DIM = 0;
        private const int Y_DIM = 1;
        private const int Z_DIM = 2;

        public Vector3 Min;
        public Vector3 Max;

        public BoundingBox(List<Vector3> points)
        {
            Max.X = Min.X = points[0].X;
            Max.Y = Min.Y = points[0].Y;
            Max.Z = Min.Z = points[0].Z;

            for (int i = 1; i < points.Count; i++)
            {
                if (points[i].X > Max.X)
                    Max.X = points[i].X;

                if (points[i].Y > Max.Y)
                    Max.Y = points[i].Y;

                if (points[i].Z > Max.Z)
                    Max.Z = points[i].Z;

                if (points[i].X < Min.X)
                    Min.X = points[i].X;

                if (points[i].Y < Min.Y)
                    Min.Y = points[i].Y;

                if (points[i].Z < Min.Z)
                    Min.Z = points[i].Z;
            }
        }

        internal BoundingBox(Accessor accessor)
        {
            Max.X = accessor.Max[X_DIM];
            Max.Y = accessor.Max[Y_DIM];
            Max.Z = accessor.Max[Z_DIM];

            Min.X = accessor.Min[X_DIM];
            Min.Y = accessor.Min[Y_DIM];
            Min.Z = accessor.Min[Z_DIM];
        }

        public BoundingBox(float[] max, float[] min)
        {
            Max.X = max[X_DIM];
            Max.Y = max[Y_DIM];
            Max.Z = max[Z_DIM];

            Min.X = min[X_DIM];
            Min.Y = min[Y_DIM];
            Min.Z = min[Z_DIM];
        }

        public BoundingBox(Vector3 max, Vector3 min)
        {
            Max = max;
            Min = min;
        }

        // TODO Decide if it's needed.
        internal bool IsCollideWith2(BoundingBox other)
        {
            float delta = CollisionConstants.Tolerance;

            var a = Math.Max(this.Max.X, other.Max.X) - Math.Min(this.Min.X, other.Min.X) + delta < this.Max.X - this.Min.X + other.Max.X - other.Min.X;
            var b = (Math.Max(this.Max.Y, other.Max.Y) - Math.Min(this.Min.Y, other.Min.Y) + delta < this.Max.Y - this.Min.Y + other.Max.Y - other.Min.Y);
            var c = (Math.Max(this.Max.Z, other.Max.Z) - Math.Min(this.Min.Z, other.Min.Z) + delta < this.Max.Z - this.Min.Z + other.Max.Z - other.Min.Z);

            return
                (Math.Max(this.Max.X, other.Max.X) - Math.Min(this.Min.X, other.Min.X) + delta < this.Max.X - this.Min.X + other.Max.X - other.Min.X) &&
                (Math.Max(this.Max.Y, other.Max.Y) - Math.Min(this.Min.Y, other.Min.Y) + delta < this.Max.Y - this.Min.Y + other.Max.Y - other.Min.Y) &&
                (Math.Max(this.Max.Z, other.Max.Z) - Math.Min(this.Min.Z, other.Min.Z) + delta < this.Max.Z - this.Min.Z + other.Max.Z - other.Min.Z);
        }

        internal bool IsCollideWith(BoundingBox other, float delta)
        {
            var a = this.Min.X + delta <= other.Max.X;
            var b = this.Max.X >= other.Min.X + delta;
            var c = this.Min.Y <= other.Max.Y;
            var d = this.Max.Y >= other.Min.Y;
            var f = this.Min.Z <= other.Max.Z;
            var e = this.Max.Z >= other.Min.Z;


            return (this.Min.X + delta <= other.Max.X  && this.Max.X >= other.Min.X + delta) &&
                (this.Min.Y + delta <= other.Max.Y + delta && this.Max.Y >= other.Min.Y + delta) &&
                (this.Min.Z + delta <= other.Max.Z + delta && this.Max.Z >= other.Min.Z + delta);
        }

        internal BoundingBox GetCollisionBoundingBox(BoundingBox other)
        {
            if (this.Max == other.Max && this.Min == other.Min)
            {
                return new BoundingBox(Max, Min); //equal BB
            }
            if ((this.Max.X > other.Max.X) && (this.Max.Y > other.Max.Y) && (this.Max.Z > other.Max.Z) &&
               (this.Min.X < other.Min.X) && (this.Min.Y < other.Min.Y) && (this.Min.Z < other.Min.Z))
            {
                return new BoundingBox(this.Max, this.Min); //this include other
            }
            if ((other.Max.X >= this.Max.X) && (other.Max.Y >= this.Max.Y) && (other.Max.Z >= this.Max.Z) &&
               (other.Min.X <= this.Min.X) && (other.Min.Y <= this.Min.Y) && (other.Min.Z <= this.Min.Z))
            {
                return new BoundingBox(this.Max, this.Min); //other include this
            }
            var firstDist = Vector3.Distance(this.Max, other.Min);
            var secondDist = Vector3.Distance(this.Min, other.Max);

            Vector3 COLLISION_MIN_ALLIGN = new Vector3(-0.01f, -0.01f, -0.01f);
            Vector3 COLLISION_MAX_ALLIGN = new Vector3(0.01f, 0.01f, 0.01f);


            var othMin = Vector3.Add(other.Min, COLLISION_MIN_ALLIGN);
            var othMax = Vector3.Add(other.Max, COLLISION_MAX_ALLIGN);

            var min = Vector3.Add(this.Min, COLLISION_MIN_ALLIGN);
            var max = Vector3.Add(this.Max, COLLISION_MAX_ALLIGN);

            if (firstDist >= secondDist)
            {
                return new BoundingBox(othMax, min);
            }
            else
            {
                return new BoundingBox(max, othMin);
            }

        }

        internal BoundingBox GetBigCollisionBoundingBox(BoundingBox other)
        {
            if (this.Max == other.Max && this.Min == other.Min)
            {
                return new BoundingBox(Max, Min); //equal BB
            }

            var MaxMax = Vector3.Max(this.Max, other.Max);
            var MinMin = Vector3.Min(this.Min, other.Min);

            return new BoundingBox(MaxMax, MinMin);
        }
    }
}
