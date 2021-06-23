using System;
using System.Numerics;
using GS.Gltf.Collision.Geometry;
using SharpGLTF.Geometry;
using SharpGLTF.Geometry.VertexTypes;
using SharpGLTF.Materials;
using SharpGLTF.Schema2;


namespace GS.Gltf.Collision.SharpGltf
{
    public static class ModelRootExtensions
    {
        public static void AddCollisionBBNode(this ModelRoot model, BoundingBox box)
        {
            int SCENE_INDEX = 0;

            var collisionNode = new CollisionPrimitive(box).MeshBuilder;

            Mesh mesh = model.CreateMesh(collisionNode);
            var scene = model.UseScene(SCENE_INDEX);
            scene.CreateNode().WithMesh(mesh);
        }
    }
}
