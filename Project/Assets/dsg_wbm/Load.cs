using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MathNet.Numerics.LinearAlgebra;

namespace dsg_wbm
{
    public class Load
    {
        public Vector<double> Position;
        public Vector<double> LoadValues;
        public int Index;


        public Load(List<Node> nodes, Vector<double> position, Vector<double> load)
        {
            List<double> distances = new List<double>(); 
            for(int i = 0; i < nodes.Count; i++)
            {
                distances.Add((nodes[i].Position - position).L2Norm());
            }

            // Find minimum value
            double minDistance = distances.Min();

            if (minDistance > Utilities.GeometricTolerance)
            {
                throw new Exception("Geometric tolerance for node-association too large.");
            }
            else
            {
                int index = distances.IndexOf(minDistance);

                if (load.Count != nodes[index].nDOFS)
                {
                    throw new Exception("Dimension of load is not equal to number of DOFs at associated node.");
                }

                this.Position = nodes[index].Position;
                this.LoadValues = load;
                this.Index = index;
            }
        }

        public Load(List<Node> nodes, int index, Vector<double> load)
        {
            if (index >= nodes.Count)
            {
                throw new Exception("index is greater than number of nodes.");
            }
            else if (load.Count!= nodes[index].nDOFS)
            {
                throw new Exception("Dimension of load is not equal to number of DOFs at associated node.");
            }
            else
            {
                this.Position = nodes[index].Position;
                this.LoadValues = load;
                this.Index = index;
            }
        }
    }
}