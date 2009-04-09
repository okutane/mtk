﻿using System;
using System.Collections.Generic;
using System.Text;

using NUnit.Framework;

using Matveev.Common;
using Matveev.Mtk.Tests;
using Matveev.Mtk.Tests.FunctionOptimization;

namespace Matveev.Mtk.Library.Tests
{
    [TestFixture]
    public class GradientDescentTest
    {
        [Test]
        public void SquareTest()
        {
            MyDouble[] x = new MyDouble[] { 0.1 };
            Func<MyDouble[], double> f = a => Math.Pow(a[0] - 1, 2);
            GradientDelegate<MyDouble, MyDouble> grad = (a, result) => result[0] = 2 * (a[0] - 1);

            for (int i = 0; i < 100; i++)
            {
                FunctionOptimization<MyDouble, MyDouble>.GradientDescent(f, grad, x, 1e-9, 1,
                    NullProgressMonitor.Instance);
            }
            Assert.AreEqual(1, x[0], 1e-4);
        }

        [Test]
        public void CircleTest()
        {
            MyDouble[] x = new MyDouble[] { 0.1, 0.1 };
            Func<MyDouble[], double> implicitCircle = a => a[0] * a[0] + a[1] + a[1] - 1;
            Func<MyDouble[], double> f = a => Math.Pow(implicitCircle(a), 2);
            GradientDelegate<MyDouble, MyDouble> grad = (a, result) =>
            {
                result[0] = 2 * a[0] * implicitCircle(a);
                result[1] = 2 * a[1] * implicitCircle(a);
            };

            FunctionOptimization<MyDouble, MyDouble>.GradientDescent(f, grad, x, 1e-4, 100,
                NullProgressMonitor.Instance);
            Assert.AreEqual(0, f(x), 1e-4);
        }

        [Test]
        public void RosenbrockTest()
        {
            ITestFunction function = new Rosenbrock();
            MyDouble[] x = new MyDouble[] { 0, 0 };
            FunctionOptimization<MyDouble, MyDouble>.GradientDescent(function.Function, function.Gradient, x, 0,
                10000, NullProgressMonitor.Instance);
            double[] expected = function.Minimum;
            Assert.AreEqual(0, function.Function(x), 1e-4, "f(x)");
            Assert.AreEqual(expected[0], x[0], 1e-2, "x[0]");
            Assert.AreEqual(expected[1], x[1], 1e-2, "x[1]");
        }

        [Test]
        public void PolylineCircleTest()
        {
            Func<double[], double> localValue = delegate(double[] x)
            {
                double x0 = x[0];
                double y0 = x[1];
                double x1 = x[2];
                double y1 = x[3];

                return 0.1e1 - 0.2e1 / 0.3e1 * x0 * x0 - 0.2e1 / 0.3e1 * y0 * y0
                    + 0.2e1 / 0.5e1 * x1 * x1 * y1 * y1 + Math.Pow(y1, 0.3e1) * y0 / 0.5e1
                    + y1 * y1 * x1 * x0 / 0.5e1 + 0.2e1 / 0.5e1 * y0 * y0 * x0 * x0 + y1 * y0 * x0 * x0 / 0.5e1
                    + x1 * x0 * y0 * y0 / 0.5e1 + Math.Pow(x1, 0.3e1) * x0 / 0.5e1 - 0.2e1 / 0.3e1 * x1 * x1
                    + 0.4e1 / 0.15e2 * y1 * y0 * x1 * x0 + Math.Pow(x1, 0.4e1) / 0.5e1
                    + Math.Pow(y1, 0.4e1) / 0.5e1 - 0.2e1 / 0.3e1 * y1 * y1 + x1 * x1 * x0 * x0 / 0.5e1
                    + x1 * x1 * y0 * y0 / 0.15e2 + y1 * y1 * x0 * x0 / 0.15e2 + y1 * y1 * y0 * y0 / 0.5e1
                    + Math.Pow(y0, 0.4e1) / 0.5e1 + Math.Pow(x0, 0.4e1) / 0.5e1 - 0.2e1 / 0.3e1 * y1 * y0
                    - 0.2e1 / 0.3e1 * x1 * x0 + y1 * Math.Pow(y0, 0.3e1) / 0.5e1
                    + x1 * Math.Pow(x0, 0.3e1) / 0.5e1 + x1 * x1 * y1 * y0 / 0.5e1;
            };

            Func<double[], double[]> localGrad = delegate(double[] x)
            {
                double x0 = x[0];
                double y0 = x[1];
                double x1 = x[2];
                double y1 = x[3];
                double[] cg0 = new double[4];

                cg0[0] = -0.4e1 / 0.3e1 * x0 + y1 * y1 * x1 / 0.5e1 + 0.4e1 / 0.5e1 * y0 * y0 * x0
                    + 0.2e1 / 0.5e1 * y1 * y0 * x0 + x1 * y0 * y0 / 0.5e1 + Math.Pow(x1, 0.3e1) / 0.5e1
                    + 0.4e1 / 0.15e2 * y1 * y0 * x1 + 0.2e1 / 0.5e1 * x1 * x1 * x0 + 0.2e1 / 0.15e2 * y1 * y1 * x0
                    + 0.4e1 / 0.5e1 * Math.Pow(x0, 0.3e1) - 0.2e1 / 0.3e1 * x1 + 0.3e1 / 0.5e1 * x1 * x0 * x0;
                cg0[1] = -0.4e1 / 0.3e1 * y0 + Math.Pow(y1, 0.3e1) / 0.5e1 + 0.4e1 / 0.5e1 * y0 * x0 * x0
                    + y1 * x0 * x0 / 0.5e1 + 0.2e1 / 0.5e1 * x1 * x0 * y0 + 0.4e1 / 0.15e2 * y1 * x1 * x0
                    + 0.2e1 / 0.15e2 * x1 * x1 * y0 + 0.2e1 / 0.5e1 * y1 * y1 * y0
                    + 0.4e1 / 0.5e1 * Math.Pow(y0, 0.3e1) - 0.2e1 / 0.3e1 * y1 + 0.3e1 / 0.5e1 * y1 * y0 * y0
                    + x1 * x1 * y1 / 0.5e1;
                cg0[2] = 0.4e1 / 0.5e1 * y1 * y1 * x1 + y1 * y1 * x0 / 0.5e1 + y0 * y0 * x0 / 0.5e1
                    + 0.3e1 / 0.5e1 * x1 * x1 * x0 - 0.4e1 / 0.3e1 * x1 + 0.4e1 / 0.15e2 * y1 * y0 * x0
                    + 0.4e1 / 0.5e1 * Math.Pow(x1, 0.3e1) + 0.2e1 / 0.5e1 * x1 * x0 * x0
                    + 0.2e1 / 0.15e2 * x1 * y0 * y0 - 0.2e1 / 0.3e1 * x0 + Math.Pow(x0, 0.3e1) / 0.5e1
                    + 0.2e1 / 0.5e1 * y1 * y0 * x1;
                cg0[3] = 0.4e1 / 0.5e1 * x1 * x1 * y1 + 0.3e1 / 0.5e1 * y1 * y1 * y0
                    + 0.2e1 / 0.5e1 * y1 * x1 * x0 + y0 * x0 * x0 / 0.5e1 + 0.4e1 / 0.15e2 * x1 * x0 * y0
                    + 0.4e1 / 0.5e1 * Math.Pow(y1, 0.3e1) - 0.4e1 / 0.3e1 * y1 + 0.2e1 / 0.15e2 * y1 * x0 * x0
                    + 0.2e1 / 0.5e1 * y1 * y0 * y0 - 0.2e1 / 0.3e1 * y0 + Math.Pow(y0, 0.3e1) / 0.5e1
                    + x1 * x1 * y0 / 0.5e1;

                return cg0;
            };

            Func<MyDouble[], double> globalValue = delegate(MyDouble[] x)
            {
                double result = 0;

                int n = x.Length / 2;
                for (int i = 0; i < n; i++)
                {
                    result += localValue(new double[] { x[2 * i], x[2 * i + 1],
                        x[2 * ((i + 1) % n)], x[2 * ((i + 1) % n) + 1] }); 
                }

                return result;
            };

            GradientDelegate<MyDouble, MyDouble> globalGrad = delegate(MyDouble[] x, MyDouble[] result)
            {
                int n = x.Length / 2;
                for (int i = 0; i < n; i++)
                {
                    double[] grad = localGrad(new double[] { x[2 * i], x[2 * i + 1],
                        x[2 * ((i + 1) % n)], x[2 * ((i + 1) % n) + 1] });
                    result[2 * i] += grad[0];
                    result[2 * i + 1] += grad[1];
                    result[2 * ((i + 1) % n)] += grad[2];
                    result[2 * ((i + 1) % n) + 1] += grad[3];
                }
            };
            MyDouble[] arg = { 1, 0, 0, 1, -1, 0, 0, -1 };
            FunctionOptimization<MyDouble, MyDouble>.GradientDescent(globalValue, globalGrad, arg, 0, 10000,
                NullProgressMonitor.Instance);
            Assert.AreEqual(0.19, globalValue(arg), 1e-3);
        }
    }
}
