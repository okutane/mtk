﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

using NUnit.Framework;

using Matveev.Mtk.Core;
using Matveev.Mtk.Library;
using Matveev.Mtk.Library.Fields;
using Matveev.Mtk.Library.Utilities;
using Matveev.Mtk.Library.Validators;

namespace Matveev.Mtk.Tests
{
    [TestFixture]
    public class EdgeCollapseTest
    {
        [Test]
        public void Impossible()
        {
            Mesh mesh = Configuration.Default.MeshFactory.Create();
            Vertex v0 = mesh.AddVertex(new Point(0, 0, 0), new Vector());
            Vertex v1 = mesh.AddVertex(new Point(1, 0, 0), new Vector());
            Vertex v2 = mesh.AddVertex(new Point(0, 1, 0), new Vector());
            Vertex vFar = mesh.AddVertex(new Point(0, 0, 1), new Vector());
            Vertex vNear = mesh.AddVertex(new Point(0, 0, -1), new Vector());
            mesh.CreateFace(v0, v1, vNear);
            mesh.CreateFace(v1, v2, vNear);
            mesh.CreateFace(v2, v0, vNear);
            mesh.CreateFace(v1, v0, vFar);
            mesh.CreateFace(v2, v1, vFar);
            mesh.CreateFace(v0, v2, vFar);
            Assert.IsFalse(new EdgeCollapse().IsPossible(FindEdge(mesh, v0, v1), null));
        }

        [Test]
        public void ImpossibleTetrahedron()
        {
            Mesh mesh = Configuration.Default.MeshFactory.Create();
            Vertex v0 = mesh.AddVertex(new Point(0, 0, 0), new Vector());
            Vertex v1 = mesh.AddVertex(new Point(1, 0, 0), new Vector());
            Vertex v2 = mesh.AddVertex(new Point(0, 1, 0), new Vector());
            Vertex vFar = mesh.AddVertex(new Point(0, 0, 1), new Vector());
            mesh.CreateFace(v1, v0, vFar);
            mesh.CreateFace(v2, v1, vFar);
            mesh.CreateFace(v0, v2, vFar);
            mesh.CreateFace(v0, v1, v2);
            Assert.IsFalse(new EdgeCollapse().IsPossible(FindEdge(mesh, v0, v1), null));
        }

        [Test]
        [Ignore("This is currently done via validators")]
        public void ImpossibleNonConvex()
        {
            Mesh mesh = Configuration.Default.MeshFactory.Create();
            Vertex v0 = mesh.AddVertex(new Point(0, 0, 0), new Vector());
            Vertex v1 = mesh.AddVertex(new Point(1, -1, 0), new Vector());
            Vertex v2 = mesh.AddVertex(new Point(0, -0.5, 0), new Vector());
            Vertex v3 = mesh.AddVertex(new Point(-1, -1, 0), new Vector());
            Vertex v4 = mesh.AddVertex(new Point(0, 1, 0), new Vector());
            mesh.CreateClosedFan(v0, v1, v2, v3, v4);
            Assert.IsFalse(new EdgeCollapse(1).IsPossible(FindEdge(mesh, v0, v1), null));
        }

        [Test]
        public void Possible()
        {
            Mesh mesh = Configuration.Default.MeshFactory.Create();
            Vertex v0 = mesh.AddVertex(new Point(0, 1, 0), new Vector());
            Vertex v1 = mesh.AddVertex(new Point(0, -1, 0), new Vector());
            Vertex v2 = mesh.AddVertex(new Point(1, 0, 0), new Vector());
            Vertex v3 = mesh.AddVertex(new Point(1, 2, 0), new Vector());
            Vertex v4 = mesh.AddVertex(new Point(-1, 2, 0), new Vector());
            Vertex v5 = mesh.AddVertex(new Point(-1, 0, 0), new Vector());
            Vertex v6 = mesh.AddVertex(new Point(-1, -2, 0), new Vector());
            Vertex v7 = mesh.AddVertex(new Point(1, -2, 0), new Vector());
            mesh.CreateClosedFan(v0, v1, v2, v3, v4, v5);
            mesh.CreateFan(v1, v5, v6, v7, v2);
            Assert.AreEqual(8, mesh.Faces.Count());
            Assert.IsTrue(new EdgeCollapse().IsPossible(FindEdge(mesh, v0, v1), null));
        }

        private static Edge FindEdge(Mesh mesh, Vertex v0, Vertex v1)
        {
            return mesh.Edges.Single(edge => edge.Begin == v0 && edge.End == v1);
        }

        [Test]
        public void ExecuteBegin()
        {
            ExecuteWithWeight(0);
        }

        [Test]
        public void ExecuteMiddle()
        {
            ExecuteWithWeight(0.5);
        }

        [Test]
        public void ExecuteEnd()
        {
            ExecuteWithWeight(1);
        }

        [Test]
        public void ExecuteEndPlane()
        {
            Configuration.Default.Surface = CompactQuadraticForm.Plane;
            Mesh mesh = MC.Instance.Create(Configuration.Default, 2, 2, 2);
            Edge edge = mesh.Edges.First(e => e.Begin.Point == new Point(-1, -1, 0) && e.End.Point == new Point(0, 0, 0));
            new EdgeCollapse(0).Execute(edge);
            mesh.Validate();
            YamlSerializerTest.TestSerialize("EdgeCollapse_Plane.yaml", mesh);
        }

        [Test]
        public void ExecuteEndPlaneBorder()
        {
            Configuration.Default.Surface = CompactQuadraticForm.Plane;
            Mesh mesh = MC.Instance.Create(Configuration.Default, 2, 2, 2);
            EdgeTransform target = new EdgeCollapse(1);
            Edge edge = FindEdge(mesh, UMeshTestHelper.FindVertex(mesh, 0, -1),
                UMeshTestHelper.FindVertex(mesh, -1, -1));
            Assert.IsTrue(target.IsPossible(edge, new BoundingBox(-1, 1, -1, 1, -1, 1)));
            target.Execute(edge);
            mesh.Validate();
            YamlSerializerTest.TestSerialize("EdgeCollapse_PlaneBorder.yaml", mesh);
        }

        private static void ExecuteWithWeight(double weight)
        {
            Configuration.Default.Surface = CompactQuadraticForm.Plane;
            Mesh mesh = MC.Instance.Create(Configuration.Default, 3, 3, 3);
            Edge edge = (from e in mesh.Edges
                         where (Math.Abs(e.Begin.Point.X + e.End.Point.X) < 1e-4)
                            && (Math.Abs(e.Begin.Point.Y + e.End.Point.Y) < 1e-4)
                         select e).First();
            new EdgeCollapse(weight).Execute(edge);
            YamlSerializerTest.TestSerialize("EdgeCollapse_" + weight.ToString("G2",
                NumberFormatInfo.InvariantInfo) + ".yaml", mesh);
        }
    }
}
