using System;
using System.Collections.Generic;
using System.Text;

using Matveev.Mtk.Core;

namespace Matveev.Mtk.Library.EdgeFunctions
{
    public class Length : IEdgeFunction
    {
        public double Evaluate(Edge edge)
        {
            return (edge.End.Point - edge.Begin.Point).Norm;
        }
    }
}
