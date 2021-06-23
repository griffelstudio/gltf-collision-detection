using GS.Gltf.Collision.Interfaces;
using SharpGLTF.Schema2;
using SharpGLTF.Transforms;
using System.Collections.Generic;
using System.Linq;

namespace GS.Gltf.Collision.Models
{
    /// <summary>
    /// Equivalent to the whole glTF file.
    /// </summary>
    internal class ModelData : ICollidable
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
                resultList.Add(new Element(nodeIndex, index, nodePrimitives, node.Name, NodeTransforms[nodeIndex]));
            }
            return resultList;
        }
    }
}
