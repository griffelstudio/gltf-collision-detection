using glTFLoader.Schema;
using System.Numerics;
using GS.Gltf.Collisions.Geometry;

namespace GS.Gltf.Collisions.Tests
{
    internal static class TestObjects
    {
        public static BoundingBox Box1 = new BoundingBox(new Accessor
        {   Min = new float[] { 0f, 0f, 0f },
            Max = new float[] { 1f, 1f, 1f },
        });

        public static BoundingBox Box2 = new BoundingBox(new Accessor
        {
            Min = new float[] { -0.5f, -0.5f, -0.5f },
            Max = new float[] { 0.5f, 0.5f, 0.5f },
        });

        public static BoundingBox Box3 = new BoundingBox(new Accessor
        {
            Min = new float[] { -0.99f, -0.99f, -0.99f },
            Max = new float[] { 0.99f, 0.99f, 0.99f },
        });

        public static BoundingBox Box4 = new BoundingBox(new Accessor
        {
            Min = new float[] { 1f, 1f, 1f },
            Max = new float[] { 2f, 2f, 2f },
        });

        public static BoundingBox Box5 = new BoundingBox(new Accessor
        {
            Min = new float[] { 1f, 3f, 1f },
            Max = new float[] { 2f, 4f, 2f },
        });

        public static Triangle Triangle1 = new Triangle()
        {
            A = new Vector3(0, 0, 10),
            B = new Vector3(10, 10, 0),
            C = new Vector3(20, 0, 10)
        };

        public static Triangle Triangle2 = new Triangle()
        {
            A = new Vector3(0, 0, 10),
            B = new Vector3(10, 10, 0),
            C = new Vector3(20, 0, 10)
        };

        public static Triangle Triangle3 = new Triangle()
        {
            A = new Vector3(0, 0, 10),
            B = new Vector3(10, 10, 0),
            C = new Vector3(10, 0, 10)
        };

        public static Triangle Triangle4 = new Triangle()
        {
            A = new Vector3(1, 1, 11),
            B = new Vector3(11, 11, 11),
            C = new Vector3(11, 1, 11)
        };

        public static Ray Ray1 = new Ray(new Vector3(10, 5, 0), new Vector3(10, 5, 10));
        public static Ray Ray2 = new Ray(new Vector3(10, 5, 0), new Vector3(10, 5, 10));
        public static Ray Ray3 = new Ray(new Vector3(10, 5, 0), new Vector3(10, 5, 10));

        public static Ray Ray4 = new Ray(new Vector3(10, 5, 10), new Vector3(10, 5, 0));
        public static Ray Ray5 = new Ray(new Vector3(10, 5, 10), new Vector3(10, 5, 0));
        public static Ray Ray6 = new Ray(new Vector3(0, 0, 10), new Vector3(10, 10, 0));
    }
}
