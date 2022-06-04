using GS.Gltf.Collisions.Geometry;
using GS.Gltf.Collisions.Helper;
using GS.Gltf.Collisions.Helpers;
using GS.Gltf.Collisions.Interfaces;
using GS.Gltf.Collisions.Models;
using Microsoft.Extensions.Logging;
using SharpGLTF.Schema2;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Threading;
using System.Threading.Tasks;

namespace GS.Gltf.Collisions
{
    /// <summary>
    /// Main class of the library which processes files, finds collisions and produces output according to the <see cref="CollisionSettings"/> defined.
    /// </summary>
    public class CollisionDetector
    {
        private List<ModelData> models;
        private List<ModelRoot> rawModels;
        private ILogger logger;

        /// <summary>
        /// Creates an instance of the <see cref="CollisionDetector"/> class
        /// </summary>
        /// <param name="settings">Settings, according to which files are being processed and output is calculated.</param>
        /// <param name="logger">Optional logger.</param>
        public CollisionDetector(CollisionSettings settings, ILogger logger = null)
        {
            this.Settings = settings;
            this.logger = logger;
        }

        /// <summary>
        /// Settings, according to which files are being processed and output is calculated.
        /// </summary>
        public CollisionSettings Settings { get; }

        /// <summary>
        /// Starts collision detection with the <see cref="CollisionSettings"/> defined.
        /// </summary>
        /// <returns></returns>
        public List<CollisionResult> Detect()
        {
            logger.LogInformation("Start collision detection.\n");
            logger.LogInformation("Finding elements to check for collisions...\n");

            var reader = new GltfReader(Settings.ModelPaths);
            rawModels = reader.RawModels;
            models = reader.Models;
            var modelCollisionPairs = MakeModelsCollisionPairs(models);
            var checkedModelCollisionPairs = CheckModelsCollisionPairs(modelCollisionPairs); //TODO make parallel?
            logger.LogInformation("Detecting collisions...\n");
            var result = CheckElementCollisionPair(checkedModelCollisionPairs);

            List<CollisionResult> new_result = default;
            new_result = result.ToList().Where(x => x.MinIntersectionBoundaries != null && x.Element1.Value != x.Element2.Value).ToList();
            logger.LogInformation("Writing results...\n");
            SaveCollisionModels(new_result);
            logger.LogInformation($"Result saved to {Path.Combine(Settings.OutputSavePath, Settings.OutputFilename)}\n");
            logger.LogInformation("Finish collision detection.\n");

            return result.ToList();
        }

        private Dictionary<ModelData, ModelData> MakeModelsCollisionPairs(List<ModelData> models)
        {
            var res = new Dictionary<ModelData, ModelData>();
            for (int i = 0; i < models.Count; i++)
            {
                for (int j = i; j < models.Count; j++)
                {
                    if (i == j && !Settings.InModelDetection)
                    {
                        continue;
                    }

                    res[models[i]] = models[j];
                }
            }

            return res;
        }

        private Dictionary<ModelData, ModelData> CheckModelsCollisionPairs(Dictionary<ModelData, ModelData> pairs)
        {
            return pairs
                .Where(pair => CheckCollision(pair.Key, pair.Value))
                .ToDictionary(p => p.Key, p => p.Value);
        }

        private ConcurrentBag<CollisionResult> CheckElementCollisionPair(Dictionary<ModelData, ModelData> pairs)
        {
            var result = new ConcurrentBag<CollisionResult>();
            List<Tuple<string, string>> a = new List<Tuple<string, string>>();
            //var errors = new ConcurrentBag<string>();
            int count = 0;
            var totalPairs = pairs.Select(p => p.Key.ElementMeshPrimitives.Count * p.Value.ElementMeshPrimitives.Count).Sum();
            Parallel.ForEach(pairs, pair =>
            {
                foreach (var element in pair.Key.ElementMeshPrimitives)
                {
                    Parallel.ForEach(pair.Value.ElementMeshPrimitives, othElement =>
                    {
                        bool isElemsCollide = element.GetBoundingBox().IsCollideWith(othElement.GetBoundingBox(), Settings.Delta);
                        Interlocked.Increment(ref count);
                        logger.LogInformation($"\r\t{count} meshes from {totalPairs} checked");

                        if (isElemsCollide)
                        {

                            if (element.NodeName != othElement.NodeName || !Settings.InModelDetection) // filter element self collisions
                            {
                                var indexPair = new KeyValuePair<string, string>(pair.Key.modelIndex.ToString(),
                                element.NodeName);
                                var indexPair2 = new KeyValuePair<string, string>(pair.Value.modelIndex.ToString(),
                                    othElement.NodeName);
                                var collisionBoundingBox = element.GetBoundingBox().GetBigCollisionBoundingBox(othElement.GetBoundingBox());
                                ConcurrentBag<TriangleCollision> triangleCollisions = null;
                                if (Settings.CheckTriangles)
                                {
                                    triangleCollisions = CheckTriangleCollisions(element, othElement);
                                }
                                var intersectionBB = element.BoundingBox.GetCollisionBoundingBox(othElement.BoundingBox);
                                var collision = new CollisionResult(indexPair, indexPair2, collisionBoundingBox, intersectionBB, triangleCollisions);
                                result.Add(collision);
                            }
                        }
                    });

                }
            });
            logger.LogInformation($"\r\t{totalPairs} meshes from {totalPairs} checked\n");
            return result;
        }


        private ConcurrentBag<TriangleCollision> CheckTriangleCollisions(Element e1, Element e2)
        {
            var result = new ConcurrentBag<TriangleCollision>();
            var intersectionBB = e1.BoundingBox.GetCollisionBoundingBox(e2.BoundingBox);
            var firstElemCollidedTriangles = new ConcurrentBag<Triangle>();
            var secondElemCollidedTriangles = new ConcurrentBag<Triangle>();
            Parallel.ForEach(e1.Triangles, triangle =>
            {
                if (GeometryHelper.TriangleInBB(intersectionBB,triangle))
                {
                    firstElemCollidedTriangles.Add(triangle);
                }
            });

            Parallel.ForEach(e2.Triangles, triangle =>
            {
                if (GeometryHelper.TriangleInBB(intersectionBB, triangle))
                {
                    secondElemCollidedTriangles.Add(triangle);
                }
            });

            var firstElementTriangles = firstElemCollidedTriangles.ToList();
            var secondElementTriangles = secondElemCollidedTriangles.ToList();
            Parallel.For(0, firstElementTriangles.Count, (i, state) =>
            {
                for (int j = 0; j < secondElementTriangles.Count; j++)
                //Parallel.For(0, secondElementTriangles.Count, (j, state) =>
                 {
                     var triange1 = firstElementTriangles[i];
                     var triange2 = secondElementTriangles[j];

                     var check = Triangle.TriangleTriangle(triange1, triange2); //TODO make out isCoplanar param

                     if (check)
                     {
                         var intersectionPoints = new List<Vector3>();
                         foreach (var ray in triange1.GetEdgesRays())
                         {
                             var point = GeometryHelper.RaycastPoint(triange2, ray);
                             if (point != new Vector3())
                             {
                                 intersectionPoints.Add(point);
                             }
                         }

                         foreach (var ray in triange2.GetEdgesRays())
                         {
                             var point = GeometryHelper.RaycastPoint(triange1, ray);
                             if (point != new Vector3())
                             {
                                 intersectionPoints.Add(point);
                             }
                         }

                         if (intersectionPoints.Count > 0)
                         {
                             result.Add(new TriangleCollision
                             {
                                 IntersectionPoints = intersectionPoints,
                             });
                         }
                     }

                 }
            });
            return result;
        }

        private bool CheckCollision(ICollidable firstObject, ICollidable secondObject)
        {
            return firstObject.GetBoundingBox().IsCollideWith(secondObject.GetBoundingBox(), Settings.Delta);
        }

        private void SaveCollisionModels(List<CollisionResult> collisions)
        {
            var model = Settings.OutputMode switch
            {
                OutputMode.InMemory => rawModels.First(),
                OutputMode.MergeAll => GltfHelper.MergeModels(rawModels),
                OutputMode.SeparateFile => GltfHelper.CreateCleanModel(),
            };

            foreach (var collision in collisions)
            {
                model.AddCollisionBBNode(collision.MinIntersectionBoundaries);
            }

            SaveModel(model);
        }

        private void SaveModel(ModelRoot model)
        {
            if (Settings.OutputMode == OutputMode.InMemory)
            {
                return;
            }

            Directory.CreateDirectory(Settings.OutputSavePath);
            var fullPath = Path.Combine(Settings.OutputSavePath, Settings.OutputFilename);
            if (Path.GetExtension(Settings.OutputFilename).Equals(CollisionConstants.GlbFileExtension))
            {
                model.SaveGLB(fullPath);
            }
            else
            {
                model.SaveGLTF(fullPath);
            }
        }
    }
}
