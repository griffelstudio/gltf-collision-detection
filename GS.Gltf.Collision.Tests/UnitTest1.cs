using NUnit.Framework;
using glTFLoader.Schema;
using glTFLoader;
using GS.Gltf.Collision;

namespace GS.Gltf.Collision.Tests
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {

        }

        [Test]
        public void Test1()
        {

            new Main();
            Assert.Pass();
        }

        [Test]
        public void TestCollider()
        {
            Accessor accessor1 = new Accessor();
            Accessor accessor2 = new Accessor();
            Accessor accessor3 = new Accessor();
            Accessor accessor4 = new Accessor();
            Accessor accessor5 = new Accessor();

            accessor1.Max = new float[] { 1f,1f,1f };
            accessor1.Min = new float[] { 0f, 0f, 0f };

            accessor2.Max = new float[] { 0.5f, 0.5f, 0.5f };
            accessor2.Min = new float[] { -0.5f, -0.5f, -0.5f };

            accessor3.Max = new float[] { 0.99f, 0.99f, 0.99f };
            accessor3.Min = new float[] { -0.99f, -0.99f, -0.99f };

            accessor4.Max = new float[] { 2f, 2f, 2f };
            accessor4.Min = new float[] { 1f, 1f, 1f };

            accessor5.Max = new float[] { 2f, 4f, 2f };
            accessor5.Min = new float[] { 1f, 3f, 1f };

            BoundingBox box1 = new BoundingBox(accessor1);
            BoundingBox box2 = new BoundingBox(accessor2);
            BoundingBox box3 = new BoundingBox(accessor3);
            BoundingBox box4 = new BoundingBox(accessor4);
            BoundingBox box5 = new BoundingBox(accessor5);

            Assert.IsTrue(box1.IsCollideWith(box2, 1));
            Assert.IsFalse(box2.IsCollideWith(box4, 1));
            Assert.IsFalse(box1.IsCollideWith(box4, 1));
            Assert.IsFalse(box4.IsCollideWith(box5, 1));
        }
    }
}