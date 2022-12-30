using GS.Gltf.Collisions.Geometry;
using NUnit.Framework;

namespace GS.Gltf.Collisions.Tests
{
    public class TestGeometry
    {
        [Test]
        public void TestCollider()
        {
            Assert.IsTrue(TestObjects.Box1.IsCollideWith(TestObjects.Box2, 0.01f));
            Assert.IsFalse(TestObjects.Box2.IsCollideWith(TestObjects.Box4, 0.01f));
            Assert.IsFalse(TestObjects.Box1.IsCollideWith(TestObjects.Box4, 0.01f));
            Assert.IsFalse(TestObjects.Box4.IsCollideWith(TestObjects.Box5, 0.01f));
        }

        [Test]
        public void TestTriangleCollisionPoints()
        {
            var point = Helpers.GeometryHelper.RaycastPoint(TestObjects.Triangle1, TestObjects.Ray1);
            Assert.IsTrue(point == new System.Numerics.Vector3(10, 5, 5));

            var res1 = Triangle.TriangleTriangle(TestObjects.Triangle2, TestObjects.Triangle3);
            var res2 = Triangle.TriangleTriangle(TestObjects.Triangle1, TestObjects.Triangle3);
            var res3 = Triangle.TriangleTriangle(TestObjects.Triangle1, TestObjects.Triangle2);
            var res4 = Triangle.TriangleTriangle(TestObjects.Triangle1, TestObjects.Triangle4);
            var res5 = Triangle.TriangleTriangle(TestObjects.Triangle2, TestObjects.Triangle4);
            var res6 = Triangle.TriangleTriangle(TestObjects.Triangle3, TestObjects.Triangle4);

            Assert.IsTrue(res1);
            Assert.IsTrue(res2);
            Assert.IsTrue(res3);
            Assert.IsFalse(res4);
            Assert.IsFalse(res5);
            Assert.IsFalse(res6);
        }
    }
}