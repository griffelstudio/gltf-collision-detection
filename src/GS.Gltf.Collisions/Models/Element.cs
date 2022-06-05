using GS.Gltf.Collisions.Geometry;
using GS.Gltf.Collisions.Interfaces;
using SharpGLTF.Schema2;
using SharpGLTF.Transforms;
using System;
using System.Collections.Generic;
using System.Numerics;

namespace GS.Gltf.Collisions.Models
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

        // TODO Remove?
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
            var maxX = PositionVectors[0].X;
            var minY = PositionVectors[0].Y;

            var maxY = PositionVectors[0].Y;
            var minZ = PositionVectors[0].Z;
            var maxZ = PositionVectors[0].Z;

            for (int i = 1; i < PositionVectors.Count; i++)
            {
                minX = Math.Min(PositionVectors[i].X, minX);
                minY = Math.Min(PositionVectors[i].Y, minY);
                minZ = Math.Min(PositionVectors[i].Z, minZ);

                maxX = Math.Max(PositionVectors[i].X, maxX);
                maxY = Math.Max(PositionVectors[i].Y, maxY);
                maxZ = Math.Max(PositionVectors[i].Z, maxZ);

                Xs.Add(PositionVectors[i].X);
                Ys.Add(PositionVectors[i].Y);
                Zs.Add(PositionVectors[i].Z);
            }

            BoundingBox = new BoundingBox(new Vector3(maxX, maxY, maxZ), new Vector3(minX, minY, minZ));
        }
    }
}
