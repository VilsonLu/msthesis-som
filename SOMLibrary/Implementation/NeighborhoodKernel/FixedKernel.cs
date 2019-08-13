using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SOMLibrary.Implementation.NeighborhoodKernel
{
    public class FixedKernel : INeighborhoodKernel
    {
        public double CalculateNeighborhoodFunction(double distance, double radius)
        {
            return 1.0;
        }
    }
}
