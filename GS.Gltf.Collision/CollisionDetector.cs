using GS.Gltf.Collision.Geometry;
using GS.Gltf.Collision.Helper;
using GS.Gltf.Collision.Helpers;
using GS.Gltf.Collision.Interfaces;
using GS.Gltf.Collision.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading;

namespace GS.Gltf.Collision
{
    public class CollisionDetector
    {
        private CollisionSettings settings { get; }
        private List<ModelData> models;

        public CollisionDetector(CollisionSettings settings)
        {
            this.settings = settings;
        }


        public CollisionIntermediateResult Detect1()
        {
            //var models = settings.ModelPaths...

            // Формируем пары для пересечений.
            // Если только одна модель - то создается копия модели и формируется одна пара с идентичными моделями.

            // После того как метод Detect1(ниже) нашел коллизии
            // в зависимости от настроек мы мержим файлы, дорисовываем кубики и т.д.

            // кык красиво дорисовывать кубики:
            // создадим отдельный хелпер, который ест CollisionIntermediateResult и создаёт необходимые файлы.

            // Вызывать проверку Detect1 асинхронно.

            return null;
        }

        private CollisionIntermediateResult Detect1(ModelData one, ModelData another)
        {
            // Где-то внутри будет ещё один асинхронный вызов проверки коллизий треугольников.

            return null;
        }

        public List<CollisionIntermediateResult> Detect()
        {
            var reader = new GltfReader(settings.ModelPaths);
            models = reader.Models;
            var modelCollisionPairs = MakeModelsCollisionPairs(models);
            var checkedModelCollisionPairs = CheckModelsCollisionPairs(modelCollisionPairs);
            var result = CheckElementCollisionPair(checkedModelCollisionPairs);
            
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
            foreach (var pair in pairs)
            {
                
                foreach (var element in pair.Item1.ElementMeshPrimitives)
                {
                    foreach (var othElement in pair.Item2.ElementMeshPrimitives)
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
            return result;
        }

        private List<TriangleCollision> CheckTriangleCollisions(Element e1, Element e2)
        {
            var result = new List<TriangleCollision>();

            for (int i = 0; i < e1.Triangles.Count; i++)
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
            }
            return result;
        }

        private bool CheckCollision(ICollidable firstObject, ICollidable secondObject)
        {
            return firstObject.GetBoundingBox().IsCollideWith(secondObject.GetBoundingBox());
        }

    }



}
