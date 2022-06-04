using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Numerics;

namespace GS.Gltf.Collision
{
    /// <summary>
    /// Class which describes a single geometry intersection.
    /// </summary>
    public class CollisionResult
    {
        /// <summary>
        /// Key defines model index and value defines node name.
        /// </summary>
        public KeyValuePair<string, string> Element1 { get; set; }

        /// <summary>
        /// Key defines model index and value defines node name.
        /// </summary>
        public KeyValuePair<string, string> Element2 { get; set; }

        /// <summary>
        /// Max AABB.
        /// </summary>
        public BoundingBox Boundaries { get; set; }

        /// <summary>
        /// BB based on points of intersection.
        /// </summary>
        public BoundingBox MinIntersectionBoundaries { get; set; }

        /// <summary>
        /// BB based on AABB intersection.
        /// </summary>
        public BoundingBox IntersectionBoundaties { get; set; }

        /// <summary>
        /// Collection of intersected triangles.
        /// </summary>
        internal ConcurrentBag<TriangleCollision> Collisions { get; set; }

        internal CollisionResult(KeyValuePair<string, string> element1, KeyValuePair<string, string> element2, BoundingBox boundaries,
            BoundingBox intersecionBoundaries, ConcurrentBag<TriangleCollision> collisions)
        {
            Element1 = element1;
            Element2 = element2;
            Boundaries = boundaries;
            Collisions = collisions;
            IntersectionBoundaties = intersecionBoundaries;

            var collisionPoints = new List<Vector3>();
            foreach (var collision in Collisions)
            {
                if (collision != null)
                {
                    collisionPoints.AddRange(collision.IntersectionPoints);
                }
            }

            if (collisionPoints.Count > 0)
            {
                MinIntersectionBoundaries = new BoundingBox(collisionPoints);
            }
        }
    }
}