﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using NUnit.Framework;

using Matveev.Mtk.Core;
using Matveev.Mtk.Library;
using Matveev.Mtk.Library.Fields;

namespace Matveev.Mtk.Tests
{
    [TestFixture]
    public class PlanePolygonizationTests
    {
        [Test]
        public void Create()
        {
            Mesh mesh = MC.Instance.Create(Configuration.Default, QuadraticForm.Plane, 2, 2, 2);
            
            //Dictionary<
        }
    }
}
