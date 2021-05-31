using NUnit.Framework;
using glTFLoader.Schema;
using glTFLoader;
using GS.Gltf.Collision;
using System.Collections.Generic;
using GS.Gltf.Collision.SharpGltf;
using System.IO;
using System.Linq;

namespace GS.Gltf.Collision.Tests
{
    public class Tests
    {
        [Test]
        public void MainTest()
        {

            List<string> inputfiles = new List<string>()
            {
                Path.Combine("C:","gltf","collision","1","test2021InsideCollision.gltf"),
                Path.Combine("C:","gltf","collision","2","test2021InsideCollision.gltf"),
                Path.Combine("C:","gltf","collision","3","test2021InsideCollision.gltf"),
            };

            var settings = new CollisionSettings(inputfiles)
            {
                InterModelDetection  = true,
                InModelDetection  = true,
                Delta = 0.1f,
            };

            var detector = new CollisionDetector(settings);
            var detectResult = detector.Detect();
            detectResult = detectResult.Where(x => x.Collisions.Count > 0).ToList();
            Assert.Pass();
        }

        [Test]
        public void TestCollider()
        {
            Assert.IsTrue(TestObjects.box1.IsCollideWith(TestObjects.box2));
            Assert.IsFalse(TestObjects.box2.IsCollideWith(TestObjects.box4));
            Assert.IsFalse(TestObjects.box1.IsCollideWith(TestObjects.box4));
            Assert.IsFalse(TestObjects.box4.IsCollideWith(TestObjects.box5));
        }
    }
}