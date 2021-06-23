using GS.Gltf.Collision.Geometry;
using NUnit.Framework;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace GS.Gltf.Collision.Tests
{
    public class TestGeometry
    {
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
            var point = GeometryHelper.RaycastPoint(TestObjects.triangle1, TestObjects.ray1);
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