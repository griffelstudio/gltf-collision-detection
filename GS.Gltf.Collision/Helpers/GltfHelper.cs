using SharpGLTF.Schema2;
using System;
using System.Collections.Generic;
using System.Text;

namespace GS.Gltf.Collision.Helpers
{
    internal class GltfHelper
    {
        public static ModelRoot MergeModels(List<ModelRoot> models)
        {
            ModelRoot newModel = ModelRoot.CreateModel();
            Scene scene = newModel.UseScene(0);
            var newModelRootNode = scene.CreateNode();

            foreach (var model in models)
            {
                var startNode = model.LogicalNodes[0];
                var rootNode = newModelRootNode.CreateNode(startNode.Name);
                rootNode.WithLocalTransform(startNode.LocalTransform).WithMesh(GetNewMesh(startNode.Mesh, newModel));
                AddNodes(model, ref newModel, startNode, rootNode);
            }

            return newModel;
        }

        private static void AddNodes(ModelRoot recipientModel, ref ModelRoot patienteModel, Node startNode, Node startPatienteNode)
        {
            var Node = startNode;
            var enumerator = Node.VisualChildren.GetEnumerator();

            while (enumerator.MoveNext())
            {
                var transforms = enumerator.Current.LocalTransform;
                Mesh mesh = GetNewMesh(enumerator.Current.Mesh, patienteModel);
                var newNode = startPatienteNode.CreateNode().WithLocalTransform(transforms).WithMesh(mesh);
                AddNodes(recipientModel, ref patienteModel, enumerator.Current, newNode);
            }

        }

        private static Mesh GetNewMesh(Mesh mesh, ModelRoot model)
        {
            if (mesh != null)
            {
                var meshBuilder = mesh.ToMeshBuilder();
                return model.CreateMesh(meshBuilder);
            }
            return null;
        }


    }
}
