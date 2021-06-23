using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;
using SharpGLTF.Geometry;
using SharpGLTF.Geometry.VertexTypes;
using SharpGLTF.Materials;
using SharpGLTF.Schema2;
using SharpGLTF;

namespace GS.Gltf.Collision.Geometry
{
    using VERTEX = SharpGLTF.Geometry.VertexTypes.VertexPosition;

    public class CollisionPrimitive
    {

        public MeshBuilder<VERTEX> MeshBuilder;

        public CollisionPrimitive(BoundingBox box)
        {
            var min = box.MinV;
            var max = box.MaxV;

            Vector3 COLLISION_MIN_ALLIGN = new Vector3(-0.01f,-0.01f,-0.01f);
            Vector3 COLLISION_MAX_ALLIGN = new Vector3(0.01f, 0.01f, 0.01f);


            min = Vector3.Add(min, COLLISION_MIN_ALLIGN);
            max = Vector3.Add(max, COLLISION_MAX_ALLIGN);

            VERTEX vertex1 = new VERTEX(min.X, min.Y, min.Z);
            VERTEX vertex2 = new VERTEX(min.X, min.Y, max.Z);
            VERTEX vertex3 = new VERTEX(max.X, min.Y, max.Z);
            VERTEX vertex4 = new VERTEX(max.X, min.Y, min.Z);
            VERTEX vertex5 = new VERTEX(min.X, max.Y, min.Z);
            VERTEX vertex6 = new VERTEX(min.X, max.Y, max.Z);
            VERTEX vertex7 = new VERTEX(max.X, max.Y, max.Z);
            VERTEX vertex8 = new VERTEX(max.X, max.Y, min.Z);

            var material = new MaterialBuilder()
                .WithAlpha(SharpGLTF.Materials.AlphaMode.BLEND)
                .WithDoubleSide(true)
                .WithMetallicRoughnessShader()
                .WithChannelParam("BaseColor", new Vector4(1, 0, 0, 0.8f));

            var mesh = new MeshBuilder<VERTEX>("mesh");

            var prim = mesh.UsePrimitive(material);
            prim.AddTriangle(vertex1, vertex2, vertex3);
            prim.AddTriangle(vertex3, vertex4, vertex1);
            prim.AddTriangle(vertex2, vertex6, vertex3);
            prim.AddTriangle(vertex3, vertex6, vertex7);
            prim.AddTriangle(vertex1, vertex6, vertex2);
            prim.AddTriangle(vertex1, vertex5, vertex6);
            prim.AddTriangle(vertex1, vertex4, vertex5);
            prim.AddTriangle(vertex5, vertex4, vertex8);
            prim.AddTriangle(vertex3, vertex7, vertex4);
            prim.AddTriangle(vertex4, vertex7, vertex8);
            prim.AddTriangle(vertex6, vertex5, vertex7);
            prim.AddTriangle(vertex7, vertex5, vertex8);

            MeshBuilder = mesh;
        }
    }
}
