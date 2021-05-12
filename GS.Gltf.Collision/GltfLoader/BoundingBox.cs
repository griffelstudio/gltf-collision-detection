using System;
using System.Collections.Generic;
using System.Text;
using glTFLoader.Schema;
using glTFLoader;
using System.Linq;

namespace GS.Gltf.Collision
{
    public class BoundingBox
    {
        private const int BOUNDING_BOX_DIMENSIONALITY = 3;
        private const int X_DIM = 0;
        private const int Y_DIM = 1;
        private const int Z_DIM = 2;

        public float[] Min = new float[BOUNDING_BOX_DIMENSIONALITY];
        public float[] Max = new float[BOUNDING_BOX_DIMENSIONALITY];

        private float MaxX { get; }
        private float MaxY { get; }
        private float MaxZ { get; }

        private float MinX { get; }
        private float MinY { get; }
        private float MinZ { get; }

        public BoundingBox(List<Accessor> accessors)
        {
            var maxVectors = new List<float[]>();
            var minVectors = new List<float[]>();

            foreach (var accessor in accessors)
            {
                maxVectors.Add(accessor.Max);
                minVectors.Add(accessor.Min);
            }

            for (int dim = 0; dim < BOUNDING_BOX_DIMENSIONALITY; dim++)
            {
                Min[dim] = GetMinValueByIndex(minVectors, dim);
                Max[dim] = GetMaxValueByIndex(maxVectors, dim);
            }

            MaxX = Max[X_DIM];
            MaxY = Max[Y_DIM];
            MaxZ = Max[Z_DIM];

            MinX = Min[X_DIM];
            MinY = Min[Y_DIM];
            MinZ = Min[Z_DIM];
        }

        public BoundingBox(Accessor accessor)
        {
            MaxX = accessor.Max[X_DIM];
            MaxY = accessor.Max[Y_DIM];
            MaxZ = accessor.Max[Z_DIM];

            MinX = accessor.Min[X_DIM];
            MinY = accessor.Min[Y_DIM];
            MinZ = accessor.Min[Z_DIM];
        }

        public BoundingBox(float[] max, float[] min)
        {
            MaxX = max[X_DIM];
            MaxY = max[Y_DIM];
            MaxZ = max[Z_DIM];

            MinX = min[X_DIM];
            MinY = min[Y_DIM];
            MinZ = min[Z_DIM];
        }

        private float GetMaxValueByIndex(List<float[]> arrays, int index)
        {
            var maxValue = (from array in arrays select array[index]).Max();
            return maxValue;
        }

        private float GetMinValueByIndex(List<float[]> arrays, int index)
        {
            var minValue = (from array in arrays select array[index]).Min();
            return minValue;
        }

        public bool IsCollideWith(BoundingBox other, float delta = 0)
        {
            BoundingBox xLeftObject;
            BoundingBox xRightObject;

            BoundingBox yLeftObject ;
            BoundingBox yRightObject;

            BoundingBox zLeftObject;
            BoundingBox zRightObject;

            if (MinX >= other.MinX)
            {
                xLeftObject = other;
                xRightObject = this;
            }
            else
            {
                xLeftObject = this;
                xRightObject = other;
            }

            if (MinY >= other.MinY)
            {
                yLeftObject = other;
                yRightObject = this;
            }
            else
            {
                yLeftObject = this;
                yRightObject = other;
            }

            if (MinZ >= other.MinZ)
            {
                zLeftObject = other;
                zRightObject = this;
            }
            else
            {
                zLeftObject = this;
                zRightObject = other;
            }

            bool isCollideByX = xRightObject.MinX < xLeftObject.MaxX + delta && xLeftObject.MaxX + delta > xRightObject.MinX;
            bool isCollideByY = yRightObject.MinY < yLeftObject.MaxY + delta && yLeftObject.MaxY + delta > yRightObject.MinY;
            bool isCollideByZ = zRightObject.MinZ < zLeftObject.MaxZ + delta && zLeftObject.MaxZ + delta > zRightObject.MinZ;

            return isCollideByX && isCollideByY && isCollideByZ;
        }
    }
}
