using NUnit.Framework;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace GS.Gltf.Collision.Tests
{
    public class TestCollisions
    {
        private readonly string path = typeof(TestCollisions).Assembly.Location; //TODO" C:\\work\\new_gltf_exporter\\gltf-collision-detection\\GS.Gltf.Collision.Tests\\bin\\Debug\\netcoreapp3.1\\GS.Gltf.Collision.Tests.dll"

        string testRootPath = Directory.GetParent(Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName).ToString();

        [Test]
        public void InterModelCollisionsTest()
        {
            
            List<string> inputfiles = new List<string>()
            {
                Path.Combine(testRootPath,"Resources","box1","box1.gltf"),
                Path.Combine(testRootPath,"Resources","box2","box2.gltf"),
                Path.Combine(testRootPath,"Resources","box3","box3.gltf"),
                Path.Combine(testRootPath,"Resources","box4","box4.gltf"),
            };

            var settings = new CollisionSettings(inputfiles)
            {
                InModelDetection = false,
                Delta = 0.1f,
                HiglightCollisions = CollisionHighlighing.MergeAll,
            };

            var detector = new CollisionDetector(settings);
            var detectResult = detector.Detect();
            
            Assert.Pass();
        }

        [Test]
        public void InModelCollisonsTest()
        {

            List<string> inputfiles = new List<string>()
            {
                Path.Combine(testRootPath,"Resources","BigModel","Hockey Arena.gltf"),
            };

            var settings = new CollisionSettings(inputfiles)
            {
                InModelDetection = true,
                Delta = 0.1f,
                HiglightCollisions = CollisionHighlighing.MergeAll
            };

            var detector = new CollisionDetector(settings);
            var detectResult = detector.Detect();

            Assert.Pass();
        }
    }
}