using System;
using System.Collections.Generic;
using System.Text;

namespace GS.Gltf.Collision.SharpGltf
{
    public interface ICollidable
    {
        public BoundingBox GetBoundingBox();
    }
}
