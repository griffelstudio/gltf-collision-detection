using System;
using System.Collections.Generic;
using System.Text;
using System.Numerics;
using SharpGLTF.Transforms;
using SharpGLTF.Schema2;

namespace GS.Gltf.Collision.SharpGltf
{
    class Transformation
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
            List<Vector3> result = new List<Vector3>();
            foreach (var vector in accessor.AsVector3Array())
            {
                var vec = TransformVector(vector, transforms);
                result.Add(vec);
            }

            return result;
        }
    }
}
