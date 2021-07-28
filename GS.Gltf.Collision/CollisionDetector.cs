using GS.Gltf.Collision.Geometry;
using GS.Gltf.Collision.Helper;
using GS.Gltf.Collision.Helpers;
using GS.Gltf.Collision.Interfaces;
using GS.Gltf.Collision.Models;
using SharpGLTF.Schema2;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Threading;
using System.Threading.Tasks;

namespace GS.Gltf.Collision
{
    public class CollisionDetector
    {
        private CollisionSettings settings { get; }
        private List<ModelData> models;
        private List<ModelRoot> rawModels;

        public CollisionDetector(CollisionSettings settings)
        {
            this.settings = settings;
        }

        public List<CollisionResult> Detect()
        {
            var reader = new GltfReader(settings.ModelPaths);
            rawModels = reader.RawModels;
            models = reader.Models;
            var modelCollisionPairs = MakeModelsCollisionPairs(models);
            var checkedModelCollisionPairs = CheckModelsCollisionPairs(modelCollisionPairs);
            var result = CheckElementCollisionPair(checkedModelCollisionPairs);
            List<CollisionResult> new_result = null;
            if(settings.HiglightCollisions == CollisionHighlighing.AABB)
            {
                new_result = result.ToList();
            }
            else
            {
                new_result = result.ToList().Where(x => x.MinIntersectionBoundaries != null && x.Element1.Value != x.Element2.Value).ToList();
            }
            
            SaveCollisionModels(new_result);
            
            return result.ToList();
        }

        private List<Tuple<ModelData, ModelData>> MakeModelsCollisionPairs(List<ModelData> models)
        {
            var result = new List<Tuple<ModelData, ModelData>>();
            //combinations by 2 without mirror copies
            for (int i = 0; i < models.Count; i++)
            {
                for (int j = i; j < models.Count; j++)
                {
                    var tuple = new Tuple<ModelData, ModelData>(models[i], models[j]);
                    if (i != j)
                    {
                        result.Add(tuple);
                    }
                    else
                    {
                        if (settings.InModelDetection)
                        {
                            result.Add(tuple);
                        }
                    }
                    
                }
            }
            return result;
        }

        private List<Tuple<ModelData, ModelData>> CheckModelsCollisionPairs(List<Tuple<ModelData,ModelData>> pairs)
        {
            return pairs.Where(pair => CheckCollision(pair.Item1, pair.Item2)).ToList();
        }

        private ConcurrentBag<CollisionResult> CheckElementCollisionPair(List<Tuple<ModelData,ModelData>> pairs)
        {
            var result = new ConcurrentBag<CollisionResult>();
            List<Tuple<string, string>> a = new List<Tuple<string, string>>();
            Parallel.ForEach(pairs, pair =>
            {
                foreach (var element in pair.Item1.ElementMeshPrimitives)
                {
                    Parallel.ForEach(pair.Item2.ElementMeshPrimitives, othElement =>
                    {
                        bool isElemsCollide = element.GetBoundingBox().IsCollideWith(othElement.GetBoundingBox(), settings.Delta);
                        if (isElemsCollide)
                        {

                            if (element.NodeName != othElement.NodeName || !settings.InModelDetection) // filter element self collisions
                            {
                                var indexPair = new KeyValuePair<string, string>(pair.Item1.modelIndex.ToString(),
                                element.NodeName);
                                var indexPair2 = new KeyValuePair<string, string>(pair.Item2.modelIndex.ToString(),
                                    othElement.NodeName);
                                var collisionBoundingBox = element.GetBoundingBox().GetBigCollisionBoundingBox(othElement.GetBoundingBox());
                                ConcurrentBag<TriangleCollision> triangleCollisions = null;
                                if (settings.CheckTriangles)
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
                Parallel.For(0, secondElementTriangles.Count, (j, state) =>
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
                         else
                         {
                             
                         }
                         
                     }
                 });
            });

            return result;
        }

        private bool CheckCollision(ICollidable firstObject, ICollidable secondObject)
        {
            return firstObject.GetBoundingBox().IsCollideWith(secondObject.GetBoundingBox(), settings.Delta);
        }

        private void SaveCollisionModels(List<CollisionResult> collisions)
        {
            if (settings.HiglightCollisions == CollisionHighlighing.None && settings.InModelDetection)
            {
                var model = rawModels.First();
                foreach (var collision in collisions)
                {
                    model.AddCollisionBBNode(collision.MinIntersectionBoundaries);
                    model.SaveGLTF(Path.Combine(settings.OutputSavePath, "inModelSave.gltf"));
                }

            }
            else
            {
                if (settings.HiglightCollisions == CollisionHighlighing.SeparateFile)
                {
                    var model = GltfHelper.CreateCleanModel();
                    foreach (var collision in collisions)
                    {
                        model.AddCollisionBBNode(collision.MinIntersectionBoundaries);
                    }
                    model.SaveGLTF(Path.Combine(settings.OutputSavePath, "cleantest.gltf"));
                }
                else
                {
                    if (settings.HiglightCollisions == CollisionHighlighing.MergeAll)
                    {
                        var mergedModel = GltfHelper.MergeModels(rawModels);
                        foreach (var collision in collisions)
                        {
                            mergedModel.AddCollisionBBNode(collision.MinIntersectionBoundaries);
                        }
                        mergedModel.SaveGLTF(Path.Combine(settings.OutputSavePath, "mergedtest.gltf"));
                    }
                    else
                    {
                        if (settings.HiglightCollisions == CollisionHighlighing.FastMerge)
                        {
                            Directory.CreateDirectory(settings.OutputSavePath);
                            var mergedModelpath = Path.Combine(settings.OutputSavePath, "mergedtest.gltf");
                            GltfHelper.Merge(mergedModelpath, settings.ModelPaths);
                            var reader = new GltfReader();
                            var mergedModel = reader.LoadGltfFromFile(mergedModelpath);
                            foreach (var collision in collisions)
                            {
                                mergedModel.AddCollisionBBNode(collision.MinIntersectionBoundaries);
                            }
                            mergedModel.SaveGLTF(mergedModelpath);
                        }
                        else
                        {
                            if (settings.HiglightCollisions == CollisionHighlighing.AABB)
                            {
                                var mergedModel = GltfHelper.MergeModels(rawModels);
                                foreach (var collision in collisions)
                                {
                                    mergedModel.AddCollisionBBNode(collision.IntersectionBoundaties);
                                }
                                SaveModel(mergedModel);
                            }
                            else
                            {
                                throw new ArgumentException("Invalid Highlighing mode");
                            }
                            
                        }
                    }
                }
            }
        }

        private void SaveModel(ModelRoot model)
        {
            Directory.CreateDirectory(settings.OutputSavePath);
            model.SaveGLTF(Path.Combine(settings.OutputSavePath, "mergedtest.gltf"));
        }

    }



}
