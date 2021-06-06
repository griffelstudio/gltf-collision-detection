using glTFLoader.Schema;
using System;
using System.Collections.Generic;
using System.Text;
using System.Numerics;
using GS.Gltf.Collision.Geometry;

namespace GS.Gltf.Collision.Tests
{
    static class TestObjects
    {
        public static BoundingBox box1 = new BoundingBox(new Accessor
        {   Min = new float[] { 0f, 0f, 0f },
            Max = new float[] { 1f, 1f, 1f },
        });

        public static BoundingBox box2 = new BoundingBox(new Accessor
        {
            Min = new float[] { -0.5f, -0.5f, -0.5f },
            Max = new float[] { 0.5f, 0.5f, 0.5f },
        });

        public static BoundingBox box3 = new BoundingBox(new Accessor
        {
            Min = new float[] { -0.99f, -0.99f, -0.99f },
            Max = new float[] { 0.99f, 0.99f, 0.99f },
        });

        public static BoundingBox box4 = new BoundingBox(new Accessor
        {
            Min = new float[] { 1f, 1f, 1f },
            Max = new float[] { 2f, 2f, 2f },
        });

        public static BoundingBox box5 = new BoundingBox(new Accessor
        {
            Min = new float[] { 1f, 3f, 1f },
            Max = new float[] { 2f, 4f, 2f },
        });

        public static Triangle triangle1 = new Triangle()
        {
            A = new Vector3(0, 0, 10),
            B = new Vector3(10, 10, 0),
            C = new Vector3(20, 0, 10)
        };

        public static Triangle triangle2 = new Triangle()
        {
            A = new Vector3(0, 0, 10),
            B = new Vector3(10, 10, 0),
            C = new Vector3(20, 0, 10)
        };

        public static Triangle triangle3 = new Triangle()
        {
            A = new Vector3(0, 0, 10),
            B = new Vector3(10, 10, 0),
            C = new Vector3(10, 0, 10)
        };

        public static Triangle triangle4 = new Triangle()
        {
            A = new Vector3(1, 1, 11),
            B = new Vector3(11, 11, 11),
            C = new Vector3(11, 1, 11)
        };

        public static Ray ray1 = new Ray(new Vector3(10, 5, 0), new Vector3(10, 5, 10));
        public static Ray ray2 = new Ray(new Vector3(10, 5, 0), new Vector3(10, 5, 10));
        public static Ray ray3 = new Ray(new Vector3(10, 5, 0), new Vector3(10, 5, 10));

        public static Ray ray4 = new Ray(new Vector3(10, 5, 10), new Vector3(10, 5, 0));
        public static Ray ray5 = new Ray(new Vector3(10, 5, 10), new Vector3(10, 5, 0));
        public static Ray ray6 = new Ray(new Vector3(0, 0, 10), new Vector3(10, 10, 0));







    }
}
