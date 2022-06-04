using SharpGLTF.Schema2;
using SharpGLTF.Transforms;
using System.Collections.Generic;
using System.Numerics;

namespace GS.Gltf.Collisions.Geometry
{
    internal class Transformation
    {
        public static Vector3 TransformVector(Vector3 vec, List<AffineTransform> transformations)
        {
            foreach (var transformation in transformations)
            {
                vec = Vector3.Transform(vec,transformation.Matrix);
            }

            return vec;
        }

        public static List<Vector3> TransformAccessor(Accessor accessor, List<AffineTransform> transforms)
        {
            var result = new List<Vector3>();
            foreach (var vector in accessor.AsVector3Array())
            {
                var vec = TransformVector(vector, transforms);
                result.Add(vec);
            }

            return result;
        }
    }
}
