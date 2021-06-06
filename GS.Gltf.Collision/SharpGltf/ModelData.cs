using System;
using System.Collections.Generic;
using System.Text;
using SharpGLTF;
using SharpGLTF.Schema2;
using System.Linq;
using SharpGLTF.Transforms;
using System.Numerics;
using GS.Gltf.Collision.SharpGltf;
using GS.Gltf.Collision.Geometry;

namespace GS.Gltf.Collision
{
    public class ModelData : ICollidable
    {
        public int modelIndex;
        private List<Node> nodes;
        private List<Node> nodesWithGeometry;
        public List<Element> ElementMeshPrimitives;
        public Dictionary<int, List<AffineTransform>> NodeTransforms;
        public ModelPrimitive ModelPrimitive;
        public List<CollisionElement> InterModelCollisions;

        public ModelData(ModelRoot model, int modelIndex)
        {
            var nodes = new List<Node>();
            nodes.AddRange(model.LogicalNodes);
            this.nodes = nodes;
            nodesWithGeometry = nodes.Where(n => n.Mesh is object).ToList();
            
            NodeTransforms = CollectNodesTransforms();
            ElementMeshPrimitives = GetNodePrimitives(nodesWithGeometry, modelIndex);

            ModelPrimitive = new ModelPrimitive(modelIndex, ElementMeshPrimitives);
            this.modelIndex = modelIndex;
        }

        public Dictionary<int, List<AffineTransform>> CollectNodesTransforms()
        {
            var NodeTransformsData = new Dictionary<int, List<AffineTransform>>();
            
            foreach (var node in nodesWithGeometry)
            {
                var transformations = new List<AffineTransform>();
                var currentNode = node;
                transformations.Add(currentNode.LocalTransform);
                while (currentNode.VisualParent is object)
                {
                    transformations.Add(currentNode.VisualParent.LocalTransform);
                    currentNode = nodes[currentNode.VisualParent.LogicalIndex];
                }
                NodeTransformsData.Add(node.LogicalIndex,transformations);
            }
            return NodeTransformsData;
        }

        public BoundingBox GetBoundingBox()
        {
            return ModelPrimitive.BoundingBox;
        }

        public List<Element> GetNodePrimitives(List<Node> nodes, int index)
        {
            var resultList = new List<Element>();
            foreach (var node in nodes)
            {
                var nodePrimitives = new List<MeshPrimitive>();
                foreach (var primitive in node.Mesh.Primitives)
                {
                    nodePrimitives.Add(primitive);
                }
                var nodeIndex = node.LogicalIndex;
                resultList.Add(new Element(nodeIndex, index, nodePrimitives, node.Name,NodeTransforms[nodeIndex]));
            }
            return resultList;
        }
    }

    public class Element : ICollidable
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
                CreateTriangles(accessorVector, indices.AsScalarArray());
                PositionVectors.AddRange(accessorVector);
            }
            CreateBoundingBox();
        }

        public BoundingBox GetBoundingBox()
        {
            return BoundingBox;
        }

        private void CreateTriangles(List<Vector3> points, IList<float> indeces)
        {
            for (int i = 2; i < indeces.Count; i=i+3)
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

    public class ModelPrimitive
    {
        public int ModelIndex;
        public List<Element> Primitives;
        public BoundingBox BoundingBox;
        public List<float> Xs;
        public List<float> Ys;
        public List<float> Zs;

        public ModelPrimitive(int modelIndex, List<Element> primitives)
        {
            ModelIndex = modelIndex;
            Primitives = primitives;
            CreateBoundingBox();
        }

        public void CreateBoundingBox()
        {
            Xs = new List<float>();
            Ys = new List<float>();
            Zs = new List<float>();

            foreach (var primitive in Primitives)
            {
                Xs.AddRange(primitive.Xs);
                Ys.AddRange(primitive.Ys);
                Zs.AddRange(primitive.Zs);
            }

            Xs.Sort();
            Ys.Sort();
            Zs.Sort();

            float[] min = new float[3]
            {
                Xs.First(), Ys.First(), Zs.First()
            };

            float[] max = new float[3]
            {
                Xs.Last(), Ys.Last(), Zs.Last()
            };
            BoundingBox = new BoundingBox(max, min);
        }
    }
}
