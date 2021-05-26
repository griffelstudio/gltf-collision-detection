using System;
using System.Collections.Generic;
using System.Text;

namespace GS.Gltf.Collision.SharpGltf
{
    class Model
    {
        public readonly IEnumerable<Element2> Elements;

        public Model(IEnumerable<Element2> elements)
        {
            Elements = elements;
        }

        public IEnumerable<Collision> CollideWith(Model model) 
        {
            return null;
        }
    }
}
