using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace GS.Gltf.Collision
{
    public enum OutputMode
    {
        /// <summary>
        /// Don't create any output files, just keep results in memory.
        /// </summary>
        InMemory,

        /// <summary>
        /// Red cubes are created in a separate file. only collision primitives
        /// </summary>
        SeparateFile,

        /// <summary>
        /// Merges all gltf files and writes collision primitives there.
        /// </summary>
        MergeAll,
    }

    /// <summary>
    /// Settings applied to check collisions.
    /// </summary>
    public class CollisionSettings
    {
        /// <summary>
        /// Paths to existing glTF files which are inter-checked for collisions.
        /// </summary>
        public List<string> ModelPaths {get; }

        private bool inModelDetection = false;

        /// <summary>
        /// Value indicating if collision detector should find intersections between elements of the same model.
        /// </summary>
        public bool InModelDetection
        {
            get => ModelPaths.Count > 1 ? inModelDetection : true;
            set => inModelDetection = value;
        }

        /// <summary>
        /// Minimum distance between points to consider elements intersecting.
        /// </summary>
        public float Delta { get; set; } = CollisionConstants.Tolerance;

        /// <summary>
        /// Output mode.
        /// </summary>
        public OutputMode OutputMode { get; set; } = OutputMode.MergeAll;

        // TODO Get rid of this property.
        [Obsolete("OutputFilename is enough")]
        /// <summary>
        /// Path to directory where collision result will be saved
        /// </summary>
        public string OutputSavePath { get; set; } = Path.Combine("C:", "gltf", "testFastMerge");

        /// <summary>
        /// Output file name.
        /// </summary>
        public string OutputFilename { get; set; } = "result.gltf";

        /// <summary>
        /// Perform collision detections on triangles or just check bounding boxes.
        /// </summary>
        public bool CheckTriangles { get; set; } = true;

        // TODO Implement this option.
        public int MaxDegreeOfParallelism
        {
            get;
            set;
        }

        /// <summary>
        /// Settings applied to check collisions.
        /// </summary>
        /// <param name="modelPaths">Paths to existing glTF files which are inter-checked for collisions.</param>
        /// <exception cref="System.InvalidOperationException">Thrown if there is no any existing files.</exception>
        public CollisionSettings(List<string> modelPaths, ILogger logger = null)
        {
            if (modelPaths is null)
            {
                return;
            }

            var realFiles = modelPaths.Where(path => File.Exists(path)).ToList();

            if (realFiles.Count == 0)
            {
                throw new InvalidOperationException("There is no any valid path in the list.");
            }

            if (realFiles.Count < modelPaths.Count)
            {
                // TODO Specify which files won't be calculated during the collision detection.
                logger?.LogWarning("Some files won't be calculated.");
            }

            ModelPaths = realFiles;
        }
    }
}
