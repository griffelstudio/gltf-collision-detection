using GS.Gltf.Collision.Geometry;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace GS.Gltf.Collision.SharpGltf
{
    public class CollisionDetector
    {
        private CollisionSettings Settings { get; }

        public CollisionDetector(CollisionSettings settings)
        {
            Settings = settings;
        }

        public List<Collision> Detect()
        {
            var reader = new GltfReader(Settings.ModelPaths);
            var models = reader.Models;
            if (Settings.InModelDetection)
            {
                foreach (var model in models)
                {
                    model.NodesCollision = CheckCollisionsIntoModel(model);
                }
            }

            var modelCollisionPairs = MakeModelsCollisionPairs(models);
            var checkedModelCollisionPairs = CheckModelsCollisionPairs(modelCollisionPairs);

            List<Collision> checkedNodesCollisionPairs = null;
            if (Settings.ModelPaths.Count > 1)
            {
                checkedNodesCollisionPairs = MakeAndCheckElementCollisionPair(checkedModelCollisionPairs);
            }


            return checkedNodesCollisionPairs;
        }

        private List<ElementNodesCollision> CheckCollisionsIntoModel(ModelData model)
        {
            var result = new List<ElementNodesCollision>();
            for (int i = 0; i < model.ElementMeshPrimitives.Count; i++)
            {
                for (int j = i; j < model.ElementMeshPrimitives.Count; j++)
                {
                    if (i != j)
                    {
                        var firstNode = model.ElementMeshPrimitives[i];
                        var secondNode = model.ElementMeshPrimitives[j];
                        result.Add(new ElementNodesCollision()
                        {
                            firstElement = firstNode,
                            secondElement = secondNode,
                            IsElementCollide = CheckCollision(firstNode, secondNode),
                        });
                    }

                }
            }
            return result;
        }

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

        private List<Collision> MakeAndCheckElementCollisionPair(List<ModelsCollisionPair> pairs)
        {
            var result = new List<Collision>();
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
                            var collision = new Collision(indexPair, indexPair2, collisionBoundingBox, triangleCollisions);
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
                            var point = HelperUtils.RaycastPoint(triange2, ray);
                            if (point != new Vector3())
                            {
                                intersectionPoints.Add(point);
                            }
                            
                            
                        }

                        foreach (var ray in triange2.GetEdgesRays())
                        {
                            var point = HelperUtils.RaycastPoint(triange1, ray);
                            if (point != new Vector3())
                            {
                                intersectionPoints.Add(point);
                            }
                        }



                        result.Add(new TriangleCollision
                        {
                            //DEBUG
                            //FirstModel = new KeyValuePair<string, string>(string.Format("{0}_{1}",e1.ModelIndex,e1.NodeName),i.ToString()),
                            //SecondModel = new KeyValuePair<string, string>(string.Format("{0}_{1}", e2.ModelIndex, e2.NodeName), j.ToString())
                            FirstModel = new KeyValuePair<string, string>(e1.NodeName, i.ToString()),
                            SecondModel = new KeyValuePair<string, string>(e2.NodeName, j.ToString()),
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

    public class ModelsCollisionPair
    {
        public ModelData firstModel;
        public ModelData secondModel;
        public bool IsModelCollide { get; set; } = false;
    }

    public class ElementCollisionPair : ModelsCollisionPair
    {
        public Element firstModelElement;
        public Element secondModelElement;
        public bool IsElementCollide;
    }

    public class ElementNodesCollision
    {
        public Element firstElement;
        public Element secondElement;
        public bool IsElementCollide;
    }


    public class Collision
    {
        /// <summary>
        /// blalblabl
        /// </summary>
        public KeyValuePair<string, string> Element1;
        public KeyValuePair<string, string> Element2;
        public BoundingBox Boundaries;
        public BoundingBox MinIntersectionBoundaries;
        public List<TriangleCollision> Collisions;
        

        public Collision(KeyValuePair<string, string> element1, KeyValuePair<string, string> element2, BoundingBox boundaries, List<TriangleCollision> collisions)
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

    public class TriangleCollision
    {
        public KeyValuePair<string, string> FirstModel;
        public KeyValuePair<string, string> SecondModel;
        public List<Vector3> IntersectionPoints;
        //public string ElementTriangle1
        //public string ElementTriangle2

    }
}
