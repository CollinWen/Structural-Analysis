using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dsg_wbm
{
    /// <summary>
    /// Meant to store standard structural sections
    /// </summary>
    public class Section
    {
        public double E;
        public double A;
        public double G;
        public double Iz;
        public double Iy;
        public double J;

        /// <summary>
        /// Base initializer assumes steel HSS141x13 in m, kN
        /// </summary>
        public Section()
        {
            this.E = 200e6;
            this.A = 0.00466;
            this.G = 79e6;
            this.Iz = 9.91e-6;
            this.Iy = 9.91e-6;
            this.J = 19.8e-6;
        }

        /// <summary>
        /// Must supply enough properties to perform 3D frame analysis
        /// </summary>
        /// <param name="E"></param>
        /// <param name="A"></param>
        /// <param name="G"></param>
        /// <param name="Iz"></param>
        /// <param name="Iy"></param>
        /// <param name="J"></param>
        public Section(double E, double A, double G, double Iz, double Iy, double J)
        {
            this.E = E;
            this.A = A;
            this.G = G;
            this.Iz = Iz;
            this.Iy = Iy;
            this.J = J;
        }
    }
}