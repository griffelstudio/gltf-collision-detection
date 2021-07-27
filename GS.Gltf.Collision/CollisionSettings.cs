using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace GS.Gltf.Collision
{
    public enum CollisionHighlighing
    {
        None,

        /// <summary>
        /// Red cubes are created in a separate file. only collision primitives
        /// </summary>
        SeparateFile,

        /// <summary>
        /// Merges all gltf files and writes collision primitives there.
        /// </summary>
        MergeAll,

        /// <summary>
        /// Merge all file by copying all resourses on text level
        /// </summary>
        FastMerge,

        AABB,
    }

    public class CollisionSettings
    {
        public List<string> ModelPaths {get; set;}

        /// <summary>
        /// Gets or sets value indicating if collision detector
        /// should find intersections between elements of the same model.
        /// </summary>
        public bool InModelDetection { get; set; } = false;

        public float Delta { get; set; } = CollisionConstants.Tolerance;
        /// <summary>
        /// collision save mode
        /// </summary>
        
        public CollisionHighlighing HiglightCollisions { get; set; } = CollisionHighlighing.None;

        /// <summary>
        /// Path to directory where collison result will be saved
        /// </summary>
        public string OutputSavePath = Path.Combine("C:", "gltf", "testFastMerge");

        /// <summary>
        /// perform collicions by triangles
        /// </summary>
        public bool CheckTriangles { get; } = true;

        public CollisionSettings(List<string> modelPaths)
        {
            if (modelPaths is null)
            {
                return;
            }

            modelPaths = modelPaths.Where(path => File.Exists(path)).ToList();


            if (modelPaths.Count == 0)
            {
                throw new InvalidOperationException("There aren't any valid path in list.");
            }

            if (modelPaths.Count == 1)
            {
                InModelDetection = true;
            }

            ModelPaths = modelPaths;
        }
    }
}
