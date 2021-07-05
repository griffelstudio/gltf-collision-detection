using GS.Gltf.Collision.Geometry;
using GS.Gltf.Collision.Helper;
using GS.Gltf.Collision.Helpers;
using GS.Gltf.Collision.Interfaces;
using GS.Gltf.Collision.Models;
using SharpGLTF.Schema2;
using System;
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

        public List<CollisionIntermediateResult> Detect()
        {
            var reader = new GltfReader(settings.ModelPaths);
            rawModels = reader.RawModels;
            models = reader.Models;
            var modelCollisionPairs = MakeModelsCollisionPairs(models);
            var checkedModelCollisionPairs = CheckModelsCollisionPairs(modelCollisionPairs);
            var result = CheckElementCollisionPair(checkedModelCollisionPairs);
            result = result.Where(x => x.MinIntersectionBoundaries != null && x.Element1.Value != x.Element2.Value).ToList();
            SaveCollisionModels(result);
            
            return result;
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

        private List<CollisionIntermediateResult> CheckElementCollisionPair(List<Tuple<ModelData,ModelData>> pairs)
        {
            var result = new List<CollisionIntermediateResult>();
            //Parallel.ForEach(numbers, number =>
            //{
            //    if (IsPrime(number))
            //    {
            //        primeNumbers.Add(number);
            //    }
            //});
            //foreach (var pair in pairs)
            //{

            //    foreach (var element in pair.Item1.ElementMeshPrimitives)
            //    {
            //        foreach (var othElement in pair.Item2.ElementMeshPrimitives)
            //        {
            //            var indexPair = new KeyValuePair<string, string>(pair.Item1.modelIndex.ToString(),
            //                element.NodeName);
            //            var indexPair2 = new KeyValuePair<string, string>(pair.Item2.modelIndex.ToString(),
            //                othElement.NodeName);
            //            var collisionBoundingBox = element.GetBoundingBox().GetBigCollisionBoundingBox(othElement.GetBoundingBox());
            //            var triangleCollisions = CheckTriangleCollisions(element, othElement);
            //            var collision = new CollisionIntermediateResult(indexPair, indexPair2, collisionBoundingBox, triangleCollisions);
            //            result.Add(collision);
            //        }
            //    }

            //}
            Parallel.ForEach(pairs, pair =>
            {
                foreach (var element in pair.Item1.ElementMeshPrimitives)
                {
                    foreach (var othElement in pair.Item2.ElementMeshPrimitives)
                    {
                        if (element.NodeName != othElement.NodeName || !settings.InModelDetection) // filter element self collisions
                        {
                            var indexPair = new KeyValuePair<string, string>(pair.Item1.modelIndex.ToString(),
                            element.NodeName);
                            var indexPair2 = new KeyValuePair<string, string>(pair.Item2.modelIndex.ToString(),
                                othElement.NodeName);
                            var collisionBoundingBox = element.GetBoundingBox().GetBigCollisionBoundingBox(othElement.GetBoundingBox());
                            var triangleCollisions = CheckTriangleCollisions(element, othElement);
                            var collision = new CollisionIntermediateResult(indexPair, indexPair2, collisionBoundingBox, triangleCollisions);
                            result.Add(collision);
                        }
                        
                    }
                }
            });
            return result;
        }

        private List<TriangleCollision> CheckTriangleCollisions(Element e1, Element e2)
        {
            var result = new List<TriangleCollision>();

            //for (int i = 0; i < e1.Triangles.Count; i++)
            //{
            //    for (int j = 0; j < e2.Triangles.Count; j++)
            //    {
            //        var triange1 = e1.Triangles[i];
            //        var triange2 = e2.Triangles[j];

            //        var check = Triangle.TriangleTriangle(triange1, triange2);

            //        if (check)
            //        {
            //            var intersectionPoints = new List<Vector3>();
            //            foreach (var ray in triange1.GetEdgesRays())
            //            {
            //                var point = GeometryHelper.RaycastPoint(triange2, ray);
            //                if (point != new Vector3())
            //                {
            //                    intersectionPoints.Add(point);
            //                }


            //            }

            //            foreach (var ray in triange2.GetEdgesRays())
            //            {
            //                var point = GeometryHelper.RaycastPoint(triange1, ray);
            //                if (point != new Vector3())
            //                {
            //                    intersectionPoints.Add(point);
            //                }
            //            }

            //            result.Add(new TriangleCollision
            //            {
            //                ElementTriangle1 = i.ToString(),
            //                ElementTriangle2 = j.ToString(),
            //                IntersectionPoints = intersectionPoints,
            //            });
            //        }
            //    }
            //}

            Parallel.For(0, e1.Triangles.Count, (i, state) =>
            {
                 for (int j = 0; j < e2.Triangles.Count; j++)
                 {
                     var triange1 = e1.Triangles[i];
                     var triange2 = e2.Triangles[j];

                     var check = Triangle.TriangleTriangle(triange1, triange2);

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

                         result.Add(new TriangleCollision
                         {
                             ElementTriangle1 = i.ToString(),
                             ElementTriangle2 = j.ToString(),
                             IntersectionPoints = intersectionPoints,
                         });
                     }
                 }
            });

            return result;
        }

        private bool CheckCollision(ICollidable firstObject, ICollidable secondObject)
        {
            return firstObject.GetBoundingBox().IsCollideWith(secondObject.GetBoundingBox());
        }

        private void SaveCollisionModels(List<CollisionIntermediateResult> collisions)
        {
            if (settings.HiglightCollisions == CollisionHighlighing.None)
            {
                return;
            }
            else
            {
                if (settings.HiglightCollisions == CollisionHighlighing.SeparateFile)
                {
         
                }
                else
                {
                    if (settings.HiglightCollisions == CollisionHighlighing.MergeAll)
                    {
                        var mergedModel = GltfHelper.MergeModels(rawModels);
                        foreach (var collision in collisions)
                        {
                            mergedModel.AddCollisionBBNode(collision.MinIntersectionBoundaries);
                            mergedModel.SaveGLTF(Path.Combine(settings.OutputSavePath, "mergedtest.gltf"));
                        }
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
