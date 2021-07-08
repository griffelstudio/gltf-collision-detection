using GS.Gltf.Collision.Geometry;
using GS.Gltf.Collision.Interfaces;
using SharpGLTF.Schema2;
using SharpGLTF.Transforms;
using System;
using System.Collections.Generic;
using System.Numerics;

namespace GS.Gltf.Collision.Models
{
    /// <summary>
    /// Equivalent to glTF Node.
    /// </summary>
    internal class Element : ICollidable
    {
        public int NodeIndex;
        public string NodeName;
        public int ModelIndex;
        public List<MeshPrimitive> Primitives;
        public List<Vector3> PositionVectors;
        public BoundingBox BoundingBox;
        public List<float> Xs = new List<float>();
        public List<float> Ys = new List<float>();
        public List<float> Zs = new List<float>();
        public List<Triangle> Triangles = new List<Triangle>();

        public Element(int nodeIndex, int modelIndex, List<MeshPrimitive> primitives, string nodeName, List<AffineTransform> transforms)
        {
            Primitives = primitives;
            NodeIndex = nodeIndex;
            NodeName = nodeName;
            ModelIndex = modelIndex;

            PositionVectors = new List<Vector3>();
            foreach (var primitive in Primitives)
            {
                var positionAccessor = primitive.VertexAccessors["POSITION"];
                var accessorVector = Transformation.TransformAccessor(positionAccessor, transforms);
                var indices = primitive.IndexAccessor;

                CreateTriangles(accessorVector, indices.AsIndicesArray());

                PositionVectors.AddRange(accessorVector);
            }
            
            CreateBoundingBox();

        }

        public BoundingBox GetBoundingBox()
        {
            return BoundingBox;
        }

        private void CreateTriangles(List<Vector3> points, SharpGLTF.Memory.IntegerArray indeces)
        {
            for (int i = 2; i < indeces.Count; i = i + 3)
            {
                Triangles.Add(new Triangle(points[(int)indeces[i]], points[(int)indeces[i - 1]], points[(int)indeces[i - 2]]));
            }
        }

        private void CreateBoundingBox()
        {
            var minX = PositionVectors[0].X;
            var minY = PositionVectors[0].Y;
            var minZ = PositionVectors[0].Z;

            var maxX = PositionVectors[0].X;
            var maxY = PositionVectors[0].Y;
            var maxZ = PositionVectors[0].Z;

            foreach (var vector in PositionVectors)
            {
                minX = Math.Min(vector.X, minX);
                minY = Math.Min(vector.Y, minY);
                minZ = Math.Min(vector.Z, minZ);

                maxX = Math.Max(vector.X, maxX);
                maxY = Math.Max(vector.Y, maxY);
                maxZ = Math.Max(vector.Z, maxZ);

                Xs.Add(vector.X);
                Ys.Add(vector.Y);
                Zs.Add(vector.Z);
            }

            float[] min = new float[3]
            {
                minX, minY, minZ
            };

            float[] max = new float[3]
            {
                maxX, maxY, maxZ
            };

            BoundingBox = new BoundingBox(max, min);
        }
    }
}
