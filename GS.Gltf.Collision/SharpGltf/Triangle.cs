using System;
using System.Numerics;

namespace GS.Gltf.Collision.SharpGltf
{
    public class Interval
    {
        public float min;
        public float max;

        public static Interval GetInterval(Triangle triangle, Vector3 axis)
        {
            Interval result = new Interval();
            result.min = Vector3.Dot(axis, triangle.A);
            result.max = result.min;

            float value = Vector3.Dot(axis, triangle.B);
            result.min = Math.Min(result.min, value);
            result.max = Math.Max(result.max, value);

            value = Vector3.Dot(axis, triangle.C);
            result.min = Math.Min(result.min, value);
            result.max = Math.Max(result.max, value);

            

            return result;
        }


    }

    public class Triangle
    {
        public Vector3 A;
        public Vector3 B;
        public Vector3 C;

        public Triangle(Vector3 a, Vector3 b, Vector3 c)
        {
            A = a;
            B = b;
            C = c;
        }

        public static bool OverlapOnAxis(Triangle t1, Triangle t2, Vector3 axis)
        {
            Interval a = Interval.GetInterval(t1, axis);
            Interval b = Interval.GetInterval(t2, axis);

            return ((b.min <= a.max) && (a.min <= b.max));
        }

        public static bool TriangleTriangle(Triangle t1, Triangle t2)
        {
            Vector3 t1_f0 = t1.B - t1.A;
            Vector3 t1_f1 = t1.C - t1.B;
            Vector3 t1_f2 = t1.A - t1.C;

            Vector3 t2_f0 = t2.B - t2.A;
            Vector3 t2_f1 = t2.C - t2.B;
            Vector3 t2_f2 = t2.A - t2.C;

            Vector3[] axisToTest =
            {
                Vector3.Cross(t1_f0, t1_f1),
                Vector3.Cross(t2_f0, t2_f1),
                Vector3.Cross(t2_f0, t1_f0),
                Vector3.Cross(t2_f0, t1_f1),
                Vector3.Cross(t2_f0, t1_f2),
                Vector3.Cross(t2_f1, t1_f0),
                Vector3.Cross(t2_f1, t1_f1),
                Vector3.Cross(t2_f1, t1_f2),
                Vector3.Cross(t2_f2, t1_f0),
                Vector3.Cross(t2_f2, t1_f1),
                Vector3.Cross(t2_f2, t1_f2),
            };

            for (int i = 0; i < 11; i++)
            {
                if (!OverlapOnAxis(t1, t2, axisToTest[i]))
                {
                    return false;
                }
            }
            return true;
        }
    }
}


