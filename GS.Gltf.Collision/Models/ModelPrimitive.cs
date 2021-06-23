using System.Collections.Generic;
using System.Linq;

namespace GS.Gltf.Collision.Models
{
    internal class ModelPrimitive
    {
        public int ModelIndex;
        public List<Element> Primitives;
        public BoundingBox BoundingBox;
        public List<float> Xs;
        public List<float> Ys;
        public List<float> Zs;

        public ModelPrimitive(int modelIndex, List<Element> primitives)
        {
            ModelIndex = modelIndex;
            Primitives = primitives;
            CreateBoundingBox();
        }

        public void CreateBoundingBox()
        {
            Xs = new List<float>();
            Ys = new List<float>();
            Zs = new List<float>();

            foreach (var primitive in Primitives)
            {
                Xs.AddRange(primitive.Xs);
                Ys.AddRange(primitive.Ys);
                Zs.AddRange(primitive.Zs);
            }

            Xs.Sort();
            Ys.Sort();
            Zs.Sort();

            float[] min = new float[3]
            {
                Xs.First(), Ys.First(), Zs.First()
            };

            float[] max = new float[3]
            {
                Xs.Last(), Ys.Last(), Zs.Last()
            };
            BoundingBox = new BoundingBox(max, min);
        }
    }
}
