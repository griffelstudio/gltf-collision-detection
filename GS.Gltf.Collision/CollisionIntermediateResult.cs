using System.Collections.Generic;
using System.Numerics;

namespace GS.Gltf.Collision
{
    
    public class CollisionIntermediateResult
    {
        /// <summary>
        /// path to model
        /// </summary>
        public string Path { get; }

        /// <summary>
        /// key defines model intex and value defines node name
        /// </summary>
        public KeyValuePair<string, string> Element1;

        /// <summary>
        /// key defines model intex and value defines node name
        /// </summary>
        public KeyValuePair<string, string> Element2;

        /// <summary>
        /// Max AABB
        /// </summary>
        public BoundingBox Boundaries;

        /// <summary>
        /// BB based on points of intersection
        /// </summary>
        public BoundingBox MinIntersectionBoundaries;

        // TODO Это должно формироваться в отдельном месте изнутри CollisionDetector при необходимости.
        /// <summary>
        /// collection of interseted triangles
        /// </summary>
        public List<TriangleCollision> Collisions;

        public CollisionIntermediateResult(KeyValuePair<string, string> element1, KeyValuePair<string, string> element2, BoundingBox boundaries, List<TriangleCollision> collisions)
        {
            Element1 = element1;
            Element2 = element2;
            Boundaries = boundaries;
            Collisions = collisions;

            var collisionPoints = new List<Vector3>();
            foreach (var collision in Collisions)
            {
                collisionPoints.AddRange(collision.IntersectionPoints);
            }
            if (collisionPoints.Count > 0)
            {
                MinIntersectionBoundaries = new BoundingBox(collisionPoints);
            }
        }
    }

    internal class CollisionResult
    {
        public string Path { get; }
    }
}