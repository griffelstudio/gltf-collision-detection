using GS.Gltf.Collision.Geometry;
using GS.Gltf.Collision.Helper;
using GS.Gltf.Collision.Helpers;
using GS.Gltf.Collision.Interfaces;
using GS.Gltf.Collision.Models;
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


        public CollisionResult Detect1()
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








        public List<CollisionElement> Detect()
        {
            var reader = new GltfReader(settings.ModelPaths);
            models = reader.Models;
            if (settings.InModelDetection)
            {
                foreach (var model in models)
                {
                    model.InterModelCollisions = CheckCollisionsIntoModel(model);
                }
                
            }

            var modelCollisionPairs = MakeModelsCollisionPairs(models);
            var checkedModelCollisionPairs = CheckModelsCollisionPairs(modelCollisionPairs);

            List<CollisionElement> checkedNodesCollisionPairs = null;
            if (settings.ModelPaths.Count > 1)
            {
                checkedNodesCollisionPairs = MakeAndCheckElementCollisionPair(checkedModelCollisionPairs);
            }


            return checkedNodesCollisionPairs;
        }

        // Fufufu
        private List<CollisionElement> CheckCollisionsIntoModel(ModelData model)
        {
            
            var result = new List<CollisionElement>();
            for (int i = 0; i < model.ElementMeshPrimitives.Count; i++)
            {
                for (int j = i; j < model.ElementMeshPrimitives.Count; j++)
                {
                    if (i != j)
                    {
                        var firstNode = model.ElementMeshPrimitives[i];
                        var secondNode = model.ElementMeshPrimitives[j];

                        var IsElementCollide = CheckCollision(firstNode, secondNode);

                        if (IsElementCollide)
                        {
                            var indexPair = new KeyValuePair<string, string>(model.modelIndex.ToString(),firstNode.NodeName);
                            var indexPair2 = new KeyValuePair<string, string>(model.modelIndex.ToString(),secondNode.NodeName);
                            var z = firstNode.BoundingBox.GetBigCollisionBoundingBox(secondNode.BoundingBox);
                            var triangleCollisions = CheckTriangleCollisions(firstNode, secondNode);
                            var collision = new CollisionElement(indexPair, indexPair2, z, triangleCollisions);
                            result.Add(collision);
                        }
                        

                    }

                }
            }
            result = result.Where(n => n.MinIntersectionBoundaries != null).ToList();
            return result;
        }

        // Tuple<ModelData, ModelData> 
        private List<ModelsCollisionPair> MakeModelsCollisionPairs(List<ModelData> models)
        {
            var result = new List<ModelsCollisionPair>();
            //combinations by 2 without combinations and mirror copies
            for (int i = 0; i < models.Count; i++)
            {
                for (int j = i; j < models.Count; j++)
                {
                    if (i != j)
                    {
                        result.Add(new ModelsCollisionPair()
                        {
                            firstModel = models[i],
                            secondModel = models[j]
                        });
                    }
                }
            }
            return result;
        }

        private List<ModelsCollisionPair> CheckModelsCollisionPairs(List<ModelsCollisionPair> pairs) // or ref
        {
            foreach (var pair in pairs)
            {
                if (CheckCollision(pair.firstModel, pair.secondModel))
                {
                    pair.IsModelCollide = true;
                }
            }
            return pairs;
        }

        private List<CollisionElement> MakeAndCheckElementCollisionPair(List<ModelsCollisionPair> pairs)
        {
            var result = new List<CollisionElement>();
            foreach (var pair in pairs)
            {
                if (pair.IsModelCollide)
                {
                    foreach (var element in pair.firstModel.ElementMeshPrimitives)
                    {
                        foreach (var othElement in pair.secondModel.ElementMeshPrimitives)
                        {
                            var indexPair = new KeyValuePair<string, string>(pair.firstModel.modelIndex.ToString(),
                                element.NodeName);
                            var indexPair2 = new KeyValuePair<string, string>(pair.secondModel.modelIndex.ToString(),
                                othElement.NodeName);
                            var collisionBoundingBox = element.GetBoundingBox().GetBigCollisionBoundingBox(othElement.GetBoundingBox());
                            var triangleCollisions = CheckTriangleCollisions(element, othElement);
                            var collision = new CollisionElement(indexPair, indexPair2, collisionBoundingBox, triangleCollisions);
                            result.Add(collision);
                        }
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

    // TODO Should we remove this?
    internal class ModelsCollisionPair
    {
        public ModelData firstModel;
        public ModelData secondModel;
        public bool IsModelCollide { get; set; } = false;
    }

    // TODO Should we remove this?
    internal class ElementNodesCollision
    {
        public Element firstElement;
        public Element secondElement;
        public bool IsElementCollide;
    }

    // TODO Move to the separate file and rename to CollisionResult.
    public class CollisionElement
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

        // TODO Это должно формироваться в отдельном месте изнутри CollisionDetector при необходимости.
        /// <summary>
        /// collection of interseted triangles
        /// </summary>
        public List<TriangleCollision> Collisions;

        public CollisionElement(KeyValuePair<string, string> element1, KeyValuePair<string, string> element2, BoundingBox boundaries, List<TriangleCollision> collisions)
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

    // TODO Move to the separate file, make internal.
    public class TriangleCollision
    {
        public List<Vector3> IntersectionPoints;
        public string ElementTriangle1;
        public string ElementTriangle2;
    }
}
