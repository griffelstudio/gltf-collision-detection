using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace GS.Gltf.Collision.SharpGltf
{
    public class CollisionSettings
    {
        public List<string> ModelPaths {get; set;}

        public bool InterModelDetection  { get; set; } = false;

        public bool InModelDetection { get; set; } = false;

        public float Delta { get; set; } = CollisionConstants.Tolerance;

        public CollisionSettings()
        {

        }

        public CollisionSettings(List<string> modelPaths)
        {
            if (modelPaths.Count == 0)
            {
                throw new InvalidOperationException("Add at least one path into list");
            }
            foreach (var path in modelPaths)
            {
                if (!File.Exists(path))
                {
                    modelPaths.Remove(path);
                }
            }
            if (modelPaths.Count == 0)
            {
                throw new InvalidOperationException("there arent any valid path in list");
            }
            if (modelPaths.Count == 1)
            {
                InModelDetection = true;
            }
        }
    }
}
