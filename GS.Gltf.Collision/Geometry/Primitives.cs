using System;
using System.Collections.Generic;
using System.Text;
using System.Numerics;

namespace GS.Gltf.Collision.Geometry
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

        public Triangle()
        {
        }

        public Triangle(Vector3 a, Vector3 b, Vector3 c)
        {
            A = RoundVector(a);
            B = RoundVector(b);
            C = RoundVector(c);
        }

        public Vector3 RoundVector(Vector3 vec)
        {
            int round = 2;
            return new Vector3((float)Math.Round(vec.X, round), (float)Math.Round(vec.Y, round), (float)Math.Round(vec.Z, round));
        }

        public static bool OverlapOnAxis(Triangle t1, Triangle t2, Vector3 axis)
        {
            Interval a = Interval.GetInterval(t1, axis);
            Interval b = Interval.GetInterval(t2, axis);

            return ((b.min <= a.max) && (a.min <= b.max));
        }

        public List<Ray> GetEdgesRays()
        {
            var result = new List<Ray>();
            result.Add(new Ray(A, B));
            result.Add(new Ray(B, C));
            result.Add(new Ray(C, A));
            result.Add(new Ray(B, A));
            result.Add(new Ray(C, B));
            result.Add(new Ray(A, C));
            return result;
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

    public class Line3D
    {
        public Vector3 start;
        public Vector3 end;
    }

    public class Plane
    {
        public Vector3 normal;
        public float distanse;
    }

    public class Ray
    {
        public Vector3 origin;
        public Vector3 direction;

        public Ray(Vector3 from, Vector3 to)
        {
            origin = from;
            direction = HelperUtils.Normalized(to - from);
        }
    }
}
