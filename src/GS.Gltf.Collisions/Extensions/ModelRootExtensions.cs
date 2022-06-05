using GS.Gltf.Collisions;
using GS.Gltf.Collisions.Geometry;


namespace SharpGLTF.Schema2
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
