using System;
using System.Collections.Generic;
using System.Text;
using SharpGLTF;
using SharpGLTF.Schema2;

namespace GS.Gltf.Collision
{
    class GltfReader
    {
        public List<ModelData> Models { get; set; }

        public GltfReader(List<string> filePaths)
        {
            Models = new List<ModelData>();
            int modelIndex = 0;
            foreach (var path in filePaths)
            {
                var model = LoadGltfFromFile(path);
                var modelData = new ModelData(model, modelIndex);
                Models.Add(modelData);
                modelIndex++;
            }
        }

        public ModelRoot LoadGltfFromFile(string path)
        {
            return ModelRoot.Load(path);
        }

    }
}
