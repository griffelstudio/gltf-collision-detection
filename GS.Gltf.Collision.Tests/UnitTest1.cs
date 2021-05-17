using NUnit.Framework;
using glTFLoader.Schema;
using glTFLoader;
using GS.Gltf.Collision;
using System.Collections.Generic;
using GS.Gltf.Collision.SharpGltf;
using System.IO;

namespace GS.Gltf.Collision.Tests
{
    public class Tests
    {
        [Test]
        public void MainTest()
        {

            List<string> inputfiles = new List<string>()
            {
                Path.Combine("C:","gltf","collision","transofrms","test2021.gltf"),
                Path.Combine("C:","gltf","collision","test3","test2021InsideCollision.gltf"),
                Path.Combine("C:","gltf","collision","Incollision","test2021InsideCollision.gltf"),
            };

            var settings = new CollisionSettings()
            {
                ModelPaths = inputfiles,
                InterModelDetection  = true,
                InModelDetection  = true,
                Delta = 0.1f,
            };

            var detector = new CollisionDetector(settings);
            var detectResult = detector.Detect();
            Assert.Pass();
        }

        [Test]
        public void TestCollider()
        {
            Assert.IsTrue(TestObjects.box1.IsCollideWith(TestObjects.box2, 1));
            Assert.IsFalse(TestObjects.box2.IsCollideWith(TestObjects.box4, 1));
            Assert.IsFalse(TestObjects.box1.IsCollideWith(TestObjects.box4, 1));
            Assert.IsFalse(TestObjects.box4.IsCollideWith(TestObjects.box5, 1));
        }
    }
}