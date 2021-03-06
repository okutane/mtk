using System;
using System.Collections.Generic;

namespace Matveev.Mtk.Core
{
    public abstract class Mesh
    {
        public abstract IEnumerable<Vertex> Vertices
        {
            get;
        }

        public abstract IEnumerable<Edge> Edges
        {
            get;
        }

        public abstract IEnumerable<Face> Faces
        {
            get;
        }

        public abstract int EdgesCount
        {
            get;
        }

        public abstract Vertex AddVertex(Point p, Vector n);

        public abstract void RemoveVertex(Vertex vert);

        public abstract Face CreateFace(Vertex v1, Vertex v2, Vertex v3);

        public abstract void DeleteFace(Face face);

        public abstract void Attach(Mesh submesh, Dictionary<Edge, Edge> edgeMap);
    }
}
