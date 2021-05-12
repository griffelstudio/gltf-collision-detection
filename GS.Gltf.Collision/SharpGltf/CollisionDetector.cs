using System;
using System.Collections.Generic;
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

        public DetectionResult Detect()
        {
            var reader = new GltfReader(Settings.ModelPaths);
            var models = reader.Models;
            if (Settings.CheckNodesCollisionIntoModels)
            {
                foreach (var model in models)
                {
                    model.NodesCollision = CheckCollisionsIntoModel(model);
                }
            }

            var modelCollisionPairs = MakeModelsCollisionPairs(models);
            var checkedModelCollisionPairs = CheckModelsCollisionPairs(modelCollisionPairs);

            List<ElementCollisionPair> checkedNodesCollisionPairs = null;
            if (Settings.CheckNodesCollisionBetweenModels)
            {
                checkedNodesCollisionPairs = MakeAndCheckElementCollisionPair(checkedModelCollisionPairs);
            }


            return new DetectionResult()
            {
                ModelsCollisionPairs = checkedModelCollisionPairs,
                ElementCollisionPairs = checkedNodesCollisionPairs,
            };
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

        private List<ElementCollisionPair> MakeAndCheckElementCollisionPair(List<ModelsCollisionPair> pairs)
        {
            var result = new List<ElementCollisionPair>();
            foreach (var pair in pairs)
            {
                if (pair.IsModelCollide)
                {
                    foreach (var element in pair.firstModel.ElementMeshPrimitives)
                    {
                        foreach (var othElement in pair.secondModel.ElementMeshPrimitives)
                        {
                            result.Add(new ElementCollisionPair()
                            {
                                firstModel = pair.firstModel,
                                secondModel = pair.secondModel,
                                IsModelCollide = pair.IsModelCollide,
                                firstModelElement = element,
                                secondModelElement = othElement,
                                IsElementCollide = CheckCollision(element, othElement),
                            });
                        }
                    }
                }
            }
            return result;
        }

        private bool CheckCollision(ICollidable firstObject, ICollidable secondObject)
        {
            return firstObject.GetBoundingBox().IsCollideWith(secondObject.GetBoundingBox(), Settings.Delta);
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

    public class DetectionResult
    {
        public List<ElementCollisionPair> ElementCollisionPairs;
        public List<ModelsCollisionPair> ModelsCollisionPairs;
    }
}
