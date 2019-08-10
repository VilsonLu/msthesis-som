using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SOMLibrary.Implementation.NeighborhoodKernel
{
    public class GaussianKernel : INeighborhoodKernel
    {
        public double CalculateNeighborhoodFunction(double distance, double radius)
        {
            double distanceSquared = Math.Pow(distance, 2);
            double size = 2 * Math.Pow(radius, 2);
            double influence = Math.Exp(-((double)distanceSquared / (double)size));

            return influence;
        }
    }
}
