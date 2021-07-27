using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Numerics;

namespace GS.Gltf.Collision
{
    
    public class CollisionResult
    {

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

        /// <summary>
        /// BB based on AABB intersecion
        /// </summary>
        public BoundingBox IntersectionBoundaties;

        /// <summary>
        /// collection of interseted triangles
        /// </summary>
        internal ConcurrentBag<TriangleCollision> Collisions;

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