using System;
using System.Drawing;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using static System.Math;
using MathNet.Numerics.LinearAlgebra;
using UnityEngine;


namespace dsg_wbm
{


    public enum Fixity
    {
        Free,
        Fixed,
        Pinned
    }


    public class Node
    {
        // Fields
        public AnalysisType? NodeType;
        public Vector<double> Position; // [x,y,z] position of node
        public List<bool> DOFS; // [dx, dy; dz, rx, ry, rz]
        public int nDOFS
        {
            get
            {
                if (DOFS == null)
                {
                    return 0;
                }
                else
                {
                    return DOFS.Count;
                }
            }
        } // number of DOFs
        public Vector<double> Load; // [px, py; pz, mx, my, mz]
        public Vector<double> Reaction; // [rpx, rpy; rpz, rmx, rmy, rmz]
        public Vector<double> Disp; // [dx, dy; dz, mx, my, mz]
        public List<(int,int)> Elements; // [(elementIndex, 0 = start/1 = end), ...] of all attached elements
        public List<int> GlobalIndex; // [positions of DOF in global index order]

        // Base initializer
        public Node()
        {
            this.NodeType = AnalysisType.Frame;
            this.Position = Vector<double>.Build.DenseOfArray(new double[] { 0, 0, 0});
            this.DOFS = new List<bool>(new bool[] {true, true, true, true, true, true});
            this.Load = Vector<double>.Build.Dense(this.nDOFS);
            this.Reaction = Vector<double>.Build.Dense(this.nDOFS);
            this.Disp = Vector<double>.Build.Dense(this.nDOFS);
            this.Elements = new List<(int,int)>();
            this.GlobalIndex = new List<int>();
        }

        // Passing in DOFs
        public Node(Vector<double> position, List<bool> dofs)
        {
            this.Position = position;
            this.DOFS = dofs;
            this.Load = Vector<double>.Build.Dense(this.nDOFS);
            this.Reaction = Vector<double>.Build.Dense(this.nDOFS);
            this.Disp = Vector<double>.Build.Dense(this.nDOFS);
            this.Elements = new List<(int, int)>();
            this.GlobalIndex = new List<int>();

            // Infer node type
            if (dofs.Count == 3)
            {
                this.NodeType = AnalysisType.Truss;
            }
            else if (dofs.Count == 6)
            {
                this.NodeType = AnalysisType.Frame;
            }
            else
            {
                throw new Exception("Length of DOFs must be either 3 (truss) or 6 (frame). For now!!");
            }
        }

        // Position only
        public Node(Vector<double> position)
        {
            this.Position = position;
            this.NodeType = AnalysisType.Frame;
            this.DOFS = new List<bool>(new bool[] { true, true, true, true, true, true });
            this.Load = Vector<double>.Build.Dense(this.nDOFS);
            this.Reaction = Vector<double>.Build.Dense(this.nDOFS);
            this.Disp = Vector<double>.Build.Dense(this.nDOFS);
            this.Elements = new List<(int, int)>();
            this.GlobalIndex = new List<int>();
        }

    }
    
}