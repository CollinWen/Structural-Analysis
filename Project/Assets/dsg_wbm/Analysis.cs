using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MathNet.Numerics.LinearAlgebra;
using UnityEngine;

namespace dsg_wbm
{
    public static class Analysis
    {
        public static void DOFExpander(List<Element> elements, List<Node> nodes) {
            foreach (Element e in elements)
                e.DOFIndex = nodes[e.NodeIndex.Item1].GlobalIndex.Concat(nodes[e.NodeIndex.Item2].GlobalIndex).ToList();
        }

        public static void NodeGlobalIndex(List<Node> nodes) {
            for (int i = 0; i < nodes.Count; i++)
                nodes[i].GlobalIndex = Enumerable.Range(0, nodes[i].DOFS.Count).ToList();
        }

        public static void AddNodeElements(List<Element> elements, List<Node> nodes) {
            foreach (Node n in nodes)
                n.Elements = new List<(int, int)>();
            for (int i = 0; i < elements.Count; i++) {
                var (startIdx, endIdx) = elements[i].NodeIndex;
                nodes[startIdx].Elements.Add((i, 0));
                nodes[endIdx].Elements.Add((i, 1));
            }
            foreach (Node n in nodes)
                n.Elements = n.Elements.Distinct().ToList();
        }

        public static void AddNodeLoads(List<Load> loads, List<Node> nodes) {
            if (loads != null)
                for (int i = 0; i < loads.Count; i++)
                    nodes[loads[i].Index].Load = loads[i].LoadValues;
        }

        /// <summary>
        /// Local coordinate system of element
        /// </summary>
        /// <param name="posStart"></param>
        /// <param name="posEnd"></param>
        /// <param name="Psi"></param>
        /// <returns></returns>
        public static List<Vector<double>> LCS(Vector<double> posStart, Vector<double> posEnd, double Psi)
        {
            Vector<double> x = posEnd - posStart;
            x = x.Normalize(2);
            
            // Initializing local coordinate system
            Vector<double> xvec = Vector<double>.Build.Dense(3);
            Vector<double> yvec = Vector<double>.Build.Dense(3);
            Vector<double> zvec = Vector<double>.Build.Dense(3);

            // special case where local x is aligned with global Y
            if (Cross(x, Utilities.globalY).L2Norm() < Utilities.GeometricTolerance)
            {
                double Cy = x[1];
                xvec = Cy * Utilities.globalY;
                yvec = -Cy * Utilities.globalX * Math.Cos(Psi) + Math.Sin(Psi) * Utilities.globalZ;
                zvec = Cy * Utilities.globalX * Math.Sin(Psi) + Math.Cos(Psi) * Utilities.globalZ;
            }
            // all other cases
            else
            {
                Vector<double> zbar = Cross(x, Utilities.globalY);
                Vector<double> ybar = Cross(zbar, x);

                xvec = x;
                yvec = Math.Cos(Psi) * ybar + Math.Sin(Psi) * zbar;
                zvec = -Math.Sin(Psi) * ybar + Math.Cos(Psi) * zbar;
            }

            // list of Local coordinate system vectors
            return new List<Vector<double>> { xvec, yvec, zvec };
        }

        /// <summary>
        /// Manual cross product method to stay in MathNet.LinearAlgebra
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public static Vector<double> Cross(Vector<double> left, Vector<double> right)
        {
            if ((left.Count != 3 || right.Count != 3))
            {
                string message = "Vectors must have a length of 3.";
                throw new Exception(message);
            }
            Vector<double> result = Vector<double>.Build.Dense(3);
            result[0] = left[1] * right[2] - left[2] * right[1];
            result[1] = -left[0] * right[2] + left[2] * right[0];
            result[2] = left[0] * right[1] - left[1] * right[0];

            return result;
        }

        /*
         * Rotation matrix formulations are based on Kassimali's Matrix Analysis of Structures
         */
        /// <summary>
        /// Rotation matrix for 3D truss element
        /// </summary>
        /// <param name="posStart"></param>
        /// <param name="posEnd"></param>
        /// <param name="L"></param>
        /// <returns></returns>
        public static Matrix<double> Rtruss(Vector<double> posStart, Vector<double> posEnd, double L){
            Matrix<double> R = Matrix<double>.Build.Dense(2, 6);

            // Cosines of element to global axes
            Vector<double> cosines = (posEnd - posEnd) / L;
            double Cx = cosines[0]; // x axis
            double Cy = cosines[1]; // y axis
            double Cz = cosines[2]; // z axis

            // Row vector of cosines
            Matrix<double> a = Matrix<double>.Build.DenseOfRowArrays(new[] { Cx, Cy, Cz });

            // Populate
            R.SetSubMatrix(0, 0, 0, 2, a);
            R.SetSubMatrix(1, 1, 3, 5, a);

            return R;
        }

        /// <summary>
        /// Rotation matrix for 3D frame element
        /// </summary>
        /// <param name="posStart"></param>
        /// <param name="posEnd"></param>
        /// <param name="L"></param>
        /// <param name="Psi"></param>
        /// <returns></returns>
        public static Matrix<double> Rframe(Vector<double> posStart, Vector<double> posEnd, double L, double Psi)
        {
            Matrix<double> R = Matrix<double>.Build.Dense(12, 12);
            // Cosines of element to global axes
            Vector<double> cosines = (posEnd - posEnd) / L;
            double Cx = cosines[0]; // x axis
            double Cy = cosines[1]; // y axis
            double Cz = cosines[2]; // z axis

            Matrix<double> r = Matrix<double>.Build.Dense(3, 3);


            if (Cross(cosines, Utilities.globalY).L2Norm() < Utilities.GeometricTolerance)
            {
                Matrix<double> Lambda = Matrix<double>.Build.DenseOfRowArrays(new double[] { 0, Cy, 0 },
                    new double[] { (-Cy * Math.Cos(Psi)), 0, Math.Sin(Psi) },
                    new double[] { (Cy * Math.Sin(Psi)), 0, Math.Cos(Psi) });
                r = Lambda;
            }
            else
            {
                // Second row
                double b1 = (-Cx * Cy * Math.Cos(Psi) - Cz * Math.Sin(Psi)) / Math.Sqrt(Cx * Cx + Cz * Cz);
                double b2 = Math.Sqrt(Cx * Cx + Cz * Cz) * Math.Cos(Psi);
                double b3 = (-Cy * Cz * Math.Cos(Psi) + Cx * Math.Sin(Psi)) / Math.Sqrt(Cx * Cx + Cz * Cz);

                // Third row
                double c1 = (Cx * Cy * Math.Sin(Psi) - Cz * Math.Cos(Psi)) / Math.Sqrt(Cx * Cx + Cz * Cz);
                double c2 = -Math.Sqrt(Cx * Cx + Cz * Cz) * Math.Sin(Psi);
                double c3 = (Cy * Cz * Math.Sin(Psi) + Cx * Math.Cos(Psi)) / Math.Sqrt(Cx * Cx + Cz * Cz);

                Matrix<double> Lambda = Matrix<double>.Build.DenseOfRowArrays(new double[] {Cx, Cy, Cz}, 
                    new double[] {b1, b2, b3}, 
                    new double[] {c1, c2, c3});
                r = Lambda;
            }

            // Populate
            R.SetSubMatrix(0, 2, 0, 2, r);
            R.SetSubMatrix(3, 5, 3, 5, r);
            R.SetSubMatrix(6, 8, 6, 8, r);
            R.SetSubMatrix(9, 11, 9, 11, r);

            return R;
        }

        /*
         * Stiffness matrix formulations are based on Kassimali's Matrix Analysis of Structures
         */

        /// <summary>
        /// Base stiffness matrix for truss without transformation
        /// </summary>
        /// <param name="E"></param>
        /// <param name="A"></param>
        /// <param name="L"></param>
        /// <returns></returns>
        public static Matrix<double> kElementalTruss(double E, double A, double L)
        {
            Matrix<double> k = Matrix<double>.Build.Dense(4, 4);
            k[0, 0] = E * A / L;
            k[0, 1] = - E * A / L;
            k[1, 0] = - E * A / L;
            k[1, 1] = E * A / L;
            return k;
        }

        /// <summary>
        /// Base stiffness matrix for frame without transformation
        /// </summary>
        /// <param name="E"></param>
        /// <param name="A"></param>
        /// <param name="G"></param>
        /// <param name="L"></param>
        /// <param name="Iz"></param>
        /// <param name="Iy"></param>
        /// <param name="J"></param>
        /// <returns></returns>
        public static Matrix<double> kElementalFrame(double E, double A, double G, double L, double Iz, double Iy, double J)
        {
            // Block A
            Matrix<double> a = Matrix<double>.Build.Dense(3, 3);
            a[0, 0] = A * L * L;
            a[1, 1] = 12 * Iz;
            a[2, 2] = 12 * Iy;

            // Block B
            Matrix<double> b = Matrix<double>.Build.Dense(3, 3);
            b[0, 0] = 0;
            b[1, 2] = 6 * L * Iz;
            b[2, 1] = -6 * L * Iy;

            // Block C
            Matrix<double> c = Matrix<double>.Build.Dense(3, 3);
            c[0, 0] = G * J * L * L / E;
            c[1, 1] = 4 * L * L * Iy;
            c[2, 2] = 4 * L * L * Iz;

            // Block D
            Matrix<double> d = Matrix<double>.Build.Dense(3, 3);
            d[0, 0] = - G * J * L * L / E;
            d[1, 1] = 2 * L * L * Iy;
            d[2, 2] = 2 * L * L * Iz;

            // Stiffness matrix
            Matrix<double> k = Matrix<double>.Build.Dense(12, 12);

            // ROW 1-3
            k.SetSubMatrix(0, 3, 0, 3, a); // block [0:2,0:2]
            k.SetSubMatrix(0, 3, 3, 3, b); // block [0:2,3:5]
            k.SetSubMatrix(0, 3, 6, 3, a.Negate()); // block [0:2,6:8]
            k.SetSubMatrix(0, 3, 9, 3, b); // block [0:2, 9:11]

            // ROW 4-6
            k.SetSubMatrix(3, 3, 0, 3, b.Transpose());
            k.SetSubMatrix(3, 3, 3, 3, c);
            k.SetSubMatrix(3, 3, 6, 3, b.Transpose().Negate());
            k.SetSubMatrix(3, 3, 9, 3, d);

            // ROW 7-9
            k.SetSubMatrix(6, 3, 0, 3, a.Negate());
            k.SetSubMatrix(6, 3, 3, 3, b.Negate());
            k.SetSubMatrix(6, 3, 6, 3, a);
            k.SetSubMatrix(6, 3, 9, 3, b.Negate());

            // ROW 10-12
            k.SetSubMatrix(9, 3, 0, 3, b.Transpose());
            k.SetSubMatrix(9, 3, 3, 3, d);
            k.SetSubMatrix(9, 3, 6, 3, b.Transpose().Negate());
            k.SetSubMatrix(9, 3, 9, 3, c);

            k *= E / L / L / L;

            return k;
        }

        public static Matrix<double> K(List<Element> elements, int nDOFS) {
            var K = Matrix<double>.Build.Dense(nDOFS, nDOFS);

            foreach (Element e in elements)
                K[e.DOFIndex, e.DOFIndex] += e.K;

            return K;
        }

        public static void Reactions(Structure structure) {

        }

        public static void PostProcess(Structure structure, double scaleFactor = 0) {
            
        }

        public static void analyze(Structure structure, bool forceK = false) {
            if (structure)
                throw new Exception("Structure must be defined.");

            AddNodeElements(structure.elements, structure.nodes);
            AddNodeLoads(structure.loads, structure.nodes);

            if (structure.K && !forceK) {
                var U = structure.F[structure.freeDOFS].Transpose() * structure.K[structure.freeDOFS, structure.freeDOFS];
                structure.complicance = U.Transpose() * structure.F[structure.freeDOFS];
                return;
            }

            NodeGlobalIndex(structure.nodes);
            DOFExpander(structure.elements, structure.nodes);

            foreach (Element e in structure.elements) {
                if (e.Type == AnalysisType.Truss)
                    kElementalTruss(e.E, e.A, e.Length);
                else if (e.Type == AnalysisType.Frame) {
                    kElementalFrame(e.E, e.A, e.G, e.Length, e.Iz, e.Iy);
                } else {
                    throw new Exception("Unknown analysis type.");
                }
            }

            structure.K = K(structure.elements, structure.nDOFS);

            var U = structure.K[structure.freeDOFS, structure.freeDOFS].Transpose() * structure.F[structure.freeDOFS];

            structure.complicance = U.Transpose() * structure.F[structure.freeDOFS];
            structure.U = U;

            Reactions(structure);
            PostProcess(stucture, scaleFactor = SF);
        }
    }
}