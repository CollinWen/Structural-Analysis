using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MathNet.Numerics.LinearAlgebra;

namespace dsg_wbm
{ 
    public class Element
    {
        // Fields
        public Vector<double> PosStart; // [x, y,;z] position of start node
        public Vector<double> PosEnd; // [x, y; z] position of end node
        public Tuple<int, int>? NodeIndex; // {(start node index, end node index)}
        public int? Dim // dimension of element
        {
            get
            {
                if (PosStart == null || PosEnd == null)
                {
                    return null;
                }
                else
                {
                    return PosStart.Count;
                }
            }
        }
        public double Length
        {
            get
            {
                if (PosStart == null || PosEnd == null)
                {
                    return 0;
                }
                else
                {
                    return (PosEnd - PosStart).L2Norm();
                }
            }
        }
        // length of element
        public Matrix<double>? K; // stiffness matrix in GCS USE AS DENSE MATRIX
        public List<int>? DOFIndex; // [DOF indices in global ordering system] 
        public Vector<double>? LocalForces; // [member forces in LCS]
        public Vector<double>? GlobalForces; // [member forces in GCS]
        public double? AxialForce; // Axial force (- compression, + tension)
        public double A; // Area
        public double E; // Young's Modulus
        public double G; // Shear Modulus
        public double Iz; // Strong axis moment of inertia
        public double Iy; // weak axis moment of inertia
        public double J; // Torsional constant 
        public double Psi = Math.PI / 2; // LCS reference angle from local x axis
        public AnalysisType? Type;
        public Matrix<double>? R // Rotation matrix
        {
            get
            {
                switch (this.Type)
                {
                    case AnalysisType.Truss:
                        {
                            return Analysis.Rtruss(this.PosStart, this.PosEnd, this.Length);
                        }
                    case AnalysisType.Frame:
                        {
                            return Analysis.Rframe(this.PosStart, this.PosEnd, this.Length, this.Psi);
                        }
                    default:
                        {
                            return null;
                        }
                }
            }
        }
        public List<Vector<double>>? LCS
        {
            get
            {
                if (this.PosStart == null || this.PosEnd == null)
                {
                    return null;
                }
                else
                {
                    return Analysis.LCS(this.PosStart, this.PosEnd, this.Psi);
                }
            }
        }

        /// <summary>
        /// Basic constructor
        /// </summary>
        public Element()
        {
            this.PosStart = Vector<double>.Build.DenseOfArray(new double[] { 0, 0, 0, });
            this.PosEnd = Vector<double>.Build.DenseOfArray(new double[] { 1, 0, 0 });
            PopulateSection(Utilities.defaultSection);
        }

        /// <summary>
        /// Node association
        /// </summary>
        /// <param name="nodes"></param>
        /// <param name="nodeIndex"></param>
        public Element(List<Node> nodes, Tuple<int, int> nodeIndex)
        {
            this.NodeIndex = nodeIndex;
            this.PosStart = nodes[nodeIndex.Item1].Position;
            this.PosEnd = nodes[nodeIndex.Item2].Position;
            PopulateSection(Utilities.defaultSection);
            this.Type = nodes[nodeIndex.Item1].NodeType;
        }

        // 2d frame
        //public Element(List<Node> nodes, Tuple<int, int> nodeIndex, double e, double a, double i)
        //{
        //    this.NodeIndex = nodeIndex;
        //    this.PosStart = nodes[nodeIndex.Item1].Position;
        //    this.PosEnd = nodes[nodeIndex.Item2].Position;
        //    this.E = e;
        //    this.A = a;
        //    this.Iz = i;
        //    this.Type = AnalysisType.Frame;
        //}

        /// <summary>
        /// 3D frame constructor
        /// </summary>
        /// <param name="nodes"></param>
        /// <param name="nodeIndex"></param>
        /// <param name="e"></param>
        /// <param name="a"></param>
        /// <param name="g"></param>
        /// <param name="iz"></param>
        /// <param name="iy"></param>
        /// <param name="j"></param>
        public Element(List<Node> nodes, Tuple<int, int> nodeIndex, double e, double a, double g, double iz, double iy, double j)
        {
            this.NodeIndex = nodeIndex;
            this.PosStart = nodes[nodeIndex.Item1].Position;
            this.PosEnd = nodes[nodeIndex.Item2].Position;
            this.E = e;
            this.A = a;
            this.G = g;
            this.Iz = iz;
            this.Iy = iy;
            this.J = j;
            this.Type = nodes[nodeIndex.Item1].NodeType;
        }

        /// <summary>
        /// Use when base Element() constructor is used without node connectivity
        /// </summary>
        /// <param name="nodes"></param>
        /// <param name="nodeIndex"></param>
        public void AssociateNodes(List<Node> nodes, Tuple<int, int> nodeIndex)
        {
            this.NodeIndex = nodeIndex;
            this.PosStart = nodes[nodeIndex.Item1].Position;
            this.PosEnd = nodes[nodeIndex.Item2].Position;
            this.Type = nodes[nodeIndex.Item1].NodeType;
        }

        /// <summary>
        /// Populate element material properties with a defined Section
        /// </summary>
        /// <param name="section"></param>
        public void PopulateSection(Section section)
        {
            this.E = section.E;
            this.A = section.A;
            this.G = section.G;
            this.Iz = section.Iz;
            this.Iy = section.Iy;
            this.J = section.J;
        }

    }
}