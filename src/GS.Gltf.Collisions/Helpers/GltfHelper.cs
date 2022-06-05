using Newtonsoft.Json.Linq;
using SharpGLTF.Schema2;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.IO;

namespace GS.Gltf.Collisions.Helpers
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

        public static ModelRoot CreateCleanModel()
        {
            ModelRoot newModel = ModelRoot.CreateModel();
            Scene scene = newModel.UseScene(0);
            return newModel;
        }

        

        public static void Merge(string outputPath, List<string> files)
        {
            var jsonFiles = ConvertFilesToJObject(files);
            var mergedObject = jsonFiles.First();
            for (int i = 1; i < jsonFiles.Count; i++)
            {
                mergedObject = MergeGltfFiles(mergedObject, jsonFiles[i]);
            }
            string json = mergedObject.ToString();
            File.WriteAllText(outputPath, json);
            CopyFiles(files, outputPath);
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

        private static JObject MergeGltfFiles(JObject patienceJson, JObject recipienceJson)
        {

            var nodesCount = patienceJson["nodes"].Count();
            var meshesCount = patienceJson["meshes"].Count();
            var scenesCount = patienceJson["scenes"].Count();
            var accessorsCount = patienceJson["accessors"].Count();
            var assetCount = patienceJson["asset"].Count();
            var bufferViewsCount = patienceJson["bufferViews"].Count();
            var buffersCount = patienceJson["buffers"].Count();

            foreach (var mesh in recipienceJson["meshes"])
            {
                foreach (var primivite in mesh["primitives"])
                {
                    primivite["attributes"]["POSITION"] = ((int)primivite["attributes"]["POSITION"] + accessorsCount);
                    primivite["attributes"]["NORMAL"] = ((int)primivite["attributes"]["NORMAL"] + accessorsCount);
                    primivite["indices"] = ((int)primivite["indices"] + accessorsCount);
                }
            }

            foreach (var bufferView in recipienceJson["bufferViews"])
            {
                bufferView["buffer"] = ((int)bufferView["buffer"] + buffersCount);
            }

            foreach (var bufferView in recipienceJson["accessors"])
            {
                bufferView["bufferView"] = ((int)bufferView["bufferView"] + accessorsCount);
            }

            foreach (var scene in recipienceJson["scenes"])
            {
                if (scene["nodes"] != null)
                {
                    for (int i = 0; i < scene["nodes"].Count(); i++)
                    {
                        scene["nodes"][i] = ((int)scene["nodes"][i] + nodesCount);
                    }
                }

            }

            foreach (var node in recipienceJson["nodes"])
            {
                if (node["children"] != null)
                {
                    for (int i = 0; i < node["children"].Count(); i++)
                    {
                        node["children"][i] = ((int)node["children"][i] + nodesCount);
                    }
                }
            }

            patienceJson.Merge(recipienceJson, new JsonMergeSettings
            {
                MergeArrayHandling = MergeArrayHandling.Concat
            });

            List<JToken> tmp = new List<JToken>();
            foreach (var scene in patienceJson["scenes"])
            {
                if (scene["nodes"] != null)
                {
                    for (int i = 0; i < scene["nodes"].Count(); i++)
                    {
                        var a = scene["nodes"][i];
                        tmp.Add(a);
                    }
                }
            }

            var sceneObject = patienceJson["scenes"] as JArray;
            sceneObject.Clear();
            JToken j = JToken.Parse("{\"name\": \"{ 3D}\", \"nodes\": []}");
            sceneObject.Add(j);

            var obj = new JArray();
            foreach (var token in tmp)
            {
                obj.Add(token);
            }
            sceneObject[0]["nodes"] = obj;

            return patienceJson;
        }

        private static List<JObject> ConvertFilesToJObject(List<string> gltfFiles)
        {
            List<JObject> result = new List<JObject>();
            foreach (var file in gltfFiles)
            {
                var jsontext = File.ReadAllText(file);
                JObject json = JObject.Parse(jsontext);
                result.Add(json);
            }
            return result;
        }

        private static void CopyFiles(List<string> filePaths, string outputPath)
        {
            foreach (var file in filePaths)
            {
                var jsontext = File.ReadAllText(file);
                JObject json = JObject.Parse(jsontext);
                var buffers = new List<string>();
                var outputDirectory = Directory.GetParent(outputPath).ToString();
                foreach (var buffer in json["buffers"])
                {
                    var bufferName = buffer["uri"].ToString();
                    var fileDirectory = Directory.GetParent(file).ToString();
                    File.Copy(Path.Combine(fileDirectory, bufferName),
                        Path.Combine(outputDirectory, bufferName), true);
                }
            }
        }







    }
}
