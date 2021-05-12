using System;
using System.Collections.Generic;
using System.IO;
using glTFLoader;
using glTFLoader.Schema;
using System.Numerics;
using SharpGLTF;
using SharpGLTF.Schema2;
using GS.Gltf.Collision.SharpGltf;

namespace GS.Gltf.Collision
{
    public class Main
    {
      
        public Main()
        {

            List<string> inputfiles = new List<string>()
            {
               "C:\\gltf\\collision\\transofrms\\test2021.gltf",
               "C:\\gltf\\collision\\test3\\test2021InsideCollision.gltf",
               "C:\\gltf\\collision\\Incollision\\test2021InsideCollision.gltf"
            };

            var settings = new CollisionSettings()
            {
                ModelPaths = inputfiles,
                CheckNodesCollisionBetweenModels = true,
                CheckNodesCollisionIntoModels = true,
                Delta = 0.1f,
            };

            var detector = new CollisionDetector(settings);
            var detectResult = detector.Detect();

        }    
    }
}
