using NUnit.Framework;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace GS.Gltf.Collision.Tests
{
    public class TestCollisions
    {

        string testRootPath = Directory.GetParent(Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName).ToString();

        [Test]
        public void InterModelCollisionsTest()
        {
            // TODO Add test files to repo (rvt and gltf).
            // Set pathes to the file as relative:
            //typeof(TestMain).Assembly.Location



            List<string> inputfiles = new List<string>()
            {
                //Path.Combine("C:","gltf","collision","tests","box1","box1.gltf"),
                //Path.Combine("C:","gltf","collision","tests","box2","box2.gltf"),
                //Path.Combine("C:","gltf","collision","tests","box3","box3.gltf"),
                Path.Combine(testRootPath,"Resources","box1","box1.gltf"),
                Path.Combine(testRootPath,"Resources","box2","box2.gltf"),
            };

            var settings = new CollisionSettings(inputfiles)
            {
                InModelDetection = false,
                Delta = 0.1f,
            };

            var detector = new CollisionDetector(settings);
            var detectResult = detector.Detect();
            detectResult = detectResult.Where(x => x.Collisions.Count > 0).ToList();


            Assert.Pass();
        }

        [Test]
        public void InModelCollisonsTest()
        {

            List<string> inputfiles = new List<string>()
            {
                Path.Combine(testRootPath,"Resources","multicoliision_boxes","all_boxes.gltf"),
            };

            var settings = new CollisionSettings(inputfiles)
            {
                InModelDetection = true,
                Delta = 0.1f,
            };

            var detector = new CollisionDetector(settings);
            detector.Detect();

            // TODO Add elements to the file from inside the CollisionDetector depending on the property HiglightCollisions.
            //var root = new GltfReader(inputfiles).RawModels[0];
            //var testModel = detector.Models[0];
            //foreach (var collision in testModel.InterModelCollisions)
            //{
            //    root.AddCollisionBBNode(collision.MinIntersectionBoundaries);
            //}
            
            //root.SaveGLTF(Path.Combine("C:", "Resources", "all_boxes", "collision.gltf"));
            //root.SaveGLB(Path.Combine("C:", "Resources", "all_boxes", "test.glb"));


            Assert.Pass();
        }
    }
}