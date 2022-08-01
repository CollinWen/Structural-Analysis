using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MathNet.Numerics.LinearAlgebra;

namespace dsg_wbm
{
    public enum AnalysisType
    {
        Truss,
        Frame
    }

    public enum NodeFixity
    {
        Free,
        Fixed,
        Xfree,
        Yfree,
        Zfree,
        Xfixed,
        Yfixed,
        Zfixed,
        R1fixed,
        R2fixed,
        R3fixed,
        Pinned
    }

    public class Utilities
    {
        // Calculation tolerance
        public static double GeometricTolerance = 0.0001;


        // Global XYZ vectors
        public static Vector<double> globalX = Vector<double>.Build.DenseOfArray(
            new double[] { 1, 0, 0 });
        public static Vector<double> globalY = Vector<double>.Build.DenseOfArray(
            new double[] { 0, 1, 0 });
        public static Vector<double> globalZ = Vector<double>.Build.DenseOfArray(
            new double[] { 0, 0, 1 });

        // Default section
        public static Section defaultSection = new Section();
    }
}