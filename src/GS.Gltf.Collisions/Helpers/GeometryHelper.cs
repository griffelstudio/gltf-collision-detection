using GS.Gltf.Collisions.Geometry;
using System;
using System.Numerics;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("GS.Gltf.Collisions.Tests")]
namespace GS.Gltf.Collisions.Helpers
{
    internal static class GeometryHelper
    {
        public static Vector3 ClosestPoint(Line3D line, Vector3 point)
        {
            Vector3 lVec = line.end - line.start;

            float t = Vector3.Dot(point - line.start, lVec) / Vector3.Dot(lVec, lVec);
            t = Math.Max(t, 0.0f);
            t = Math.Min(t, 0.0f);
            return line.start + lVec * t;

        }

        public static bool PointInBB(BoundingBox box, Vector3 point)
        {
            if (point.X < box.Min.X || point.Y < box.Min.Y || point.Z < box.Min.Z)
            {
                return false;
            }
            if (point.X > box.Max.X || point.Y > box.Max.Y || point.Z > box.Max.Z)
            {
                return false;
            }
            return true;
        }

        public static bool TriangleInBB(BoundingBox box, Triangle triangle)
        {
            Vector3 f0 = triangle.B - triangle.A;
            Vector3 f1 = triangle.C - triangle.B;
            Vector3 f2 = triangle.A - triangle.C;

            Vector3 u0 = new Vector3(1.0f, 0.0f, 0.0f);
            Vector3 u1 = new Vector3(0.0f, 1.0f, 0.0f);
            Vector3 u2 = new Vector3(0.0f, 0.0f, 1.0f);

            Vector3[] test =
            {
                u0,
                u1,
                u2,
                Vector3.Cross(f0,f1),
                Vector3.Cross(u0,f0),
                Vector3.Cross(u0,f1),
                Vector3.Cross(u0,f2),
                Vector3.Cross(u1,f0),
                Vector3.Cross(u1,f1),
                Vector3.Cross(u1,f2),
                Vector3.Cross(u2,f0),
                Vector3.Cross(u2,f1),
                Vector3.Cross(u2,f2),
            };

            for (int i = 0; i < 13; i++)
            {
                if (!OverlapOnAxis(box,triangle,test[i]))
                {
                    return false;
                }
            }
            return true;
        }

        public static bool OverlapOnAxis(BoundingBox bb, Triangle triangle, Vector3 axis)
        {
            Interval a = Interval.GetInterval(bb, axis);
            Interval b = Interval.GetInterval(triangle, axis);

            return ((b.min <= a.max) && (a.min <= b.max)); //TODO delta
        }

        public static float MagnitudeSq(Vector3 v)
        {
            return Vector3.Dot(v, v);
        }

        public static bool PointOnLine(Vector3 point, Line3D line)
        {
            Vector3 closest = ClosestPoint(line, point);
            float distanceSq = MagnitudeSq(closest - point);
            return distanceSq == 0.0f;

        }

        public static float Raycast(Geometry.Plane plane, Ray ray)
        {
            float nd = Vector3.Dot(ray.direction, plane.normal);
            float pn = Vector3.Dot(ray.origin, plane.normal);

            if (nd >= 0.0f)
            {
                return -1;
            }

            float t = (plane.distanse - pn) / nd;

            if (t >= 0.0f)
            {
                return t;
            }

            return -1;
        }

        public static Vector3 Project(Vector3 lenth, Vector3 direction)
        {
            float dot = Vector3.Dot(lenth, direction);
            float magSq = MagnitudeSq(direction);
            return direction * (dot / magSq);
        }

        public static Vector3 Barycentric(Vector3 point, Triangle triangle)
        {
            Vector3 ap = point - triangle.A;
            Vector3 bp = point - triangle.B;
            Vector3 cp = point - triangle.C;

            Vector3 ab = triangle.B - triangle.A;
            Vector3 ac = triangle.C - triangle.A;
            Vector3 bc = triangle.C - triangle.B;
            Vector3 cb = triangle.B - triangle.C;
            Vector3 ca = triangle.A - triangle.C;

            Vector3 v = ab - Project(ab, cb);
            float a = 1.0f - (Vector3.Dot(v, ap)) / Vector3.Dot(v, ab);
            v = bc - Project(bc, ac);
            float b = 1.0f - (Vector3.Dot(v, bp)) / Vector3.Dot(v, bc);
            v = ca - Project(ca, ab);
            float c = 1.0f - (Vector3.Dot(v, cp)) / Vector3.Dot(v, ca);

            return new Vector3(a, b, c);

        }

        public static float Magnitude(Vector3 vec)
        {
            var d = Vector3.Dot(vec, vec);
            var sq = Math.Sqrt(d);
            return (float)sq; // is valid cast?
        }

        public static Vector3 Normalized(Vector3 vec)
        {
            var m = Magnitude(vec);
            return vec * (1.0f / m);
        }

        public static Geometry.Plane FromTriangle(Triangle triangle)
        {
            var plane = new Geometry.Plane();
            plane.normal = Normalized(Vector3.Cross(triangle.B - triangle.A, triangle.C - triangle.A));
            plane.distanse = Vector3.Dot(plane.normal, triangle.A);
            return plane;
        }

        public static float Raycast(Triangle triangle, Ray ray)
        {
            Geometry.Plane plane = FromTriangle(triangle);
            float t = Raycast(plane, ray);
            if (t < 0.0f)
            {
                return t;
            }
            Vector3 result = ray.origin + ray.direction * t;

            Vector3 barycentric = Barycentric(result, triangle);
            if (barycentric.X >= 0.0f && barycentric.X <= 1.0f &&
                barycentric.Y >= 0.0f && barycentric.Y <= 1.0f &&
                barycentric.Z >= 0.0f && barycentric.Z <= 1.0f)
            {
                return t;
            }
            return -1;
        }

        public static Vector3 RaycastPoint(Triangle triangle, Ray ray)
        {
            Geometry.Plane plane = FromTriangle(triangle);
            float t = Raycast(plane, ray);
            if (t < 0.0f)
            {
                return new Vector3();
            }
            Vector3 result = ray.origin + ray.direction * t;

            Vector3 barycentric = Barycentric(result, triangle);
            if (barycentric.X >= 0.0f && barycentric.X <= 1.0f &&
                barycentric.Y >= 0.0f && barycentric.Y <= 1.0f &&
                barycentric.Z >= 0.0f && barycentric.Z <= 1.0f)
            {
                return result;
            }
            return new Vector3();
        }
    }
}
