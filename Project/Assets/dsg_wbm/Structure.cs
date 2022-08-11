using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MathNet.Numerics.LinearAlgebra;
using UnityEngine;

namespace dsg_wbm {
    public class Structure {
        public List<Node> nodes; 
        public List<Element> elements;
        public List<Load> loads;
        public int dims; // 2, 3
        public List<Node> displacedNodes;
        public List<Element> displacedElements;
        public List<bool> DOFS;
        public Matrix<double> K; // global stiffness matrix
        public List<double> F; // external loads
        public List<double> U; // displacement after analysis
        public List<double> reactions; // external reactions
        public float complicance; // compliance after analysis
        public int nNodes; // number of nodes
        public int nElements; // number of elements
        public int nDOFS; // number of DOFs (total)
        public List<int> freeDOFS; // array of free DOFs

        public Structure(List<Node> nodes, List<Element> elements, List<Load> loads) {
            this.nodes = nodes;
            this.elements = elements;
            this.loads = loads;
            this.dims = nodes[0].Position.Count;
            this.DOFS = new List<bool>();
            foreach (Node n in nodes)
                this.DOFS = this.DOFS.Concat(n.DOFS).ToList();
            
            this.nNodes = nodes.Count;
            this.nElements = elements.Count;
            this.nDOFS = this.DOFS.Count;
            this.freeDOFS = new List<int>();
            for (int i = 0; i < this.DOFS.Count; i++)
                if(this.DOFS.ElementAt(i))
                    this.freeDOFS.Add(i);

            this.F = new List<double>();
            for (int i = 0; i < this.nDOFS; i++)
                this.F.Add(0);

            var nodalDOFLength = nodes[0].DOFS.Count;
            foreach (Load l in loads)
                for (int i = 0; i < nodalDOFLength; i++)
                    this.F[nodalDOFLength * l.Index + i] = l.LoadValues.ElementAt(i);
        }

        public Structure(List<Node> nodes, List<Element> elements) {
            this.nodes = nodes;
            this.elements = elements;
            this.dims = nodes[0].Position.Count;
            this.DOFS = new List<bool>();
            foreach (Node n in nodes)
                this.DOFS = this.DOFS.Concat(n.DOFS).ToList();

            this.nNodes = nodes.Count;
            this.nElements = elements.Count;
            this.nDOFS = this.DOFS.Count;
            this.freeDOFS = new List<int>();
            for (int i = 0; i < this.DOFS.Count; i++)
                if(this.DOFS.ElementAt(i))
                    this.freeDOFS.Add(i);
        }
    }
}