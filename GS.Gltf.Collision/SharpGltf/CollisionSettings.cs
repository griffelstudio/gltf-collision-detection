using System;
using System.Collections.Generic;
using System.Text;

namespace GS.Gltf.Collision.SharpGltf
{
    public class CollisionSettings
    {
        public List<string> ModelPaths {get; set;}

        public bool CheckNodesCollisionBetweenModels { get; set; } = false;

        public bool CheckNodesCollisionIntoModels { get; set; } = false;

        public float Delta { get; set; } = 0;
    }
}
