using NUnit.Framework;
using glTFLoader.Schema;
using glTFLoader;
using GS.Gltf.Collision;
using System.Collections.Generic;
using GS.Gltf.Collision.SharpGltf;
using System.IO;
using System.Linq;
using GS.Gltf.Collision.Geometry;

namespace GS.Gltf.Collision.Tests
{
    public class Tests
    {
        [Test]
        public void MainTest()
        {

            List<string> inputfiles = new List<string>()
            {
                //Path.Combine("C:","gltf","collision","tests","box1","box1.gltf"),
                //Path.Combine("C:","gltf","collision","tests","box2","box2.gltf"),
                //Path.Combine("C:","gltf","collision","tests","box3","box3.gltf"),
                //Path.Combine("C:","gltf","collision","tests","box4","box4.gltf"),
                Path.Combine("C:","gltf","collision","tests","all_boxes","all_boxes.gltf"),
            };

            var settings = new CollisionSettings(inputfiles)
            {
                InterModelDetection = true,
                InModelDetection = true,
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

        [Test]
        public void TestTriangleCollisionPoints()
        {
            var point = HelperUtils.RaycastPoint(TestObjects.triangle1, TestObjects.ray1);
            Assert.IsTrue(point == new System.Numerics.Vector3(10, 5, 5));

            var res1 = Triangle.TriangleTriangle(TestObjects.triangle2, TestObjects.triangle3);
            var res2 = Triangle.TriangleTriangle(TestObjects.triangle1, TestObjects.triangle3);
            var res3 = Triangle.TriangleTriangle(TestObjects.triangle1, TestObjects.triangle2);
            var res4 = Triangle.TriangleTriangle(TestObjects.triangle1, TestObjects.triangle4);
            var res5 = Triangle.TriangleTriangle(TestObjects.triangle2, TestObjects.triangle4);
            var res6 = Triangle.TriangleTriangle(TestObjects.triangle3, TestObjects.triangle4);

            Assert.IsTrue(res1);
            Assert.IsTrue(res2);
            Assert.IsTrue(res3);
            Assert.IsFalse(res4);
            Assert.IsFalse(res5);
            Assert.IsFalse(res6);

        }
    }
}