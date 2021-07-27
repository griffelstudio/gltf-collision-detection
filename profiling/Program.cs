using GS.Gltf.Collision;
using GS.Gltf.Collision.Tests;
using System;
using System.Collections.Generic;
using System.IO;

namespace profiling
{
    class Program
    {

        private static readonly string path = typeof(TestCollisions).Assembly.Location;
        private static readonly string path2 = Directory.GetParent(Directory.GetParent(Directory.GetParent(path).ToString()).ToString()).ToString();
        private static readonly string path3 = Directory.GetParent(Directory.GetParent(path2).ToString()).ToString();
        static void Main(string[] args)
        {

            
        List<string> inputfiles = new List<string>()
            {
                Path.Combine(path3,"GS.Gltf.Collision.Tests","Resources","cutted_arena","Hockey Arena.gltf"),
            };

            var settings = new CollisionSettings(inputfiles)
            {
                InModelDetection = true,
                Delta = 5f,
                HiglightCollisions = CollisionHighlighing.MergeAll
            };

            var detector = new CollisionDetector(settings);
            var detectResult = detector.Detect();
        }
    }
}
