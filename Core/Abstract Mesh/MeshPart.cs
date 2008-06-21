using System;
using System.Collections.Generic;
using System.Text;
using Matveev.Mtk.Core;

namespace Matveev.Mtk.Core
{
    public abstract class MeshPart
    {
        public abstract Mesh Mesh
        {
            get;
        }

        public abstract ICollection<Vertex> GetVertices(int radius);

        public abstract ICollection<Edge> GetEdges(int radius);
    }
}
