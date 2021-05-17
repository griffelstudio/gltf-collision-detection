using glTFLoader.Schema;
using System;
using System.Collections.Generic;
using System.Text;

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





    }
}
